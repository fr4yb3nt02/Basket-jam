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
        Task<List<Noticia>> ListarNoticias();
        Task<List<Noticia>> ListarNoticiasPorFecha(DateTime fecha);
        Task<Noticia> BuscarNoticia(string id);
        Task<Noticia> CrearNoticia(Noticia noticia);
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
        public async Task<List<Noticia>> ListarNoticias()
        {
            return await _noticias.Find(noticia => true).ToListAsync();
        }

        public async Task<List<Noticia>> ListarNoticiasPorFecha(DateTime fecha)
        {
            return await _noticias.Find<Noticia>(noticia => noticia.Fecha==fecha).ToListAsync();            
        }

        public async Task<Noticia> CrearNoticia(Noticia noticia)
        {
            await _noticias.InsertOneAsync(noticia);
            return noticia;
        }

        public async Task<Noticia> BuscarNoticia(string id)
        {
            return await _noticias.Find<Noticia>(noticia => noticia.Id == id).FirstOrDefaultAsync();
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