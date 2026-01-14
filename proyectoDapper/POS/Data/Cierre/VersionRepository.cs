using Dapper;
using Oracle.ManagedDataAccess.Client;
using proyectoDapper.Models.Pos;
using proyectoDapper.POS.Data.Endpoint;
using proyectoDapper.POS.Models.Surtidor.Cliente.Cierre;
using System.Data;

namespace proyectoDapper.POS.Data.Cierre
{
    public class VersionRepository
    {
        private readonly string _connectionString;
        private readonly FechaTurnoRepository _fechaTurnoRepository;


        public VersionRepository(IConfiguration configuration, FechaTurnoRepository fechaTurnoRepository)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
            _fechaTurnoRepository = fechaTurnoRepository;
        }
        public bool RealizarCierreVersion(string codRecurso, List<ValorCierreDto> listaValores, DateTime fechaTurno)
        {
            try
            {
                var empresa = EmpresaGlobalDto.Empresa;
                var fecha = fechaTurno.ToString("dd/MM/yyyy");

                using IDbConnection connection = new OracleConnection(_connectionString);
                connection.Open();

                using var tx = connection.BeginTransaction();

                var consecutivo = connection.ExecuteScalar<long>(
                    @"SELECT S_POS_CIERRE.NEXTVAL FROM dual", transaction: tx);

                if (consecutivo <= 0)
                {
                    tx.Rollback();
                    return false;
                }

                var insertQueryVersion = @"
            insert into pos_bitacora_cierres
            (empresa, cod_recurso, fecha, version, id_cierre)
            values
            (:empresa, :codRecurso, to_date(:fecha,'dd/MM/yyyy'),
             (select count(*) + 1
                from pos_bitacora_cierres
               where cod_recurso = :codRecurso
                 and empresa = :empresa
                 and fecha = to_date(:fecha,'dd/MM/yyyy')),
             :consecutivo)";

                var filasInsertadasVersion = connection.Execute(insertQueryVersion, new
                {
                    empresa,
                    codRecurso,
                    fecha,
                    consecutivo
                }, tx);

                if (filasInsertadasVersion != 1)
                {
                    tx.Rollback();
                    return false;
                }

                var insertQueryCierre = @"
            insert into pos_bitacora_valores_cierre
            (id_cierre, id_linea, valor, monto)
            values
            (:consecutivo, :idlinea, :valor, :monto)";

                var parametros = listaValores.Select(v => new
                {
                    consecutivo,
                    idlinea = v.idlinea,
                    valor = v.codigo,
                    monto = v.monto
                });

                var filasInsertadasValores = connection.Execute(insertQueryCierre, parametros, tx);

                if (filasInsertadasValores != listaValores.Count)
                {
                    tx.Rollback();
                    return false;
                }

                tx.Commit();
                return true;
            }
            catch (Exception e)
            {
                e.Message.ToString();
                return false;
            }
        }
    }
}