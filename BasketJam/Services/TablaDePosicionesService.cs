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
    public interface ITablaDePosicionesService
    {
        Task<TablaDePosiciones> BuscarTablaDePosiciones(string id);
        Task<TablaDePosiciones> CrearTablaDePosiciones(TablaDePosiciones tablaDePosiciones);
        Task<Boolean> ActualizarTablaDePosiciones(string id, List<TablaDePosiciones.EquipoTablaPosicion> tp);
    }

    public class TablaDePosicionesService : ITablaDePosicionesService
    {
        private readonly IMongoCollection<TablaDePosiciones> _tablaDePosiciones;
        private readonly IMongoCollection<Equipo> _equipos;

        public TablaDePosicionesService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
             _tablaDePosiciones=database.GetCollection<TablaDePosiciones>("tablaDePosiciones");
            _equipos = database.GetCollection<Equipo>("equipos");

        }

        public async Task<TablaDePosiciones> BuscarTablaDePosiciones(string id)
        {
            return await _tablaDePosiciones.Find<TablaDePosiciones>(t => t.IdTorneo == id).FirstOrDefaultAsync();
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
                      foreach(TablaDePosiciones.EquipoTablaPosicion etp in tp)
                      {               
                    Equipo equipo= await _equipos.Find<Equipo>(e => e.Id==etp.idEquipo).FirstOrDefaultAsync();
                    TablaDePosiciones tabla = await _tablaDePosiciones.Find<TablaDePosiciones>(t => t.Id == id).FirstOrDefaultAsync();

                    /* var posicionEquipoIndex = await _tablaDePosiciones
                      .Find(t => t.Id == id)
                      .Project(ta => ta.EquiposTablaPosicion.FindIndex(e => e.idEquipo==etp.idEquipo))
                      .SingleOrDefaultAsync();*/

                    int indexEquipo = tabla.EquiposTablaPosicion.IndexOf(etp);

                    var UpdateDefinitionBuilder = Builders<TablaDePosiciones>.Update.Set(t => t.EquiposTablaPosicion[indexEquipo], etp);

                    await _tablaDePosiciones.UpdateOneAsync(t=> t.Id == id, UpdateDefinitionBuilder);
                       }
                return true;
                    }                                               
            
            catch
            {
                return false;
            }
            //_jueces.ReplaceOne(tablaDePosiciones => tablaDePosiciones.Id == id, tp);
        }

    }
}
 