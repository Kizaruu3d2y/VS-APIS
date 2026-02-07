
using Microsoft.AspNetCore.Mvc;
using Models.Articulos;
using Data.Articulos;
using Data.Endpoint;

namespace Controller.Articulos
{
    [ApiController]
    public class InventarioArticulosController : ControllerBase
    {
        private readonly InventarioArticulosRepository _repository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public InventarioArticulosController(
            InventarioArticulosRepository repository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpPost("articulos/inventario")]
        public async Task<IActionResult> Post([FromBody] InventarioArticulosRequestDto request)
        {
            try
            {
                // 400 - Validación de parámetros
                if (request == null ||
                    string.IsNullOrEmpty(request.codRecurso) ||
                    string.IsNullOrEmpty(request.registro) ||      
                    request.idArticulos == null || !request.idArticulos.Any() ||
                    request.cantidades == null || !request.cantidades.Any())
                {
                    return BadRequest();
                }

                // 401 - Autenticación fallida
                var usuario =  _usuarioRepository.ValidarUsuario(request.codRecurso);
                if (usuario == null)
                    return Unauthorized();

                // Lógica principal
                AceitesResponse resultado = await _repository.RegistrarAceites(request);

                // 500 - Caso especial (si quieres agregar algún mensaje de BD)
                if (string.IsNullOrEmpty(resultado.Mensaje))
                    return StatusCode(500, new  {resultado.Mensaje });

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