using Data.Cliente;
using Data.Endpoint;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Controller.Cliente
{
    [ApiController]
    [Route("cliente")]
    public class ActividadesClienteController : ControllerBase
    {
        private readonly ActividadEconomicaRepository _actividadEconomicaRepository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public ActividadesClienteController(
            ActividadEconomicaRepository actividadEconomicaRepository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _actividadEconomicaRepository = actividadEconomicaRepository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("actividades")]
        public async Task<IActionResult> ActividadesCliente([FromQuery] string codCliente)
        {
            try
            {
                // Validación del codCliente
                if (string.IsNullOrWhiteSpace(codCliente))
                    return BadRequest(new { Mensaje = "El parámetro 'codCliente' es obligatorio." });

                // Llamada al repositorio
                var actividades = await _actividadEconomicaRepository
                                        .CargarActividadesClienteAsync(codCliente);

                // 🔥 CAMBIO ÚNICO: devolver lista directa
                return Ok(actividades);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Mensaje = "Error interno del servidor.",
                    Error = ex.Message
                });
            }
        }
    }
}