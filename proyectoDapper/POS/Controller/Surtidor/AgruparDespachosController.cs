using Microsoft.AspNetCore.Mvc;
using proyectoDapper.Models.Pos;
using System.Threading.Tasks;
using proyectoDapper.POS.Data.Endpoint;
using proyectoDapper.POS.Data.Surtidor;

namespace proyectoDapper.POS.Controller.Surtidor
{
    [ApiController]
    [Route("api/pos/despacho")]
    public class AgruparDespachosController : ControllerBase
    {
        private readonly AgruparDespachosRepository _repository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public AgruparDespachosController(
            AgruparDespachosRepository repository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("agrupar-despachos")]
        public async Task<IActionResult> GetAgruparDespachos(
            [FromQuery] string pSurtidor,
            [FromQuery] string codBarras)
        {
            try
            {
                // 400 - Validación de parámetros
                if (string.IsNullOrWhiteSpace(pSurtidor) ||
                    string.IsNullOrWhiteSpace(codBarras))
                    return BadRequest(new { Mensaje = "Parámetros pSurtidor, pRecurso y codBarras son requeridos" });

                // 401 - Validación de usuario
                var usuario = _usuarioRepository.ValidarUsuario(codBarras);
                if (usuario == null)
                    return Unauthorized();

                // Lógica principal
                var despachos = await _repository.ObtenerDespachosAgrupadosAsync(pSurtidor,usuario.Codigo);

                // 500 - Caso especial si no hay datos
                if (despachos == null || !despachos.Any())
                    return StatusCode(500, new { Mensaje = "No se encontraron despachos" });

                // 200 - Respuesta correcta
                return Ok(despachos);
            }
            catch (Exception ex)
            {
                // 409 - Error general
                return StatusCode(409, new { ex.Message });
            }
        }
    }
}