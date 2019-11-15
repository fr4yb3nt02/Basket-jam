using BasketJam.Helper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FluentFTP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using BasketJam.Models;
using System.Web;
using System.Dynamic;

namespace BasketJam.Services
{
    public interface IEquipoService
    {
        Task<List<ExpandoObject>> ListarEquipos();
        Task<Equipo> BuscarEquipo(string id);
        Task<Equipo> CrearEquipo(Equipo equipo);
        void ActualizarEquipo(string id, Equipo eq);
        void EliminarEquipo(string id);
        Task<string> subirImagen(Imagen img);
        Task<List<ExpandoObject>> ListarEquiposPorTorneo(string idTorneo);

        Task<List<Jugador>> ListarJugadoresEquipo(string id);
    }

    public class EquipoService : IEquipoService
    {
        private readonly IMongoCollection<Equipo> _equipos;
        private readonly IMongoCollection<Jugador> _jugadores;

        private readonly IGridFSBucket _bucket;
        private readonly string entidadParaImagen = "Equipos";

        public EquipoService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
            var bucket = new GridFSBucket(database);
            _bucket = bucket;
            _equipos = database.GetCollection<Equipo>("equipos");
            _jugadores = database.GetCollection<Jugador>("jugadores");

        }

        public async Task<string> subirImagen(Imagen img)
        {
            try
            {
                string claseImagen = "Equipos";
                string url = ImagenService.subirImagen(img, claseImagen);
                await _equipos.UpdateOneAsync(pa => pa.Id.Equals(img.Nombre),
                                                      Builders<Equipo>.Update.
                                                      Set(b => b.UrlFoto, url));
                return url;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<List<ExpandoObject>> ListarEquipos()
        {
            List<Equipo> equipos = await _equipos.Find(equipo => equipo.Activo == true).ToListAsync();
            List<ExpandoObject> eqConFotos = new List<ExpandoObject>();
            foreach (Equipo e in equipos)
            {

                dynamic eq = new ExpandoObject();
                dynamic estadio = new ExpandoObject();
                eq.Id = e.Id;
                eq.Nombre = e.NombreEquipo;
                eq.FechaFundacion = e.FechaFundacion;
                eq.ColorCaracteristico = e.ColorCaracteristico;
                eq.Categoria = e.Categoria;
                estadio = e.Estadio;
                eq.Estadio = estadio;
                eq.foto = e.UrlFoto;
                eqConFotos.Add(eq);
            }
            return eqConFotos;
        }

        public async Task<List<ExpandoObject>> ListarEquiposPorTorneo(string idTorneo)
        {
            List<Equipo> equipos = await _equipos.Find(equipo => equipo.Activo == true && idTorneo.Equals(idTorneo)).ToListAsync();
            List<ExpandoObject> eqConFotos = new List<ExpandoObject>();
            foreach (Equipo e in equipos)
            {

                dynamic eq = new ExpandoObject();
                dynamic estadio = new ExpandoObject();
                eq.Id = e.Id;
                eq.Nombre = e.NombreEquipo;
                eq.FechaFundacion = e.FechaFundacion;
                eq.ColorCaracteristico = e.ColorCaracteristico;
                eq.Categoria = e.Categoria;
                estadio = e.Estadio;
                eq.Estadio = estadio;
                eq.foto = e.UrlFoto;
                eqConFotos.Add(eq);
            }
            return eqConFotos;

        }

        public async Task<List<Jugador>> ListarJugadoresEquipo(string id)
        {

            return await _jugadores.Find<Jugador>(e => e.IdEquipo == id).ToListAsync();
        }

        public async Task<Equipo> BuscarEquipo(string id)
        {

            return await _equipos.Find<Equipo>(equipo => equipo.Id == id).FirstOrDefaultAsync();
        }


        public async Task<Equipo> CrearEquipo(Equipo equipo)
        {
            try
            {
                if (equipo.NombreEquipo.Length > 20)
                    throw new Exception("El nombre del equipo debe tener 20 caracteres como máximo.");
                await _equipos.InsertOneAsync(equipo);

                return equipo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void ActualizarEquipo(string id, Equipo eq)
        {
            try
            {
                if (eq.NombreEquipo.Length > 20)
                    throw new Exception("El nombre del equipo debe tener 20 caracteres como máximo.");
                _equipos.ReplaceOne(equipo => equipo.Id == id, eq);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async void EliminarEquipo(string id)
        {
            await _equipos.UpdateOneAsync(
                 ju => ju.Id.Equals(id),
                 Builders<Equipo>.Update.
                 Set(b => b.Activo, false));
        }
    }
}
