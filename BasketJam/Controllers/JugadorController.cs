using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using BasketJam.Models;
using System;
using System.Dynamic;

namespace BasketJam.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class JugadorController : ControllerBase
    {
      private IJugadorService _jugadorService;

        public JugadorController(IJugadorService jugadorService)
        {
            _jugadorService = jugadorService;
        }

       [HttpGet]
        public async Task<ActionResult<List<ExpandoObject>>> Get()
        {
            return await _jugadorService.ListarJugadores();
        }

        [HttpGet("jugadoresPorEquipo/{id:length(24)}")]
        public async Task<ActionResult<List<Jugador>>> ListarJugadoresPorEquipo(string idEquipo)
        {
            return await _jugadorService.ListarJugadoresPorEquipo(idEquipo);
        }

        [HttpGet("{id:length(24)}", Name = "ObtenerJugador")]
        public async Task<ActionResult<Jugador>> Get(string id)
        {
            try
            { 
            var jugador = await _jugadorService.BuscarJugador(id);

            if (jugador == null)
            {
                return NotFound(new { Error = "No se ha encontrado el jugador." });
            }

            return jugador;
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }

        [HttpPost("crearJugador")]
        public async Task<ActionResult<Jugador>> Create(Jugador jugador)
        {
            try
            {
                await _jugadorService.CrearJugador(jugador);

                return CreatedAtRoute("ObtenerJugador", new { id = jugador.Id.ToString() }, jugador);
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Jugador jugadorIn)
        {
            try
            { 
            var jugador = _jugadorService.BuscarJugador(id);

            if (jugador == null)
            {
                    return NotFound(new { Error = "No se ha encontrado el jugador." });
                }

            _jugadorService.ActualizarJugador(id,jugadorIn);

                return Ok(new { Resultado = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            try
            { 
            var jugador = _jugadorService.BuscarJugador(id);

            if (jugador == null)
            {
                    return NotFound(new { Error = "No se ha encontrado el jugador." });
                }

            _jugadorService.EliminarJugador(id);

            return Ok(new { Resultado = true });
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("subirFoto/")]
        public IActionResult subirFoto(Imagen img)
        {
            try
            {
                _jugadorService.subirImagen(img);
                return Ok(new { Resultado = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }
    }
}