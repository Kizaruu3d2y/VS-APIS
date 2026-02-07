using Dapper;
using Models.Endpoint;
using Oracle.ManagedDataAccess.Client;


namespace Data.Endpoint
{
    public class TipoCambioRepository
    {
        private readonly string _connectionString;
        private readonly FechaTurnoRepository _fechaTurnoRepository;

        public TipoCambioRepository(IConfiguration configuration, FechaTurnoRepository fechaTurnoRepository)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
            _fechaTurnoRepository = fechaTurnoRepository;
        }

        public async Task<TipoCambioDto?> ObtenerTipoCambioAsync()
        {
            var empresa = EmpresaGlobalDto.Empresa;
            var fechaTurno = await _fechaTurnoRepository.ObtenerFechaTurnoAsync();
            string fechaBD = fechaTurno.Fecha.ToShortDateString();

            using var connection = new OracleConnection(_connectionString);

            var query = @"
                select fecha, compra
                  from cor_tipos_de_cambio
                 where fecha = to_date(:fechaBD,'dd/MM/yyyy')
                 order by fecha desc";

            var parametros = new
            {
                fechaBD,
                empresa
            };

            var resultado = await connection.QueryFirstOrDefaultAsync<TipoCambioDto>(query, parametros);
            return resultado;
        }
    }
}