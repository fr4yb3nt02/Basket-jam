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
        List<Juez> ListarJueces();
        Juez BuscarJuez(string id);
        Juez CrearJuez(Juez equipo);
        void ActualizarJuez(string id, Juez eq);
        void EliminarJuez(string id);
    }

    public class JuezService : IJuezService
{
        private readonly IMongoCollection<Juez> _jueces;      

        public JuezService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
             _jueces=database.GetCollection<Juez>("jueces");

        }
        public List<Juez> ListarJueces()
        {
            return _jueces.Find(juez => true).ToList();
        }

        public Juez BuscarJuez(string id)
        {
            return _jueces.Find<Juez>(juez => juez.Id == id).FirstOrDefault();
        }

        public Juez CrearJuez(Juez juez)
        {
            _jueces.InsertOne(juez);
            return juez;
        }

        public void ActualizarJuez(string id, Juez eq)
        {
            _jueces.ReplaceOne(juez => juez.Id == id, eq);
        }

        public void EliminarJuez(Juez eq)
        {
            _jueces.DeleteOne(juez => juez.Id == eq.Id);
        }

        public void EliminarJuez(string id)
        {
            _jueces.DeleteOne(juez => juez.Id == id);
        }
    }
}