using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using System;
using BasketJam.Models;

namespace BasketJam.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BitacoraPartidoController : ControllerBase

    {
        private IBitacoraService _bitacoraService;

        public BitacoraPartidoController(IBitacoraService bitacoraService)
        {
            _bitacoraService = bitacoraService;
        }

        //[AllowAnonymous]
        [HttpPost("GenerarBitacora")]
        public async Task<ActionResult> GenerarBitacora(BitacoraPartido bp)
        {
            try
            {
                return Ok(new { resultado = await _bitacoraService.GenerarBitacora(bp) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        //[AllowAnonymous]
        [HttpGet("consultarEstadisticasPeriodo")]
        public async Task<ActionResult> consultarEstadisticasPeriodo(string idPartido, int periodo)
        {
            try
            {
                return Ok(await _bitacoraService.consultarEstadisticasPeriodo(idPartido, periodo));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        //[AllowAnonymous]
        [HttpGet("mejoresJugadoresPorRubro")]
        public async Task<ActionResult> mejoresJugadoresPorRubro(int rubro, string idTorneo)
        {
            try
            {
                return Ok(await _bitacoraService.mejoresDiezCadaRubro(rubro, idTorneo));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

    }
}