using Models.Endpoint;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Data.Cliente
{
    public class RegistraClienteRepository
    {
        private readonly IConfiguration _configuration;

        public RegistraClienteRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task RegistrarClienteAsync(string codigo, string nombre, string telefono, string correo, string tipoId, string identificacion)
        {
            using var connection = new OracleConnection(_configuration.GetConnectionString("OracleDb"));
            await connection.OpenAsync();

            using var command = new OracleCommand("pos_util.registra_cliente", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Parámetros de entrada
            command.Parameters.Add("pempresa", OracleDbType.Varchar2).Value = EmpresaGlobalDto.Empresa;
            command.Parameters.Add("pCodCliente", OracleDbType.Varchar2).Value = codigo;
            command.Parameters.Add("pNombreCliente", OracleDbType.Varchar2).Value = nombre;
            command.Parameters.Add("pTelefono", OracleDbType.Varchar2).Value = telefono;
            command.Parameters.Add("pCorreo", OracleDbType.Varchar2).Value = correo;
            command.Parameters.Add("pTipoIdentificacion", OracleDbType.Varchar2).Value = tipoId;
            command.Parameters.Add("pIdentificacion", OracleDbType.Varchar2).Value = identificacion;

            await command.ExecuteNonQueryAsync();
        }
    }
}