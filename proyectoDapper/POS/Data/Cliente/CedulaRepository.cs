using Dapper;
using Oracle.ManagedDataAccess.Client;
using proyectoDapper.POS.Models.Surtidor.Cliente;

namespace proyectoDapper.POS.Data.Cliente
{
    public class CedulaRepository
    {
        private readonly IConfiguration _configuration;

        public CedulaRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<CedulaDto?> ValidarCedulaExisteAsync(string tipoCedula, string numeroCedula)
        {
            using var connection = new OracleConnection(_configuration.GetConnectionString("OracleDb"));
            await connection.OpenAsync();

            const string sql = @"
                select e.cod_entidad as CodCliente,
                       e.nombre_entidad as Nombre,
                       't' as Existe
                  from cor_identificaciones i
                  join cor_entidades e on i.cod_entidad = e.cod_entidad
                 where i.tipo_identificacion = :tipo
                   and i.identificacion = :cedula";

            return await connection.QueryFirstOrDefaultAsync<CedulaDto>(
                sql,
                new
                {
                    tipo = tipoCedula,
                    cedula = numeroCedula
                });
        }
    }
}