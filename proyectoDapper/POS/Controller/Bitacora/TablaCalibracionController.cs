using Microsoft.AspNetCore.Mvc;
using proyectoDapper.POS.Data.Bitacora;
using proyectoDapper.POS.Data.Endpoint;

namespace proyectoDapper.POS.Controller.Bitacora
{
    [ApiController]
    [Route("api/pos")]
    public class TablaCalibracionController : ControllerBase
    {
        private readonly CalibracionRepository _repository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public TablaCalibracionController(
            CalibracionRepository repository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("tabla-calibracion")]
        public async Task<IActionResult> GetCalibracion()
        {
            try
            {
                var resultado = await _repository.ObtenerCalibracionAsync();

                if (resultado == null || resultado.Count == 0)
                    return StatusCode(405, new { Mensaje = "No se encontraron resultados"});

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor", ex.Message });
            }
        }
    }
}