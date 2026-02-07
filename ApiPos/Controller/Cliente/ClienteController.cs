using Data.Cliente;
using Data.Endpoint;
using Microsoft.AspNetCore.Mvc;
using Models.Cliente;

namespace Controller.Cliente
{
    [ApiController]
    [Route("cliente")]
    public class ClienteController : ControllerBase
    {
        private readonly RegistraClienteRepository _clienteRepository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public ClienteController(
            RegistraClienteRepository clienteRepository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _clienteRepository = clienteRepository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarCliente([FromBody] ClienteRequestDto request)
        {
            try
            {
                // 400 - Validación de parámetros
                if (
                    string.IsNullOrWhiteSpace(request.codCliente) ||
                    string.IsNullOrWhiteSpace(request.nombre) ||
                    string.IsNullOrWhiteSpace(request.tipoCedula) ||
                    string.IsNullOrWhiteSpace(request.numeroCedula))
                    return BadRequest(new { Mensaje = "Parámetros inválidos" });

                // 401 - Validar usuario
                var usuario = _usuarioRepository.ValidarUsuario(request.codRecurso);
                if (usuario == null)
                    return Unauthorized(new { Mensaje = "Usuario no autorizado" });

                // Ejecutar SP
                await _clienteRepository.RegistrarClienteAsync(
                     request.codCliente, request.nombre, request.telefono, request.correo, request.tipoCedula, request.numeroCedula);



                // 200 - Ok
                return Ok();
            }
            catch (Exception ex)
            {
                // 500 - Error interno
                return StatusCode(500, new { ex.Message });
            }
        }
    }
}