using Microsoft.AspNetCore.Mvc;
using proyectoDapper.POS.Models.Surtidor;
using proyectoDapper.POS.Data.Endpoint;
using proyectoDapper.POS.Data.Surtidor;

namespace proyectoDapper.POS.Controller.Surtidor
{
    [ApiController]
    [Route("api/pos/surtidor")]
    public class PosController : ControllerBase
    {
        private readonly LecturaRepository _lecturaRepository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public PosController(
            LecturaRepository lecturaRepository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _lecturaRepository = lecturaRepository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("lectura")]
        public async Task<IActionResult> GetLectura([FromQuery] string tag, [FromQuery] string surtidor)
        {
            try
            {
                // 400 - Validación de parámetros
                if (string.IsNullOrWhiteSpace(tag) || string.IsNullOrWhiteSpace(surtidor))
                    return BadRequest();

                // 401 - Autenticación fallida
                var usuario =  _usuarioRepository.ValidarUsuario(tag);
                if (usuario == null)
                    return Unauthorized();

                // Lógica principal
                LecturaDto resultado = await _lecturaRepository.ObtenerLecturaAsync(surtidor);

                // 500 - Caso especial si no devuelve lectura
                if (resultado == null)
                    return StatusCode(500, new { Mensaje = "No se encontró lectura válida" });

                // 200 - Respuesta correcta
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                // 409 - Error general
                return StatusCode(409, new { ex.Message });
            }
        }
    }
}