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
    public interface IEstadisticasEquipoPartidoService
    {
        Task<ReplaceOneResult> Save(EstadisticasEquipoPartido eep);
        Task<Boolean> CargarEstadistica(EstadisticasEquipoPartido eep);
        Task<Boolean> CargarEstadistica(string IdPartido , string IdEquipo , int ptos);
        Task<Boolean> CargarEstadistica(EstadisticasJugadorPartido ejp);
        EstadisticasEquipoPartido BuscarEstadisticasEquipoPartido(string IdPartido, string IdEquipo);
        Task<List<EstadisticasEquipoPartido>> EstadisticasEquipoPorPartido(string IdPartido);
    }

    public class EstadisticasEquipoPartidoService : IEstadisticasEquipoPartidoService
{
        private readonly IMongoCollection<EstadisticasEquipoPartido> _estadisticasEquipoPartido;      

        private IPartidoService _partidoService;

        private IEquipoService _equipoService;

        private IJugadorService _jugadorService;

        public EstadisticasEquipoPartidoService(IConfiguration config,IPartidoService partidoService,IEquipoService equipoService,IJugadorService jugadorService)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
             _estadisticasEquipoPartido=database.GetCollection<EstadisticasEquipoPartido>("EstadisticasEquipoPartido");
             _partidoService=partidoService;
            _equipoService=equipoService;
            _jugadorService=jugadorService;
        }

        public EstadisticasEquipoPartido BuscarEstadisticasEquipoPartido(string IdPartido,string IdEquipo)
        {    
            try
            { 
            return  _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(a => a.IdEquipo.Equals(IdEquipo)&& a.IdPartido.Equals(IdPartido)).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<EstadisticasEquipoPartido>> EstadisticasEquipoPorPartido(string IdPartido)
        {    
            return await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(a => a.IdPartido.Equals(IdPartido)).ToListAsync();
        }

        public  async Task<Boolean> CargarEstadistica(EstadisticasEquipoPartido eep)
        {
            EstadisticasEquipoPartido EstadisticasEquipoPartido=BuscarEstadisticasEquipoPartido(eep.IdPartido,eep.IdEquipo);
            int puntos=EstadisticasEquipoPartido.Puntos;
            if(EstadisticasEquipoPartido==null)
            {
                 await _estadisticasEquipoPartido.InsertOneAsync(eep);
                 return true;
            }
            else
            {
                await _estadisticasEquipoPartido.UpdateOneAsync(
    a => a.IdEquipo.Equals(eep.IdEquipo)&& a.IdPartido.Equals(eep.IdPartido), // find this match
    Builders<EstadisticasEquipoPartido>.Update.Set(c => c.Puntos,EstadisticasEquipoPartido.Puntos+eep.Puntos));     // -1 means update first matching array element
                //await _estadisticasEquipoPartido.UpdateOneAsync(a => a.IdEquipo.Equals(eep.IdEquipo)&& a.IdPartido==eep.IdPartido,{$set});
                return true;
            }
        }

        public  async Task<Boolean> CargarEstadistica(EstadisticasJugadorPartido ejp)
        {
            Jugador Jugador=await _jugadorService.BuscarJugador(ejp.IdJugador);
            //Equipo Equipo=await _equipoService.BuscarEquipo(Jugador.IdEquipo);
            EstadisticasEquipoPartido EstadisticasEquipoPartido=BuscarEstadisticasEquipoPartido(ejp.IdPartido,Jugador.IdEquipo);
            Partido Partido= await _partidoService.BuscarPartido(ejp.IdPartido);
            if(EstadisticasEquipoPartido==null)
            {
                double porcentajeTirosLibres=0;
                if(ejp.TirosLibresIntentados!=0)
                 porcentajeTirosLibres=100;

                                 double porcentajeCanastas2Puntos=0;
                if(ejp.DosPuntosIntentados!=0)
                 porcentajeCanastas2Puntos=100;

                double porcentajeCanastas3Puntos=0;
                if(ejp.TresPuntosIntentados!=0)
                 porcentajeCanastas3Puntos=100;

               await _estadisticasEquipoPartido.InsertOneAsync(new EstadisticasEquipoPartido{
                   IdPartido=ejp.IdPartido,
                    IdEquipo=Jugador.IdEquipo,
                    Puntos = ejp.Puntos,
                    PuntosPrimerCuarto = ejp.Puntos,
                    PuntosSegundoCuarto=0,
                    PuntosTercerCuarto=0,
                    PuntosCuartoCuarto=0,
                    PuntosOverTime=0,
                    TirosLibresExitosos=ejp.TirosLibresConvertidos,
                    TotalTirosLibres=ejp.TirosLibresIntentados,
                    PorcentajeTirosLibres=porcentajeTirosLibres,
                    Canasta2PuntosExitosas=ejp.DosPuntosConvertidos,
                    TotalCanastas2Puntos=ejp.DosPuntosIntentados,
                    PorcentajeCanastas2Puntos=porcentajeCanastas2Puntos,
                    Canasta3PuntosExitosas=ejp.TresPuntosConvertidos,
                    TotalCanastas3Puntos=ejp.TresPuntosIntentados,
                    PorcentajeCanastas3Puntos=porcentajeCanastas3Puntos,
                    RebotesDefensivos=ejp.RebotesDefensivos,
                    RebotesOfensivos=ejp.RebotesOfensivos,
                    Faltas=ejp.FaltasCometidas+ejp.FaltasPersonales,
                    Perdidas=ejp.Perdidas

                });
                 //await _estadisticasEquipoPartido.InsertOneAsync(eep);
                 return true;
            }
            else
            { 
               // int puntos=EstadisticasEquipoPartido.Puntos;
          /*      await _estadisticasEquipoPartido.UpdateOneAsync(
    a => a.IdEquipo.Equals(Jugador.IdEquipo)&& a.IdPartido.Equals(ejp.IdPartido), // find this match
    Builders<EstadisticasEquipoPartido>.Update.Set(c => c.Puntos,EstadisticasEquipoPartido.Puntos+ptos)); */
    
       

            int ptos=0;
            if(ejp.TresPuntosConvertidos!=0)
            ptos=3;
            if(ejp.DosPuntosConvertidos!=0)
            ptos=2;
            if(ejp.TirosLibresConvertidos!=0)
            ptos=1;

                //int puntos=EstadisticasEquipoPartido.Puntos+ptos;
                int puntos = 0;
                int puntosTotales = EstadisticasEquipoPartido.Puntos + ptos; ;
                int puntosPrimerCuarto=0;
                int puntosSegundoCuarto=0;
                int puntosTercerCuarto=0;
                int puntosCuartoCuarto=0;
                int puntosOverTime=0;
                if (Partido.cuarto == 1)
                {
                    puntos = EstadisticasEquipoPartido.PuntosPrimerCuarto + ptos;
                    puntosPrimerCuarto = puntos;
                    puntosSegundoCuarto = EstadisticasEquipoPartido.PuntosSegundoCuarto;
                    puntosTercerCuarto = EstadisticasEquipoPartido.PuntosTercerCuarto;
                    puntosCuartoCuarto = EstadisticasEquipoPartido.PuntosCuartoCuarto;
                    puntosOverTime = EstadisticasEquipoPartido.PuntosOverTime;
                }
                if (Partido.cuarto == 2)
                {
                    puntos = EstadisticasEquipoPartido.PuntosSegundoCuarto + ptos;
                    puntosSegundoCuarto = puntos;
                    puntosPrimerCuarto = EstadisticasEquipoPartido.PuntosPrimerCuarto;
                    puntosTercerCuarto = EstadisticasEquipoPartido.PuntosTercerCuarto;
                    puntosCuartoCuarto = EstadisticasEquipoPartido.PuntosCuartoCuarto;
                    puntosOverTime = EstadisticasEquipoPartido.PuntosOverTime;
                }
                if (Partido.cuarto == 3)
                {
                    puntos = EstadisticasEquipoPartido.PuntosTercerCuarto + ptos;
                    puntosTercerCuarto = puntos;
                    puntosSegundoCuarto = EstadisticasEquipoPartido.PuntosSegundoCuarto;
                    puntosPrimerCuarto = EstadisticasEquipoPartido.PuntosPrimerCuarto;
                    puntosCuartoCuarto = EstadisticasEquipoPartido.PuntosCuartoCuarto;
                    puntosOverTime = EstadisticasEquipoPartido.PuntosOverTime;
                }
                if (Partido.cuarto == 4)
                {
                    puntos = EstadisticasEquipoPartido.PuntosCuartoCuarto + ptos;
                    puntosCuartoCuarto = puntos;
                    puntosSegundoCuarto = EstadisticasEquipoPartido.PuntosSegundoCuarto;
                    puntosTercerCuarto = EstadisticasEquipoPartido.PuntosTercerCuarto;
                    puntosPrimerCuarto = EstadisticasEquipoPartido.PuntosPrimerCuarto;
                    puntosOverTime = EstadisticasEquipoPartido.PuntosOverTime;
                }
                if (Partido.cuarto == 5)
                {
                    puntos = EstadisticasEquipoPartido.PuntosOverTime + ptos;
                    puntosOverTime = puntos;
                    puntosSegundoCuarto = EstadisticasEquipoPartido.PuntosSegundoCuarto;
                    puntosTercerCuarto = EstadisticasEquipoPartido.PuntosTercerCuarto;
                    puntosCuartoCuarto = EstadisticasEquipoPartido.PuntosCuartoCuarto;
                    puntosPrimerCuarto = EstadisticasEquipoPartido.PuntosPrimerCuarto;
                }


                int tirosLibresExitosos= EstadisticasEquipoPartido.TirosLibresExitosos+ejp.TirosLibresConvertidos;
                int totalTirosLibres=EstadisticasEquipoPartido.TotalTirosLibres+ ejp.TirosLibresIntentados;
                double porcentajeTirosLibres=0;
                if(totalTirosLibres!=0)
                 porcentajeTirosLibres=(tirosLibresExitosos*100)/totalTirosLibres;
                int canasta2PuntosExitosas= EstadisticasEquipoPartido.Canasta2PuntosExitosas+ ejp.DosPuntosConvertidos;
                int totalCanastas2Puntos=EstadisticasEquipoPartido.TotalCanastas2Puntos+ ejp.DosPuntosIntentados;
                double porcentajeCanastas2Puntos=0;
                if(totalCanastas2Puntos!=0)
                 porcentajeCanastas2Puntos=(canasta2PuntosExitosas*100)/totalCanastas2Puntos;
                int canasta3PuntosExitosas=EstadisticasEquipoPartido.Canasta3PuntosExitosas+ ejp.TresPuntosConvertidos;
                int totalCanastas3Puntos=EstadisticasEquipoPartido.TotalCanastas3Puntos+ ejp.TresPuntosIntentados;
                double porcentajeCanastas3Puntos=0;
                if(totalCanastas3Puntos!=0)
                 porcentajeCanastas3Puntos=(canasta3PuntosExitosas*100)/totalCanastas3Puntos;
                int rebotesDefensivos=EstadisticasEquipoPartido.RebotesDefensivos+ ejp.RebotesDefensivos;
                int rebotesOfensivos=EstadisticasEquipoPartido.RebotesDefensivos+ ejp.RebotesOfensivos;
                int faltas=EstadisticasEquipoPartido.Faltas+ ejp.FaltasCometidas+ejp.FaltasPersonales;
                int perdidas=EstadisticasEquipoPartido.Perdidas+ ejp.Perdidas;

                  await _estadisticasEquipoPartido.UpdateOneAsync(
                 a => a.IdEquipo.Equals(Jugador.IdEquipo) && a.IdPartido.Equals(ejp.IdPartido),// Filtros para encontrar al jugador y partido correcto
                Builders<EstadisticasEquipoPartido>.Update
                .Set(b => b.TirosLibresExitosos,tirosLibresExitosos)
                .Set(c => c.TotalTirosLibres,totalTirosLibres)
                .Set(d => d.PorcentajeTirosLibres,porcentajeTirosLibres)
                .Set(b => b.Canasta2PuntosExitosas,canasta2PuntosExitosas)
                .Set(c => c.TotalCanastas2Puntos,totalCanastas2Puntos)
                .Set(d => d.PorcentajeCanastas2Puntos,porcentajeCanastas2Puntos)
                .Set(b => b.Canasta3PuntosExitosas,canasta3PuntosExitosas)
                .Set(c => c.TotalCanastas3Puntos,totalCanastas3Puntos)
                .Set(d => d.PorcentajeCanastas3Puntos,porcentajeCanastas3Puntos)
                .Set(b => b.RebotesOfensivos,rebotesOfensivos)
                .Set(c => c.RebotesDefensivos,rebotesDefensivos)
                .Set(d => d.Faltas,faltas)
                .Set(f => f.Puntos, puntosTotales)
                .Set(c => c.PuntosPrimerCuarto,puntosPrimerCuarto)
                .Set(d => d.PuntosSegundoCuarto,puntosSegundoCuarto)
                .Set(b => b.PuntosTercerCuarto,puntosTercerCuarto)
                .Set(c => c.PuntosCuartoCuarto,puntosCuartoCuarto)
                .Set(d => d.PuntosOverTime,puntosOverTime)
                .Set(b => b.Perdidas,perdidas));     
                //await _estadisticasEquipoPartido.UpdateOneAsync(a => a.IdEquipo.Equals(eep.IdEquipo)&& a.IdPartido==eep.IdPartido,{$set});
                return true;
            }
        }

        public  async Task<Boolean> CargarEstadistica(string IdPartido , string IdEquipo , int ptos)
        {
            EstadisticasEquipoPartido EstadisticasEquipoPartido=BuscarEstadisticasEquipoPartido(IdPartido,IdEquipo);
            Partido Partido= await _partidoService.BuscarPartido(IdPartido);
            if(EstadisticasEquipoPartido==null)
            {

               await _estadisticasEquipoPartido.InsertOneAsync(new EstadisticasEquipoPartido{
                   IdPartido=IdPartido,
                    IdEquipo=IdEquipo,
                    Puntos = ptos
                });
                 //await _estadisticasEquipoPartido.InsertOneAsync(eep);
                 return true;
            }
            else
            { 
                int puntos=EstadisticasEquipoPartido.Puntos;
                await _estadisticasEquipoPartido.UpdateOneAsync(
    a => a.IdEquipo.Equals(IdEquipo)&& a.IdPartido.Equals(IdPartido), // find this match
    Builders<EstadisticasEquipoPartido>.Update.Set(c => c.Puntos,EstadisticasEquipoPartido.Puntos+ptos));     // -1 means update first matching array element
                //await _estadisticasEquipoPartido.UpdateOneAsync(a => a.IdEquipo.Equals(eep.IdEquipo)&& a.IdPartido==eep.IdPartido,{$set});
                return true;
            }
        }

        public async Task<ReplaceOneResult> Save(EstadisticasEquipoPartido eep)
    {
        return await _estadisticasEquipoPartido.ReplaceOneAsync<EstadisticasEquipoPartido>(x => x.IdEquipo.Equals(eep.IdEquipo) && x.IdPartido.Equals(eep.IdPartido),eep, new UpdateOptions { IsUpsert = true });
    }

    }
}