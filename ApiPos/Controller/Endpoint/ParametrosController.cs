using Data.Endpoint;
using Microsoft.AspNetCore.Mvc;

namespace Controller.Endpoint
{
    [ApiController]
    [Route("parametros")]
    public class ParametrosController : ControllerBase
    {
        private readonly ParametrosRepository _repository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public ParametrosController(
            ParametrosRepository repository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet]
        public IActionResult GetParametros()
        {
            try
            {
                // Lógica principal
                var resultado = _repository.ObtenerParametrosAsync().Result;

                // 500 - Caso especial definido por ti
                if (resultado == null)
                    return StatusCode(402, new { Mensaje = "No se encontraron parámetros" });

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