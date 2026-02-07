using Data.Cliente;
using Microsoft.AspNetCore.Mvc;
using Models.Cliente;
using Models;

namespace Controller.Cliente
{
    [ApiController]
    [Route("cliente")]
    public class CorreoClienteController : ControllerBase
    {
        private readonly ActualizaCorreoClienteRepository _repository;

        public CorreoClienteController(ActualizaCorreoClienteRepository repository)
        {
            _repository = repository;
        }

        [HttpPut("correo")]
        public async Task<IActionResult> Put([FromBody] CorreoClienteRequestDto request)
        {
            try
            {
                // 400 - Validación de parámetros
                if (string.IsNullOrEmpty(request.codCliente) || string.IsNullOrEmpty(request.correo))
                    return BadRequest(new { Resultado = false, Mensaje = "Parámetros incompletos." });

                // Lógica principal
                var resultado = await _repository.ActualizaCorreoClienteAsync(request.codCliente, request.correo);

                if (!resultado.Resultado)
                    return StatusCode(405, resultado);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(409, new ActualizaCorreoClienteDto { Resultado = false, Mensaje = ex.Message });
            }
        }
    }
}