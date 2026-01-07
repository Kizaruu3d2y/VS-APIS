using Microsoft.AspNetCore.Mvc;
using proyectoDapper.POS.Data.Endpoint;
using proyectoDapper.POS.Data.Surtidor;
using System.Threading.Tasks;

namespace proyectoDapper.POS.Controller.Surtidor
{
    [ApiController]
    [Route("api/pos/surtidor")]
    public class ListadoSurtidorController : ControllerBase
    {
        private readonly ListadoSurtidorRepository _repository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public ListadoSurtidorController(
            ListadoSurtidorRepository repository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("listado")]
        public async Task<IActionResult> GetListado()
        {
            try
            {
                // Lógica principal
                var resultado = await _repository.ObtenerListadoSurtidoresAsync();

                // 500 - Caso especial definido por ti
                if (resultado == null)
                    return StatusCode(405, new { Mensaje = "No se pudo obtener el listado de surtidores" });

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