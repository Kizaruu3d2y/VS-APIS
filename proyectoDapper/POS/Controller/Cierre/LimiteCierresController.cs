using Microsoft.AspNetCore.Mvc;
using proyectoDapper.POS.Data.Cierre;
using proyectoDapper.POS.Data.Endpoint;

namespace proyectoDapper.POS.Controller.Cierre
{
    [ApiController]
    [Route("api/pos/cierre")]
    public class LimiteCierresController : ControllerBase
    {
        private readonly LimiteCierresRepository _limiteCierresRepository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public LimiteCierresController(
            LimiteCierresRepository limiteCierresRepository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _limiteCierresRepository = limiteCierresRepository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("limite")]
        public async Task<IActionResult> VerificarLimite([FromQuery] string codRecurso)
        {
            try
            {
                var usuario = _usuarioRepository.ValidarUsuario(codRecurso);
                if (usuario == null)
                    return Unauthorized();

                var Disponible = await _limiteCierresRepository.VerificarLimiteCierresAsync(usuario.Codigo);

                if (Disponible == null)
                    return StatusCode(405, new { Mensaje = "No se pudo obtener fecha turno o datos" });

                return Ok(new { Disponible });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }
    }
}