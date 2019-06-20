using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using System;

namespace WebApi.Controllers
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
        
    }
    }