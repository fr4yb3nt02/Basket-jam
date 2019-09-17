using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

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
            try
            {
                return await _torneoService.ListarTorneos();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }

        [HttpGet("{id:length(24)}", Name = "ObtenerTorneo")]
        public async Task<ActionResult<Torneo>> Get(string id)
        {
            try
            {
                var torneo = await _torneoService.BuscarTorneo(id);

                if (torneo == null)
                {
                    return NotFound(new { Error = "No se ha encontrado el torneo." });
                }

                return torneo;
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Torneo>> Create([FromBody]Torneo torneo)
        {
            try
            {
                await _torneoService.CrearTorneo(torneo);

                return CreatedAtRoute("ObtenerTorneo", new { id = torneo.Id.ToString() }, torneo);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Torneo torneoIn)
        {
            try
            {
                var torneo = _torneoService.BuscarTorneo(id);

                if (torneo == null)
                {
                    return NotFound(new { Error = "No se ha encontrado el torneo." });
                }

                _torneoService.ActualizarTorneo(id, torneoIn);

                return Ok(new { Resultado= true});
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            try
            {
                var torneo = _torneoService.BuscarTorneo(id);

                if (torneo == null)
                {
                    return NotFound(new { Error = "No se ha encontrado el torneo." });
                }

                _torneoService.EliminarTorneo(torneo.Id.ToString());

                return Ok(new { Resultado = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }
    }
}