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
    public interface IEstadisticasJugadorPartidoService
    {
        Task<ReplaceOneResult> Save(EstadisticasJugadorPartido eep);
        Task<Boolean> CargarEstadistica(EstadisticasJugadorPartido eep);
        EstadisticasJugadorPartido BuscarEstadisticasJugadorPartido(string IdPartido,string IdJugador);
    }

    public class EstadisticasJugadorPartidoService : IEstadisticasJugadorPartidoService
{

        private IEstadisticasEquipoPartidoService _estadisticasEquipoPartidoService;

        private readonly IMongoCollection<EstadisticasJugadorPartido> _estadisticasJugadorPartido;   

        private readonly IMongoCollection<Jugador> _jugadores;         

        public EstadisticasJugadorPartidoService(IConfiguration config,IEstadisticasEquipoPartidoService estadisticasEquipoPartidoService)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
             _estadisticasJugadorPartido=database.GetCollection<EstadisticasJugadorPartido>("EstadisticasJugadorPartido");
             _jugadores=database.GetCollection<Jugador>("jugadores");
             _estadisticasEquipoPartidoService = estadisticasEquipoPartidoService;

        }

        public EstadisticasJugadorPartido BuscarEstadisticasJugadorPartido(string IdPartido,string IdJugador)
        {    
            return  _estadisticasJugadorPartido.Find<EstadisticasJugadorPartido>(a => a.IdJugador.Equals(IdJugador)&& a.IdPartido.Equals(IdPartido)).FirstOrDefault();
        }

        public async Task<Boolean> CargarEstadistica(EstadisticasJugadorPartido ejp)
        {
            EstadisticasJugadorPartido EstadisticasJugadorPartido=BuscarEstadisticasJugadorPartido(ejp.IdPartido,ejp.IdJugador);
            int ptos=0;
            if(ejp.TresPuntosConvertidos!=0)
            ptos=3;
            if(ejp.DosPuntosConvertidos!=0)
            ptos=2;
            if(ejp.TirosLibresConvertidos!=0)
            ptos=1;
            List<Coordenada> coors=new List<Coordenada>();
          //   Coordenada unaCoordenada=new Coordenada();
            // unaCoordenada.X=x;
            // unaCoordenada.Y=y;


            var equipo=await _jugadores.Find<Jugador>(a => a.Id.Equals(ejp.IdJugador)).FirstOrDefaultAsync();
            if(EstadisticasJugadorPartido==null)
            {
                if(ejp.CoordenadasAcciones==null)
                {
                    ejp.CoordenadasAcciones=coors;
                }
                if(ejp.TresPuntosConvertidos!=0 && ejp.TresPuntosIntentados!=0)
                {
                    ejp.TresPuntosPorcentaje=100;
                    ejp.Puntos=ptos;
                    
                }
                if(ejp.TresPuntosConvertidos==0 && ejp.TresPuntosIntentados!=0)
                {
                    ejp.TresPuntosPorcentaje=50;                    
                }
                if(ejp.DosPuntosConvertidos!=0 && ejp.DosPuntosIntentados!=0)
                {
                    ejp.DosPuntosPorcentaje=100;
                    ejp.Puntos=ptos;
                }
                if(ejp.DosPuntosConvertidos==0 && ejp.DosPuntosIntentados!=0)
                {
                    ejp.DosPuntosPorcentaje=50;                    
                }
                if(ejp.TirosLibresConvertidos!=0 && ejp.TirosLibresIntentados!=0)
                {
                    ejp.TirosLibresPorcentaje=100;
                    ejp.Puntos=ptos;
                }
                if(ejp.TirosLibresConvertidos==0 && ejp.TirosLibresIntentados!=0)
                {
                    ejp.TirosLibresPorcentaje=50;                    
                }
                if(ejp.RebotesOfensivos!=0 || ejp.RebotesDefensivos!=0)
                {                    
                    ejp.RebotesTotales++;
                }
                //ejp.CoordenadasAcciones.Add(unaCoordenada);

                 await _estadisticasJugadorPartido.InsertOneAsync(ejp);
                 await _estadisticasEquipoPartidoService.CargarEstadistica(ejp);
                 //await _estadisticasEquipoPartidoService.CargarEstadistica(ejp.IdPartido,equipo.IdEquipo,ptos);
                 return true;
            }
            else
            {
//          .Set(d => d.TresPuntosPorcentaje,(EstadisticasJugadorPartido.TresPuntosConvertidos*100)/ejp.TresPuntosIntentados)
                /*Datos a actualizar */
                int puntos=EstadisticasJugadorPartido.Puntos+ptos;
                int tresPuntosIntentados=EstadisticasJugadorPartido.TresPuntosIntentados+ejp.TresPuntosIntentados;
                int tresPuntosConvertidos=EstadisticasJugadorPartido.TresPuntosConvertidos+ejp.TresPuntosConvertidos;
                double porcentaje=0;
                if(tresPuntosIntentados!=0)
                    porcentaje=(tresPuntosConvertidos*100)/tresPuntosIntentados;
                int dosPuntosIntentados=EstadisticasJugadorPartido.DosPuntosIntentados+ejp.DosPuntosIntentados;
                int dosPuntosConvertidos=EstadisticasJugadorPartido.DosPuntosConvertidos+ejp.DosPuntosConvertidos;
                double porcentaje2pts=0;
                if(dosPuntosIntentados!=0)
                    porcentaje2pts=(dosPuntosConvertidos*100)/dosPuntosIntentados;
                int tirosLibresIntentados=EstadisticasJugadorPartido.TirosLibresIntentados+ejp.TirosLibresIntentados;
                int tirosLibresConvertidos=EstadisticasJugadorPartido.TirosLibresConvertidos+ejp.TirosLibresConvertidos;
                double porcentajeTirosLibres=0;
                if(tirosLibresIntentados!=0)
                 porcentajeTirosLibres=(tirosLibresConvertidos*100)/tirosLibresIntentados;
                int rebotesOfensivos=EstadisticasJugadorPartido.RebotesOfensivos+ejp.RebotesOfensivos;
                int rebotesDefensivos=EstadisticasJugadorPartido.RebotesDefensivos+ejp.RebotesDefensivos;
                int rebotesTotales=rebotesOfensivos+rebotesDefensivos;
                int bloqueos=EstadisticasJugadorPartido.Bloqueos+ejp.Bloqueos;
                int asistencias=EstadisticasJugadorPartido.Asistencias+ejp.Asistencias;
                int perdidas=EstadisticasJugadorPartido.Perdidas+ejp.Perdidas;
                int recuperos=EstadisticasJugadorPartido.Recuperos+ejp.Recuperos;
                int faltasPersonales=EstadisticasJugadorPartido.FaltasPersonales+ejp.FaltasPersonales;
                int faltasRecibidas=EstadisticasJugadorPartido.FaltasRecibidas+ejp.FaltasRecibidas;
                int faltasCometidas=EstadisticasJugadorPartido.FaltasCometidas+ejp.FaltasCometidas;

               Coordenada unaCoor= ejp.CoordenadasAcciones[0];
               /* unaCoor.X = ejp.CoordenadasAcciones[0].X;
                unaCoor.Y = ejp.CoordenadasAcciones[0].Y;*/

                await _estadisticasJugadorPartido.UpdateOneAsync(
                 a => a.IdJugador.Equals(ejp.IdJugador) && a.IdPartido.Equals(ejp.IdPartido),// Filtros para encontrar al jugador y partido correcto
                Builders<EstadisticasJugadorPartido>.Update
                .Set(b => b.TresPuntosConvertidos,tresPuntosConvertidos)
                .Set(c => c.TresPuntosIntentados,tresPuntosIntentados)
                .Set(d => d.TresPuntosPorcentaje,porcentaje)
                .Set(b => b.DosPuntosConvertidos,dosPuntosConvertidos)
                .Set(c => c.DosPuntosIntentados,dosPuntosIntentados)
                .Set(d => d.DosPuntosPorcentaje,porcentaje2pts)
                .Set(b => b.TirosLibresConvertidos,tirosLibresConvertidos)
                .Set(c => c.TirosLibresIntentados,tirosLibresIntentados)
                .Set(d => d.TirosLibresPorcentaje,porcentajeTirosLibres)
                .Set(b => b.RebotesOfensivos,rebotesOfensivos)
                .Set(c => c.RebotesDefensivos,rebotesDefensivos)
                .Set(d => d.RebotesTotales,rebotesTotales)
                .Set(b => b.Bloqueos,bloqueos)
                .Set(c => c.Asistencias,asistencias)
                .Set(d => d.Perdidas,perdidas)
                .Set(d => d.FaltasPersonales,faltasPersonales)
                .Set(b => b.FaltasRecibidas,faltasRecibidas)
                .Set(c => c.FaltasCometidas,faltasCometidas)
                .Set(d => d.Recuperos,recuperos)
                .Set(e => e.Puntos,puntos)
               
                );

                if(unaCoor != null) 
               { await _estadisticasJugadorPartido.UpdateOneAsync(
                 a => a.IdJugador.Equals(ejp.IdJugador) && a.IdPartido.Equals(ejp.IdPartido),// Filtros para encontrar al jugador y partido correcto
                Builders<EstadisticasJugadorPartido>.Update .Push(e => e.CoordenadasAcciones, unaCoor));}
                //await _estadisticasJugadorPartido.UpdateOneAsync(a => a.IdEquipo.Equals(eep.IdEquipo)&& a.IdPartido==eep.IdPartido,{$set});
                await _estadisticasEquipoPartidoService.CargarEstadistica(ejp);
                return true;
                /*                 var filter = Builders<Partido>
                  .Filter.Eq(e => e.Id, id);
                 var update = Builders<EstadisticasJugadorPartido>.Update
                        .Push<Coordenada>(e => e.CoordenadasAcciones, unaCoordenada);
                 await  _estadisticasJugadorPartido.FindOneAndUpdateAsync(filter, update); */
            }
            
            
        }

        public async Task<ReplaceOneResult> Save(EstadisticasJugadorPartido ejp)
    {
        return await _estadisticasJugadorPartido.ReplaceOneAsync<EstadisticasJugadorPartido>(x => x.IdJugador.Equals(ejp.IdJugador) && x.IdPartido.Equals(ejp.IdPartido),ejp, new UpdateOptions { IsUpsert = true });
    }

    }
}