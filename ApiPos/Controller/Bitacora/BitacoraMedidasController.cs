using Data.Bitacora;
using Data.Endpoint;
using Microsoft.AspNetCore.Mvc;
using Models.Bitacora;

namespace Controller.Bitacora
{
    [ApiController]
    [Route("bitacora")]
    public class TanqueController : ControllerBase
    {
        private readonly IngresarMedidaTanqueRepository _repository;
        private readonly ValidaUsuarioRepository _usuarioRepository;
        private readonly FechaTurnoRepository _fechaTurnoRepository;

        public TanqueController(
            IngresarMedidaTanqueRepository repository,
            ValidaUsuarioRepository usuarioRepository,
            FechaTurnoRepository fechaTurnoRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
            _fechaTurnoRepository = fechaTurnoRepository;
        }

        [HttpPut("medidas")]
        public async Task<IActionResult> Put([FromBody] MedidaTanqueDto request)
        {
            try
            {
                // 400 - Validación de parámetros
                if (string.IsNullOrEmpty(request.codRecurso) ||
                    request == null ||
                    request.cantSuper < 0 ||
                    request.cantPlus91 < 0 ||
                    request.cantDiesel < 0)
                    return BadRequest();

                // 401 - Validación de usuario
                var usuario = _usuarioRepository.ValidarUsuario(request.codRecurso);
                if (usuario == null)
                    return Unauthorized();

                // Lógica principal
                var resultado = await _repository.GuardarMedidaTanqueAsync(
                    recurso: usuario.Codigo,
                    superComb: request.cantSuper,
                    plus91: request.cantPlus91,
                    diesel: request.cantDiesel,
                    fechaTurno: request.fechaCorte
                );

                if (!resultado)
                    return StatusCode(405, new { mensaje = "No se pudo guardar la medida" });

                // 200 - Respuesta correcta
                return Ok();
            }
            catch (Exception ex)
            {
                // 500 - Error general
                return StatusCode(500, new { ex.Message });
            }
        }
    }
}