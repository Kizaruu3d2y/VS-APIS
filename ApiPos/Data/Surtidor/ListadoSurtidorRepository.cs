using Dapper;
using Models.Endpoint;
using Models.Surtidor;
using Oracle.ManagedDataAccess.Client;
using System.Data;
namespace Data.Surtidor
{
    public class ListadoSurtidorRepository
    {
        private readonly string _connectionString;

        public ListadoSurtidorRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        public async Task<IEnumerable<ListadoSurtidorDto>> ObtenerListadoSurtidoresAsync()
        {
            var empresa = EmpresaGlobalDto.Empresa;

            var query = @"
            select s.surtidor surtidor, s.estado estado, s.recurso recurso, s.autorizando autorizando
              from pos_estado_surtidor s, cor_entidades c  
             where s.recurso = c.cod_entidad  
               and s.empresa = :empresa
             order by 1";

            using var connection = new OracleConnection(_connectionString);
            await connection.OpenAsync();

            var resultados = await connection.QueryAsync<ListadoSurtidorDto>(
                query,
                new { empresa }
            );

            return resultados;
        }
    }
}
