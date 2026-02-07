using Dapper;
using System.Data;
using System.Threading.Tasks;
using Models.Endpoint;

namespace Data.Endpoint
{
    public class FechaTurnoRepository
    {
        private readonly IConfiguration _configuration;

        public FechaTurnoRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task<FechaTurnoDto?> ObtenerFechaTurnoAsync()
        {
            using IDbConnection connection = new Oracle.ManagedDataAccess.Client.OracleConnection(_configuration.GetConnectionString("OracleDb"));

            string empresa = EmpresaGlobalDto.Empresa;

            var query = @"SELECT fecha_turno AS fecha
                           FROM fac_puestos_trab
                          WHERE empresa = :empresa
                            AND cod_puesto = (SELECT cod_puesto
                                                FROM fac_comb_surtidores
                                               WHERE empresa = :empresa
                                                 AND cod_division = '01'
                                                 AND cod_surtidor = '1')";

            return await connection.QueryFirstOrDefaultAsync<FechaTurnoDto>(query, new { empresa });
        }
    }
}