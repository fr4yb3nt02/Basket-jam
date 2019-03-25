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
        List<CuerpoTecnico> ListarMiembroCuerpoTecnico();
        CuerpoTecnico BuscarMiembroCuerpoTecnico(string id);
        CuerpoTecnico CrearMiembroCuerpoTecnico(CuerpoTecnico equipo);
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
        public List<CuerpoTecnico> ListarMiembroCuerpoTecnico()
        {
            return _cuerpoTecnico.Find(cuerpoTecnico => true).ToList();
        }

        public CuerpoTecnico BuscarMiembroCuerpoTecnico(string id)
        {
            return _cuerpoTecnico.Find<CuerpoTecnico>(cuerpoTecnico => cuerpoTecnico.Id == id).FirstOrDefault();
        }

        public CuerpoTecnico CrearMiembroCuerpoTecnico(CuerpoTecnico cuerpoTecnico)
        {
            _cuerpoTecnico.InsertOne(cuerpoTecnico);
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