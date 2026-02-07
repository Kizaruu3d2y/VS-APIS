using Data.Endpoint;
using Microsoft.AspNetCore.Mvc;

namespace Controller.Bitacora
{
    [ApiController]
    [Route("bitacora")]
    public class BitacoraDescargaController : ControllerBase
    {
        private readonly BitacoraDescargasRepository _repository;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public BitacoraDescargaController(
            BitacoraDescargasRepository repository,
            ValidaUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpPost("descarga")]
        public async Task<IActionResult> RegistrarDescarga([FromBody] BitacoraDescargaRequest request)
        {
            try
            {
                // 400 - Validación de parámetros
                if (string.IsNullOrWhiteSpace(request.codRecurso) ||
                    request == null ||
                    string.IsNullOrWhiteSpace(request.noFactura) ||
                    string.IsNullOrWhiteSpace(request.notas))

                {
                    return BadRequest(new { Mensaje = "Parámetros inválidos" });
                }

                // 401 - Validar usuario
                var usuario = _usuarioRepository.ValidarUsuario(request.codRecurso);
                if (usuario == null)
                    return Unauthorized(new { Mensaje = "Usuario no autorizado" });

                // Registro en BD
                var super = await _repository.RegistrarDescargaAsync(
                    request.codRecurso,
                    request.noFactura,
                    "03",
                    request.cantSuper,
                    request.iniSuper,
                    request.finSuper,
                    request.notas
                );
                var plus = await _repository.RegistrarDescargaAsync(
                    request.codRecurso,
                    request.noFactura,
                    "01",
                    request.cantPlus,
                    request.iniPlus,
                    request.finPlus,

                    request.notas
                     );
                var diesel = await _repository.RegistrarDescargaAsync(
                    request.codRecurso,
                    request.noFactura,
                    "02",
                    request.cantDiesel,
                    request.iniDiesel,
                    request.finDiesel,
                    request.notas
                    );

                if (!super && !plus && !diesel)
                    return StatusCode(405, new { Mensaje = "No se pudo registrar la descarga" });

                return Ok(new { Mensaje = "OK" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }
    }

    public class BitacoraDescargaRequest
    {
        public string codRecurso { get; set; }
        public string noFactura { get; set; }
        public double cantSuper { get; set; }
        public double iniSuper { get; set; }
        public double finSuper { get; set; }
        public double cantPlus { get; set; }
        public double iniPlus { get; set; }
        public double finPlus { get; set; }
        public double cantDiesel { get; set; }
        public double iniDiesel { get; set; }
        public double finDiesel { get; set; }
        public string notas { get; set; }
    }
}