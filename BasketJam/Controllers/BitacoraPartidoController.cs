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
    public class BitacoraPartidoController : ControllerBase
    
    {
        private IBitacoraService _bitacoraService;

        public BitacoraPartidoController(IBitacoraService bitacoraService)
        {
            _bitacoraService = bitacoraService;
        }

[AllowAnonymous]
 [HttpPost("GenerarBitacora/{id:length(24)}")]
                public async Task<ActionResult>  GenerarBitacora(BitacoraPartido bp)
        {    
            return Ok(new {resultado=await _bitacoraService.GenerarBitacora(bp)});
}

      [AllowAnonymous]
 [HttpGet("consultarEstadisticasPeriodo")]
                public async Task<ActionResult>  consultarEstadisticasPeriodo(string idPartido , int periodo)
        {    
            return Ok(await _bitacoraService.consultarEstadisticasPeriodo(idPartido,periodo));
}  
        
    }
    }