using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
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
        public async Task<ActionResult<List<Jugador>>> Get()
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
            var jugador = await _jugadorService.BuscarJugador(id);

            if (jugador == null)
            {
                return NotFound();
            }

            return jugador;
        }

        [HttpPost("crearJugador")]
        public async Task<ActionResult<Jugador>> Create(Jugador jugador)
        {
            await _jugadorService.CrearJugador(jugador);

            return CreatedAtRoute("ObtenerJugador", new { id = jugador.Id.ToString() }, jugador);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Jugador jugadorIn)
        {
            var jugador = _jugadorService.BuscarJugador(id);

            if (jugador == null)
            {
                return NotFound();
            }

            _jugadorService.ActualizarJugador(id,jugadorIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var jugador = _jugadorService.BuscarJugador(id);

            if (jugador == null)
            {
                return NotFound();
            }

            _jugadorService.EliminarJugador(jugador.Id.ToString());

            return NoContent();
        }
    }
}