using Dapper;
using Models.Endpoint;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Data.Bitacora
{
    public class IngresarMedidaTanqueRepository
    {
        private readonly IConfiguration _configuration;

        public IngresarMedidaTanqueRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> GuardarMedidaTanqueAsync(
            string recurso,
            decimal superComb,
            decimal plus91,
            decimal diesel,
            DateTime fechaTurno)
        {
            using var connection = new OracleConnection(_configuration.GetConnectionString("OracleDb"));
            await connection.OpenAsync();

            string empresa = EmpresaGlobalDto.Empresa;

            // Primero intentamos actualizar
            string updateSql = @"
                update bit_tanques_medidas 
                   set fecha_registro = sysdate,
                       recurso = :recurso,
                       super = :superComb,
                       plus91 = :plus91,
                       diesel = :diesel
                 where fecha_turno = to_date(:fechaTurno, 'dd/MM/yyyy HH24:mi')
                   and empresa = :empresa";

            var filas = await connection.ExecuteAsync(updateSql, new
            {
                recurso,
                superComb,
                plus91,
                diesel,
                fechaTurno = fechaTurno.ToString("dd/MM/yyyy HH:mm"),
                empresa
            });

            // Si no existe, insertamos
            if (filas == 0)
            {
                string insertSql = @"
                    insert into bit_tanques_medidas
                        (empresa, fecha_turno, recurso, super, plus91, diesel, fecha_registro)
                    values
                        (:empresa, to_date(:fechaTurno, 'dd/MM/yyyy HH24:mi'), :recurso, :superComb, :plus91, :diesel, sysdate)";

                filas = await connection.ExecuteAsync(insertSql, new
                {
                    empresa,
                    fechaTurno = fechaTurno.ToString("dd/MM/yyyy HH:mm"),
                    recurso,
                    superComb,
                    plus91,
                    diesel
                });
            }

            return filas > 0;
        }
    }
}