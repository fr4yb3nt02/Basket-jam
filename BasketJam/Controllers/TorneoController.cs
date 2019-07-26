using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BasketJam.Controllers
{
   // [Authorize]
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
        public async Task<ActionResult<List<Torneo>>> Get()
        {
            return await _torneoService.ListarTorneos();
        }

        [HttpGet("{id:length(24)}", Name = "ObtenerTorneo")]
        public async Task<ActionResult<Torneo>> Get(string id)
        {
            var torneo = await _torneoService.BuscarTorneo(id);

            if (torneo == null)
            {
                return NotFound();
            }

            return torneo;
        }

        [HttpPost]
        public async Task<ActionResult<Torneo>> Create([FromBody]Torneo torneo)
        {
            await _torneoService.CrearTorneo(torneo);

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

            _torneoService.EliminarTorneo(torneo.Id.ToString());

            return NoContent();
        }
    }
}