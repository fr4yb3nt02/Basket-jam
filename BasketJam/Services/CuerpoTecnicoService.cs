using BasketJam.Helper;
using BasketJam.Models;
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
        Task<string> subirImagen(Imagen img);
    }

    public class CuerpoTecnicoService : ICuerpoTecnicoService
    {
        private readonly IMongoCollection<CuerpoTecnico> _cuerpoTecnico;
        private readonly IMongoCollection<Equipo> _equipos;

        public CuerpoTecnicoService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
            _cuerpoTecnico = database.GetCollection<CuerpoTecnico>("cuerpoTecnico");
            _equipos = database.GetCollection<Equipo>("equipos");

        }
        public async Task<List<ExpandoObject>> ListarMiembroCuerpoTecnico()
        {
            List<CuerpoTecnico> cuerpoTecnico = await _cuerpoTecnico.Find(ct => ct.Activo == true).ToListAsync();
            List<ExpandoObject> ctt = new List<ExpandoObject>();
            foreach (CuerpoTecnico j in cuerpoTecnico)
            {
                Equipo e = await _equipos.Find<Equipo>(eq => eq.Id.Equals(j.IdEquipo)).FirstOrDefaultAsync();
                dynamic ju = new ExpandoObject();
                ju.id = j.Id;
                ju.Nombre = j.Nombre;
                ju.Apellido = j.Apellido;
                ju.ci = j.Ci;
                ju.IdEquipo = e.Id;
                ju.NombreEquipo = e.NombreEquipo;
                ju.FechaDeNacimiento = j.FechaDeNacimiento;
                ju.Activo = j.Activo;
                ju.Cargo = j.Cargo.ToString();
                ju.Foto = j.UrlFoto;
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
            try
            {
                CuerpoTecnico j = await _cuerpoTecnico.Find<CuerpoTecnico>(ct => ct.Ci.Equals(cuerpoTecnico.Ci)).FirstOrDefaultAsync();
                if (j != null)
                {
                    throw new Exception("Ya existe un miembro del cuerpopo técnico con la C.I ingresada.");
                }
                await _cuerpoTecnico.InsertOneAsync(cuerpoTecnico);
                return cuerpoTecnico;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void ActualizarMiembroCuerpoTecnico(string id, CuerpoTecnico ct)
        {
            _cuerpoTecnico.ReplaceOne(cuerpoTecnico => cuerpoTecnico.Id == id, ct);
        }


        public async void EliminarMiembroCuerpoTecnico(string id)
        {
            await _cuerpoTecnico.UpdateOneAsync(
                   ju => ju.Id.Equals(id),
                   Builders<CuerpoTecnico>.Update.
                   Set(b => ((MiembroDeEquipo)b).Activo, false));
        }

        public async Task<string> subirImagen(Imagen img)
        {

            try
            {
                string claseImagen = "CuerpoTecnico";
                string url = ImagenService.subirImagen(img, claseImagen);
                await _cuerpoTecnico.UpdateOneAsync(pa => pa.Id.Equals(img.Nombre),
                                       Builders<CuerpoTecnico>.Update.
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