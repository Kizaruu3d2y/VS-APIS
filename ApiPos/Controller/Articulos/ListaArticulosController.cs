using Data.Articulos;
using Data.Endpoint;
using Microsoft.AspNetCore.Mvc;

namespace Controller.Articulos
{
    [ApiController]
    public class ListaArticulosController : ControllerBase
    {
        private readonly ListaArticulosRepository _repository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public ListaArticulosController(
            ListaArticulosRepository repository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("articulos/lista")]
        public async Task<IActionResult> GetListaArticulos()
        {
            try
            {
                var resultado = await _repository.ObtenerListaArticulosAsync();

                if (resultado == null)
                    return StatusCode(405, new { Mensaje = "No se pudo obtener la lista de artículos" });

                // ✔️ Devolvemos directamente el array
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(409, new { ex.Message });
            }
        }
    }
}