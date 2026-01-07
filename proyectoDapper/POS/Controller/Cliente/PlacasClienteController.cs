using Microsoft.AspNetCore.Mvc;
using proyectoDapper.POS.Data.Cliente;
using proyectoDapper.POS.Data.Endpoint;

namespace proyectoDapper.POS.Controller.Cliente
{
    [ApiController]
    [Route("api/pos/cliente")]
    public class PlacasClienteController : ControllerBase
    {
        private readonly CargaPlacasClienteRepository _repository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public PlacasClienteController(
            CargaPlacasClienteRepository repository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("placas")]
        public async Task<IActionResult> GetPlacas([FromQuery] string codCliente)
        {
            try
            {
                // 400 - Validación de parámetros
                if (string.IsNullOrEmpty(codCliente))
                    return BadRequest();

                // Lógica principal
                var resultado = await _repository.ObtenerPlacasClienteAsync(codCliente);

                if (resultado == null || !resultado.Any())
                    return StatusCode(405, "No se encontraron datos");

                // 200 - Respuesta correcta
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                // 500 - Error general
                return StatusCode(500, new { ex.Message });
            }
        }
    }
}