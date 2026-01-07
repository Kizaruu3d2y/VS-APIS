using Dapper;
using Oracle.ManagedDataAccess.Client;
using proyectoDapper.Models.Pos;

namespace proyectoDapper.POS.Data.Endpoint
{
    public class ValidaUsuarioRepository
    {
        private readonly string _connectionString;

        public ValidaUsuarioRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        public Usuario? ValidarUsuario(string codRecurso)
        {
            var empresa = EmpresaGlobalDto.Empresa;

            using var connection = new OracleConnection(_connectionString);

            var query = @"
                select cod_recurso AS Codigo,  
                       tipo_recurso AS Rol
                  from fac_recursos 
                 where ind_activo = 'S'
                   and cod_recurso  = :codRecurso
                   and empresa = :empresa ";

            var parametros = new
            {
                codRecurso,
                empresa
            };

            // Método sincrono
            return connection.QueryFirstOrDefault<Usuario>(query, parametros);
        }
    }
}