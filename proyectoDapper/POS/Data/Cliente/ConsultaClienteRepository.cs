using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace proyectoDapper.POS.Data.Cliente
{
    public class ConsultaClienteRepository
    {
        private readonly IConfiguration _configuration;

        public ConsultaClienteRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string?> ObtenerCodigoPorCedulaAsync(string tipoId, string cedula)
        {
            using var connection = new OracleConnection(_configuration.GetConnectionString("OracleDb"));

            string query = @"
                select cod_entidad 
                  from cor_identificaciones 
                 where cod_entidad not like 'P%' 
                   and cod_entidad not like 'C%' 
                   and cod_entidad not like 'D%' 
                   and tipo_identificacion = :tipoId 
                   and identificacion = :cedula";

            return await connection.QueryFirstOrDefaultAsync<string>(query, new { tipoId, cedula });
        }
    }
}