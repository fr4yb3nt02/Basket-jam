using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.IO;

namespace BasketJam.Controllers
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
        public async Task<ActionResult<List<Equipo>>> Get()
        {
            return await _equipoService.ListarEquipos();
        }

        [HttpGet("{id:length(24)}", Name = "ObtenerEquipo")]
        public async Task<ActionResult<Equipo>> Get(string id)
        {
            var equipo = await _equipoService.BuscarEquipo(id);

            if (equipo == null)
            {
                return NotFound();
            }

            return equipo;
        }

        //[Route("listarJugadores")]
        [HttpGet("listarJugadores/{id:length(24)}")]
        public async Task<ActionResult<List<Jugador>>> ListarJugadoresEquipo(string id)
        {
            var jugadores = await _equipoService.ListarJugadoresEquipo(id);

            if (jugadores == null)
            {
                return NotFound();
            }

            return jugadores;
        }

        [HttpPost]
        public async Task<ActionResult<Equipo>> Create(Equipo equipo)
        {
           // Estadio e=new Estadio();
           // e.Nombre=equipo.Estadio.Nombre;
           // e.Direccion=equipo.Estadio.Direccion;
           // e.Id=equipo.Estadio.Id;
            //Estadio estadio=new Estadio(equipo.Estadio.Id,equipo.Estadio.Nombre,equipo.Estadio.Nombre);
            await _equipoService.CrearEquipo(equipo);

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

            _equipoService.EliminarEquipo(equipo.Id.ToString());

            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("subirFoto")]
  public async Task<IActionResult> subirFoto(IFormFile file)  
  {  
      if (file == null || file.Length == 0)  
          return Content("No se ha adjuntado imágen.");  
  

         var filePath = Path.GetTempFileName();

      var path = Path.Combine(  
                  Directory.GetCurrentDirectory(), "wwwroot",   
                  file.FileName);  
  
      using (var stream = new FileStream(path, FileMode.Create))  
      {  
          await file.CopyToAsync(stream);  
      }  
  
      return RedirectToAction("Files");  
  } 
    }
}