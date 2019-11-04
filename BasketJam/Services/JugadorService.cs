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
         void subirImagen(Imagen img);
    }

    public class JugadorService : IJugadorService
{
        private readonly IMongoCollection<Jugador> _jugadores;
        private readonly IMongoCollection<Equipo> _equipos;

        public JugadorService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
            _jugadores=database.GetCollection<Jugador>("jugadores");
            _equipos = database.GetCollection<Equipo>("equipos");


        }
        public async Task<List<ExpandoObject>> ListarJugadores()
        {
            List<Jugador> jugadores= await _jugadores.Find(jugador => jugador.Activo==true).ToListAsync();
            List<ExpandoObject> jugs = new List<ExpandoObject>();
            foreach (Jugador j in jugadores)
            {
                Equipo e = await _equipos.Find<Equipo>(eq => eq.Id.Equals(j.IdEquipo)).FirstOrDefaultAsync();
                string imgUrl = ImagenService.buscarImagen(j.Id, "Jugadores");

                dynamic ju = new ExpandoObject();
                ju.id = j.Id;
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
                ju.Foto = imgUrl;
                jugs.Add(ju);
            }
            return jugs;

        }

        public async Task<List<Jugador>> ListarJugadoresPorEquipo(string idEquipo)
        {
            return await _jugadores.Find(jugador => jugador.IdEquipo==idEquipo && jugador.Activo==true).ToListAsync();
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


        public async void EliminarJugador(string id)
        {
            await _jugadores.UpdateOneAsync(
                     ju => ju.Id.Equals(id),
                     Builders<Jugador>.Update.
                     Set(b => ((MiembroDeEquipo)b).Activo, false));
        }

        public void subirImagen(Imagen img)
        {

            try
            {
                string claseImagen = "Jugadores";
                ImagenService.subirImagen(img, claseImagen);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
                /*
                try
                {
                    Account account = new Account(
                                         "dregj5syg",
                                         "373562826237252",
                                          "pyLkt3TJd5dlmm1krFbwkb1g5Ws");


                    Cloudinary cloudinary = new Cloudinary(account);
                    var uploadParams = new ImageUploadParams()
                    {

                        File = new FileDescription(img.ImgBase64),
                        PublicId = "Equipos/" + img.Nombre,
                        Overwrite = true,

                    };
                    var uploadResult = cloudinary.Upload(uploadParams);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }*/
            }
    }
