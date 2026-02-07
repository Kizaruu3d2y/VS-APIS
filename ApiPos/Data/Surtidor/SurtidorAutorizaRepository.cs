using Dapper;
using Models.Endpoint;
using System.Data;
using System.Threading.Tasks;

namespace Data.Surtidor
{
    public class SurtidorAutorizaRepository
    {
        private readonly IConfiguration _configuration;

        public SurtidorAutorizaRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> AutorizarSurtidorAsync(string recurso, string surtidor)
        {
            using IDbConnection connection = new Oracle.ManagedDataAccess.Client.OracleConnection(_configuration.GetConnectionString("OracleDb"));

            string empresa = EmpresaGlobalDto.Empresa;

            var query = @"UPDATE pos_estado_surtidor
                             SET autorizando = :recurso, estado = DECODE(estado, 'DIS', 'AUT', 'AUT', 'DIS', estado), stamp = SYSDATE
                           WHERE empresa = :empresa 
                             AND surtidor = :surtidor";

            var result = await connection.ExecuteAsync(query, new { recurso, empresa, surtidor });

            return result > 0; // filas afectadas
        }
    }
}