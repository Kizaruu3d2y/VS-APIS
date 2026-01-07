using Dapper;
using Oracle.ManagedDataAccess.Client;
using proyectoDapper.Models.Pos;
using System;
using System.Data;
using System.Threading.Tasks;

namespace proyectoDapper.POS.Data.Cierre
{
    public class TurnoNocturnoRepository
    {
        private readonly string _connectionString;

        public TurnoNocturnoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        public bool VerificarTurnoNocturno(string tag, DateTime fechaTurno)
        {
            var empresa = EmpresaGlobalDto.Empresa;

            using IDbConnection connection = new OracleConnection(_connectionString);

            // Determinar desde y hasta para el turno nocturno
            var fechaDesde = fechaTurno + " 22:00:00"; // 10pm día anterior
            var fechaHasta = fechaTurno.AddDays(1).ToString("yyyy/MM/dd") + " 04:00:00"; // 4am día actual

            var query = @"
                SELECT 'S' 
                  FROM pos_despachos
                 WHERE empresa = :Empresa
                   AND fecha BETWEEN TO_DATE(:Desde,'yyyy/MM/dd HH24:MI:SS') AND TO_DATE(:Hasta,'yyyy/MM/dd HH24:MI:SS')
                   AND recurso = :Tag";

            var parametros = new
            {
                Empresa = empresa,
                Desde = fechaDesde,
                Hasta = fechaHasta,
                Tag = tag
            };

            var resultado = connection.QueryFirstOrDefault<string>(query, parametros);
            return resultado == "S";
        }
    }
}