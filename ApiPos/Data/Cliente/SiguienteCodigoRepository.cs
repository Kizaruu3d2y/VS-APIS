using Models.Endpoint;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Data.Cliente
{
    public class SiguienteCodigoRepository
    {
        private readonly IConfiguration _configuration;

        public SiguienteCodigoRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string?> ObtenerSiguienteCodigoAsync()
        {
            using var connection = new OracleConnection(_configuration.GetConnectionString("OracleDb"));
            await connection.OpenAsync();

            using var command = new OracleCommand(
                "begin :result := factura_electronica.siguiente_numero(:pEmpresa); end;",
                connection);

            command.CommandType = CommandType.Text;

            // Parámetro de retorno (primer parámetro)
            command.Parameters.Add("result", OracleDbType.Varchar2, 100).Direction = ParameterDirection.ReturnValue;

            // Parámetro 1
            command.Parameters.Add("pEmpresa", OracleDbType.Varchar2).Value = EmpresaGlobalDto.Empresa;

            // Parámetro 2 (igual que Java: Func.getDbLink())
            //command.Parameters.Add("pDblink", OracleDbType.Varchar2).Value = "TU_DB_LINK"; // o el valor que uses

            await command.ExecuteNonQueryAsync();

            return command.Parameters["result"].Value?.ToString();
        }
    }
}