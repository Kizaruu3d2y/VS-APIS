using Dapper;
using Oracle.ManagedDataAccess.Client;
using proyectoDapper.Models.Pos;
using System.Data;

namespace proyectoDapper.POS.Data.Cliente
{
    public class PlacaRepository
    {
        private readonly IConfiguration _configuration;

        public PlacaRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<(string codCuenta, string placa)?> ObtenerPlacaPorTagAsync(string tag)
        {
            using var connection = new OracleConnection(_configuration.GetConnectionString("OracleDb"));

            string empresa = EmpresaGlobalDto.Empresa;

            string query = @"
                select cod_cuenta, placa
                  from sav_placa
                 where empresa = :empresa
                   and placa = :tag";

            return await connection.QueryFirstOrDefaultAsync<(string codCuenta, string placa)>(query, new { empresa, tag });
        }
    }
}