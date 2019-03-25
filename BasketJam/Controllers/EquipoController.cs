using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EquipoController : ControllerBase
    {
        private IEquipoService _equipoService;

        public EquipoController(IEquipoService equipoService)
        {
            _equipoService = equipoService;
        }

       [HttpGet]
        public ActionResult<List<Equipo>> Get()
        {
            return _equipoService.ListarEquipos();
        }

        [HttpGet("{id:length(24)}", Name = "ObtenerEquipo")]
        public ActionResult<Equipo> Get(string id)
        {
            var equipo = _equipoService.BuscarEquipo(id);

            if (equipo == null)
            {
                return NotFound();
            }

            return equipo;
        }

        [HttpPost]
        public ActionResult<Equipo> Create(Equipo equipo)
        {
            _equipoService.CrearEquipo(equipo);

            return CreatedAtRoute("ObtenerEquipo", new { id = equipo.Id.ToString() }, equipo);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Equipo equipoIn)
        {
            var equipo = _equipoService.BuscarEquipo(id);

            if (equipo == null)
            {
                return NotFound();
            }

            _equipoService.ActualizarEquipo(id, equipoIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var equipo = _equipoService.BuscarEquipo(id);

            if (equipo == null)
            {
                return NotFound();
            }

            _equipoService.EliminarEquipo(equipo.Id);

            return NoContent();
        }
    }
}