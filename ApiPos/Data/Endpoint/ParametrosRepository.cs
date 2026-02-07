using Dapper;
using Models.Endpoint;
using Oracle.ManagedDataAccess.Client;

namespace Data.Endpoint
{
    public class ParametrosRepository
    {
        private readonly string _connectionString;

        public ParametrosRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        public async Task<ParametrosDto?> ObtenerParametrosAsync()
        {
            var sql = @"
                select  e.razon_social empresa, e.id_fiscal cedula, e.formato_id_tributario formatoCedula, 
                       c.telefono1 telefono, '####-####' formatoTelefono, e.direccion direccion
                  from cor_empresas e, cor_entidades c, fac_puestos_trab f, fac_comb_parametros p  
                 where c.cod_entidad = e.cod_entidad  
                   and f.empresa = e.compania 
                   and e.compania = p.empresa 
                   and p.ind_estacion = 'S'  
                   and p.ult_cierre is not null 
                   and f.cod_puesto = (select cod_puesto  
                                         from fac_comb_surtidores  
                                        where empresa = e.compania  
                                          and cod_division = '01'  
                                          and cod_surtidor = '1')";

            using var connection = new OracleConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<ParametrosDto>(sql);
        }
    }
}