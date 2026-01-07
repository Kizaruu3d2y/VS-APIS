using Microsoft.AspNetCore.Mvc;
using proyectoDapper.POS.Data.Cliente;
using proyectoDapper.POS.Data.Endpoint;
using static proyectoDapper.POS.Models.Surtidor.Cliente.ClienteDatosRequest;

namespace proyectoDapper.POS.Controller.Cliente
{
    [ApiController]
    [Route("api/pos/cliente")]
    public class DatosClienteController : ControllerBase
    {
        private readonly CargaDatosClienteRepository _repository;

        public DatosClienteController(
            CargaDatosClienteRepository repository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _repository = repository;
        }


        [HttpGet("datos")]
        public IActionResult GetDatosCliente([FromQuery] string codCliente, [FromQuery] string tag)
        {
            try
            {
                if (string.IsNullOrEmpty(tag) && string.IsNullOrEmpty(codCliente))
                    return BadRequest();

                var resultado = _repository.ObtenerDatosCliente(tag, codCliente);

                if (resultado == null)
                    return NotFound();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }
    }
}