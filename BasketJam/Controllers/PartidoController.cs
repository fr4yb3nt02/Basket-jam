using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;

namespace WebApi.Controllers
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
        public ActionResult<List<Partido>> Get()
        {
            return _partidoService.ListarPartidos();
        }

        [HttpGet("{id:length(24)}", Name = "ObtenerPartidos")]
        public ActionResult<Partido> Get(string id)
        {
            var partido = _partidoService.BuscarPartido(id);

            if (partido == null)
            {
                return NotFound();
            }

            return partido;
        }

        [HttpPost]
        public ActionResult<Partido> Create(Partido partido)
        {
            _partidoService.CrearPartido(partido);

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

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var partido = _partidoService.BuscarPartido(id);

            if (partido == null)
            {
                return NotFound();
            }

            _partidoService.EliminarPartido(partido.Id);

            return NoContent();
        }
    }
}