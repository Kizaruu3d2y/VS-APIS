using Dapper;
using Models.Endpoint;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Threading.Tasks;

namespace Data.Endpoint
{
    public class BitacoraDescargasRepository
    {
        private readonly IConfiguration _configuration;

        public BitacoraDescargasRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> RegistrarDescargaAsync(string recurso, string noFactura, string codVox, double cantidad, double inicial, double final, string observaciones)
        {
            using var connection = new OracleConnection(_configuration.GetConnectionString("OracleDb"));

            string empresa = EmpresaGlobalDto.Empresa;

            string query = @"
                insert into bit_descargas
                    (empresa, fecha, cod_recurso, id_articulo, cantidad, inicial, final, observaciones)
                values
                    (:empresa, sysdate, :recurso,
                     (select id_articulo from inv_articulos where empresa = :empresa and codigo_vox = :codVox),
                     :cantidad, :inicial, :final, :observaciones)";

            var parametros = new
            {
                empresa,
                recurso,
                noFactura,
                codVox,
                cantidad,
                inicial,
                final,
                observaciones
            };

            var rows = await connection.ExecuteAsync(query, parametros);
            return rows > 0;
        }
    }
}