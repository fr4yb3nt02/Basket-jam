using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using System;

namespace BasketJam.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EstadisticasJugadorPartidoController : ControllerBase
    
    {
        private IEstadisticasEquipoPartidoService _estadisticasEquipoPartidoService;

        private IEstadisticasJugadorPartidoService _estadisticasJugadorPartidoService;

        public EstadisticasJugadorPartidoController(IEstadisticasEquipoPartidoService estadisticasEquipoPartidoService , IEstadisticasJugadorPartidoService estadisticasJugadorPartidoService)
        {
            _estadisticasEquipoPartidoService = estadisticasEquipoPartidoService;
            _estadisticasJugadorPartidoService=estadisticasJugadorPartidoService;
        }

       [HttpPost("cargarPuntosJugador")]
        public async Task<ActionResult<ReplaceOneResult>> Create(EstadisticasEquipoPartido eep)
        {
            await _estadisticasEquipoPartidoService.Save(eep);
            return Ok();
          //  return Ok(new{si=true,eep});
        }

        [HttpPost("cargarPtosJugador")]
        public async Task<ActionResult<Boolean>> CargarEstadistica(EstadisticasJugadorPartido ejp)
        {
            await _estadisticasJugadorPartidoService.CargarEstadistica(ejp);
            return Ok();
        }
    }
    }