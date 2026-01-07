using Microsoft.AspNetCore.Mvc;
using proyectoDapper.POS.Data.Cliente;
using proyectoDapper.POS.Data.Endpoint;

namespace proyectoDapper.POS.Controller.Cliente
{
    [ApiController]
    [Route("api/pos/cliente")]
    public class CodigoClienteController : ControllerBase
    {
        private readonly CedulaRepository _cedulaRepository;
        private readonly ClienteRepository _clienteRepository;
        private readonly SiguienteCodigoRepository _siguienteCodigoRepository;
        private readonly ValidaUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;

        public CodigoClienteController(
            CedulaRepository cedulaRepository,
            ClienteRepository clienteRepository,
            SiguienteCodigoRepository siguienteCodigoRepository,
            ValidaUsuarioRepository usuarioRepository,
            IConfiguration configuration)
        {
            _cedulaRepository = cedulaRepository;
            _clienteRepository = clienteRepository;
            _siguienteCodigoRepository = siguienteCodigoRepository;
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
        }

         //--------------------------------------------------------------------
         //GET api/pos/cliente/codigo? tagUsuario = X & tipoCedula = X & numeroCedula = X
         //--------------------------------------------------------------------
        [HttpGet("codigo")]
        public async Task<IActionResult> GetCodigoCliente([FromQuery] string tipoCedula, [FromQuery] string numeroCedula) 
        {
            try
            {
                // 400 - Validación de parámetros
                if (string.IsNullOrEmpty(tipoCedula) ||
                    string.IsNullOrEmpty(numeroCedula))
                {
                    return BadRequest(new { Mensaje = "Faltan parámetros requeridos." });
                }
                // --------------------------------------------------------------------
                // 1. Validar si la cédula existe
                // --------------------------------------------------------------------
                 var existe = await _cedulaRepository.ValidarCedulaExisteAsync(tipoCedula, numeroCedula);

                if (existe != null)
                {

                    // EXISTE → retornar datos de BD
                    return Ok(new
                    {
                        existe.CodCliente,
                        existe.Nombre,
                        Existe = true
                    });
                }

                // --------------------------------------------------------------------
                // 2. NO existe → obtener nombre desde API hacienda
                // --------------------------------------------------------------------
                var nombre = await _clienteRepository.ObtenerNombreClienteAsync(tipoCedula, numeroCedula);

                if (string.IsNullOrWhiteSpace(nombre))
                    nombre = "SIN NOMBRE"; // fallback mínimo

                // --------------------------------------------------------------------
                // 3. Obtener siguiente código para crear cliente
                // --------------------------------------------------------------------
                var nuevoCodigo = await _siguienteCodigoRepository.ObtenerSiguienteCodigoAsync();

                if (string.IsNullOrEmpty(nuevoCodigo))
                    return StatusCode(500, new { Mensaje = "No se pudo obtener el siguiente código." });

                // --------------------------------------------------------------------
                // Respuesta final
                // --------------------------------------------------------------------
                return Ok(new
                {
                    CodCliente = nuevoCodigo,
                    Nombre = nombre,
                    Existe = false
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }
    }
}