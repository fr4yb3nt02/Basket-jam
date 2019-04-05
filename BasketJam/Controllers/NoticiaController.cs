using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;
using System;

namespace WebApi.Controllers
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
        public ActionResult<List<Noticia>> Get()
        {
            return _noticiaService.ListarNoticias();
        }

        [HttpGet]
        public ActionResult<List<Noticia>> Get(DateTime fecha)
        {
            return _noticiaService.ListarNoticiasPorFecha(fecha);
        }


        [HttpPost("crearNoticia")]
        public ActionResult<Noticia> Create(Noticia noticia)
        {
            _noticiaService.CrearNoticia(noticia);

            return noticia;
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Noticia noticiaIn)
        {
            var noticia = _noticiaService.BuscarNoticia(id);

            if (noticia == null)
            {
                return NotFound();
            }

            _noticiaService.ActualizarNoticia(id,noticiaIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var noticia = _noticiaService.BuscarNoticia(id);

            if (noticia == null)
            {
                return NotFound();
            }

            _noticiaService.EliminarNoticia(noticia.Id);

            return NoContent();
        }
    }
}