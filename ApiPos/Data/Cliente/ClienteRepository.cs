using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Data.Cliente
{
    public class ClienteRepository
    {
        private readonly IConfiguration _configuration;

        public ClienteRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string?> ObtenerNombreClienteAsync(string tipoId, string cedula)
        {
            using var connection = new OracleConnection(_configuration.GetConnectionString("OracleDb"));
            await connection.OpenAsync();

            using var command = new OracleCommand("consulta_cedula", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Parámetro de retorno
            command.Parameters.Add("p_nombre", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.ReturnValue;

            // Parámetros de entrada
            command.Parameters.Add("p_cedula", OracleDbType.Varchar2).Value = cedula;
            command.Parameters.Add("p_tipo_id", OracleDbType.Varchar2).Value = tipoId;


            await command.ExecuteNonQueryAsync();

            return command.Parameters["p_nombre"].Value?.ToString();
        }
    }
}