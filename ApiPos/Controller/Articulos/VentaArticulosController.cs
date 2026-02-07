using Data.Articulos;
using Data.Endpoint;
using Microsoft.AspNetCore.Mvc;
using Models.Endpoint;

namespace Controller.Articulos
{
    [ApiController]
    [Route("articulos")]
    public class VentasAceiteController : ControllerBase
    {
        private readonly VentasAceiteRepository _repository;
        private readonly ValidaUsuarioRepository _usuarioRepository;


        public VentasAceiteController(
            VentasAceiteRepository repository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;

        }

        [HttpGet("venta")]
        public async Task<IActionResult> GetVentas([FromQuery] string codRecurso)
        {
            try
            {
                // 400 - Validación de parámetros
                if (string.IsNullOrWhiteSpace(codRecurso))
                    return BadRequest();

                // 401 - Autenticación fallida
                var usuario =  _usuarioRepository.ValidarUsuario(codRecurso);
                if (usuario == null)
                    return Unauthorized();
                // Lógica principal: obtener reporte de ventas
                var resultado = await _repository.ObtenerReporteVentasAsync(codRecurso, EmpresaGlobalDto.Empresa);

                // 500 - Caso especial si la BD no retorna resultados
                if (resultado == null)
                    return StatusCode(500, new { Mensaje = "No se pudo obtener el reporte de ventas" });

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