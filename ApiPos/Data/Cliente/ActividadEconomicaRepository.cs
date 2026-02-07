using Models.Cliente;
using Models.Endpoint;
using Oracle.ManagedDataAccess.Client;

namespace Data.Cliente
{
    public class ActividadEconomicaRepository
    {
        private readonly string _connectionString;

        public ActividadEconomicaRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        /// <summary>
        /// Retorna SOLO las actividades del cliente (NO el response del API)
        /// </summary>
        public async Task<List<ActividadCliente>> CargarActividadesClienteAsync(string codCliente)
        {
            var lista = new List<ActividadCliente>();
            var empresa = EmpresaGlobalDto.Empresa;

            using var connection = new OracleConnection(_connectionString);
            using var command = connection.CreateCommand();

            command.CommandText = @"
                SELECT ind_principal,
                       cod_actividad
                  FROM fac_clientes_actividad
                 WHERE empresa     = :empresa
                   AND cod_cliente = :codCliente";

            command.Parameters.Add(new OracleParameter("empresa", empresa));
            command.Parameters.Add(new OracleParameter("codCliente", codCliente));

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new ActividadCliente(
                    reader.IsDBNull(0) ? null : reader.GetString(0),
                    reader.IsDBNull(1) ? null : reader.GetString(1)
                ));
            }

            return lista;
        }
    }
}
