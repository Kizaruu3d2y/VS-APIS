using Microsoft.AspNetCore.Mvc;
using proyectoDapper.POS.Data.Cierre;
using proyectoDapper.POS.Data.Endpoint;
using System;

namespace proyectoDapper.POS.Controller.Cierre
{
    [ApiController]
    [Route("api/pos/cierre")]
    public class VersionController : ControllerBase
    {
        private readonly LimiteCierresRepository _limiteRepository;
        private readonly ValidaUsuarioRepository _usuarioRepository;
        private readonly FechaTurnoRepository _fechaTurnoRepository;

        public VersionController(
                      LimiteCierresRepository limiteRepository,
            ValidaUsuarioRepository usuarioRepository,
            FechaTurnoRepository fechaTurnoRepository)
        {
            _limiteRepository = limiteRepository;
            _usuarioRepository = usuarioRepository;
            _fechaTurnoRepository = fechaTurnoRepository;
        }

        [HttpPost("version")]
        public async Task<IActionResult> PostCierreResumenAsync([FromQuery] string tag)
        {
            try
            {
                // 400 - Validación de parámetros
                if (string.IsNullOrWhiteSpace(tag))
                    return BadRequest();

                // 401 - Autenticación fallida
                var usuario = _usuarioRepository.ValidarUsuario(tag);
                if (usuario == null)
                    return Unauthorized();

                // Obtener fecha turno
                var fechaTurno = await _fechaTurnoRepository.ObtenerFechaTurnoAsync();
                if (fechaTurno == null)
                    return StatusCode(500, new { Mensaje = "No se pudo obtener la fecha turno" });

                // Lógica principal: update/insert/select versión
                var version = _limiteRepository.VerificarLimiteCierresAsync(usuario.Codigo);

                // 200 - Respuesta correcta
                return Ok(version);

            }
            catch (Exception ex)
            {
                // 409 - Error general
                return StatusCode(409, new { ex.Message });
            }
        }
    }
}