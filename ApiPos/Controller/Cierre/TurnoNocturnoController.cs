using Data.Cierre;
using Data.Endpoint;
using Microsoft.AspNetCore.Mvc;
using Models.Cierre;

namespace Controller.Cierre
{
    [ApiController]
    [Route("cierre")]
    public class TurnoNocturnoController : ControllerBase
    {
        private readonly TurnoNocturnoRepository _repository;
        private readonly ValidaUsuarioRepository _usuarioRepository;
        private readonly FechaTurnoRepository _fechaTurnoRepository;

        public TurnoNocturnoController(
            TurnoNocturnoRepository repository,
            ValidaUsuarioRepository usuarioRepository,
            FechaTurnoRepository fechaTurnoRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
            _fechaTurnoRepository = fechaTurnoRepository;
        }

        [HttpGet("turno-nocturno")]
        public IActionResult GetTurnoNocturno([FromQuery] string tag)
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
                var fechaTurno = _fechaTurnoRepository.ObtenerFechaTurnoAsync().Result;
                if (fechaTurno == null)
                    return StatusCode(500, new { Mensaje = "No se pudo obtener la fecha turno" });

                // Lógica principal
                var resultado = _repository.VerificarTurnoNocturno(tag, fechaTurno.Fecha);

                // 200 - Respuesta correcta
                //  return Ok(new TurnoNocturno { Respuesta = resultado });
                return Ok(new TurnoNocturnoDto { Respuesta = resultado });
            }
            catch (Exception ex)
            {
                // 409 - Error general
                return StatusCode(409, new { ex.Message });
            }
        }
    }
}