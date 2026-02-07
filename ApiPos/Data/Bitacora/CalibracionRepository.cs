using Dapper;
using Models.Bitacora;
using Models.Endpoint;
using Oracle.ManagedDataAccess.Client;

namespace Data.Bitacora
{
    public class CalibracionRepository
    {
        private readonly IConfiguration _configuration;

        public CalibracionRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Dictionary<string, List<object>>> ObtenerCalibracionAsync()
        {
            using var connection = new OracleConnection(_configuration.GetConnectionString("OracleDb"));

            string empresa = EmpresaGlobalDto.Empresa;

            // Queries separados
            string querySuper = @"
                select litros, 'TablaSuper' as Desc_Articulo
                from bit_tanques_tabla t
                join inv_articulos a on t.id_articulo = a.id_articulo and t.empresa = a.empresa
                where t.empresa = :empresa
                  and a.id_articulo = '3432107'
                order by cm";

            string queryPlus = @"
                select litros, 'TablaPlus' as Desc_Articulo
                from bit_tanques_tabla t
                join inv_articulos a on t.id_articulo = a.id_articulo and t.empresa = a.empresa
                where t.empresa = :empresa
                  and a.id_articulo = '3433107'
                order by cm";

            string queryDiesel = @"
                select litros, 'TablaDiesel' as Desc_Articulo
                from bit_tanques_tabla t
                join inv_articulos a on t.id_articulo = a.id_articulo and t.empresa = a.empresa
                where t.empresa = :empresa
                  and a.id_articulo = '3434107'
                order by cm";

            // Ejecutar cada query
            var listaSuper = (await connection.QueryAsync<CalibracionDto>(querySuper, new { empresa })).ToList();
            var listaPlus = (await connection.QueryAsync<CalibracionDto>(queryPlus, new { empresa })).ToList();
            var listaDiesel = (await connection.QueryAsync<CalibracionDto>(queryDiesel, new { empresa })).ToList();

            // Diccionario de resultado
            var resultado = new Dictionary<string, List<object>>();

            // Función auxiliar para agregar 0 litros al inicio
            void AgregarConCero(List<CalibracionDto> lista, string nombre)
            {
                var valores = lista.Select(x => x.Litros).Cast<object>().ToList();
                valores.Insert(0, 0m); // primer registro como número, no objeto
                resultado[nombre] = valores;
            }

            AgregarConCero(listaSuper, "tablaSuper");
            AgregarConCero(listaPlus, "tablaPlus");
            AgregarConCero(listaDiesel, "tablaDiesel");

            return resultado;
        }
    }
}