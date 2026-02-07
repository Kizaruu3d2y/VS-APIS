using Data.Cliente;
using Data.Endpoint;
using Microsoft.AspNetCore.Mvc;
using Models.Cliente;

namespace Controller.Cliente
{
    [ApiController]
    [Route("cliente")]
    public class TagClienteController : ControllerBase
    {
        private readonly RegistrarTagRepository _repository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public TagClienteController(
            RegistrarTagRepository repository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpPut("tag")]
        public async Task<IActionResult> RegistrarTag([FromBody] SolicitarTagRequestDto request)
        {
            try
            {
                // 400 - Validación de parámetros
                if (string.IsNullOrEmpty(request.codRecurso) ||
                    request == null ||
                    string.IsNullOrEmpty(request.codCliente) ||
                    string.IsNullOrEmpty(request.placa) ||
                    string.IsNullOrEmpty(request.tag))
                {
                    return BadRequest();
                }

                // 401 - Validación de usuario
                var usuario = _usuarioRepository.ValidarUsuario(request.codRecurso);
                if (usuario == null)
                    return Unauthorized();

                // Lógica principal
                bool resultado = await _repository.SolicitarTagAsync(
                    request.codCliente,
                    request.placa,
                    request.tag,
                    usuario.Codigo
                );

                // 403 - No se pudo procesar la solicitud
                if (!resultado)
                    return StatusCode(403, new { Mensaje = "No se pudo registrar el tag" });

                // 200 - Respuesta correcta
                return Ok(new { Mensaje = "Tag registrado correctamente" });
            }
            catch (Exception ex)
            {
                // 500 - Error general
                return StatusCode(500, new { ex.Message });
            }
        }
    }
}