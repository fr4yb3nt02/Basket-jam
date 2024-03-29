using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using BasketJam.Models;

namespace BasketJam.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class NoticiaController : ControllerBase
    {
        private INoticiaService _noticiaService;

        public NoticiaController(INoticiaService noticiaService)
        {
            _noticiaService = noticiaService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Object>>> Get()
        {
            try
            {
                return await _noticiaService.ListarNoticias();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }

        [HttpGet("BuscarNoticia/{id:length(24)}")]
        public async Task<ActionResult<dynamic>> BuscarNoticia(string id)
        {
            try
            {
                return await _noticiaService.BuscarNoticia(id);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }

        [HttpGet("ListarNoticiasPorFecha")]
        public async Task<ActionResult<List<Object>>> ListarNoticiasPorFecha(DateTime fecha)
        {
            try
            {
                return await _noticiaService.ListarNoticiasPorFecha(fecha);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }

        [HttpGet("ListarUltimasDiez")]
        public async Task<ActionResult<List<Object>>> ListarUltimasDiez()
        {
            try
            {
                return await _noticiaService.ListarUltimasDiezNoticias();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }


        [HttpPost]
        public async Task<ActionResult<Noticia>> Create(Noticia noticia)
        {
            try
            {
                await _noticiaService.CrearNoticia(noticia);

                return noticia;
            }

            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });

            }
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Noticia noticiaIn)
        {
            try
            {
                var noticia = _noticiaService.BuscarNoticia(id);

                if (noticia == null)
                {
                    return NotFound(new { Error = "No se ha encontrado la noticia." });
                }

                _noticiaService.ActualizarNoticia(id, noticiaIn);

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
                Noticia noticia = _noticiaService.BuscarNoticia(id).Result;

                if (noticia == null)
                {
                    return NotFound(new { Error = "No se ha encontrado la noticia." });
                }

                _noticiaService.EliminarNoticia(noticia.Id);

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
        [HttpPost("subirFoto/")]
        public IActionResult subirFoto(Imagen img)
        {
            try
            {

                return Ok(new { Resultado = _noticiaService.subirImagen(img).Result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }
    }
}