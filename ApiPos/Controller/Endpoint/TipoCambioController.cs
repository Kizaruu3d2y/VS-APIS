using Data.Endpoint;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Controller.Endpoint
{
    [ApiController]
    public class TipoCambioController : ControllerBase
    {
        private readonly TipoCambioRepository _repository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public TipoCambioController(
            TipoCambioRepository repository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("tipo-cambio")]
        public async Task<IActionResult> GetTipoCambio()
        {
            try
            {
                // Lógica principal
                var resultado = await _repository.ObtenerTipoCambioAsync();

                // 500 - Caso especial definido por ti
                if (resultado == null)
                    return StatusCode(405, new { Mensaje = "No se pudo obtener el tipo de cambio" });

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