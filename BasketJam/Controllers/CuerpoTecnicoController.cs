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
    public class CuerpoTecnicoController : ControllerBase
    {
      private ICuerpoTecnicoService _cuerpoTecnicoService;

        public CuerpoTecnicoController(ICuerpoTecnicoService cuerpoTecnicoService)
        {
            _cuerpoTecnicoService = cuerpoTecnicoService;
        }

       [HttpGet]
        public async Task<List<CuerpoTecnico>> Get()
        {
            return await _cuerpoTecnicoService.ListarMiembroCuerpoTecnico();
        }

        [HttpGet("{id:length(24)}", Name = "ObtenerMiembroCuerpoTecnico")]
        public async Task<ActionResult<CuerpoTecnico>> Get(string id)
        {
            var miembroCuerpoTecnico = await _cuerpoTecnicoService.BuscarMiembroCuerpoTecnico(id);

            if (miembroCuerpoTecnico == null)
            {
                return NotFound();
            }

            return miembroCuerpoTecnico;
        }

        [HttpPost]
        public async Task<ActionResult<CuerpoTecnico>> Create(CuerpoTecnico miembroCuerpoTecnico)
        {
            await _cuerpoTecnicoService.CrearMiembroCuerpoTecnico(miembroCuerpoTecnico);

            return CreatedAtRoute("ObtenerMiembroCuerpoTecnico", new { id = miembroCuerpoTecnico.Id.ToString() }, miembroCuerpoTecnico);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, CuerpoTecnico miembroCuerpoTecnicoIn)
        {
            var miembroCuerpoTecnico = _cuerpoTecnicoService.BuscarMiembroCuerpoTecnico(id);

            if (miembroCuerpoTecnico == null)
            {
                return NotFound();
            }

            _cuerpoTecnicoService.ActualizarMiembroCuerpoTecnico(id,miembroCuerpoTecnicoIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var miembroCuerpoTecnico = _cuerpoTecnicoService.BuscarMiembroCuerpoTecnico(id);

            if (miembroCuerpoTecnico == null)
            {
                return NotFound();
            }

            _cuerpoTecnicoService.EliminarMiembroCuerpoTecnico(miembroCuerpoTecnico.Id.ToString());

            return NoContent();
        }
    }
}