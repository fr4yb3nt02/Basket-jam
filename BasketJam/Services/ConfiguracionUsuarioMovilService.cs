using BasketJam.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasketJam.Services
{
    public interface IConfiguracionUsuarioMovilService
    {
        Task<ConfiguracionUsuarioMovil> CrearConfiguracionUsuarioMovil(ConfiguracionUsuarioMovil unaConf);
        Task<ConfiguracionUsuarioMovil> BuscarConfiguracionUsuarioMovil(string id);
        Task<Boolean> ActualizarConfiguracionUsuarioMovil(string id, ConfiguracionUsuarioMovil unaConf);
        Task<Boolean> AgregarEquiposFavoritos(string idUsuario, List<string> equipos);
        Task<Boolean> EquipoEsFavorito(string idUsuario, string idEquipo);
    }

    public class ConfiguracionUsuarioMovilService : IConfiguracionUsuarioMovilService
    {
        private readonly IMongoCollection<ConfiguracionUsuarioMovil> _configuracionUsuarioMovil;

        public ConfiguracionUsuarioMovilService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
            _configuracionUsuarioMovil = database.GetCollection<ConfiguracionUsuarioMovil>("configuracionUsuarioMovil");

        }


        public async Task<ConfiguracionUsuarioMovil> BuscarConfiguracionUsuarioMovil(string id)
        {
            return await _configuracionUsuarioMovil.Find<ConfiguracionUsuarioMovil>(conf => conf.Usuario == id).FirstOrDefaultAsync();
        }

        public  async Task<ConfiguracionUsuarioMovil> CrearConfiguracionUsuarioMovil(ConfiguracionUsuarioMovil unaConf)
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

        public async Task<Boolean> AgregarEquiposFavoritos(string idUsuario,List<string> equipos)
        {
            try
            {
                ConfiguracionUsuarioMovil conf= await _configuracionUsuarioMovil.Find<ConfiguracionUsuarioMovil>(c => c.Usuario == idUsuario).FirstOrDefaultAsync();

                var filter = Builders<ConfiguracionUsuarioMovil>.Filter.Eq(co => co.Usuario , idUsuario);

                foreach (string id in equipos)
                {
                    Boolean yaExisteEquipo = conf.EquiposFavoritos.Any(e => e == id);
                    if (yaExisteEquipo == false)
                    {
                        var update = Builders<ConfiguracionUsuarioMovil>.Update
                        .Push<string>(e => e.EquiposFavoritos, id);
                        await _configuracionUsuarioMovil.FindOneAndUpdateAsync(filter, update);
                    }                   
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
    }
}