using Dapper;
using proyectoDapper.Models.Pos;

namespace proyectoDapper.POS.Data.Endpoint
{
    public class StartupService
    {
        private readonly string _connectionString;

        public StartupService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        public async Task InicializarEmpresaAsync()
        {
            var query = @"select e.empresa 
                            from fac_comb_parametros e 
                           where e.ult_cierre is not null ";

            using var connection = new Oracle.ManagedDataAccess.Client.OracleConnection(_connectionString);
            await connection.OpenAsync();

            var compania = await connection.QueryFirstOrDefaultAsync<string>(query);

            if (!string.IsNullOrWhiteSpace(compania))
            {
                EmpresaGlobalDto.SetCompania(compania);
            }
        }
    }
}
