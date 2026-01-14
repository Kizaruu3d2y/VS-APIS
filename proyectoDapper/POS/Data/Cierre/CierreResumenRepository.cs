using Dapper;
using Oracle.ManagedDataAccess.Client;
using proyectoDapper.POS.Models.Surtidor.Cliente.Cierre;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace proyectoDapper.POS.Data.Cierre
{
    public class CierreResumenRepository
    {
        private readonly string _connectionString;

        public CierreResumenRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        public async Task<List<CierreResumenDto>> ObtenerCierreResumenAsync(string empresa)
        {
            try
            {
                using IDbConnection connection = new OracleConnection(_connectionString);

                var query = @"
                SELECT 
                    v.id_linea idLinea, 
                    v.codigo_valor codigo, 
                    v.descripcion descripcion, 
                    v.cod_moneda moneda, 
                    DECODE(v.cod_moneda, 'COL', 1,
                        (SELECT compra
                           FROM (
                                SELECT fecha, compra
                                  FROM cor_tipos_de_cambio
                                 WHERE moneda_origen = 'DOL'
                                   AND moneda_destino = 'COL'
                                 ORDER BY fecha DESC
                               )
                          WHERE ROWNUM = 1
                        )
                    ) AS tipo_cambio,
                    0 AS monto, 
                    v.no_datafono
                FROM pos_cierre_valores v
                WHERE v.empresa = :empresa
                ORDER BY v.id_linea ASC";

                var parametros = new { empresa };

                var resultado = await connection.QueryAsync<CierreResumenDto>(query, parametros);
                return resultado.AsList();
            }
            catch (Exception e)
            {
                e.ToString();
                return null;
            }
        }
    }
}
