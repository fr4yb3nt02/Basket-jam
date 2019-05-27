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
        Task<List<Torneo>> ListarTorneos();
        Task<Torneo> BuscarTorneo(string id);
        Task<Torneo> CrearTorneo(Torneo equipo);
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
        public async Task<List<Torneo>> ListarTorneos()
        {
            return await _torneos.Find(torneo => true).ToListAsync();
        }

        public async Task<Torneo> BuscarTorneo(string id)
        {
            return await _torneos.Find<Torneo>(torneo => torneo.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Torneo> CrearTorneo(Torneo torneo)
        {
            await _torneos.InsertOneAsync(torneo);
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