using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TorneoController : ControllerBase
    {
      private ITorneoService _torneoService;

        public TorneoController(ITorneoService torneoService)
        {
            _torneoService = torneoService;
        }

       [HttpGet]
        public ActionResult<List<Torneo>> Get()
        {
            return _torneoService.ListarTorneos();
        }

        [HttpGet("{id:length(24)}", Name = "ObtenerTorneo")]
        public ActionResult<Torneo> Get(string id)
        {
            var torneo = _torneoService.BuscarTorneo(id);

            if (torneo == null)
            {
                return NotFound();
            }

            return torneo;
        }

        [HttpPost]
        public ActionResult<Torneo> Create(Torneo torneo)
        {
            _torneoService.CrearTorneo(torneo);

            return CreatedAtRoute("ObtenerTorneo", new { id = torneo.Id.ToString() }, torneo);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Torneo torneoIn)
        {
            var torneo = _torneoService.BuscarTorneo(id);

            if (torneo == null)
            {
                return NotFound();
            }

            _torneoService.ActualizarTorneo(id,torneoIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var torneo = _torneoService.BuscarTorneo(id);

            if (torneo == null)
            {
                return NotFound();
            }

            _torneoService.EliminarTorneo(torneo.Id);

            return NoContent();
        }
    }
}