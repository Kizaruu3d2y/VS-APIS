using Dapper;
using Oracle.ManagedDataAccess.Client;
using proyectoDapper.Models.Pos;
using proyectoDapper.POS.Models.Surtidor;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace proyectoDapper.POS.Data.Surtidor
{
    public class SurtidorHistoricoRepository
    {
        private readonly string _connectionString;

        public SurtidorHistoricoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        public async Task<List<HistoricoSurtidorResponseDto>> ObtenerHistoricoDespachosAsync(string surtidor)
        {
            var empresa = EmpresaGlobalDto.Empresa;

            using var connection = new OracleConnection(_connectionString);

            var query = @"select decode(monto, null, '------', to_char(fecha, 'HH24:MI')) fecha, monto, litros, producto, cajero 
                            from (select dat.fecha, dat.monto, dat.litros, dat.producto, dat.cajero
                                    from (select d.fecha, d.monto, d.cantidad litros, d.producto, substr(e.nombre_entidad, 1, instr(e.nombre_entidad, ' ') + 1) cajero 
                                            from pos_despachos d, cor_entidades e 
                                           where d.recurso = e.cod_entidad 
                                             and d.empresa = :empresa
                                             and d.surtidor like :surtidor
                                             and d.fecha_turno = (select fecha_turno 
                                                                    from fac_puestos_trab 
                                                                   where empresa = :empresa
                                                                     and cod_puesto = 'I_01')) dat
                                     union 
                                    select trunc(sysdate) fecha, null monto, null litros, '------' producto, '------' cajero 
                                     from dual 
                                     order by fecha desc) 
                             where rownum <= 10";

            var parametros = new
            {
                empresa,
                surtidor = "%" + surtidor + "%"
            };

            var resultado = await connection.QueryAsync<HistoricoSurtidorResponseDto>(query, parametros);
            return resultado.AsList();
        }
    }
}