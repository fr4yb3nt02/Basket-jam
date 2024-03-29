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
    public interface ITablaDePosicionesService
    {
        Task<Object> BuscarTablaDePosiciones(string id);
        Task<TablaDePosiciones> CrearTablaDePosiciones(TablaDePosiciones tablaDePosiciones);
        Task<Boolean> ActualizarTablaDePosiciones(string id, List<TablaDePosiciones.EquipoTablaPosicion> tp);
        Task<List<String>> ListarFotosEquiposTorneo(string idTorneo);
    }

    public class TablaDePosicionesService : ITablaDePosicionesService
    {
        private readonly IMongoCollection<TablaDePosiciones> _tablaDePosiciones;
        private readonly IMongoCollection<Equipo> _equipos;

        public TablaDePosicionesService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
            _tablaDePosiciones = database.GetCollection<TablaDePosiciones>("tablaDePosiciones");
            _equipos = database.GetCollection<Equipo>("equipos");

        }

        public async Task<Object> BuscarTablaDePosiciones(string id)
        {
            try
            {
                TablaDePosiciones t = await _tablaDePosiciones.Find<TablaDePosiciones>(to => to.IdTorneo.Equals(id)).FirstOrDefaultAsync();
                dynamic tabla = new ExpandoObject();

                tabla.Id = t.Id;
                tabla.IdTorneo = t.IdTorneo;
                List<ExpandoObject> equiposTablaPos = new List<ExpandoObject>();
                foreach (TablaDePosiciones.EquipoTablaPosicion e in t.EquiposTablaPosicion)
                {
                    Equipo equi = await _equipos.Find<Equipo>(eq => eq.Id == e.idEquipo).FirstOrDefaultAsync();
                    dynamic equipo = new ExpandoObject();
                    equipo.idEquipo = e.idEquipo;
                    equipo.nombre = equi.NombreEquipo;
                    equipo.Puntos = e.Puntos;
                    equipo.Posicion = e.Posicion;
                    equipo.PG = e.PG;
                    equipo.PP = e.PP;
                    equipo.PF = e.PF;
                    equipo.PC = e.PC;
                    equipo.DIF = e.DIF;
                    equipo.Foto = equi.UrlFoto;
                    equiposTablaPos.Add(equipo);
                }
                tabla.EquiposTablaPosicion = equiposTablaPos;

                return tabla;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<String>> ListarFotosEquiposTorneo(string idTorneo)
        {
            try
            {
                TablaDePosiciones tabla = await _tablaDePosiciones.Find<TablaDePosiciones>(t => t.Id == idTorneo).FirstOrDefaultAsync();
                List<String> stringFotos = new List<string>();
                foreach (TablaDePosiciones.EquipoTablaPosicion e in tabla.EquiposTablaPosicion)
                {
                    Equipo equi = await _equipos.Find<Equipo>(eq => eq.Id == e.idEquipo).FirstOrDefaultAsync();
                    stringFotos.Add(equi.UrlFoto);
                }
                return stringFotos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TablaDePosiciones> CrearTablaDePosiciones(TablaDePosiciones tablaDePosiciones)
        {
            await _tablaDePosiciones.InsertOneAsync(tablaDePosiciones);
            return tablaDePosiciones;
        }

        public async Task<Boolean> ActualizarTablaDePosiciones(string id, List<TablaDePosiciones.EquipoTablaPosicion> tp)
        {

            try
            {
                foreach (TablaDePosiciones.EquipoTablaPosicion etp in tp)
                {
                    Equipo equipo = await _equipos.Find<Equipo>(e => e.Id == etp.idEquipo).FirstOrDefaultAsync();
                    TablaDePosiciones tabla = await _tablaDePosiciones.Find<TablaDePosiciones>(t => t.Id == id).FirstOrDefaultAsync();


                    int indexEquipo = tabla.EquiposTablaPosicion.IndexOf(etp);

                    var UpdateDefinitionBuilder = Builders<TablaDePosiciones>.Update.Set(t => t.EquiposTablaPosicion[indexEquipo], etp);

                    await _tablaDePosiciones.UpdateOneAsync(t => t.Id == id, UpdateDefinitionBuilder);
                }
                return true;
            }

            catch
            {
                return false;
            }

        }

    }
}
 