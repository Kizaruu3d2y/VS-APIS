using Microsoft.AspNetCore.Mvc;
using proyectoDapper.POS.Data.Cliente;
using proyectoDapper.POS.Data.Endpoint;

namespace proyectoDapper.POS.Controller.Cliente
{
    [ApiController]
    [Route("api/pos/cliente")]
    public class ClientePlacaController : ControllerBase
    {
        private readonly PlacaRepository _placaRepository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public ClientePlacaController(
            PlacaRepository placaRepository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _placaRepository = placaRepository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("placa")]
        public async Task<IActionResult> ObtenerPlaca([FromQuery] string codBarras, [FromQuery] string tag)
        {
            try
            {
                // 400 - Validación de parámetros
                if (string.IsNullOrWhiteSpace(codBarras) || string.IsNullOrWhiteSpace(tag))
                    return BadRequest(new { Mensaje = "Parámetros inválidos" });

                // 401 - Validar usuario
                var usuario = _usuarioRepository.ValidarUsuario(codBarras);
                if (usuario == null)
                    return Unauthorized(new { Mensaje = "Usuario no autorizado" });

                // Consulta
                var resultado = await _placaRepository.ObtenerPlacaPorTagAsync(tag);

                if (resultado == null)
                    return NotFound(new { Mensaje = "No se encontró la placa" });

                // 200 - Ok
                return Ok(new { CodCuenta = resultado?.codCuenta, Placa = resultado?.placa });
            }
            catch (Exception ex)
            {
                // 500 - Error general
                return StatusCode(500, new { ex.Message });
            }
        }
    }
}