using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Dynamic;
using BasketJam.Models;

namespace BasketJam.Controllers
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
        public async Task<List<ExpandoObject>> Get()
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
            try
            {
                await _cuerpoTecnicoService.CrearMiembroCuerpoTecnico(miembroCuerpoTecnico);

                return CreatedAtRoute("ObtenerMiembroCuerpoTecnico", new { id = miembroCuerpoTecnico.Id.ToString() }, miembroCuerpoTecnico);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, CuerpoTecnico miembroCuerpoTecnicoIn)
        {
            try
            {
                var miembroCuerpoTecnico = _cuerpoTecnicoService.BuscarMiembroCuerpoTecnico(id);

                if (miembroCuerpoTecnico == null)
                {
                    return NotFound();
                }

                _cuerpoTecnicoService.ActualizarMiembroCuerpoTecnico(id, miembroCuerpoTecnicoIn);

                return Ok(new { Resultado = true });
            }

            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            try
            {
                var miembroCuerpoTecnico = _cuerpoTecnicoService.BuscarMiembroCuerpoTecnico(id);

                if (miembroCuerpoTecnico == null)
                {
                    return NotFound();
                }

                _cuerpoTecnicoService.EliminarMiembroCuerpoTecnico(id);

                return Ok(new { Resultado = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("subirFoto/")]
        public IActionResult subirFoto(Imagen img)
        {
            try
            {
                
                return Ok(new { Resultado = _cuerpoTecnicoService.subirImagen(img).Result
            });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }
    }
}