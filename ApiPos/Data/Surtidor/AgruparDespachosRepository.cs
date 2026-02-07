using Dapper;
using Models.Endpoint;
using Models.Surtidor;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Data.Surtidor
{
    public class AgruparDespachosRepository
    {
        private readonly string _connectionString;

        public AgruparDespachosRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        public async Task<List<DespachoAgrupadoDto>> ObtenerDespachosAgrupadosAsync(string pSurtidor, string pRecurso)
        {
            using IDbConnection connection = new OracleConnection(_connectionString);

            string empresa = EmpresaGlobalDto.Empresa;

            var query = @"
                SELECT TO_CHAR(d.fecha, 'HH24:MI') AS fecha, 
                       d.monto, 
                       d.cantidad AS litros, 
                       d.producto, 
                       d.recurso, 
                       d.lectura
                  FROM pos_despachos d
                  JOIN cor_entidades e ON d.recurso = e.cod_entidad
                 WHERE d.empresa = :empresa
                   AND d.surtidor LIKE :pSurtidor
                   AND d.recurso = :pRecurso
                   AND d.tipo_factura = 'SF'
                   AND d.fecha_turno = TRUNC(SYSDATE)
                 ORDER BY d.fecha DESC";

            var parametros = new
            {
                empresa,
                pSurtidor,
                pRecurso
            };

            var resultado = await connection.QueryAsync<DespachoAgrupadoDto>(query, parametros);
            return resultado.AsList();
        }
    }
}