using Dapper;
using Oracle.ManagedDataAccess.Client;
using proyectoDapper.Models.Pos;
using System.Data;
using System.Threading.Tasks;

namespace proyectoDapper.POS.Data.Surtidor
{
    public class DespachoLibreRepository
    {
        private readonly IConfiguration _configuration;

        public DespachoLibreRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> VerificarDespachoLibreAsync(string lectura)
        {
            using var connection = new OracleConnection(_configuration.GetConnectionString("OracleDb"));

            string empresa = EmpresaGlobalDto.Empresa;

            string query = @"
                select 'S' 
                  from pos_despachos
                 where empresa = :empresa
                   and lectura = :lectura
                   and tipo_factura = 'SF' ";

            var resultado = await connection.QueryFirstOrDefaultAsync<string>(query, new { empresa, lectura });
            return !string.IsNullOrEmpty(resultado);
        }
    }
}