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
    public class JuezController : ControllerBase
    {
      private IJuezService _juezService;

        public JuezController(IJuezService juezService)
        {
            _juezService = juezService;
        }

       [HttpGet]
        public async Task<ActionResult<List<Juez>>> Get()
        {
            return await _juezService.ListarJueces();
        }

        [HttpGet("{id:length(24)}", Name = "ObtenerJuez")]
        public async Task<ActionResult<Juez>> Get(string id)
        {
            var juez = await _juezService.BuscarJuez(id);

            if (juez == null)
            {
                return NotFound();
            }

            return juez;
        }

        [HttpPost]
        public async Task<ActionResult<Juez>> Create(Juez juez)
        {
            await _juezService.CrearJuez(juez);

            return CreatedAtRoute("ObtenerJuez", new { id = juez.Id.ToString() }, juez);
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

            return Ok();
        }



        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var juez = _juezService.BuscarJuez(id);

            if (juez == null)
            {
                return NotFound();
            }

            _juezService.EliminarJuez(juez.Id.ToString());

            return Ok();
        }
    }
}