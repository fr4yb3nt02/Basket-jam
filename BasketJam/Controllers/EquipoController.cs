using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.IO;
using System;
using BasketJam.Models;
using System.Dynamic;

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
        public async Task<ActionResult<List<ExpandoObject>>> Get()
        {
            try
            {
                return await _equipoService.ListarEquipos();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }

        [HttpGet("ListarEquiposPorTorneo/{id:length(24)}")]        
        public async Task<ActionResult<List<ExpandoObject>>> ListarEquiposPorTorneo(string id)
        {
            try
            {
                return await _equipoService.ListarEquiposPorTorneo(id);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
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
            try
            {
                // Estadio e=new Estadio();
                // e.Nombre=equipo.Estadio.Nombre;
                // e.Direccion=equipo.Estadio.Direccion;
                // e.Id=equipo.Estadio.Id;
                //Estadio estadio=new Estadio(equipo.Estadio.Id,equipo.Estadio.Nombre,equipo.Estadio.Nombre);

                await _equipoService.CrearEquipo(equipo);

                return CreatedAtRoute("ObtenerEquipo", new { id = equipo.Id.ToString() }, equipo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }


        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Equipo equipoIn)
        {
            try
            {
                var equipo = _equipoService.BuscarEquipo(id);

                if (equipo == null)
                {
                    return NotFound(new { Error = "No se ha encontrado el equipo." });
                }

                _equipoService.ActualizarEquipo(id, equipoIn);

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
                var equipo = _equipoService.BuscarEquipo(id);

                if (equipo == null)
                {
                    return NotFound(new { Error = "No se ha encontrado el equipo." });
                }

                _equipoService.EliminarEquipo(equipo.Id.ToString());

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
                _equipoService.subirImagen(img);
                return Ok(new { Resultado = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }
    }
}