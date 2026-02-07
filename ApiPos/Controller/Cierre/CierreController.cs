using Data.Cierre;
using Data.Endpoint;
using Microsoft.AspNetCore.Mvc;
using Models.Cierre;

namespace Controller.Cierre
{
    [ApiController]
    [Route("cierre")]
    public class CierreController : ControllerBase
    {
 

        private readonly CierreRepository _cierreRepository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public CierreController(
        CierreRepository cierreRepository,
        ValidaUsuarioRepository usuarioRepository)
        {

            _cierreRepository = cierreRepository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CierreDiario([FromBody] EjecutarCierreRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.codRecurso))
                    return BadRequest(new { Mensaje = "El parámetro cod_recurso es obligatorio." });

                var usuario = _usuarioRepository.ValidarUsuario(request.codRecurso);
                if (usuario == null)
                    return Unauthorized(new { Mensaje = "Usuario no autorizado." });

                // Ejecutar SP
                await _cierreRepository.CierreDiario();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = ex.Message });
            }
        }
    }
}