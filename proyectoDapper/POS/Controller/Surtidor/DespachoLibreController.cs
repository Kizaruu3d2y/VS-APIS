using Microsoft.AspNetCore.Mvc;
using proyectoDapper.POS.Data.Endpoint;
using proyectoDapper.POS.Data.Surtidor;

namespace proyectoDapper.POS.Controller.Surtidor
{
    [ApiController]
    [Route("api/pos/despacho")]
    public class DespachoLibreController : ControllerBase
    {
        private readonly DespachoLibreRepository _repository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public DespachoLibreController(
            DespachoLibreRepository repository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("despacho-libre")]
        public async Task<IActionResult> GetDespachoLibre([FromQuery] string lectura, [FromQuery] string codBarras)
        {
            try
            {
                // 400 - Validación de parámetros
                if (string.IsNullOrWhiteSpace(lectura) || string.IsNullOrWhiteSpace(codBarras))
                    return BadRequest(new { Mensaje = "Parámetros lectura y codBarras son requeridos" });

                // 401 - Autenticación fallida
                var usuario = _usuarioRepository.ValidarUsuario(codBarras);
                if (usuario == null)
                    return Unauthorized();

                // Lógica principal
                var libre = await _repository.VerificarDespachoLibreAsync(lectura);

                // 500 - Si no se obtuvo resultado
                if (libre == null)
                    return StatusCode(500);

                // 200 - Respuesta correcta
                return Ok(new { DespachoLibre = libre });
            }
            catch (Exception ex)
            {
                // 409 - Error general
                return StatusCode(409, new { ex.Message });
            }
        }
    }
}