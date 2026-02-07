using Dapper;
using Models.Cliente;
using Models.Endpoint;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Cliente
{
    public class ListaClienteRepository
    {
        private readonly IConfiguration _configuration;

        public ListaClienteRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<ClienteModelDto>> CargarClientesAsync(string? codCliente = null)
        {
            using var connection = new OracleConnection(_configuration.GetConnectionString("OracleDb"));
            await connection.OpenAsync();

            string empresa = EmpresaGlobalDto.Empresa;

            // Primer query: buscar por código exacto
            string queryExacto = @"
                select c.cod_cliente as codCliente,
                       nvl(e.alias, e.nombre_entidad) as nombreCliente
                  from fac_clientes c
                  join cor_entidades e on c.cod_entidad_cliente = e.cod_entidad
                  join cor_identificaciones i on c.cod_entidad_cliente = i.cod_entidad
                 where e.empresa = corp_util.Empresa_para_Entidades(c.empresa)
                   and c.empresa = :empresa
                   and c.ind_activo = 'A'
                   and e.cod_entidad = :codCliente
                 order by nvl(e.alias, e.nombre_entidad)";

            var clientes = await connection.QueryAsync<ClienteModelDto>(queryExacto, new { empresa, codCliente });

            // Si no hay resultados, ejecutamos el query con LIKE usando subText
            if (!clientes.AsList().Any() && !string.IsNullOrEmpty(codCliente))
            {
                string queryLike = @"
                    select c.cod_cliente as codCliente,
                           nvl(e.alias, e.nombre_entidad) as nombreCliente
                      from fac_clientes c
                      join cor_entidades e on c.cod_entidad_cliente = e.cod_entidad
                      join cor_identificaciones i on c.cod_entidad_cliente = i.cod_entidad
                     where e.empresa = corp_util.Empresa_para_Entidades(c.empresa)
                       and c.empresa = :empresa
                       and c.ind_activo = 'A'
                       and (replace(upper(e.alias),'.') like replace(:codCliente,'.')
                            or replace(upper(e.nombre_entidad),'.') like replace(:codCliente,'.'))
                     order by nvl(e.alias, e.nombre_entidad)";

                clientes = await connection.QueryAsync<ClienteModelDto>(queryLike, new { empresa, codCliente = $"%{codCliente.ToUpper()}%" });
            }

            return clientes;
        }
    }
}