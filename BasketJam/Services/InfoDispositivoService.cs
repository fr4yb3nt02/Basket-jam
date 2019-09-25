using BasketJam.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasketJam.Services
{

        public interface IInfoDispositivoService
        {
            Task<InfoDispositivo> BuscarInfoDispositivo(string id);
            Task<InfoDispositivo> CrearInfoDispositivo(InfoDispositivo infoDispositivo);
        Task<Boolean> ModificarInfoDispositivo(InfoDispositivo infoDisp);
        Task<Boolean> EliminarInfoDispositivo(string id);

        }

        public class InfoDispositivoService : IInfoDispositivoService
        {
            private readonly IMongoCollection<InfoDispositivo> _infoDIspositivo;

            public InfoDispositivoService(IConfiguration config)
            {
                var client = new MongoClient(config.GetConnectionString("BasketJam"));
                var database = client.GetDatabase("BasketJam");
                _infoDIspositivo = database.GetCollection<InfoDispositivo>("infoDispositivo");

            }

            public async Task<InfoDispositivo> BuscarInfoDispositivo(string id)
            {
                return await _infoDIspositivo.Find<InfoDispositivo>(i => i.IDDispositivo == id).FirstOrDefaultAsync();
            }

            public async Task<InfoDispositivo> CrearInfoDispositivo(InfoDispositivo infoDispositivo)
            {
                await _infoDIspositivo.InsertOneAsync(infoDispositivo);
                return infoDispositivo;
            }

        public async Task<Boolean> ModificarInfoDispositivo(InfoDispositivo infoDisp)
        {
            await _infoDIspositivo.ReplaceOneAsync(i => i.Id == infoDisp.Id,infoDisp);
            return true;
        }

        public async Task<Boolean> EliminarInfoDispositivo(string id)
        {
            await _infoDIspositivo.DeleteOneAsync(info=> info.Id==id);
            return true;
        }


    }
    }
