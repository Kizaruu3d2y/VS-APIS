using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using proyectoDapper.Models.Pos;
using proyectoDapper.POS.Data.Endpoint;
using proyectoDapper.POS.Models.Surtidor;
using System.Data;

namespace proyectoDapper.POS.Data.Surtidor
{
    public class SurtidorFacturaRepository
    {
        private readonly IConfiguration _configuration;
        private readonly FechaTurnoRepository _fechaTurnoRepository;

        public SurtidorFacturaRepository(
            IConfiguration configuration,
            FechaTurnoRepository fechaTurnoRepository)
        {
            _configuration = configuration;
            _fechaTurnoRepository = fechaTurnoRepository;
        }

        // =========================
        // Helper OracleDecimal -> int
        // =========================
        private int ToInt(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            if (value is OracleDecimal od)
                return od.IsNull ? 0 : od.ToInt32();

            return Convert.ToInt32(value);
        }

        public async Task<SurtidorFacturaDto?> ObtenerFacturaSurtidorAsync(string recurso, string surtidor)
        {
            try
            {
                var fechaTurno = await _fechaTurnoRepository.ObtenerFechaTurnoAsync();
                if (fechaTurno == null)
                    return null;

                using var connection = new OracleConnection(
                    _configuration.GetConnectionString("OracleDb"));

                await connection.OpenAsync();

                var query = @"
               select de.id_factura,
                       fe.clave, fe.consecutivo, es.ult_despacho as lectura_surtidor,de.tipo_factura,
                       case
                         when ((sysdate - de.fecha) * 60 * 24) < 2 * pe.valor1 then
                          'N'
                         else
                          'S'
                       end as expiro,
                       hf.cod_cliente,
                       decode(de.tipo_factura,
                              'CA',
                              'Calibracion',
                              'FI',
                              'Cliente ocasional de contado',
                              hf.nombre_cliente) as nombre_cliente,
                       en.alias,
                       hf.plazo as credito,
                       hf.credito_contado,
                       hf.actividad_economica,
                       en.e_mail1 as correoco,
                       nvl(co.e_mail, en.e_mail1) as correocr,
                       hf.valor_opc1 as placa,
                       hf.valor_opc3 as km,
                       hf.numero_orden_compra as oc,
                       hf.observaciones
                  from pos_estado_surtidor es
                  join pos_despachos de on es.empresa = de.empresa
                                       and es.ult_despacho = de.lectura
                  left join fe_facturas fe on fe.id_factura = de.id_factura
                  left join fac_hist_facturas hf on de.id_factura = hf.id_factura
                  left join cor_entidades en on hf.cod_cliente = en.cod_entidad
                  left join cor_parametros_esp pe on pe.cod_sistema = 'FAC'
                                                 and pe.cod_parametro = 'TIMER_FAC'
                  left join (select cc.empresa, cc.cod_cuenta, min(cc.e_mail) e_mail
                               from cxc_cuentas_correos cc
                              where cc.envio_fac = 'S'
                              group by cc.empresa, cc.cod_cuenta) co on es.empresa =
                                                                        co.empresa
                                                                    and hf.cod_cliente =
                                                                        co.cod_cuenta
                 where es.empresa = :empresa
                   and es.surtidor = :surtidor
                   and de.fecha_turno = to_date(:fechaTurno, 'dd/MM/yyyy')";

                using var command = new OracleCommand(query, connection);
                command.CommandType = CommandType.Text;
                command.Parameters.Add("empresa", OracleDbType.Varchar2).Value = EmpresaGlobalDto.Empresa;
                command.Parameters.Add("surtidor", OracleDbType.Varchar2).Value = surtidor;
                command.Parameters.Add("fechaTurno", OracleDbType.Varchar2).Value = fechaTurno.Fecha.ToShortDateString();

                using var reader = await command.ExecuteReaderAsync();

                if (!reader.Read())
                    return null;

                return new SurtidorFacturaDto
                {
                    IdFactura = reader["id_factura"]?.ToString(),
                    Clave = reader["clave"]?.ToString(),
                    Consecutivo = reader["consecutivo"]?.ToString(),
                    LecturaSurtidor = reader["lectura_surtidor"]?.ToString(),
                    TipoFact = reader["tipo_factura"]?.ToString(),
                    Expiro = reader["expiro"]?.ToString() == "S",
                    CodCliente = reader["cod_cliente"]?.ToString(),
                    NombreCliente = reader["nombre_cliente"]?.ToString(),
                    Alias = reader["alias"]?.ToString(),
                    Credito = ToInt(reader["credito"]),
                    CreditoContado = reader["credito_contado"]?.ToString() == "S",
                    ActividadEconomica = reader["actividad_economica"]?.ToString(),
                    Correoco = reader["correoco"]?.ToString(),
                    Correocr = reader["correocr"]?.ToString(),
                    Placa = reader["placa"]?.ToString(),
                    Km = ToInt(reader["km"]),
                    Oc = reader["oc"]?.ToString(),
                    Observaciones = reader["observaciones"]?.ToString()
                };
            }
            catch (Exception e)
            {
                e.ToString();
                return null;
            }
        }

    }
}