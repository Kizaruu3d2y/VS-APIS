using Microsoft.AspNetCore.Mvc;
using proyectoDapper.POS.Data.Endpoint;
using proyectoDapper.POS.Data.Surtidor;

namespace proyectoDapper.POS.Controller.Surtidor
{
    [ApiController]
    [Route("api/pos")]
    public class FacturaSurtidorController : ControllerBase
    {
        private readonly LecturaRepository _lecturaRepository;
        private readonly SurtidorFacturaRepository _repository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public FacturaSurtidorController(
            LecturaRepository lecturaRepository,
            SurtidorFacturaRepository repository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _lecturaRepository = lecturaRepository;
            _repository = repository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("surtidor/factura")]
        public async Task<IActionResult> GetFactura(
            [FromQuery] string codRecurso,
            [FromQuery] string surtidor)
        {
            try
            {
                // -------------------------
                // 400 - Parámetros
                // -------------------------
                if (string.IsNullOrWhiteSpace(codRecurso) ||
                    string.IsNullOrWhiteSpace(surtidor))
                {
                    return BadRequest(new { Mensaje = "Parámetros requeridos" });
                }

                // -------------------------
                // 401 - Usuario
                // -------------------------
                var usuario = _usuarioRepository.ValidarUsuario(codRecurso);
                if (usuario == null)
                    return Unauthorized(new { Mensaje = "Usuario no autorizado" });

                // -------------------------
                // Lógica principal
                // -------------------------
                var factura = await _repository
                    .ObtenerFacturaSurtidorAsync(codRecurso, surtidor);

                // -------------------------
                // 204 - No hay factura
                // -------------------------
                if (factura == null)
                    return NoContent();

                // -------------------------
                // 200 - OK
                // -------------------------
                return Ok(factura);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Mensaje = "Error interno",
                    Detalle = ex.Message
                });
            }
        }
    }
}