using BasketJam.Models;
using BasketJam.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;


namespace BasketJam.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InfoDispositivoController : ControllerBase
    {

        private IInfoDispositivoService _infoDispositivoService;

        public InfoDispositivoController(IInfoDispositivoService infoDispositivoService)
        {
            _infoDispositivoService = infoDispositivoService;
        }

        [AllowAnonymous]
        [HttpPost("GuardarInfoDispositivo")]
        public async Task<ActionResult> GuardarInfoDispositivo(InfoDispositivo info)
        {
            try
            {
                await _infoDispositivoService.CrearInfoDispositivo(info);
                return Ok(new {Resultado= "Se ha guardado la información del dispositivo correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpGet("ObtenerInfoDispositivo/")]
        public async Task<ActionResult> ObtenerInfoDispositivo(string idDispositivo)
        {
            try
            {
                var infoDispositivo = await _infoDispositivoService.BuscarInfoDispositivo(idDispositivo);

                if (infoDispositivo == null)
                {
                    return NotFound(new { Error = "No se ha encontrado resultado para el dispositivo buscado." });
                }
                else
                { 
                return Ok(await _infoDispositivoService.BuscarInfoDispositivo(idDispositivo));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }

    }
}
