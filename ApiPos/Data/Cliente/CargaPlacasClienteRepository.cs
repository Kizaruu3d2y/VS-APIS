using Dapper;
using Models.Cliente;
using Models.Endpoint;
using Oracle.ManagedDataAccess.Client;

namespace Data.Cliente
{
    public class CargaPlacasClienteRepository
    {
        private readonly IConfiguration _configuration;

        public CargaPlacasClienteRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<PlacaClienteResponseDto>> ObtenerPlacasClienteAsync(string codCliente)
        {
            using var connection = new OracleConnection(_configuration.GetConnectionString("OracleDb"));

            string empresa = EmpresaGlobalDto.Empresa;

            string query = @"
                select case when (tag is null and cod_barras is null) then 'N' else 'S' end tag,
                       placa
                  from sav_placa
                 where empresa = :empresa
                   and cod_cuenta = :codCliente";

            return await connection.QueryAsync<PlacaClienteResponseDto>(query, new { empresa, codCliente });
        }
    }
}