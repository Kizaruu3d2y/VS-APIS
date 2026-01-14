using Microsoft.AspNetCore.Mvc;
using proyectoDapper.Models.Pos;
using proyectoDapper.POS.Data.Cierre;
using proyectoDapper.POS.Data.Endpoint;
using proyectoDapper.POS.Models.Surtidor.Cliente.Cierre;
using System.Collections.Generic;

namespace proyectoDapper.POS.Controller.Cierre
{
    [ApiController]
    [Route("api/pos/cierre")]
    public class ResumenCierreController : ControllerBase
    {
        private readonly LimiteCierresRepository _limiteRepository;
        private readonly CierreResumenRepository _cierreResumenRepository;
        private readonly CierreRepository _cierreRepository;
        private readonly ValidaUsuarioRepository _usuarioRepository;
        private readonly FechaTurnoRepository _fechaTurnoRepository;
        private readonly VersionRepository _versionCierreRepository;

        public ResumenCierreController(
                LimiteCierresRepository limiteRepository,
                CierreResumenRepository cierreResumenRepository,
                CierreRepository cierreRepository,
                ValidaUsuarioRepository usuarioRepository,
                FechaTurnoRepository fechaTurnoRepository,
                VersionRepository versionCierreRepository)
        {
            _limiteRepository = limiteRepository;
            _cierreResumenRepository = cierreResumenRepository;
            _cierreRepository = cierreRepository;
            _usuarioRepository = usuarioRepository;
            _fechaTurnoRepository = fechaTurnoRepository;
            _versionCierreRepository = versionCierreRepository;

        }

        [HttpGet("resumen")]
        public async Task<IActionResult> ResumenCierre([FromQuery] string codRecurso)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(codRecurso))
                    return BadRequest(new { Mensaje = "El parámetro 'codRecurso' es obligatorio." });

                var usuario = _usuarioRepository.ValidarUsuario(codRecurso);
                if (usuario == null)
                    return Unauthorized(new { Mensaje = "Usuario no autorizado." });

                var fechaTurno = await _fechaTurnoRepository.ObtenerFechaTurnoAsync();
                if (fechaTurno == null)
                    return StatusCode(500, new { Mensaje = "No se pudo obtener la fecha turno." });

                var version = await _limiteRepository.LimiteCierresAsync(usuario.Codigo);
                var resumenValores = await _cierreResumenRepository.ObtenerCierreResumenAsync(EmpresaGlobalDto.Empresa);
                var datosCierre = await _cierreRepository.ObtenerCierreAsync(EmpresaGlobalDto.Empresa, usuario.Codigo);

                if (resumenValores == null)
                    return StatusCode(405, new { Mensaje = "No se pudo obtener el resumen de cierre." });
                if (datosCierre == null)
                    return StatusCode(405, new { Mensaje = "No se pudieron obtener los datos del cierre." });



                var respuesta = new
                {
                    Version = version,
                    listaValoresCierre = resumenValores, 
                    vCombustible = datosCierre?.vCombustible ?? 0,
                    vAceite = datosCierre?.vAceite ?? 0,
                    vCredito = datosCierre?.vCredito ?? 0,
                    vCalibracion = datosCierre?.Calibracion ?? 0,
                    FechaTurno = fechaTurno,
                    Mensaje = "OK"
                };

                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = ex.Message });
            }
        }

        [HttpPost("resumen")]
        public async Task<IActionResult> EjecutarCierre([FromBody] EjecutarCierreRequest request)
        {
            try
            {
                // ===== Validaciones básicas =====
                if (request == null)
                    return BadRequest(new { Mensaje = "El body es obligatorio." });

                if (string.IsNullOrWhiteSpace(request.codRecurso))
                    return BadRequest(new { Mensaje = "El parámetro 'codRecurso' es obligatorio." });

                if (request.valores == null || !request.valores.Any())
                    return BadRequest(new { Mensaje = "La lista de valores es obligatoria." });

                // ===== Validar usuario =====
                var usuario = _usuarioRepository.ValidarUsuario(request.codRecurso);
                if (usuario == null)
                    return Unauthorized(new { Mensaje = "Usuario no autorizado." });

                var fechaTurno = await _fechaTurnoRepository.ObtenerFechaTurnoAsync();

                // ===== Validar versión / límite =====
                var version = _versionCierreRepository.RealizarCierreVersion(usuario.Codigo, request.valores, fechaTurno.Fecha);
                if (!version)
                    return StatusCode(405, new { Mensaje = "No se pudo obtener el resumen de cierre." });

                // ===== Guardar valores de cierre =====
                //await _cierreResumenRepository.GuardarValoresCierreAsync(
                //    EmpresaGlobal.Empresa,
                //    usuario.Codigo,
                //    request.valores,
                //    fechaTurno.Fecha
                //);

                // ===== Ejecutar cierre diario =====
                var resultado = await _cierreRepository.CierreDiario();
                if (resultado.Exito)
                    return StatusCode(500, new { Mensaje = resultado.Error });

                return Ok(new
                {
                    Mensaje = "OK",
                    FechaTurno = fechaTurno,
                    Version = version
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = ex.Message });
            }
        }
    }
}