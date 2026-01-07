using Dapper;
using Oracle.ManagedDataAccess.Client;
using proyectoDapper.POS.Data.Endpoint;
using proyectoDapper.POS.Models.Articulos;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace proyectoDapper.POS.Data.Articulos
{
    public class VentasAceiteRepository
    {
        private readonly FechaTurnoRepository _fechaTurnoRepository;
        private readonly string _connectionString;

        public VentasAceiteRepository(IConfiguration configuration, FechaTurnoRepository fechaTurnoRepository)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
            _fechaTurnoRepository = fechaTurnoRepository;
        }

        public async Task<List<VentaAceite>> ObtenerReporteVentasAsync(string tag, string empresa)
        {
            using IDbConnection connection = new OracleConnection(_connectionString);
            var fechaBD = await _fechaTurnoRepository.ObtenerFechaTurnoAsync();
            string fechaTurno = fechaBD.Fecha.ToShortDateString();
            var query = @"
                SELECT 
                    a.desc_articulo_corta AS articulo, 
                    SUM(l.unidades_mov) AS cantidad
                FROM fac_hist_facturas f
                JOIN fac_hist_lineas_factura l ON f.id_factura = l.id_factura
                JOIN inv_articulos a ON l.cod_articulo = a.cod_articulo AND f.empresa = a.empresa
                WHERE a.codigo_vox IS NULL
                  AND f.empresa = :empresa
                  AND f.fecha_turno = to_date(:fechaTurno,'dd/MM/yyyy')
                  AND f.cod_vendedor = :tag
                GROUP BY a.desc_articulo_corta";

            var parametros = new
            {
                empresa,
                fechaTurno,
                tag
            };

            var resultado = await connection.QueryAsync<VentaAceite>(query, parametros);
            return resultado.AsList();
        }
    }
}