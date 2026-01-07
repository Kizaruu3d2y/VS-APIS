using Microsoft.AspNetCore.Mvc;
using proyectoDapper.POS.Data.Bitacora;
using proyectoDapper.POS.Data.Endpoint;
using proyectoDapper.POS.Models.Bitacora;

namespace proyectoDapper.POS.Controller.Bitacora
{
    [Route("api/pos/surtidor")]
    [ApiController]
    public class CalibraSurtidor : ControllerBase
    {
        private readonly ValidaUsuarioRepository _usuarioRepository;
        private readonly IngresaCalibracionRepository _calibracionRepository;

        public CalibraSurtidor(
            ValidaUsuarioRepository usuarioRepository,
            IngresaCalibracionRepository calibracionRepository)
        {
            _usuarioRepository = usuarioRepository;
            _calibracionRepository = calibracionRepository;
        }

        [HttpPost("calibra")]
        public async Task<IActionResult> RegistrarCalibracion([FromBody] CalibracionRequestDto request)
        {
            try
            {
                bool resultado = await _calibracionRepository.RegistrarCalibracionAsync(request.Lectura);

                // 405 - Error del SP
                if (resultado)
                    return StatusCode(405, new { mensaje = resultado });

                // 200 - OK
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }
    }
}