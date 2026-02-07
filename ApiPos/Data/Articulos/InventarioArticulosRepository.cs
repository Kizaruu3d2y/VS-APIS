using Data.Endpoint;
using Models.Articulos;
using Models.Endpoint;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Linq;


namespace Data.Articulos
{
    public class InventarioArticulosRepository
    {
        private readonly string _connectionString;
        private readonly string _empresa;
        private readonly FechaTurnoRepository _fechaTurnoRepository;

        public InventarioArticulosRepository(IConfiguration configuration, FechaTurnoRepository fechaTurnoRepository)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
            
            _fechaTurnoRepository = fechaTurnoRepository;
        }

        public async Task<AceitesResponse> RegistrarAceites(InventarioArticulosRequestDto request)
        {
            var response = new AceitesResponse();

            try
            {
                using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync();

                // Obtener fecha turno async con parámetros empresa y recursoPistero
                var fechaTurno = await _fechaTurnoRepository.ObtenerFechaTurnoAsync();
                using var command = new OracleCommand("pos_util.registra_aceites", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Parámetros simples
                command.Parameters.Add("pEmpresa", OracleDbType.Varchar2).Value = EmpresaGlobalDto.Empresa;
                command.Parameters.Add("pTipo", OracleDbType.Varchar2).Value = request.registro;
                command.Parameters.Add("pTurno", OracleDbType.Date).Value = fechaTurno.Fecha;

                // ARRAY: listaArticulos
                var listaParam = command.Parameters.Add("listaArticulos", OracleDbType.Varchar2);
                listaParam.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                listaParam.Value = request.idArticulos.ToArray();
                listaParam.Size = request.idArticulos.Length;

                // ARRAY: cantidadArticulos
                var cantParam = command.Parameters.Add("cantidadArticulos", OracleDbType.Decimal);
                cantParam.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                cantParam.Value = request.cantidades.ToArray();
                cantParam.Size = request.cantidades.Length;

                // OUTPUT
                command.Parameters.Add("pMsg_Error", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;

                await command.ExecuteNonQueryAsync();

                var mensaje = command.Parameters["pMsg_Error"].Value?.ToString();

                if (string.IsNullOrWhiteSpace(mensaje))
                    response.Mensaje = "";
                else
                    response.Mensaje = mensaje;

            }
            catch (Exception ex)
            {
                response.Resultado = false;
                response.Mensaje = ex.Message;
            }

            return response;
        }
    }
}
