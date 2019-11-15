using BasketJam.Helper;
using BasketJam.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi.Helpers;

namespace BasketJam.Services
{
    public interface INoticiaService
    {
        Task<List<dynamic>> ListarNoticias();
        Task<List<Object>> ListarNoticiasPorFecha(DateTime fecha);
        Task<Noticia> BuscarNoticia(string id);
        Task<Noticia> CrearNoticia(Noticia noticia);
        void ActualizarNoticia(string id, Noticia not);
        void EliminarNoticia(string id);
        Task<List<Object>> ListarUltimasDiezNoticias();
        Task<string> subirImagen(Imagen img);
    }

    public class NoticiaService : INoticiaService
    {
        private readonly IMongoCollection<Noticia> _noticias;

        public NoticiaService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
            _noticias = database.GetCollection<Noticia>("noticias");

        }
        public async Task<List<dynamic>> ListarNoticias()
        {
            List<Noticia> noticias = await _noticias.Find(noticia => true).ToListAsync();
            List<dynamic> paraDev = new List<dynamic>();

            foreach (Noticia no in noticias)
            {
                var dat = new
                {

                    id = no.Id,
                    titulo = no.Titulo,
                    contenidoAbreviado = no.ContenidoAbreviado,
                    contenido = no.Contenido,
                    fecha = no.Fecha,
                    imagen = no.UrlFoto
                };
                paraDev.Add(dat);
            }
            return paraDev;
        }

        public async Task<List<Object>> ListarNoticiasPorFecha(DateTime fecha)
        {
            List<Noticia> noticias = await _noticias.Find<Noticia>(noticia => noticia.Fecha == fecha).ToListAsync();
            List<Object> paraDev = new List<Object>();
            foreach (Noticia no in noticias)
            {
                var dat = new
                {

                    id = no.Id,
                    titulo = no.Titulo,
                    contenidoAbreviado = no.ContenidoAbreviado,
                    contenido = no.Contenido,
                    fecha = no.Fecha,
                    imagen = no.UrlFoto
                };
                paraDev.Add(dat);
            }
            return paraDev;
        }

        public async Task<List<Object>> ListarUltimasDiezNoticias()
        {
            List<Noticia> noticias = await _noticias.Find<Noticia>(noticia => true).ToListAsync();
            List<Object> paraDev = new List<Object>();
            var ultimas10 = (from n in noticias
                             orderby n.Fecha descending
                             select n
                            ).Take(10);
            foreach (Noticia no in ultimas10)
            {
                var dat = new
                {

                    id = no.Id,
                    titulo = no.Titulo,
                    contenidoAbreviado = no.ContenidoAbreviado,
                    contenido = no.Contenido,
                    fecha = no.Fecha,
                    imagen = no.UrlFoto
                };
                paraDev.Add(dat);
            }
            return paraDev;
        }

        public async Task<Noticia> CrearNoticia(Noticia noticia)
        {
            await _noticias.InsertOneAsync(noticia);
            return noticia;
        }

        public async Task<Noticia> BuscarNoticia(string id)
        {

            Noticia not = await _noticias.Find<Noticia>(noticia => noticia.Id == id).FirstOrDefaultAsync();
            return not;
        }
        public void ActualizarNoticia(string id, Noticia not)
        {
            _noticias.ReplaceOne(noticia => noticia.Id == id, not);
        }


        public void EliminarNoticia(string id)
        {
            _noticias.DeleteOne(noticia => noticia.Id == id);
        }

        public async Task<string> subirImagen(Imagen img)
        {

            try
            {
                string claseImagen = "Noticias";
                string url = ImagenService.subirImagen(img, claseImagen);
                await _noticias.UpdateOneAsync(pa => pa.Id.Equals(img.Nombre),
                       Builders<Noticia>.Update.
                       Set(b => b.UrlFoto, url));
                return url;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}