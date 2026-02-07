using Data.Endpoint;
using Data.Surtidor;
using Microsoft.AspNetCore.Mvc;
using Models.Surtidor;

namespace Controller.Endpoint
{
    [ApiController]
    [Route("factura")]
    public class FacturaController : ControllerBase
    {
        private readonly FacturaRepository _facturaRepository;
        private readonly ValidaUsuarioRepository _usuarioRepository;
        private readonly DespachoLibreRepository _despachoLibreRepository;

        public FacturaController(
            FacturaRepository facturaRepository,
            ValidaUsuarioRepository usuarioRepository,
            DespachoLibreRepository despachoLibreRepository)
        {
            _facturaRepository = facturaRepository;
            _usuarioRepository = usuarioRepository;
            _despachoLibreRepository = despachoLibreRepository;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarFactura([FromBody] FacturaRequestDto request)
        {
            try { 
            if (request == null)
                return BadRequest("Request inválido");

            if (string.IsNullOrWhiteSpace(request.codRecurso))
                return BadRequest("codRecurso es requerido");

            var usuario = _usuarioRepository.ValidarUsuario(request.codRecurso);
            if (usuario == null)
                return Unauthorized("Usuario no válido");

            if (!string.IsNullOrWhiteSpace(request.lecturaSurtidor))
            {
                var libre = await _despachoLibreRepository
                    .VerificarDespachoLibreAsync(request.lecturaSurtidor);

                if (!libre)
                    return StatusCode(405, new { mensaje = "La factura ya fue generada automáticamente" });
            }

            var resultado = await _facturaRepository.RegistrarFacturaAsync(request);

            // 🔥 DEBUG ORACLE
            if (string.IsNullOrWhiteSpace(resultado.MensajeError))
            {
                return BadRequest(new
                {
                    oracleError = resultado.MensajeError,
                    requestRecibido = request
                });
            }

            return Ok(new
            {
                consecutivo = resultado.Consecutivo,
                cupon = resultado.Cupon
            });
        }
            catch (Exception ex)
            {
                // Error técnico (no de negocio)
                return StatusCode(500, new
                {
                    mensaje = "Error interno al registrar la factura",
                    detalle = ex.Message
                });
            }
        }
    }
}