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
    public class VotacionPartidoController : ControllerBase
    
    {
        private IVotacionPartidoService _votacionPartidoService;

        public VotacionPartidoController(IVotacionPartidoService votacionPartidoService)
        {
            _votacionPartidoService = votacionPartidoService;
        }

[AllowAnonymous]
 [HttpPost("Votar")]
                public async Task<ActionResult>  GenerarBitacora(VotacionPartido ve)
        {    
            
            return Ok(new {resultado=await _votacionPartidoService.votarEquipoPartido(ve)});
}

      [AllowAnonymous]
 [HttpGet("ConsultarVotacion/{idPartido:length(24)}")]
                public async Task<VotacionPartido>  consultarVotacionPartido(string idPartido)
        {    

            return await _votacionPartidoService.BuscarVotacionPartido(idPartido);
}

        [AllowAnonymous]
        [HttpGet("UsuarioYaVoto/")]
        public async Task<Boolean> UsuarioYaVoto(string usuario,string idPartido)
        {
            try
            { 
            return await _votacionPartidoService.usuarioYaVoto(usuario,idPartido);
            }
            catch(Exception ex)
            {
                return false;
            }
        }

    }
    }