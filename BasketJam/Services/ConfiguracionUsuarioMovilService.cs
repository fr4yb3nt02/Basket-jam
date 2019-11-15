using BasketJam.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace BasketJam.Services
{
    public interface IConfiguracionUsuarioMovilService
    {
        Task<ConfiguracionUsuarioMovil> CrearConfiguracionUsuarioMovil(ConfiguracionUsuarioMovil unaConf);
        Task<Object> BuscarConfiguracionUsuarioMovil(string id);
        Task<Boolean> ActualizarConfiguracionUsuarioMovil(string id, ConfiguracionUsuarioMovil unaConf);
        Task<Boolean> AgregarEquiposFavoritos(string idUsuario, string equipo);
        Task<Boolean> QuitarEquipooFavoritos(string idUsuario, string equipo);
        Task<Boolean> EquipoEsFavorito(string idUsuario, string idEquipo);
        Task<List<Equipo>> ListarEquiposFavoritos(string idUsuario);
    }

    public class ConfiguracionUsuarioMovilService : IConfiguracionUsuarioMovilService
    {
        private readonly IMongoCollection<ConfiguracionUsuarioMovil> _configuracionUsuarioMovil;

        private readonly IMongoCollection<Equipo> _equipos;

        private readonly IMongoCollection<Usuario> _usuarios;

        public ConfiguracionUsuarioMovilService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
            _configuracionUsuarioMovil = database.GetCollection<ConfiguracionUsuarioMovil>("configuracionUsuarioMovil");
            _equipos = database.GetCollection<Equipo>("equipos");
            _usuarios = database.GetCollection<Usuario>("usuarios");

        }


        public async Task<Object> BuscarConfiguracionUsuarioMovil(string id)
        {
            //return 
            ConfiguracionUsuarioMovil config = await _configuracionUsuarioMovil.Find<ConfiguracionUsuarioMovil>(conf => conf.Usuario == id).FirstOrDefaultAsync();
            Usuario u = await _usuarios.Find<Usuario>(user => user.Id == config.Usuario).FirstOrDefaultAsync();
            return new
            {
                idNotificacion = config.Id,
                idUsuario = config.Usuario,
                nombreUsuario = u.NomUser,
                NotificacionEquiposFavoritos = config.NotificacionEquiposFavoritos,
                EquiposFavoritos = config.EquiposFavoritos,
                NotificacionTodosLosPartidos = config.NotificacionTodosLosPartidos,
                NotificacionInicioPartido = config.NotificacionInicioPartido,
                NotificacionFinPartido = config.NotificacionFinPartido

            };
        }

        public async Task<ConfiguracionUsuarioMovil> CrearConfiguracionUsuarioMovil(ConfiguracionUsuarioMovil unaConf)
        {
            await _configuracionUsuarioMovil.InsertOneAsync(unaConf);
            return unaConf;
        }

        public async Task<Boolean> ActualizarConfiguracionUsuarioMovil(string id, ConfiguracionUsuarioMovil unaConf)
        {
            try
            {
                await _configuracionUsuarioMovil.ReplaceOneAsync(conf => conf.Usuario == id, unaConf);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Boolean> AgregarEquiposFavoritos(string idUsuario, string equipo)
        {
            try
            {
                ConfiguracionUsuarioMovil conf = await _configuracionUsuarioMovil.Find<ConfiguracionUsuarioMovil>(c => c.Usuario == idUsuario).FirstOrDefaultAsync();

                var filter = Builders<ConfiguracionUsuarioMovil>.Filter.Eq(co => co.Usuario, idUsuario);


                Boolean yaExisteEquipo = conf.EquiposFavoritos.Any(e => e == equipo);
                if (yaExisteEquipo == false)
                {
                    var update = Builders<ConfiguracionUsuarioMovil>.Update
                    .Push<string>(e => e.EquiposFavoritos, equipo);
                    await _configuracionUsuarioMovil.FindOneAndUpdateAsync(filter, update);
                }

                return true;

            }
            catch
            {
                return false;
            }
        }


        public async Task<Boolean> QuitarEquipooFavoritos(string idUsuario, string equipo)
        {
            try
            {
                ConfiguracionUsuarioMovil conf = await _configuracionUsuarioMovil.Find<ConfiguracionUsuarioMovil>(c => c.Usuario == idUsuario).FirstOrDefaultAsync();

                var filter = Builders<ConfiguracionUsuarioMovil>.Filter.Eq(co => co.Usuario, idUsuario);


                Boolean yaExisteEquipo = conf.EquiposFavoritos.Any(e => e == equipo);
                if (yaExisteEquipo == true)
                {
                    var update = Builders<ConfiguracionUsuarioMovil>.Update
                    .Pull<string>(e => e.EquiposFavoritos, equipo);
                    await _configuracionUsuarioMovil.FindOneAndUpdateAsync(filter, update);
                }

                return true;

            }
            catch
            {
                return false;
            }
        }

        public async Task<Boolean> EquipoEsFavorito(string idUsuario, string idEquipo)
        {
            try
            {
                ConfiguracionUsuarioMovil conf = await _configuracionUsuarioMovil.Find<ConfiguracionUsuarioMovil>(c => c.Usuario == idUsuario).FirstOrDefaultAsync();

                Boolean equipoEsFavorito = conf.EquiposFavoritos.Any(e => e == idEquipo);
                if (equipoEsFavorito)
                    return true;
                else
                    return false;

            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Equipo>> ListarEquiposFavoritos(string idUsuario)
        {
            try
            {
                ConfiguracionUsuarioMovil conf = await _configuracionUsuarioMovil.Find<ConfiguracionUsuarioMovil>(c => c.Usuario == idUsuario).FirstOrDefaultAsync();
                List<Equipo> retorno = new List<Equipo>();

                foreach (string e in conf.EquiposFavoritos)
                {
                    Equipo eq = await _equipos.Find<Equipo>(equ => equ.Id == e).FirstOrDefaultAsync();
                    retorno.Add(eq);
                }

                return retorno;

            }
            catch
            {
                return new List<Equipo>();
            }
        }
    }
}