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
    public interface ITorneoService
    {
        List<Torneo> ListarTorneos();
        Torneo BuscarTorneo(string id);
        Torneo CrearTorneo(Torneo equipo);
        void ActualizarTorneo(string id, Torneo tor);
        void EliminarTorneo(string id);
    }

    public class TorneoService : ITorneoService
{
        private readonly IMongoCollection<Torneo> _torneos;      

        public TorneoService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
             _torneos=database.GetCollection<Torneo>("torneos");

        }
        public List<Torneo> ListarTorneos()
        {
            return _torneos.Find(torneo => true).ToList();
        }

        public Torneo BuscarTorneo(string id)
        {
            return _torneos.Find<Torneo>(torneo => torneo.Id == id).FirstOrDefault();
        }

        public Torneo CrearTorneo(Torneo torneo)
        {
            _torneos.InsertOne(torneo);
            return torneo;
        }

        public void ActualizarTorneo(string id, Torneo tor)
        {
            _torneos.ReplaceOne(torneo => torneo.Id == id, tor);
        }

        public void EliminarTorneo(Torneo tor)
        {
            _torneos.DeleteOne(torneo => torneo.Id == tor.Id);
        }

        public void EliminarTorneo(string id)
        {
            _torneos.DeleteOne(torneo => torneo.Id == id);
        }
    }
}