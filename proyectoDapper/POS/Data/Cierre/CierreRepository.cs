using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using proyectoDapper.POS.Models.Surtidor.Cliente.Cierre;
using proyectoDapper.Models.Pos;
using System.Data;
using proyectoDapper.POS.Data.Endpoint;

namespace proyectoDapper.POS.Data.Cierre
{
    public class CierreRepository
    {
        private readonly string _connectionString;
        private readonly FechaTurnoRepository _fechaTurnoRepository;

        public CierreRepository(
            IConfiguration configuration,
            FechaTurnoRepository fechaTurnoRepository)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
            _fechaTurnoRepository = fechaTurnoRepository;
        }

        // ==============================
        // Helper seguro Oracle -> int
        // ==============================
        private int ToInt(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            if (value is OracleDecimal od)
                return od.IsNull ? 0 : od.ToInt32();

            return Convert.ToInt32(value);
        }

        // =======================================
        // Obtener datos de cierre (REPORTE)
        // =======================================
        public async Task<CierreDto> ObtenerCierreAsync(string empresa,  string recursoPistero)
        {
            try
            {
                using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync();

                var fechaTurno = await _fechaTurnoRepository.ObtenerFechaTurnoAsync();
                if (fechaTurno == null)
                    return null;

                using var command = new OracleCommand("pos_util.reporte_ventas", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("p_empresa", OracleDbType.Varchar2).Value = empresa;
                command.Parameters.Add("p_recurso_pistero", OracleDbType.Varchar2).Value = recursoPistero;
                command.Parameters.Add("p_fecha_turno", OracleDbType.Date).Value = fechaTurno.Fecha;

                command.Parameters.Add("p_ventas_comb", OracleDbType.Int32).Direction = ParameterDirection.Output;
                command.Parameters.Add("p_ventas_acei", OracleDbType.Int32).Direction = ParameterDirection.Output;
                command.Parameters.Add("p_ventas_cred", OracleDbType.Int32).Direction = ParameterDirection.Output;
                command.Parameters.Add("p_calibracion", OracleDbType.Int32).Direction = ParameterDirection.Output;

                await command.ExecuteNonQueryAsync();

                return new CierreDto
                {
                    vCombustible = ToInt(command.Parameters["p_ventas_comb"].Value),
                    vAceite = ToInt(command.Parameters["p_ventas_acei"].Value),
                    vCredito = ToInt(command.Parameters["p_ventas_cred"].Value),
                    Calibracion = ToInt(command.Parameters["p_calibracion"].Value),
                    fechaCierre = fechaTurno.Fecha
                };
            }
            catch
            {
                return null;
            }
        }

        // =======================================
        // Ejecutar cierre diario
        // =======================================
        public async Task<(bool Exito, string? Error)> CierreDiario()
        {
            try
            {
                using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync();

                var fechaTurno = await _fechaTurnoRepository.ObtenerFechaTurnoAsync();
                if (fechaTurno == null)
                    return (false, "Fecha turno no encontrada");

                if (fechaTurno.Fecha >= DateTime.Today)
                    return (false, "No se puede cerrar fecha actual o futura");

                using var command = new OracleCommand(
                    "BEGIN :resultado := pos_util.cierre_diario(:pEmpresa, :pMsgError); END;",
                    connection);

                command.CommandType = CommandType.Text;

                command.Parameters.Add("resultado", OracleDbType.Int32)
                    .Direction = ParameterDirection.ReturnValue;

                command.Parameters.Add("pEmpresa", OracleDbType.Varchar2)
                    .Value = EmpresaGlobalDto.Empresa;

                command.Parameters.Add("pMsgError", OracleDbType.Varchar2, 4000)
                    .Direction = ParameterDirection.Output;

                await command.ExecuteNonQueryAsync();

                int resultado = ToInt(command.Parameters["resultado"].Value);
                string mensajeError = command.Parameters["pMsgError"].Value?.ToString();

                if (resultado != 0)
                    return (false, mensajeError ?? "Error en cierre");

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}