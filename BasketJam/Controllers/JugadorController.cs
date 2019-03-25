using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    //[Authorize]
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
        public ActionResult<List<Jugador>> Get()
        {
            return _jugadorService.ListarJugadores();
        }

        [HttpGet("{id:length(24)}", Name = "ObtenerJugador")]
        public ActionResult<Jugador> Get(string id)
        {
            var jugador = _jugadorService.BuscarJugador(id);

            if (jugador == null)
            {
                return NotFound();
            }

            return jugador;
        }

        [HttpPost]
        public ActionResult<Jugador> Create(Jugador jugador)
        {
            _jugadorService.CrearJugador(jugador);

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

            _jugadorService.EliminarJugador(jugador.Id);

            return NoContent();
        }
    }
}