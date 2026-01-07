using Oracle.ManagedDataAccess.Client;
using proyectoDapper.Models.Pos;
using System.Data;

namespace proyectoDapper.POS.Data.Bitacora
{
    public class IngresaCalibracionRepository
    {
        private readonly IConfiguration _configuration;

        public IngresaCalibracionRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> RegistrarCalibracionAsync(string lectura)
        {
            try
            {
                using var conn = new OracleConnection(_configuration.GetConnectionString("OracleDb"));
                await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "POS_UTIL.INSERTA_CALIBRACION";
                cmd.BindByName = true;

                // RETURN de la FUNCTION
                cmd.Parameters.Add("RETURN_VALUE", OracleDbType.Varchar2, 4000)
                    .Direction = ParameterDirection.ReturnValue;

                // INPUT
                cmd.Parameters.Add("pEmpresa", OracleDbType.Varchar2)
                    .Value = EmpresaGlobalDto.Empresa;

                cmd.Parameters.Add("pLectura", OracleDbType.Varchar2)
                    .Value = lectura;

                await cmd.ExecuteNonQueryAsync();

                return string.IsNullOrEmpty(cmd.Parameters["RETURN_VALUE"].Value?.ToString() ?? "");
            }
            catch (Exception ex)
            {
                ex.ToString();
                return true;
            }
        }
    }
}