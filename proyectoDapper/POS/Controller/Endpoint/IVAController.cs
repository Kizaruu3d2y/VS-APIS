using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using proyectoDapper.POS.Data.Endpoint;

namespace proyectoDapper.POS.Controller.Endpoint
{
    [ApiController]
    [Route("api/pos")]
    public class IVAController : ControllerBase
    {
        private readonly IVARepository _repository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public IVAController(
            IVARepository repository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("iva")]
        public async Task<IActionResult> GetTasaIVA()
        {
            try
            {

                // Lógica principal
                var resultado = await _repository.ObtenerTasaIVAAsync();

                // 500 - Caso especial definido por ti
                if (resultado == null)
                    return StatusCode(402, new { Mensaje = "No se pudo obtener la tasa de IVA" });

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