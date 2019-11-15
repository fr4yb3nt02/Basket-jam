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
    public interface IJuezService
    {
        Task<List<Juez>> ListarJueces();
        Task<Juez> BuscarJuez(string id);
        Task<Juez> CrearJuez(Juez equipo);
        void ActualizarJuez(string id, Juez eq);
        void EliminarJuez(string id);
    }

    public class JuezService : IJuezService
    {
        private readonly IMongoCollection<Juez> _jueces;
        private readonly IMongoCollection<Partido> _partidos;

        public JuezService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
            _jueces = database.GetCollection<Juez>("jueces");
            _partidos = database.GetCollection<Partido>("partidos");

        }
        public async Task<List<Juez>> ListarJueces()
        {
            return await _jueces.Find(juez => juez.Activo == true).ToListAsync();
        }

        public async Task<Juez> BuscarJuez(string id)
        {
            return await _jueces.Find<Juez>(juez => juez.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Juez> CrearJuez(Juez juez)
        {
            try
            {
                Juez ju = await _jueces.Find<Juez>(j => j.Ci.Equals(juez.Ci)).FirstOrDefaultAsync();
                if (ju != null)
                {
                    throw new Exception("Ya existe un juez con la C.I ingresada.");
                }
                await _jueces.InsertOneAsync(juez);
                return juez;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void ActualizarJuez(string id, Juez eq)
        {
            _jueces.ReplaceOne(juez => juez.Id == id, eq);
        }

        public async void EliminarJuez(string id)
        {

            await _jueces.UpdateOneAsync(
                             ju => ju.Id.Equals(id),
                             Builders<Juez>.Update.
                             Set(b => b.Activo, false));
        }

    }
}