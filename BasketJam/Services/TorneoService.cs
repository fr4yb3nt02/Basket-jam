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
    public interface ITorneoService
    {
        Task<List<Torneo>> ListarTorneos();
        Task<Torneo> BuscarTorneo(string id);
        Task<Torneo> CrearTorneo(Torneo equipo);
        void ActualizarTorneo(string id, Torneo tor);
        void EliminarTorneo(string id);
        Task<Torneo> TorneoActual();
    }

    public class TorneoService : ITorneoService
    {
        private readonly IMongoCollection<Torneo> _torneos;

        private readonly ITablaDePosicionesService _tablaDePosicionesService;

        public TorneoService(IConfiguration config, ITablaDePosicionesService tablaDePosicionesService)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
            _torneos = database.GetCollection<Torneo>("torneos");
            _tablaDePosicionesService = tablaDePosicionesService;

        }
        public async Task<List<Torneo>> ListarTorneos()
        {
            return await _torneos.Find(torneo => torneo.Activo).ToListAsync();
        }

        public async Task<Torneo> TorneoActual()
        {
            return await _torneos.Find(torneo => torneo.Anio == (Convert.ToInt32(DateTime.Now.Year))).FirstOrDefaultAsync();
        }

        public async Task<Torneo> BuscarTorneo(string id)
        {
            return await _torneos.Find<Torneo>(torneo => torneo.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Torneo> CrearTorneo(Torneo torneo)
        {
            try
            {

                Torneo torneoAnio = new Torneo();
                torneoAnio = await _torneos.Find<Torneo>(t => t.Anio == torneo.Anio).FirstOrDefaultAsync();

                if (torneoAnio != null)
                {
                    throw new Exception("Ya existe un torneo para el año ingresado.");
                }

                TablaDePosiciones tp = new TablaDePosiciones();
                tp.EquiposTablaPosicion = new List<TablaDePosiciones.EquipoTablaPosicion>();

                await _torneos.InsertOneAsync(torneo);

                tp.IdTorneo = torneo.Id;
                foreach (Equipo p in torneo.Equipos)
                {
                    TablaDePosiciones.EquipoTablaPosicion etp = new TablaDePosiciones.EquipoTablaPosicion();
                    etp.idEquipo = p.Id;
                    etp.PC = 0;
                    etp.PF = 0;
                    etp.PG = 0;
                    etp.Posicion = 1;
                    etp.PP = 0;
                    etp.Puntos = 0;
                    tp.EquiposTablaPosicion.Add(etp);
                }
                await _tablaDePosicionesService.CrearTablaDePosiciones(tp);
                return torneo;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        public void ActualizarTorneo(string id, Torneo tor)
        {
            _torneos.ReplaceOne(torneo => torneo.Id == id, tor);
        }



        public async void EliminarTorneo(string id)
        {
            await _torneos.UpdateOneAsync(
                 ju => ju.Id.Equals(id),
                 Builders<Torneo>.Update.
                 Set(b => b.Activo, false));
        }
    }
}