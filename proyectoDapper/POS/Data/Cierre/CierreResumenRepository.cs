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
                select 
                    v.id_linea idLinea, 
                    v.codigo_valor codigo, 
                    v.descripcion descripcion, 
                    v.cod_moneda moneda, 
                    decode(v.cod_moneda, 'COL', 1,
                        (select compra
                           from (
                                select fecha, compra
                                  from cor_tipos_de_cambio
                                 where moneda_origen = 'DOL'
                                   and moneda_destino = 'COL'
                                 order by fecha DESC
                               )
                          where ROWNUM = 1
                        )
                    ) as tipocambio,
                    0 as monto, 
                    v.no_datafono
                from pos_cierre_valores v
                where v.empresa = :empresa
                order by v.id_linea ASC";

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
                                     where v.id_cierre = :id_cierre
                                       and t.empresa = :empresa
                                    union all
                                    select  a.id_linea         as idLinea,
                                            a.codigo_valor     as codigo,
                                            t.descripcion      as descripcion,
                                            a.cod_moneda       as moneda,
                                            a.tipo_cambio      as tipoCambio,
                                            a.monto_pagado     as monto,
                                            a.no_datafono      as noDatafono
                                    from pos_ajuste_cierre a
                                    join pos_cierre_valores t
                                        on a.id_linea = t.id_linea
                                        and a.codigo_valor = t.codigo_valor
                                    where a.id_cierre = :id_cierre
                                        and t.empresa = :empresa
                                    order by idLinea ASC";

                var resultado = await connection.QueryAsync<CierreResumenDto>(
                    sqlResumen,
                    new { id_cierre = idCierre, empresa });

                return resultado.AsList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
