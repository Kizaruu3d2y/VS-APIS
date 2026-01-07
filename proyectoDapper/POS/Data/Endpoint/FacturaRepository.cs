using Oracle.ManagedDataAccess.Client;
using proyectoDapper.Models.Pos;
using proyectoDapper.POS.Models.Surtidor;
using System.Data;

namespace proyectoDapper.POS.Data.Endpoint
{
    public class FacturaRepository
    {
        private readonly string _connectionString;

        public FacturaRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        public async Task<FacturaResponseDto> RegistrarFacturaAsync(FacturaRequestDto request)
        {
            try
            {
                using var conn = new OracleConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "POS_UTIL.INSERTA_FACTURA";
                cmd.BindByName = true;

                // ===== RETURN =====
                cmd.Parameters.Add("RETURN_VALUE", OracleDbType.Varchar2, 80)
                    .Direction = ParameterDirection.ReturnValue;

                // ===== INPUT =====
                cmd.Parameters.Add("pEmpresa", OracleDbType.Varchar2).Value = EmpresaGlobalDto.Empresa;
                cmd.Parameters.Add("pEntidad", OracleDbType.Varchar2).Value = request.codRecurso;

                cmd.Parameters.Add("pCodCuenta", OracleDbType.Varchar2)
                    .Value = string.IsNullOrWhiteSpace(request.codCliente) || request.codCliente == "0"
                        ? ""
                        : request.codCliente;

                cmd.Parameters.Add("pActividadEconomica", OracleDbType.Varchar2)
                    .Value = request.actividadEco;

                cmd.Parameters.Add("pPlacaCliente", OracleDbType.Varchar2)
                    .Value = request.placa ?? "";

                cmd.Parameters.Add("pKmCliente", OracleDbType.Varchar2)
                    .Value = string.IsNullOrWhiteSpace(request.km) ? "" : request.km;

                cmd.Parameters.Add("pOrdCompra", OracleDbType.Varchar2)
                    .Value = request.oc ?? "";

                cmd.Parameters.Add("pObservaciones", OracleDbType.Varchar2)
                    .Value = request.observaciones ?? "";

                cmd.Parameters.Add("pCreditoContado", OracleDbType.Varchar2)
                    .Value = request.creditoContado;

                cmd.Parameters.Add("pFormaPago", OracleDbType.Varchar2)
                    .Value = request.formaPago;

                cmd.Parameters.Add("pLecturaSurtidor", OracleDbType.Varchar2)
                    .Value = request.lecturaSurtidor ?? "";

                // ===== ARRAYS (CORREGIDO) =====

                var pListaArticulos = new OracleParameter("listaArticulos", OracleDbType.Varchar2)
                {
                    CollectionType = OracleCollectionType.PLSQLAssociativeArray,
                    UdtTypeName = "POS_UTIL.VARARRAY"
                };

                var pCantidadArticulos = new OracleParameter("cantidadArticulos", OracleDbType.Decimal)
                {
                    CollectionType = OracleCollectionType.PLSQLAssociativeArray,
                    UdtTypeName = "POS_UTIL.DOUARRAY"
                };

                if (request.idArticulos != null && request.idArticulos.Count > 0)
                {
                    pListaArticulos.Size = request.idArticulos.Count;
                    pListaArticulos.Value = request.idArticulos.ToArray();

                    pCantidadArticulos.Size = request.cantArticulos.Count;
                    pCantidadArticulos.Value = request.cantArticulos.ToArray();
                }
                else
                {
                    // 🔑 Workaround ODP.NET para arrays vacíos
                    pListaArticulos.Size = 1;
                    pListaArticulos.Value = new string[] { null };

                    pCantidadArticulos.Size = 1;
                    pCantidadArticulos.Value = new decimal[] { 0 };
                }

                cmd.Parameters.Add(pListaArticulos);
                cmd.Parameters.Add(pCantidadArticulos);

                // ===== OUTPUT =====
                cmd.Parameters.Add("pConsecutivo", OracleDbType.Varchar2, 50)
                    .Direction = ParameterDirection.Output;

                cmd.Parameters.Add("pCupon", OracleDbType.Varchar2, 50)
                    .Direction = ParameterDirection.Output;

                cmd.Parameters.Add("pMsg_Error", OracleDbType.Varchar2, 4000)
                    .Direction = ParameterDirection.Output;

                await cmd.ExecuteNonQueryAsync();

                return new FacturaResponseDto
                {
                    Clave = cmd.Parameters["RETURN_VALUE"].Value?.ToString(),
                    Consecutivo = cmd.Parameters["pConsecutivo"].Value?.ToString(),
                    Cupon = cmd.Parameters["pCupon"].Value?.ToString(),
                    MensajeError = cmd.Parameters["pMsg_Error"].Value?.ToString()
                };
            }
            catch (Exception ex)
            {
                return new FacturaResponseDto
                {
                    MensajeError = ex.Message
                };
            }
        }
    }
}
