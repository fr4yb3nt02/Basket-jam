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
    public interface IPartidoService
    {
        List<Partido> ListarPartidos();
        Partido BuscarPartido(string id);
        Partido CrearPartido(Partido equipo);
        void ActualizarPartido(string id, Partido pa);
        void EliminarPartido(string id);
    }

    public class PartidoService : IPartidoService
{
        private readonly IMongoCollection<Partido> _partidos;      

        public PartidoService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
             _partidos=database.GetCollection<Partido>("partidos");

        }
        public List<Partido> ListarPartidos()
        {
            return _partidos.Find(partido => true).ToList();
        }

        public Partido BuscarPartido(string id)
        {
            return _partidos.Find<Partido>(partido => partido.Id == id).FirstOrDefault();
        }

        public Partido CrearPartido(Partido partido)
        {
            _partidos.InsertOne(partido);
            return partido;
        }

        public void ActualizarPartido(string id, Partido pa)
        {
            _partidos.ReplaceOne(partido => partido.Id == id, pa);
        }

        public void EliminarPartido(Partido pa)
        {
            _partidos.DeleteOne(partido => partido.Id == pa.Id);
        }

        public void EliminarPartido(string id)
        {
            _partidos.DeleteOne(partido => partido.Id == id);
        }
    }
}