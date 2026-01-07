using Microsoft.AspNetCore.Mvc;
using proyectoDapper.POS.Data.Endpoint;
using proyectoDapper.POS.Data.Surtidor;
using proyectoDapper.POS.Models.Surtidor;
using System.Threading.Tasks;

namespace proyectoDapper.POS.Controller.Surtidor
{
    [ApiController]
    [Route("api/pos/surtidor/autoriza")]
    public class SurtidorAutorizaController : ControllerBase
    {
        private readonly SurtidorAutorizaRepository _repository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public SurtidorAutorizaController(
            SurtidorAutorizaRepository repository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Autorizar([FromBody] SurtidorAutorizaDto request)
        {
            try
            {
                // 400 - Validación de parámetros
                if (string.IsNullOrWhiteSpace(request.codRecurso) || string.IsNullOrWhiteSpace(request.surtidor))
                    return BadRequest();

                // 401 - Autenticación fallida
                var usuario = _usuarioRepository.ValidarUsuario(request.codRecurso);
                if (usuario == null)
                    return Unauthorized();

                // Lógica principal
                var ok = await _repository.AutorizarSurtidorAsync(request.codRecurso, request.surtidor);

                if (!ok)
                    return StatusCode(402, new { Mensaje = "No se pudo autorizar el surtidor." });

                // 200 - Respuesta correcta
                return Ok();
            }
            catch (Exception ex)
            {
                // 409 - Error general
                return StatusCode(409, new { ex.Message });
            }
        }
    }
}