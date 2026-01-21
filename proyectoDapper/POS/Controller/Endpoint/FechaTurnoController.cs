using Microsoft.AspNetCore.Mvc;
using proyectoDapper.POS.Data.Endpoint;

namespace proyectoDapper.POS.Controller.Endpoint
{
    [ApiController]
    [Route("api/pos")]
    public class FechaTurnoController : ControllerBase
    {
        private readonly FechaTurnoRepository _fechaTurnoRepository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public FechaTurnoController(
            FechaTurnoRepository fechaTurnoRepository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _fechaTurnoRepository = fechaTurnoRepository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("fecha-turno")]
        public async Task<IActionResult> ObtenerFechaTurno()
        {
            try
            {
                // Obtener fecha turno
                var fechaTurno = await _fechaTurnoRepository.ObtenerFechaTurnoAsync();

                // 200 - Respuesta correcta
                if (fechaTurno == null)
                    return StatusCode(402, new { Mensaje = "No se pudo obtener la fecha" });
                else
                    return Ok(fechaTurno.Fecha.ToString("yyyy-MM-dd"));
            }
            catch (Exception ex)
            {
                // 500 - Error general en BD u otro
                return StatusCode(500, new {ex.Message});
            }
        }
    }
}