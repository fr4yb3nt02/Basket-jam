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
    public class TablaDePosicionesController : ControllerBase
    {

        private ITablaDePosicionesService _tablaDePosicionesService;

        public TablaDePosicionesController(ITablaDePosicionesService tablaDePosicionesService)
        {
            _tablaDePosicionesService = tablaDePosicionesService;
        }

        [AllowAnonymous]
        [HttpPost("CrearTablaDePosiciones")]
        public async Task<ActionResult>  CrearTablaDePosiciones(TablaDePosiciones tp)
        {
            try
            {
                await _tablaDePosicionesService.CrearTablaDePosiciones(tp);
                return Ok("Se ha generado la tabla de posiciones correctamente.");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

            [AllowAnonymous]
            [HttpGet("BuscarTablaDePosiciones")]
            public async Task<ActionResult> BuscarTablaDePosiciones(string idTorneo)
            {    
            return Ok(await _tablaDePosicionesService.BuscarTablaDePosiciones(idTorneo));
            }  


        
    }
    }


