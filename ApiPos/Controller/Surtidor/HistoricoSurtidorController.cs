using Data.Endpoint;
using Data.Surtidor;
using Microsoft.AspNetCore.Mvc;

namespace Controller.Surtidor
{
    [ApiController]
    [Route("surtidor/historico")]
    public class HistoricoSurtidorController : ControllerBase
    {
        private readonly SurtidorHistoricoRepository _historicoDespachoRepository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public HistoricoSurtidorController(
            SurtidorHistoricoRepository historicoDespachoRepository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _historicoDespachoRepository = historicoDespachoRepository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetHistoricoSurtidor([FromQuery] string codRecurso, [FromQuery] string surtidor)
        {
            try
            {
                // 400 - Validación de parámetros
                if (string.IsNullOrWhiteSpace(codRecurso) || string.IsNullOrWhiteSpace(surtidor))
                    return BadRequest();

                // 401 - Autenticación fallida
                var usuario = _usuarioRepository.ValidarUsuario(codRecurso);
                if (usuario == null)
                    return Unauthorized();

                // Lógica principal
                var resultado = await _historicoDespachoRepository.ObtenerHistoricoDespachosAsync(surtidor);

                // 500 - Caso especial definido por ti
                if (resultado == null)
                    return StatusCode(405, new { Mensaje = "No se pudo obtener el histórico de surtidor" });

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