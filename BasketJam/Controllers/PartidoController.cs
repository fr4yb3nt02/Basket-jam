using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace BasketJam.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PartidoController : ControllerBase
    {
      private IPartidoService _partidoService;

        public PartidoController(IPartidoService partidoService)
        {
            _partidoService = partidoService;
        }

       [HttpGet]
        public async Task<ActionResult<List<Partido>>> Get()
        {
            return await _partidoService.ListarPartidos();
        }

        [HttpGet("{id:length(24)}", Name = "ObtenerPartido")]
        public async Task<ActionResult<Partido>> Get(string id)
        {
            var partido =await _partidoService.BuscarPartido(id);

            if (partido == null)
            {
                return NotFound();
            }

            return partido;
        }

        [HttpPost]
        public async Task<ActionResult<Partido>> Create(Partido partido)
        {
            
            await _partidoService.CrearPartido(partido);

            return CreatedAtRoute("ObtenerPartido", new { id = partido.Id.ToString() }, partido);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Partido partidoIn)
        {
            var partido = _partidoService.BuscarPartido(id);

            if (partido == null)
            {
                return NotFound();
            }

            _partidoService.ActualizarPartido(id,partidoIn);

            return NoContent();
        }

        [HttpPut("ActualizarTiempoPartido/{id:length(24)}")]
        public IActionResult ActualizarTiempoPartido(string id, string tiempo)
        {
            try
            {
            var partido = _partidoService.BuscarPartido(id);

            if (partido == null)
            {
                return NotFound();
            }

            _partidoService.ActualizarTiempoPartido(id,tiempo);
            return Ok();
            }
            catch
            {
            return NoContent();
            }
        }
        

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var partido = _partidoService.BuscarPartido(id);

            if (partido == null)
            {
                return NotFound();
            }

            _partidoService.EliminarPartido(partido.Id.ToString());

            return NoContent();
        }


        [HttpPut("AgregarJuezAPartido/{id:length(24)}")]
       // [HttpPut("{id:length(24)}", Name = "AgregarJuezAPartido")]
        public async Task<ActionResult<bool>> AgregarJuezAPartido(string id,[FromBody]List<Juez> jueces)
        {
            var partido = await _partidoService.BuscarPartido(id);

            if (partido == null)
            {
                return NotFound();
            }

          Boolean res= await _partidoService.AgregarJuezPartida(id,jueces);

            
            if(res)
            return Ok(new { mensaje = "Se han agregado los jueces correctamente." });
            else
            return BadRequest(new{error="No se ha podido realizar la acción."});
        }

        [HttpPut("AgregarJugadoresAPartido/{id:length(24)}")]
       // [HttpPut("{id:length(24)}", Name = "AgregarJuezAPartido")]
        public async Task<ActionResult<bool>> AgregarJugadoresAPartido(string id,[FromBody]List<EquipoJugador> jugadores)
        {

            var partido = await _partidoService.BuscarPartido(id);

            if (partido == null)
            {
                return NotFound();
            }

          Boolean res= await _partidoService.AgregarJugadoresAPartido(id,jugadores);

            if(res)
            return Ok(new { error = "No se han asignado los jugadores de manera correcta." });
            else
            return BadRequest(new{error="No se ha podido realizar la acción."});
            
        }

        

[AllowAnonymous]
 [HttpGet("Listpart")]
                public async Task<ActionResult>  visualizadorPartidos()
        {

          //  List<String> a = await _partidoService.DevuelvoListPartidosAndroid();
            return Ok(await _partidoService.DevuelvoListPartidosAndroid());
    //return Ok(_partidoService.DevuelvoListPartidosAndroid);
}

[AllowAnonymous]
 [HttpGet("ListPartidosProgOJug")]
                public async Task<ActionResult>  ListarPartidosProgOJug()
        {
            return Ok(await _partidoService.ListarPartidosProgOJug());
        }

[AllowAnonymous]
 [HttpGet("ConsultarHeaderPartido/{id:length(24)}")]
                public async Task<ActionResult>  ConsultarHeaderPartido(string id)
        {
            return Ok(await _partidoService.ConsultarHeaderPartido(id));
}

[AllowAnonymous]
 [HttpGet("ConsultaDetallesPartido/{id:length(24)}")]
                public async Task<ActionResult>  ConsultaDetallesPartido(string id)
        {
          
            return Ok(await _partidoService.ConsultaDetallesPartido(id));
}

[AllowAnonymous]
 [HttpGet("UltimosEventosEquipo/{id:length(24)}")]
                public async Task<ActionResult>  UltimosEventosEquipo(string id)
        {    
            return Ok(await _partidoService.UltimosEventosEquipo(id));
}

 [AllowAnonymous]
 [HttpGet("ListarEquiposJugador/{id:length(24)}")]
                public async Task<ActionResult>  ListarEquiposJugador(string id)
        {    
            try
            { 
            return Ok(await _partidoService.ListarEquipoJugador(id));
            }
            catch(Exception ex)
            {
                return BadRequest( new{Error = ex.Message });
            }

        }

        [AllowAnonymous]
        [HttpPut("ActualizarestadoPartido/")]
        public IActionResult ActualizarEstadoPartido(string id, string tiempo)
        {
            try
            {
                _partidoService.ActualizarEstadoPartido(id, tiempo);
                return Ok();
            }
            catch
            {
                return NoContent();
            }
        }

        [AllowAnonymous]
        [HttpGet("ListarJugadoresEquiposPartido/{id:length(24)}")]
        public async Task<List<Object>> ListarJugadoresEquiposPartido(string id)
        {
            try
            {
              return await _partidoService.ListarJugadoresEquiposPartido(id);
   
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("ListarEstadios/")]
        public async Task<List<Object>> ListarEstadios()
        {
            try
            {
                return await _partidoService.ListarEstadios();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



    }
}