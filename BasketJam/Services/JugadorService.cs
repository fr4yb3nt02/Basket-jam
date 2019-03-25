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
    public interface IJugadorService
    {
        List<Jugador> ListarJugadores();
        Jugador BuscarJugador(string id);
        Jugador CrearJugador(Jugador equipo);
        void ActualizarJugador(string id, Jugador eq);
        void EliminarJugador(string id);
    }

    public class JugadorService : IJugadorService
{
        private readonly IMongoCollection<Jugador> _jugadores;      

        public JugadorService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
             _jugadores=database.GetCollection<Jugador>("jugadores");

        }
        public List<Jugador> ListarJugadores()
        {
            return _jugadores.Find(jugador => true).ToList();
        }

        public Jugador BuscarJugador(string id)
        {
            return _jugadores.Find<Jugador>(jugador => jugador.Id == id).FirstOrDefault();
        }

        public Jugador CrearJugador(Jugador jugador)
        {
            _jugadores.InsertOne(jugador);
            return jugador;
        }

        public void ActualizarJugador(string id, Jugador eq)
        {
            _jugadores.ReplaceOne(jugador => jugador.Id == id, eq);
        }

        public void EliminarJugador(Jugador eq)
        {
            _jugadores.DeleteOne(jugador => jugador.Id == eq.Id);
        }

        public void EliminarJugador(string id)
        {
            _jugadores.DeleteOne(jugador => jugador.Id == id);
        }
    }
}