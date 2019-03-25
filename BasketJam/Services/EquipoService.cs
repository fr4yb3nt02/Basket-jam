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
    public interface IEquipoService
    {
        List<Equipo> ListarEquipos();
        Equipo BuscarEquipo(string id);
        Equipo CrearEquipo(Equipo equipo);
        void ActualizarEquipo(string id, Equipo eq);
        void EliminarEquipo(string id);
    }

    public class EquipoService : IEquipoService
{
        private readonly IMongoCollection<Equipo> _equipos;      

        public EquipoService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
             _equipos=database.GetCollection<Equipo>("equipos");

        }
        public List<Equipo> ListarEquipos()
        {
            return _equipos.Find(equipo => true).ToList();
        }

        public Equipo BuscarEquipo(string id)
        {
            return _equipos.Find<Equipo>(equipo => equipo.Id == id).FirstOrDefault();
        }

        public Equipo CrearEquipo(Equipo equipo)
        {
            _equipos.InsertOne(equipo);
            return equipo;
        }

        public void ActualizarEquipo(string id, Equipo eq)
        {
            _equipos.ReplaceOne(equipo => equipo.Id == id, eq);
        }

        public void EliminarEquipo(Equipo eq)
        {
            _equipos.DeleteOne(equipo => equipo.Id == eq.Id);
        }

        public void EliminarEquipo(string id)
        {
            _equipos.DeleteOne(equipo => equipo.Id == id);
        }
    }
}