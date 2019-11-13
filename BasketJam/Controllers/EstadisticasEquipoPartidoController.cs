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
    public class EstadisticasEquipoPartidoController : ControllerBase

    {
        private IEstadisticasEquipoPartidoService _estadisticasEquipoPartidoService;

        public EstadisticasEquipoPartidoController(IEstadisticasEquipoPartidoService estadisticasEquipoPartidoService)
        {
            _estadisticasEquipoPartidoService = estadisticasEquipoPartidoService;
        }

        [HttpPost("cargarPuntosEquipo")]
        public async Task<ActionResult<ReplaceOneResult>> Create(EstadisticasEquipoPartido eep)
        {
            await _estadisticasEquipoPartidoService.Save(eep);
            return Ok();
            //  return Ok(new{si=true,eep});
        }

        [HttpPost("CargarEstadistica")]
        public async Task<ActionResult<Boolean>> CargarEstadistica(EstadisticasEquipoPartido eep)
        {
            await _estadisticasEquipoPartidoService.CargarEstadistica(eep);
            return Ok();
        }

        //[AllowAnonymous]
        [HttpGet("EstadisticasEquipoPorPartido/{id:length(24)}")]
        public async Task<List<EstadisticasEquipoPartido>> EstadisticasEquipoPorPartido(string id)
        {
            return await _estadisticasEquipoPartidoService.EstadisticasEquipoPorPartido(id);

        }

    }
}