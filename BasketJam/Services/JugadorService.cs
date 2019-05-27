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
        Task<List<Jugador>> ListarJugadores();
        Task<Jugador> BuscarJugador(string id);
        Task<Jugador> CrearJugador(Jugador jugador);
        void ActualizarJugador(string id, Jugador jug);
        void EliminarJugador(string id);
        Task<List<Jugador>> ListarJugadoresPorEquipo(string idEquipo);
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
        public async Task<List<Jugador>> ListarJugadores()
        {
            return await _jugadores.Find(jugador => true).ToListAsync();
        }

        public async Task<List<Jugador>> ListarJugadoresPorEquipo(string idEquipo)
        {
            return await _jugadores.Find(jugador => jugador.IdEquipo==idEquipo).ToListAsync();
        }

        public async Task<Jugador> BuscarJugador(string id)
        {
            return await _jugadores.Find<Jugador>(jugador =>jugador.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Jugador> CrearJugador(Jugador jugador)
        {
            await _jugadores.InsertOneAsync(jugador);
            return jugador;
        }

        public void ActualizarJugador(string id, Jugador jug)
        {
            _jugadores.ReplaceOne(jugador => jugador.Id == id, jug);
        }

        public void EliminarJugador(Jugador jug)
        {
            _jugadores.DeleteOne(jugador => jugador.Id == jug.Id);
        }

        public void EliminarJugador(string id)
        {
            _jugadores.DeleteOne(jugador => jugador.Id == id);
        }
    }
}