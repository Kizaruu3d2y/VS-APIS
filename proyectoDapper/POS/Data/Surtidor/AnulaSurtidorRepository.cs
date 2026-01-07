using Dapper;
using Oracle.ManagedDataAccess.Client;
using proyectoDapper.POS.Models.Surtidor;
using System.Data;

namespace proyectoDapper.POS.Data.Surtidor
{
    public class AnulaSurtidorRepository
    {
        private readonly string _conn;
        private readonly IConfiguration _config;

        public AnulaSurtidorRepository(IConfiguration config)
        {
            _config = config;
            _conn = config.GetConnectionString("OracleDb");
        }

        public async Task<AnulaSurtidorDto?> ObtenerFacturaUltimoDespacho(string empresa, string recurso, string surtidor)
        {
            try { 
            var sql = @"
                select d.id_factura idFactura ,
                       decode(d.tipo_factura, 'CA', 'Calibracion', 'FI',
                              'Cliente ocasional de contado', f.nombre_cliente) nombre_cliente,
                       d.monto monto,
                       d.lectura lectura,
                       d.tipo_factura tipoFactura
                from pos_despachos d,
                     fac_hist_facturas f
                where d.empresa = f.empresa(+)
                  and d.id_factura = f.id_factura(+)
                  and d.empresa = :empresa
                  and d.id_factura is not null
                  and d.lectura = (
                        select ult_despacho
                        from pos_estado_surtidor
                        where empresa = :empresa
                          and surtidor = :surtidor
                  )
                  and ((sysdate - d.fecha) * 60 * 24) <
                      2 * (select valor1
                           from cor_parametros_esp
                           where cod_sistema = 'FAC'
                             and cod_parametro = 'TIMER_FAC')
            ";

            using var db = new OracleConnection(_conn);

            return await db.QueryFirstOrDefaultAsync<AnulaSurtidorDto>(sql,
                new { empresa, surtidor });
            }
            catch (Exception e) { 
                e.ToString();
                return null;
            }

        }

        public async Task<bool> AnularFactura(
    string empresa,
    long idFactura,
    string recurso,
    string lectura,
    string tipoFactura)
        {
            try
            {
                using var conn = new OracleConnection(_conn);
                await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "POS_UTIL.ANULA_FACTURA";
                cmd.BindByName = true;

                cmd.Parameters.Add("pEmpresa", OracleDbType.Varchar2).Value = empresa;

                // 🔥 CLAVE: varchar2 + ToString()
                cmd.Parameters.Add("pIdFact", OracleDbType.Varchar2)
                    .Value = idFactura.ToString();

                cmd.Parameters.Add("pRecurso", OracleDbType.Varchar2).Value = recurso;
                cmd.Parameters.Add("pLectura", OracleDbType.Varchar2).Value = lectura;
                cmd.Parameters.Add("pTipo", OracleDbType.Varchar2).Value = tipoFactura;

                cmd.Parameters.Add("pMsg_Error", OracleDbType.Varchar2, 4000)
                    .Direction = ParameterDirection.Output;

                await cmd.ExecuteNonQueryAsync();

                return string.IsNullOrEmpty(cmd.Parameters["pMsg_Error"].Value?.ToString() ?? "");
            }
            catch (Exception ex)
            {
                ex.ToString();
                return true;
            }
        }
    }
}