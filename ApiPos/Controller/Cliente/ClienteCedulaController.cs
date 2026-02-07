using Data.Cliente;
using Data.Endpoint;
using Microsoft.AspNetCore.Mvc;

namespace Controller.Cliente
{
    [ApiController]
    [Route("cliente")]
    public class ClienteCedulaController : ControllerBase
    {
        private readonly ConsultaClienteRepository _clienteRepository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public ClienteCedulaController(
            ConsultaClienteRepository clienteRepository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _clienteRepository = clienteRepository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("cedula")]
        public async Task<IActionResult> ObtenerCodigoPorCedula([FromQuery] string codBarras, [FromQuery] string tipoId, [FromQuery] string cedula)
        {
            try
            {
                // 400 - Validación de parámetros
                if (string.IsNullOrWhiteSpace(codBarras) ||
                    string.IsNullOrWhiteSpace(tipoId) ||
                    string.IsNullOrWhiteSpace(cedula))
                    return BadRequest(new { Mensaje = "Parámetros inválidos" });

                // 401 - Validar usuario
                var usuario = _usuarioRepository.ValidarUsuario(codBarras);
                if (usuario == null)
                    return Unauthorized(new { Mensaje = "Usuario no autorizado" });

                // Consulta
                var codigo = await _clienteRepository.ObtenerCodigoPorCedulaAsync(tipoId, cedula);

                if (string.IsNullOrEmpty(codigo))
                    return NotFound(new { Mensaje = "No se encontró el cliente" });

                // 200 - Ok
                return Ok(new { Codigo = codigo });
            }
            catch (Exception ex)
            {
                // 500 - Error general
                return StatusCode(500, new { ex.Message });
            }
        }
    }
}