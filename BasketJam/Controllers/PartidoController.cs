using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using BasketJam.Models;

namespace BasketJam.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PartidoController : ControllerBase
    {
        private IPartidoService _partidoService;

        public PartidoController(IPartidoService partidoService)
        {
            _partidoService = partidoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Partido>>> Get()
        {
            try
            {
                return await _partidoService.ListarPartidos();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }

        [HttpGet("{id:length(24)}", Name = "ObtenerPartido")]
        public async Task<ActionResult<Partido>> Get(string id)
        {
            try
            {
                var partido = await _partidoService.BuscarPartido(id);

                if (partido == null)
                {
                    return NotFound(new { Error = "No se ha encontrado el partio." });
                }

                return partido;
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
        public async Task<ActionResult<Partido>> Create(Partido partido)
        {
            try
            {
                await _partidoService.CrearPartido(partido);

                return CreatedAtRoute("ObtenerPartido", new { id = partido.Id.ToString() }, partido);
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
        public IActionResult Update(string id, Partido partidoIn)
        {
            try
            {
                var partido = _partidoService.BuscarPartido(id);

                if (partido == null)
                {
                    return NotFound(new { Error = "No se ha encontrado el partio." });
                }

                _partidoService.ActualizarPartido(id, partidoIn);

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

        //[AllowAnonymous]
        [HttpPut("ActualizarTiempoPartido/")]
        public IActionResult ActualizarTiempoPartido(string idpartido, string tiempo)
        {
            try
            {
                var partido = _partidoService.BuscarPartido(idpartido);

                if (partido == null)
                {
                    return NotFound(new { Error = "No se ha encontrado el partio." });
                }

                _partidoService.ActualizarTiempoPartido(idpartido, tiempo);
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

        [HttpPut("ActualizarCuartoPartido/")]
        public IActionResult ActualizarCuartoPartido(string idpartido, int cuarto)
        {
            try
            {
                var partido = _partidoService.BuscarPartido(idpartido);

                if (partido == null)
                {
                    return NotFound(new { Error = "No se ha encontrado el partio." });
                }

                _partidoService.ActualizarCuartoPartido(idpartido, cuarto);
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


        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            try
            {
                var partido = _partidoService.BuscarPartido(id);

                if (partido == null)
                {
                    return NotFound(new { Error = "No se ha encontrado el partio." });
                }

                _partidoService.EliminarPartido(partido.Id.ToString());

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


        [HttpPut("AgregarJuezAPartido/{id:length(24)}")]
        public async Task<ActionResult<bool>> AgregarJuezAPartido(string id, [FromBody]List<Juez> jueces)
        {
            try
            {
                var partido = await _partidoService.BuscarPartido(id);

                if (partido == null)
                {
                    return NotFound(new { Error = "No se ha encontrado el partio." });
                }

                Boolean res = await _partidoService.AgregarJuezPartida(id, jueces);


                if (res)
                    return Ok(new { mensaje = "Se han agregado los jueces correctamente." });
                else
                    return BadRequest(new { error = "No se ha podido realizar la acción." });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }

        [HttpPut("AgregarJugadoresAPartido/{id:length(24)}")]
        public async Task<ActionResult<bool>> AgregarJugadoresAPartido(string id, [FromBody]List<EquipoJugador> jugadores)
        {
            try
            {
                var partido = await _partidoService.BuscarPartido(id);

                if (partido == null)
                {
                    return NotFound(new { Error = "No se ha encontrado el partio." });
                }

                Boolean res = await _partidoService.AgregarJugadoresAPartido(id, jugadores);

                if (res)
                    return Ok(new { mensaje = "Se han asignado los jugadores de manera correcta." });
                else
                    return BadRequest(new { error = "No se ha podido realizar la acción." });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }

        }

        [AllowAnonymous]
        [HttpGet("Listpart")]
        public async Task<ActionResult> visualizadorPartidos()
        {
            try
            {
                //  List<String> a = await _partidoService.DevuelvoListPartidosAndroid();
                return Ok(await _partidoService.DevuelvoListPartidosAndroid());
                //return Ok(_partidoService.DevuelvoListPartidosAndroid);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }

        //[AllowAnonymous]
        [HttpGet("ListPartidosProgOJug")]
        public async Task<ActionResult> ListarPartidosProgOJug()
        {
            try
            {
                return Ok(await _partidoService.ListarPartidosProgOJug());
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }

        //[AllowAnonymous]
        [HttpGet("ListarPartidosPorEstado")]
        public async Task<ActionResult> ListarPartidosPorEstado(int estado)
        {
            try
            {
                return Ok(await _partidoService.ListarPartidosPorEstado(estado));
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }

        //[AllowAnonymous]
        [HttpGet("ListarPartidosSinJueces")]
        public async Task<ActionResult> ListarPartidosSinJueces()
        {
            try
            {
                return Ok(await _partidoService.ListarPartidosSinJueces());
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpGet("FixtureTodosLosEquipos")]
        public async Task<ActionResult> FixtureTodosLosEquipos()
        {
            try
            {
                return Ok(await _partidoService.FixtureTodosLosEquipos());
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpGet("FixturePorEquipo/{id:length(24)}")]
        public async Task<ActionResult> FixturePorEquipo(string id)
        {
            try
            {
                return Ok(await _partidoService.FixturePorEquipo(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpGet("ConsultarHeaderPartido/{id:length(24)}")]
        public async Task<ActionResult> ConsultarHeaderPartido(string id)
        {
            try
            {
                return Ok(await _partidoService.ConsultarHeaderPartido(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpGet("ConsultaDetallesPartido/{id:length(24)}")]
        public async Task<ActionResult> ConsultaDetallesPartido(string id)
        {
            try
            {
                return Ok(await _partidoService.ConsultaDetallesPartido(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpGet("UltimosEventosEquipo/{id:length(24)}")]
        public async Task<ActionResult> UltimosEventosEquipo(string id)
        {
            try
            {
                return Ok(await _partidoService.UltimosEventosEquipo(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpGet("ListarEquiposJugador/{id:length(24)}")]
        public async Task<ActionResult> ListarEquiposJugador(string id)
        {
            try
            {
                return Ok(await _partidoService.ListarEquipoJugador(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }

        }

        [AllowAnonymous]
        [HttpPut("ActualizarestadoPartido/")]
        public  IActionResult ActualizarEstadoPartido(string id, string tiempo)
        {
            try
            {

                Partido p = _partidoService.BuscarPartido(id).Result;
                int cuarto = p.cuarto;
                string time=p.Tiempo;
                int estado = Convert.ToInt32(p.estado);
                _partidoService.ActualizarEstadoPartido(id, tiempo);
                
                if(p.estado == (EstadoPartido)0 || (p.estado == (EstadoPartido)2 & tiempo == "10:00"))
                    {
                    cuarto = p.cuarto;
                    time = "10:00";
                    estado = 1;
                    }
                if (p.estado == (EstadoPartido)1 & tiempo == "00:00" & p.cuarto != 4)
                {
                    estado = 2;
                    cuarto = p.cuarto+1;
                    time = "10:00";
                }
                if (p.estado == (EstadoPartido)2 & tiempo != "00:00")
                {
                    estado = 1;
                    cuarto = p.cuarto;
                    time = p.Tiempo;
                }
                if (p.estado == (EstadoPartido)1 & tiempo == "00:00" & p.cuarto == 4)
                {
                    estado = 3;
                    cuarto = 4;
                    time = "00:00";
                }

                //return Ok(new { Resultado = true, CuartoPartido = p.cuarto, Estado=p.estado });
                return Ok(new { Resultado = true, CuartoPartido = cuarto, Estado = estado });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpGet("ListarJugadoresEquiposPartido/{id:length(24)}")]
        public async Task<List<Object>> ListarJugadoresEquiposPartido(string id)
        {
            try
            {
                return await _partidoService.ListarJugadoresEquiposPartido(id);

            }
            catch (Exception ex)
            {
                throw new Exception(

                    "Se ha producido un error: " + ex.Message
                 );
            }
        }

        //[AllowAnonymous]
        [HttpGet("ListarEstadios/")]
        public async Task<List<Object>> ListarEstadios()
        {
            try
            {
                return await _partidoService.ListarEstadios();

            }
            catch (Exception ex)
            {
                throw new Exception(

                    "Se ha producido un error: " + ex.Message
                 );
            }
        }


        [AllowAnonymous]
        [HttpGet("EstadisticasJugsEquipoPartido")]
        public async Task<List<Object>> ListarEsEstadisticasJugsEquipoPartidotadios(string idPartido, string idEquipo)
        {
            try
            {
                return await _partidoService.EstadisticasJugsEquipoPartido(idPartido, idEquipo);

            }
            catch (Exception ex)
            {
                throw new Exception(

                    "Se ha producido un error: " + ex.Message
                 );
            }
        }

        [AllowAnonymous]
        [HttpGet("DevolverEstadoActualPartido/{id:length(24)}")]
        public async Task<ActionResult<Object>> DevolverEstadoActualPartido(string id)
        {
            try
            {

                return Ok(await _partidoService.DevolverEstadoPartido(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }

        [HttpPut("DevolverPartidoEstadoInicial/")]
        public IActionResult DevolverPartidoEstadoInicial(string idpartido)
        {
            try
            {
                _partidoService.DevolverPartidoEstadoInicial(idpartido);
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