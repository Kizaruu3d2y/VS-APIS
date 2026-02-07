using Dapper;
using Models.Endpoint;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Endpoint
{
    public class IVARepository
    {
        private readonly string _connectionString;

        public IVARepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        public async Task<TasaIVADto> ObtenerTasaIVAAsync()
        {
            var empresa = EmpresaGlobalDto.Empresa;

            using var connection = new OracleConnection(_connectionString);

            var query = @"
                select (porcentaje_imp/100) tasa
                  from cor_base_impuestos 
                 where empresa = :empresa
                   and rownum = 1";

            var parametros = new { empresa };

            var resultado = await connection.QueryFirstOrDefaultAsync<TasaIVADto>(query, parametros);
            return resultado;
        }
    }
}