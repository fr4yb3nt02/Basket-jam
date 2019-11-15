using BasketJam.Helper;
using BasketJam.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
        Task<List<ExpandoObject>> ListarJugadores();
        Task<Jugador> BuscarJugador(string id);
        Task<Jugador> CrearJugador(Jugador jugador);
        void ActualizarJugador(string id, Jugador jug);
        void EliminarJugador(string id);
        Task<List<Jugador>> ListarJugadoresPorEquipo(string idEquipo);
        Task<string> subirImagen(Imagen img);
    }

    public class JugadorService : IJugadorService
    {
        private readonly IMongoCollection<Jugador> _jugadores;
        private readonly IMongoCollection<Equipo> _equipos;

        public JugadorService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
            _jugadores = database.GetCollection<Jugador>("jugadores");
            _equipos = database.GetCollection<Equipo>("equipos");


        }
        public async Task<List<ExpandoObject>> ListarJugadores()
        {
            List<Jugador> jugadores = await _jugadores.Find(jugador => jugador.Activo == true).ToListAsync();
            List<ExpandoObject> jugs = new List<ExpandoObject>();
            foreach (Jugador j in jugadores)
            {
                Equipo e = await _equipos.Find<Equipo>(eq => eq.Id.Equals(j.IdEquipo)).FirstOrDefaultAsync();

                dynamic ju = new ExpandoObject();
                ju.id = j.Id;
                ju.Ci = j.Ci;
                ju.Nombre = j.Nombre;
                ju.Apellido = j.Apellido;
                ju.IdEquipo = e.Id;
                ju.NombreEquipo = e.NombreEquipo;
                ju.FechaDeNacimiento = j.FechaDeNacimiento;
                ju.Activo = j.Activo;
                ju.Capitan = j.Capitan;
                ju.Posicion = j.Posicion;
                ju.NumeroCamiseta = j.NumeroCamiseta;
                ju.Altura = j.Altura;
                ju.Peso = j.Peso;
                ju.Foto = j.UrlFoto;
                jugs.Add(ju);
            }
            return jugs;

        }

        public async Task<List<Jugador>> ListarJugadoresPorEquipo(string idEquipo)
        {
            return await _jugadores.Find(jugador => jugador.IdEquipo == idEquipo && jugador.Activo == true).ToListAsync();
        }

        public async Task<Jugador> BuscarJugador(string id)
        {
            return await _jugadores.Find<Jugador>(jugador => jugador.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Jugador> CrearJugador(Jugador jugador)
        {
            try
            {
                Jugador j = await _jugadores.Find<Jugador>(jug => jug.Ci.Equals(jugador.Ci)).FirstOrDefaultAsync();
                if (j != null)
                {
                    throw new Exception("Ya existe un jugador con la C.I ingresada.");
                }

                await _jugadores.InsertOneAsync(jugador);
                return jugador;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void ActualizarJugador(string id, Jugador jug)
        {
            _jugadores.ReplaceOne(jugador => jugador.Id == id, jug);
        }


        public async void EliminarJugador(string id)
        {
            await _jugadores.UpdateOneAsync(
                     ju => ju.Id.Equals(id),
                     Builders<Jugador>.Update.
                     Set(b => ((MiembroDeEquipo)b).Activo, false));
        }

        public async Task<string> subirImagen(Imagen img)
        {

            try
            {
                string claseImagen = "Jugadores";
                string url = ImagenService.subirImagen(img, claseImagen);
                await _jugadores.UpdateOneAsync(pa => pa.Id.Equals(img.Nombre),
                                                       Builders<Jugador>.Update.
                                                       Set(b => b.UrlFoto, url));
                return url;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
