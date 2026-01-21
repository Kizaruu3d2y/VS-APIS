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
        private readonly FechaTurnoRepository _fechaTurnoRepository;

        public LimiteCierresController(
            LimiteCierresRepository limiteCierresRepository,
            ValidaUsuarioRepository usuarioRepository,
                    FechaTurnoRepository fechaTurnoRepository)
        {
            _limiteCierresRepository = limiteCierresRepository;
            _usuarioRepository = usuarioRepository;
            _fechaTurnoRepository = fechaTurnoRepository;
        }

        [HttpGet("limite")]
        public async Task<IActionResult> VerificarLimite([FromQuery] string codRecurso)
        {
            try
            {
                var usuario = _usuarioRepository.ValidarUsuario(codRecurso);
                if (usuario == null)
                    return Unauthorized();
                var fechaTurno = await _fechaTurnoRepository.ObtenerFechaTurnoAsync();
                if (fechaTurno == null)
                    return StatusCode(500, new { Mensaje = "No se pudo obtener la fecha turno" });

                var Disponible = await _limiteCierresRepository.LimiteCierresAsync(usuario.Codigo);

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