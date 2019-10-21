using BasketJam.Helper;
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
    public interface ICuerpoTecnicoService
    {
        Task<List<ExpandoObject>> ListarMiembroCuerpoTecnico();
        Task<CuerpoTecnico> BuscarMiembroCuerpoTecnico(string id);
        Task<CuerpoTecnico> CrearMiembroCuerpoTecnico(CuerpoTecnico equipo);
        void ActualizarMiembroCuerpoTecnico(string id, CuerpoTecnico eq);
        void EliminarMiembroCuerpoTecnico(string id);
    }

    public class CuerpoTecnicoService : ICuerpoTecnicoService
{
        private readonly IMongoCollection<CuerpoTecnico> _cuerpoTecnico;
        private readonly IMongoCollection<Equipo> _equipos;

        public CuerpoTecnicoService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
             _cuerpoTecnico=database.GetCollection<CuerpoTecnico>("cuerpoTecnico");
            _equipos = database.GetCollection<Equipo>("equipos");

        }
        public async Task<List<ExpandoObject>> ListarMiembroCuerpoTecnico()
        {
            List<CuerpoTecnico> cuerpoTecnico=await _cuerpoTecnico.Find(ct => ct.Activo==true).ToListAsync();
            List<ExpandoObject> ctt = new List<ExpandoObject>();
            foreach (CuerpoTecnico j in cuerpoTecnico)
            {
                Equipo e = await _equipos.Find<Equipo>(eq => eq.Id.Equals(j.IdEquipo)).FirstOrDefaultAsync();
                dynamic ju = new ExpandoObject();
                ju.id = j.Id;
                ju.Nombre = j.Nombre;
                ju.Apellido = j.Apellido;
                ju.IdEquipo = e.Id;
                ju.NombreEquipo = e.NombreEquipo;
                ju.FechaDeNacimiento = j.FechaDeNacimiento;
                ju.Activo = j.Activo;
                ju.Cargo = j.Cargo.ToString();
                ju.Foto = HelperCloudinary.cloudUrl + "CuerpoTecnico/" + j.Id;
                ctt.Add(ju);
            }
            return ctt;
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

        public async void EliminarMiembroCuerpoTecnico(string id)
        {
            await _cuerpoTecnico.UpdateOneAsync(
                   ju => ju.Id.Equals(id),
                   Builders<CuerpoTecnico>.Update.
                   Set(b => ((MiembroDeEquipo)b).Activo, false));
        }
    }
}