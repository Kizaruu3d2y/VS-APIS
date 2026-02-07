using System.Data;
using Dapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using Models.Endpoint;

namespace Data.Endpoint
{
    public class PisteroRepository
    {
        private readonly IConfiguration _configuration;

        public PisteroRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<PisteroDto>> ObtenerPisterosAsync()
        {
            using IDbConnection connection = new Oracle.ManagedDataAccess.Client.OracleConnection(_configuration.GetConnectionString("OracleDb"));

            var empresa = EmpresaGlobalDto.Empresa;

            var query = @"SELECT r.cod_recurso codRecurso, e.nombre_entidad nombre, i.identificacion identificacion,
                                 DECODE(r.tipo_recurso, 'SU','True','False') AS supervisor,
                                 DECODE(r.tipo_recurso, 'AD','True','False') AS administrador,
                                 cod_barras tag
                          FROM fac_recursos r,
                               cor_entidades e,
                               cor_identificaciones i,
                               fac_puestos_trab p,
                               fac_comb_surtidores s
                          WHERE r.cod_recurso = e.cod_entidad
                            AND r.cod_recurso = i.cod_entidad(+)
                            AND r.empresa = p.empresa
                            AND r.empresa = s.empresa
                            AND p.cod_puesto = s.cod_puesto
                            AND r.empresa = :empresa
                            AND cod_division = '01'
                            AND cod_surtidor = '1'
                            AND r.ind_activo = 'S'";

            var result = await connection.QueryAsync<PisteroDto>(query, new { empresa });
            return result.AsList();
        }
    }
}
