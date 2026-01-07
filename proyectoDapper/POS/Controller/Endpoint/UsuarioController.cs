using Microsoft.AspNetCore.Mvc;
using proyectoDapper.POS.Data.Endpoint;
using System.Threading.Tasks;

namespace proyectoDapper.POS.Controller.Endpoint
{
    [ApiController]
    [Route("api/pos")]
    public class UsuarioController : ControllerBase
    {
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public UsuarioController(ValidaUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("consulta-usuario")]
        public async Task<IActionResult> GetUsuario([FromQuery] string tag)
        {
            try
            {
                // 400 - Validación de parámetros
                if (string.IsNullOrWhiteSpace(tag))
                    return BadRequest();

                // Lógica principal
                var resultado =  _usuarioRepository.ValidarUsuario(tag);

                // 401 - Autenticación fallida
                if (resultado == null)
                    return Unauthorized();

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