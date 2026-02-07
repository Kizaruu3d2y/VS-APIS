using Microsoft.AspNetCore.Mvc;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Endpoint;

namespace Controller.Endpoint
{
    [ApiController]
    [Route("pisteros")]
    public class PisteroController : ControllerBase
    {
        private readonly PisteroRepository _repository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public PisteroController(
            PisteroRepository repository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetPisteros()
        {
            try
            {
                // Lógica principal
                var resultado = await _repository.ObtenerPisterosAsync();

                // 500 - Caso especial definido por ti
                if (resultado == null)
                    return StatusCode(402, new { Mensaje = "No se pudo obtener la lista de pisteros" });

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