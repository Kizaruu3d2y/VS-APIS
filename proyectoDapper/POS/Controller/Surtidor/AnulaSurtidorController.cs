using Microsoft.AspNetCore.Mvc;
using proyectoDapper.Models.Pos;
using proyectoDapper.POS.Data.Endpoint;
using proyectoDapper.POS.Data.Surtidor;
using proyectoDapper.POS.Models.Surtidor;

namespace proyectoDapper.POS.Controller.Surtidor
{
    [ApiController]
    [Route("api/pos/surtidor")]
    public class AnulaSurtidorController : ControllerBase
    {
        private readonly AnulaSurtidorRepository _repo;
        private readonly ValidaUsuarioRepository _usuarioRepository;

        public AnulaSurtidorController(
            AnulaSurtidorRepository repo,
            ValidaUsuarioRepository usuarioRepository)
        {
            _repo = repo;
            _usuarioRepository = usuarioRepository;
        }

        [HttpPost("anula")]
        public async Task<IActionResult> Anula([FromBody] AnulaSurtidorRequestDto request)
        {
            if (string.IsNullOrEmpty(request.codRecurso) ||
            string.IsNullOrEmpty(request.surtidor))
                return BadRequest(new { Mensaje = "Parámetros pSurtidor, pRecurso y codBarras son requeridos" });

            string empresa = EmpresaGlobalDto.Empresa;
            var factura = await _repo.ObtenerFacturaUltimoDespacho(empresa, request.codRecurso, request.surtidor);

            if (factura == null)
                return StatusCode(402, new { mensaje = "Error al anular la factura." });

            bool ok = await _repo.AnularFactura(empresa, factura.idFactura, request.codRecurso, factura.lectura, factura.tipoFactura);

            if (ok)
                return StatusCode(402, new { mensaje = "Error al anular la factura." + ok.ToString() });

            return Ok();
        }
    }
}