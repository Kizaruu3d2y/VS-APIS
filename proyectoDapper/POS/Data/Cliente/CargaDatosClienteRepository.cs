using Dapper;
using Oracle.ManagedDataAccess.Client;
using proyectoDapper.Models.Pos;
using proyectoDapper.POS.Models.Surtidor.Cliente;
using System.Data;
using System.Threading.Tasks;
using static proyectoDapper.POS.Models.Surtidor.Cliente.ClienteDatosRequest;

namespace proyectoDapper.POS.Data.Cliente
{
    public class CargaDatosClienteRepository
    {
        private readonly IConfiguration _configuration;

        private readonly string _connectionString;
        public CargaDatosClienteRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        /// <summary>
        /// Consulta los datos del cliente, placa y parámetros.
        /// </summary>
        public ObtenerDatosResponseDto ObtenerDatosCliente(string? tagCliente, string? codCliente)
        {
            using var connection = new OracleConnection(_connectionString);

            connection.Open();

            bool tagUser = false;
            string? placa = null;
            string clienteFinal = codCliente ?? string.Empty;

            // --------------------------------------------------------------------
            // 1. Si viene tagCliente → activar tagUser y consultar placa y codCliente
            // --------------------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(tagCliente))
            {
                tagUser = true;

                const string queryPlaca = @"
                    select cod_cuenta, placa
                    from sav_placa
                    where empresa = :empresa
                    and tag = :tag";

                var datosPlaca = connection.QueryFirstOrDefault<ClienteResponseDto>(
                    queryPlaca,
                    new
                    {
                        empresa = EmpresaGlobalDto.Empresa,
                        tag = tagCliente
                    });

                if (datosPlaca != null)
                {
                    placa = datosPlaca.PLACA;
                    clienteFinal = datosPlaca.COD_CUENTA;  // usar codCliente del tagCliente
                }
            }

            // Si NO vino tagCliente → tagUser = false
            // placa se queda null
            // clienteFinal = codCliente enviado en el API

            // --------------------------------------------------------------------
            // 2. Obtener información principal del cliente
            // --------------------------------------------------------------------
            const string queryCliente = @"
               SELECT cli.nombre,
                      cli.alias AS alias,
                      NVL(cue.plazo, 0) AS plazo,
                      cli.correoco,
                      NVL(cor.correocr, cli.correoco) AS correocr,
                      fac.cod_actividad actividadEco
                FROM fac_clientes_actividad fac
                JOIN (
                       SELECT f.empresa,  f.cod_cliente AS codigo,  
                              e.nombre_entidad AS nombre,
                              e.alias,e.e_mail1 AS correoco
                         FROM fac_clientes f
                         JOIN cor_entidades e
                           ON e.empresa = corp_util.Empresa_para_Entidades(f.empresa)
                          AND f.cod_cliente = e.cod_entidad
                        WHERE f.ind_activo = 'A'
                          AND f.empresa = :empresa
                          AND e.cod_entidad = :codCliente ) cli
                          ON fac.empresa     = cli.empresa
                         AND fac.cod_cliente = cli.codigo
                         AND fac.ind_principal = 'S'
                        LEFT JOIN (
                                    SELECT cu.empresa,
                                           cu.cod_entidad AS codigo,
                                           cu.plazo
                                    FROM cxc_cuentas cu
                                    WHERE cu.suspendido = 'N'
                                  ) cue
                          ON cli.empresa = cue.empresa
                         AND cli.codigo = cue.codigo
                        LEFT JOIN (
                                    SELECT co.empresa,
                                           co.cod_cuenta AS codigo,
                                           MIN(co.e_mail) AS correocr
                                    FROM cxc_cuentas_correos co
                                    WHERE co.envio_fac = 'S'
                                    GROUP BY co.empresa, co.cod_cuenta
                                  ) cor
                          ON cli.empresa = cor.empresa
                         AND cli.codigo = cor.codigo";

            var datosCliente = connection.QueryFirstOrDefault<ClienteDatosResultDto>(
                queryCliente,
                new
                {
                    empresa = EmpresaGlobalDto.Empresa,
                    codCliente = clienteFinal
                });

            if (datosCliente == null)
                return null;

            // --------------------------------------------------------------------
            // 3. Consultar parámetros del cliente (placa, correo, km, OC, mult)
            // --------------------------------------------------------------------
            const string queryParametros = @"
                select *
                  from (select cod_parametro
                          from cor_valores_parametros
                         where cod_parametro in ('VERIF_PLAC', 'CORREO_F', 'VERIF_KM', 'VERIF_OC', 'DESP_MULT')
                           and empresa = :empresa
                           and valor1 = :codCliente)
                  pivot (
                      count(cod_parametro)
                      for cod_parametro in ('VERIF_PLAC', 'CORREO_F', 'VERIF_KM', 'VERIF_OC', 'DESP_MULT')
                  )";

            var parametros = connection.QueryFirstOrDefault<ParametrosResultDto>(
                queryParametros,
                new
                {
                    empresa = EmpresaGlobalDto.Empresa,
                    codCliente = clienteFinal
                });

            return new ObtenerDatosResponseDto
            {
                Codigo = clienteFinal,
                Nombre = datosCliente.NOMBRE,
                Alias = datosCliente.ALIAS,
                Credito = datosCliente.PLAZO,
                FactCredito = datosCliente.PLAZO > 0,
                CorreoCo = datosCliente.CORREOCO,
                CorreoCr = datosCliente.CORREOCR,
                Placa = placa,
                ActividadEco = datosCliente.ACTIVIDADECO,
                TagUser = tagUser,    
                RestPlaca = parametros?.VERIF_PLAC == 1,
                RestCorreo = parametros?.CORREO_F == 1,
                RestKm = parametros?.VERIF_KM == 1,
                RestOc = parametros?.VERIF_OC == 1,
                DespMult = parametros?.DESP_MULT == 1

            };

        }

    }

}
