using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class JuezController : ControllerBase
    {
      private IJuezService _juezService;

        public JuezController(IJuezService juezService)
        {
            _juezService = juezService;
        }

       [HttpGet]
        public ActionResult<List<Juez>> Get()
        {
            return _juezService.ListarJueces();
        }

        [HttpGet("{id:length(24)}", Name = "ObtenerJugador")]
        public ActionResult<Juez> Get(string id)
        {
            var juez = _juezService.BuscarJuez(id);

            if (juez == null)
            {
                return NotFound();
            }

            return juez;
        }

        [HttpPost]
        public ActionResult<Juez> Create(Juez juez)
        {
            _juezService.CrearJuez(juez);

            return CreatedAtRoute("ObtenerJugador", new { id = juez.Id.ToString() }, juez);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Juez juezIn)
        {
            var juez = _juezService.BuscarJuez(id);

            if (juez == null)
            {
                return NotFound();
            }

            _juezService.ActualizarJuez(id,juezIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var juez = _juezService.BuscarJuez(id);

            if (juez == null)
            {
                return NotFound();
            }

            _juezService.EliminarJuez(juez.Id);

            return NoContent();
        }
    }
}