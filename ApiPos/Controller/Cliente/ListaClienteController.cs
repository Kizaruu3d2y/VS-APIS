using Data.Cliente;
using Data.Endpoint;
using Microsoft.AspNetCore.Mvc;

namespace Controller.Cliente
{
    [ApiController]
    [Route("cliente")]
    public class ListaClienteController : ControllerBase
    {
        private readonly ListaClienteRepository _repository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public ListaClienteController(
            ListaClienteRepository repository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("lista")]
        public async Task<IActionResult> GetClientes([FromQuery] string subText)
        {
            try
            {

                // Lógica principal
                var resultado = await _repository.CargarClientesAsync(subText);

                // 404 - No se encontraron resultados
                if (resultado == null || !resultado.Any())
                    return NotFound();

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