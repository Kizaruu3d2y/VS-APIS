using Dapper;
using Models.Endpoint;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Threading.Tasks;

namespace Data.Cliente
{
    public class RegistrarTagRepository
    {
        private readonly IConfiguration _configuration;

        public RegistrarTagRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SolicitarTagAsync(string codCuenta, string placa, string tag, string recursoIngresa)
        {
            using var connection = new OracleConnection(_configuration.GetConnectionString("OracleDb"));

            string empresa = EmpresaGlobalDto.Empresa;

            // Primero intento UPDATE
            string updateQuery = @"
                UPDATE sav_placa_aut
                   SET tag = :tag,
                       recurso_ingresa = :recurso,
                       fecha_ingresa = sysdate
                 WHERE placa = :placa
                   AND cod_cuenta = :codCuenta
                   AND estado = 'P'";

            var updateParams = new
            {
                tag,
                recurso = recursoIngresa,
                placa,
                codCuenta
            };

            var filasAfectadas = await connection.ExecuteAsync(updateQuery, updateParams);

            // Si no afectó filas, hago INSERT
            if (filasAfectadas == 0)
            {
                string insertQuery = @"
                    INSERT INTO sav_placa_aut
                        (empresa, cod_cuenta, placa, tag, estado, recurso_ingresa, fecha_ingresa, recurso_aprueba, fecha_aprueba)
                    VALUES
                        (:empresa, :codCuenta, :placa, :tag, 'P', :recurso, sysdate, '', '')";

                var insertParams = new
                {
                    empresa,
                    codCuenta,
                    placa,
                    tag,
                    recurso = recursoIngresa
                };

                filasAfectadas = await connection.ExecuteAsync(insertQuery, insertParams);
            }

            return filasAfectadas > 0;
        }
    }
}