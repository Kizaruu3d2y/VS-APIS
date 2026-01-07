using Dapper;
using Oracle.ManagedDataAccess.Client;
using proyectoDapper.Models.Pos;
using proyectoDapper.POS.Models.Surtidor.Cliente;
using System.Data;
using System.Threading.Tasks;

namespace proyectoDapper.POS.Data.Cliente
{
    public class ActualizaCorreoClienteRepository
    {
        private readonly IConfiguration _configuration;

        public ActualizaCorreoClienteRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ActualizaCorreoClienteDto> ActualizaCorreoClienteAsync(string codCliente, string correo)
        {
            var response = new ActualizaCorreoClienteDto { Resultado = false, Mensaje = "" };
            using var connection = new OracleConnection(_configuration.GetConnectionString("OracleDb"));
            await connection.OpenAsync();

            try
            {
                string empresa = EmpresaGlobalDto.Empresa;

                string query = @"
                    UPDATE cor_entidades
                       SET e_mail1 = :correo
                     WHERE cod_entidad = :codCliente";

                var parametros = new
                {
                    correo,
                    codCliente,
                    empresa
                };

                var filas = await connection.ExecuteAsync(query, parametros);
                response.Resultado = filas > 0;
            }
            catch (Exception ex)
            {
                response.Resultado = false;
                response.Mensaje = ex.Message;
            }

            return response;
        }
    }
}