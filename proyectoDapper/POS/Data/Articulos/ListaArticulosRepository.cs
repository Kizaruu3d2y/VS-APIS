using Dapper;
using Oracle.ManagedDataAccess.Client;
using proyectoDapper.Models.Pos;
using proyectoDapper.POS.Models.Articulos;


namespace proyectoDapper.POS.Data.Articulos
{
    public class ListaArticulosRepository
    {
        private readonly string _connectionString;

        public ListaArticulosRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        public async Task<List<ArticuloDto>> ObtenerListaArticulosAsync()
        {
            var empresa = EmpresaGlobalDto.Empresa;

            using var connection = new OracleConnection(_connectionString);

            var query = @"
                select i.id_articulo as IdArticulo,
                       i.desc_articulo_corta as Producto,
                       p.precio as Precio
                  from inv_articulos i,
                       inv_caracteristicas_x_art c,
                       fac_articulos_x_lista l,
                       fac_precios p
                 where i.id_articulo = c.id_articulo
                   and i.id_articulo = l.id_articulo
                   and i.empresa = l.empresa
                   and l.id_lista_articulo = p.id_lista_articulo
                   and i.empresa = :empresa
                   and i.estado = 'A'
                   and c.cod_caracteristica = 'PVENT'
                 order by producto";

            var parametros = new { empresa };

            var resultado = await connection.QueryAsync<ArticuloDto>(query, parametros);
            return resultado.AsList();
        }
    }
}