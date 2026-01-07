using Dapper;
using System.Data;
using proyectoDapper.Models.Pos;
using proyectoDapper.POS.Data.Endpoint;

namespace proyectoDapper.POS.Data.Cierre
{
    public class LimiteCierresRepository
    {
        private readonly IConfiguration _configuration;
        private readonly FechaTurnoRepository _fechaTurnoRepository;

        public LimiteCierresRepository(IConfiguration configuration, FechaTurnoRepository fechaTurnoRepository)
        {
            _configuration = configuration;
            _fechaTurnoRepository = fechaTurnoRepository;
        }

        public async Task<bool?> VerificarLimiteCierresAsync(string recursoPistero)
        {
            try
            {
                using IDbConnection connection = new Oracle.ManagedDataAccess.Client.OracleConnection(
                    _configuration.GetConnectionString("OracleDb")
                );

                string empresa = EmpresaGlobalDto.Empresa;
                var fechaTurno = await _fechaTurnoRepository.ObtenerFechaTurnoAsync();

                if (fechaTurno == null)
                    return null;

                string query = @"
                select b.version as version, e.valor1 as total
                  from pos_bitacora_cierres b, cor_parametros_esp e
                 where b.empresa = :empresa
                   and b.cod_recurso = :recurso
                   and b.fecha = TO_DATE(:fecha, 'dd-MM-yyyy')
                   and e.cod_parametro = 'MAX_CIERR'";

                var result = await connection.QueryFirstOrDefaultAsync<(int Cant, int Total)>(
                    query, new { empresa, recurso = recursoPistero, fecha = fechaTurno.Fecha.ToShortDateString() }
                );

                if (result.Cant == 0 && result.Total == 0)
                    return true;

                return result.Cant >= result.Total;
            }

            catch (Exception e)
            {
                e.ToString();
                return null;
            }
        }
        public async Task<int?> LimiteCierresAsync(string recursoPistero)
        {
            try
            {
                using IDbConnection connection = new Oracle.ManagedDataAccess.Client.OracleConnection(
                    _configuration.GetConnectionString("OracleDb")
                );

                string empresa = EmpresaGlobalDto.Empresa;
                var fechaTurno = await _fechaTurnoRepository.ObtenerFechaTurnoAsync();

                if (fechaTurno == null)
                    return null;

                string query = @"
                select b.version as version, e.valor1 as total
                  from pos_bitacora_cierres b, cor_parametros_esp e
                 where b.empresa = :empresa
                   and b.cod_recurso = :recurso
                   and b.fecha = TO_DATE(:fecha, 'dd-MM-yyyy')
                   and e.cod_parametro = 'MAX_CIERR'";

                var result = await connection.QueryFirstOrDefaultAsync<(int version, int Total)>(
                    query, new { empresa, recurso = recursoPistero, fecha = fechaTurno.Fecha.ToShortDateString() }
                );

                return result.version;
            }

            catch (Exception e)
            {
                e.ToString();
                return null;
            }
        }
    }
}