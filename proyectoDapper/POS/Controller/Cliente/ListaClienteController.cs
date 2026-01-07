using Microsoft.AspNetCore.Mvc;
using proyectoDapper.Models.Pos;
using proyectoDapper.POS.Data.Cliente;
using proyectoDapper.POS.Data.Endpoint;

namespace proyectoDapper.POS.Controller.Cliente
{
    [ApiController]
    [Route("api/pos/cliente")]
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