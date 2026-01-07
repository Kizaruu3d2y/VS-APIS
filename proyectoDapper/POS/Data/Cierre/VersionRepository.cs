using Dapper;
using Oracle.ManagedDataAccess.Client;
using proyectoDapper.Models.Pos;
using proyectoDapper.POS.Data.Endpoint;
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
        public bool RealizarCierreVersion(string codRecurso)
        {
            var empresa = EmpresaGlobalDto.Empresa;
            var fechaTurno = _fechaTurnoRepository.ObtenerFechaTurnoAsync().Result;
            var fecha = fechaTurno.Fecha.ToShortDateString();
            using IDbConnection connection = new OracleConnection(_connectionString);

            connection.Open();

            // Primero intento actualizar
            var updateQuery = @"
                UPDATE pos_bitacora_cierres
                   SET version = version+1
                 WHERE empresa = :empresa
                   AND cod_recurso = :codRecurso
                   AND fecha = to_date(:fecha,'dd/MM/yyyy')";

            var updateParams = new
            {
                empresa,
                codRecurso,
                fecha
            };

            var filasAfectadas = connection.Execute(updateQuery, updateParams);

            // Si no afectó filas, hacemos insert
            if (filasAfectadas == 0)
            {
                var insertQuery = @"
                    INSERT INTO pos_bitacora_cierres (empresa, cod_recurso, fecha, version)
                    VALUES (:empresa, :codRecurso, to_date(:fecha,'dd/MM/yyyy'), 1)";

                connection.Execute(insertQuery, updateParams);
            }
            return true;
        }
    }
}