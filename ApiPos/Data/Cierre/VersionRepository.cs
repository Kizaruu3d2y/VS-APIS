using Dapper;
using Data.Endpoint;
using Models.Cierre;
using Models.Endpoint;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Data.Cierre
{
    public class VersionRepository
    {
        private readonly string _connectionString;
        private readonly FechaTurnoRepository _fechaTurnoRepository;
        private readonly CierreRepository _cierreRepository;

        public VersionRepository(IConfiguration configuration, FechaTurnoRepository fechaTurnoRepository, CierreRepository cierreRepository)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
            _fechaTurnoRepository = fechaTurnoRepository;
            _cierreRepository = cierreRepository;
        }

        public bool RealizarCierreVersion(string codRecurso, List<ValorCierreDto> listaValores, CierreDto datosCierre, DateTime fechaTurno)
        {
            try
            {
                var empresa = EmpresaGlobalDto.Empresa;
                var fecha = fechaTurno.ToString("dd/MM/yyyy");
         
                using IDbConnection connection = new OracleConnection(_connectionString);
                connection.Open();

                using var tx = connection.BeginTransaction();
                
                // ===== Obtener cierre actual =====
                var cierre = connection.QueryFirstOrDefault<CierreVersionDto>(
                    @"select id_cierre
                        from fac_comb_cierres
                       where empresa = :empresa
                         and cod_recurso = :codRecurso
                         and fecha = to_date(:fecha,'dd/MM/yyyy')",
                    new
                    {
                        empresa,
                        codRecurso,
                        fecha
                    },
                    transaction: tx
                );
                int id_cierre = 0;
                
                if (cierre != null)
                {
                    id_cierre = cierre.id_cierre;   // Oracle NUMBER → decimal → int
                    //int no_turno = cierre.no_turno + 1;

                    // ===== Actualizar turno =====
                    var updateQueryVersion = @"update fac_comb_cierres
                                                  set version = version + 1
                                                where empresa = :empresa
                                                  and id_cierre = :id_cierre";

                    int rowsUpdated = connection.Execute(updateQueryVersion, new
                    {
                        empresa,
                        id_cierre
                    }, transaction: tx);

                    // validar que si se actualizo, si es true hacer el siguiente delete
                    if (rowsUpdated == 0)
                    {
                        //break
                    }

                    var deleteQueryVersion = @"delete fac_comb_valores_cierre 
                                                where id_cierre = :id_cierre";


                    connection.Execute(deleteQueryVersion, new
                    {
                        id_cierre
                    }, transaction: tx);
                    var deleteQueryVersion2 = @"delete pos_ajuste_cierre 
                                                where id_cierre = :id_cierre";


                    connection.Execute(deleteQueryVersion2, new
                    {
                        id_cierre
                    }, transaction: tx);
                }
                else
                {
                    string consecutivo = @"(select utl_consecutivos.siguiente('ID_CIERRE', :empresa, to_date(:fecha,'dd/MM/yyyy'), 'FAC') 
                                              from dual)";

                    id_cierre = connection.QuerySingle<int>(consecutivo, new
                    {
                        empresa,
                        fecha = fechaTurno.ToString("dd/MM/yyyy")
                    }, transaction: tx);

                    var updateQueryVersion = @"insert into fac_comb_cierres " +
                                                   "(empresa, id_cierre, cod_division, cod_puesto, cod_recurso, no_turno, fecha, estado, cod_moneda, total_vend_contado, total_vend_credito, id_transaccion_nd, tipo_cambio, id_factura_v_cont, total_fac_cont, total_fac_credi, total_vendido, faltante_sobrante, total_valores, version, estado_conta) " +
                                              "values " +
                                                   "(:empresa,:id_cierre, '01', 'I_01', :codrecurso, '1', to_date(:fecha,'dd/MM/yyyy'), 'A', 'COL', '', '', '', :tipoCambio, '', '', :total_fac_credi, :total_vendido, :faltante_sobrante, :totalvalores, 1, 'T')";
                    double total_fac_credi = datosCierre.vCredito;
                    double total_vendido = datosCierre.vCombustible + datosCierre.vAceite;
                    double faltante_sobrante = 0;
                    double totalvalores = 0;

                    foreach (var item in listaValores)
                        totalvalores += item.monto;

                    faltante_sobrante = totalvalores - total_vendido - total_fac_credi;
                    double tipoCambio = listaValores.First().tipoCambio;
                    int rowsUpdated = connection.Execute(updateQueryVersion, new
                    {
                        empresa,
                        id_cierre,
                        codRecurso,
                        fecha = fechaTurno.ToShortDateString(),
                        tipoCambio,
                        total_fac_credi,
                        total_vendido,
                        faltante_sobrante,
                        totalvalores
                    }, transaction: tx);
                }

                var InsertQueryVersion = @"insert into fac_comb_valores_cierre
                                               (id_cierre, id_linea, cod_forma_pago, cod_moneda, tipo_cambio, no_documento, cta_bancaria, cod_banco, monto_pagado, valor1, valor2, id_deposito, monto_depositado, no_datafono, version)
                                           values
                                               (:id_cierre, :id_linea, :cod_forma_pago, :cod_moneda, :tipo_cambio, null, null, null, :monto_pagado, null, null, null, null, :no_datafono, 0)";

                var InsertQueryVersion2 = @"insert into pos_ajuste_cierre
                                                  (id_cierre, id_linea, codigo_valor, cod_moneda, tipo_cambio, monto_pagado, no_datafono)
                                            values
                                                (:id_cierre, :id_linea, :codigo_valor, :cod_moneda, :tipo_cambio, :monto_pagado, :no_datafono)";

                foreach (var item in listaValores)
                {
                    if(item.codigo == "04")
                    {
                        connection.Execute(InsertQueryVersion2, new
                        {
                            id_cierre,
                            id_linea = item.idLinea,
                            codigo_valor = item.codigo,
                            cod_moneda = item.moneda,
                            tipo_cambio = item.tipoCambio,
                            monto_pagado = item.monto,
                            no_datafono = item.datafono
                        }, transaction: tx);
                    }
                    else
                    {
                        connection.Execute(InsertQueryVersion, new
                        {
                            id_cierre,
                            id_linea = item.idLinea,
                            cod_forma_pago = item.codigo,
                            cod_moneda = item.moneda,
                            tipo_cambio = item.tipoCambio,
                            monto_pagado = item.monto,
                            no_datafono = item.datafono
                        }, transaction: tx);
                    }
                }

                tx.Commit();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}