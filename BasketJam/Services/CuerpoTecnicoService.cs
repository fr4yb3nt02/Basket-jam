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
    public interface ICuerpoTecnicoService
    {
        Task<List<CuerpoTecnico>> ListarMiembroCuerpoTecnico();
        Task<CuerpoTecnico> BuscarMiembroCuerpoTecnico(string id);
        Task<CuerpoTecnico> CrearMiembroCuerpoTecnico(CuerpoTecnico equipo);
        void ActualizarMiembroCuerpoTecnico(string id, CuerpoTecnico eq);
        void EliminarMiembroCuerpoTecnico(string id);
    }

    public class CuerpoTecnicoService : ICuerpoTecnicoService
{
        private readonly IMongoCollection<CuerpoTecnico> _cuerpoTecnico;      

        public CuerpoTecnicoService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
             _cuerpoTecnico=database.GetCollection<CuerpoTecnico>("cuerpoTecnico");

        }
        public async Task<List<CuerpoTecnico>> ListarMiembroCuerpoTecnico()
        {
            return await _cuerpoTecnico.Find(cuerpoTecnico => true).ToListAsync();
        }

        public async Task<CuerpoTecnico> BuscarMiembroCuerpoTecnico(string id)
        {
            return await _cuerpoTecnico.Find<CuerpoTecnico>(cuerpoTecnico => cuerpoTecnico.Id == id).FirstOrDefaultAsync();
        }

        public async Task<CuerpoTecnico> CrearMiembroCuerpoTecnico(CuerpoTecnico cuerpoTecnico)
        {
           await  _cuerpoTecnico.InsertOneAsync(cuerpoTecnico);
            return cuerpoTecnico;
        }

        public void ActualizarMiembroCuerpoTecnico(string id, CuerpoTecnico ct)
        {
            _cuerpoTecnico.ReplaceOne(cuerpoTecnico => cuerpoTecnico.Id == id, ct);
        }

        public void EliminarMiembroCuerpoTecnico(CuerpoTecnico eq)
        {
            _cuerpoTecnico.DeleteOne(cuerpoTecnico => cuerpoTecnico.Id == eq.Id);
        }

        public void EliminarMiembroCuerpoTecnico(string id)
        {
            _cuerpoTecnico.DeleteOne(cuerpoTecnico => cuerpoTecnico.Id == id);
        }
    }
}