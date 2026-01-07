using Dapper;
using Oracle.ManagedDataAccess.Client;
using proyectoDapper.Models.Pos;
using proyectoDapper.POS.Data.Endpoint;
using proyectoDapper.POS.Models.Surtidor;
using System.Data;

namespace proyectoDapper.POS.Data.Surtidor
{
    public class LecturaRepository
    {
        private readonly IConfiguration _configuration;
        private readonly StartupService _startupService;

        public LecturaRepository(IConfiguration configuration, StartupService startupService)
        {
            _configuration = configuration;
            _startupService = startupService;
        }

        private IDbConnection CrearConexion()
        {
            return new OracleConnection(_configuration.GetConnectionString("OracleDb"));
        }

        public async Task<LecturaDto?> ObtenerLecturaAsync(string surtidor)
        {
            var empresa = EmpresaGlobalDto.Empresa;

            var query = @"
                select d.lectura UltLectura
                  from pos_despachos d, pos_anulaciones a
                 where d.empresa = a.empresa(+)
                   and d.lectura = a.lectura(+)
                   and d.empresa = :empresa
                   and d.lectura = (select ult_despacho
                                      from pos_estado_surtidor
                                     where empresa = :empresa
                                       and surtidor = :surtidor)
                   and tipo_factura = 'SF'
                   and ((sysdate - d.fecha) * 60 * 24) < decode(a.recurso, '', 1, 3) * 
                                                                  (select valor1
                                                                      from cor_parametros_esp
                                                                     where cod_sistema = 'FAC'
                                                                       and cod_parametro = 'TIMER_FAC')";

            using var connection = CrearConexion();

            return await connection.QueryFirstOrDefaultAsync<LecturaDto>(
                query,
                new { empresa, surtidor }
            );
    
        }
    }
}