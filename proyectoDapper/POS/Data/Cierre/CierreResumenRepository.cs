using Dapper;
using Oracle.ManagedDataAccess.Client;
using proyectoDapper.POS.Data.Endpoint;
using proyectoDapper.POS.Models.Surtidor.Cliente.Cierre;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace proyectoDapper.POS.Data.Cierre
{
    public class CierreResumenRepository
    {
        private readonly string _connectionString;
        private readonly FechaTurnoRepository _fechaTurnoRepository;

        public CierreResumenRepository(IConfiguration configuration, FechaTurnoRepository fechaTurnoRepository)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
            _fechaTurnoRepository = fechaTurnoRepository;
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

        public async Task<List<CierreResumenDto>> ObtenerReimpresionCierre(string empresa,string cod_recurso)
        {
            try
            {
                using IDbConnection connection = new OracleConnection(_connectionString);

                var fechaTurno = await _fechaTurnoRepository.ObtenerFechaTurnoAsync();
                var IdCierre = @"
                                    select id_cierre
                                      from fac_comb_cierres
                                     where cod_recurso = :cod_recurso
                                       and fecha = to_date(:fecha,'dd/MM/yyyy')
                                     order by fecha desc";

                var idCierre = await connection.ExecuteScalarAsync<long?>(
                    IdCierre,
                    new
                    {
                        cod_recurso,
                        fecha = fechaTurno.Fecha.ToShortDateString()
                    });

                if (idCierre == null)
                    return new List<CierreResumenDto>();


                var sqlResumen = @"
                                    select  v.id_linea         as idLinea,
                                            v.cod_forma_pago   as codigo,
                                            t.descripcion      as descripcion,
                                            v.cod_moneda       as moneda,
                                            v.tipo_cambio      as tipoCambio,
                                            v.monto_pagado     as monto,
                                            v.no_datafono      as noDatafono
                                      from fac_comb_valores_cierre v
                                      join pos_cierre_valores t
                                        on v.id_linea = t.id_linea
                                       and v.cod_forma_pago = t.codigo_valor
                                     where v.id_cierre = :id_cierre";

                var resultado = await connection.QueryAsync<CierreResumenDto>(
                    sqlResumen,
                    new { id_cierre = idCierre });

                return resultado.AsList();
            }
            catch (Exception ex)
            {
                throw; // nunca ocultes la excepción devolviendo null
            }
        }

    }
}
