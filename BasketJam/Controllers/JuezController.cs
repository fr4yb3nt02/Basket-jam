using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace BasketJam.Controllers
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
            try
            {
                var juez = await _juezService.BuscarJuez(id);

                if (juez == null)
                {
                    return NotFound(new { Error = "No se ha encontrado el juez." });
                }

                return juez;
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
        public async Task<ActionResult<Juez>> Create(Juez juez)
        {
            try
            {
                await _juezService.CrearJuez(juez);

                return CreatedAtRoute("ObtenerJuez", new { id = juez.Id.ToString() }, juez);
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
        public IActionResult Update(string id, Juez juezIn)
        {
            try
            {
                var juez = _juezService.BuscarJuez(id);

                if (juez == null)
                {
                    return NotFound(new { Error = "No se ha encontrado el juez." });
                }

                _juezService.ActualizarJuez(id, juezIn);

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
                var juez = _juezService.BuscarJuez(id);

                if (juez == null)
                {
                    return NotFound(new { Error = "No se ha encontrado el juez." });
                }

                _juezService.EliminarJuez(id);

                return Ok(new { Resultado = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }
    }
}