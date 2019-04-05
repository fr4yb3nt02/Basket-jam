using BasketJam.Helper;
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
        List<Noticia> ListarNoticias();
        List<Noticia> ListarNoticiasPorFecha(DateTime fecha);
        Noticia BuscarNoticia(string id);
        Noticia CrearNoticia(Noticia noticia);
        void ActualizarNoticia(string id, Noticia not);
        void EliminarNoticia(string id);
    }

    public class NoticiaService : INoticiaService
{
        private readonly IMongoCollection<Noticia> _noticias;      

        public NoticiaService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
            _noticias=database.GetCollection<Noticia>("noticias");

        }
        public List<Noticia> ListarNoticias()
        {
            return _noticias.Find(noticia => true).ToList();
        }

        public List<Noticia> ListarNoticiasPorFecha(DateTime fecha)
        {
            return _noticias.Find<Noticia>(noticia => noticia.Fecha==fecha).ToList();            
        }

        public Noticia CrearNoticia(Noticia noticia)
        {
            _noticias.InsertOne(noticia);
            return noticia;
        }

            public Noticia BuscarNoticia(string id)
        {
            return _noticias.Find<Noticia>(noticia => noticia.Id == id).FirstOrDefault();
        }
        public void ActualizarNoticia(string id, Noticia not)
        {
            _noticias.ReplaceOne(noticia => noticia.Id == id, not);
        }

        public void EliminarNoticia(Noticia not)
        {
            _noticias.DeleteOne(noticia => noticia.Id == not.Id);
        }

        public void EliminarNoticia(string id)
        {
            _noticias.DeleteOne(noticia => noticia.Id == id);
        }
    }
}