using BasketJam.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi.Helpers;
using BasketJam.Models;

namespace BasketJam.Services
{
    public interface IPartidoService
    {
        Task<List<Partido>> ListarPartidos();
        Task<Partido>  BuscarPartido(string id);
        Task<Partido>  CrearPartido(Partido equipo);
        void ActualizarPartido(string id, Partido pa);
        void EliminarPartido(string id);

        Task<Boolean> AgregarJuezPartida(string id,List<Juez> jueces);
        
       Task<List<Partido>> ListarPartidosPorFecha(DateTime fecha);

       //Task<List<String>> DevuelvoListPartidosAndroid();

       Task<List<Object>> DevuelvoListPartidosAndroid();
    }

    public class PartidoService : IPartidoService
{
        private readonly IMongoCollection<Partido> _partidos;
        private readonly IMongoCollection<Equipo> _equipos;
        private readonly IMongoCollection<EstadisticasEquipoPartido> _estadisticasEquipoPartido;

            public PartidoService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
             _partidos=database.GetCollection<Partido>("partidos");
             _equipos=database.GetCollection<Equipo>("equipos");             
             _estadisticasEquipoPartido=database.GetCollection<EstadisticasEquipoPartido>("EstadisticasEquipoPartido");

        }
        public async Task<List<Partido>> ListarPartidos()
        {
            return await _partidos.Find(partido => true).ToListAsync();
        }

        public async Task<List<Partido>> ListarPartidosPorFecha(DateTime fecha)
        {
            return await _partidos.Find(partido => partido.fecha==fecha).ToListAsync();
        }

        public async Task<Partido> BuscarPartido(string id)
        {
            return await _partidos.Find<Partido>(partido => partido.Id == id).FirstOrDefaultAsync();
        }



        public async Task<Partido> CrearPartido(Partido partido)
        {
            
          //  foreach(BsonElement eq in partido.equipos )
        //   {
               /* var query = _equipos.
    Find(eqs=>true).
    Project<Equipo>(Builders<Equipo>.Projection.Include(e => e.NombreEquipo).Include(e=>e.Id));
            var results = await query.ToListAsync();*/
                //Equipo e=await _equipos.Find<Equipo>(x => x.Id == eq.Id).FirstOrDefaultAsync();
                //Equipo e=await _equipos.Find<Equipo>(equipo => equipo.Id == eq.Id).First()).Project(Builders<Equipo>.Projection.Include("NombreEquipo").Include("_id")).ToListAsync();
                 // var list = await _equipos.Find(asad=>asad.Id=).Project(Builders<BsonDocument>.Projection.Include("Price").Exclude("_id")).ToListAsync();
                //partido.equipos.Add(e);
           // }
            /*var list = await _equipos.Find<Equipo>(equipo => equipo.Id == id).FirstOrDefaultAsync()).Project(Builders<Equipo>.Projection.Include("NombreEquipo").Include("_id")).ToListAsync();
              foreach (Equipo doc in list)
      {
         partido.equipos.Add(doc.ToString());
      } */
            /*foreach(Equipo eq in partido.equipos )
            {
            var coso=new { id = eq.Id.ToString(),nombre = eq.NombreEquipo };
             partido.Add(coso);
            }*/
            await _partidos.InsertOneAsync(partido);
            return partido;
        }

        public void ActualizarPartido(string id, Partido pa)
        {
            _partidos.ReplaceOne(partido => partido.Id == id, pa);
        }

        public async Task<Boolean> AgregarJuezPartida(string id,List<Juez> jueces)
        {
            try
            {
            //_partidos.ReplaceOne(partido => partido.Id == id, pa);

            var filter = Builders<Partido>
             .Filter.Eq(e => e.Id, id);

            foreach(Juez j in jueces)
            {
            var update = Builders<Partido>.Update
                        .Push<Juez>(e => e.jueces, j);
            await  _partidos.FindOneAndUpdateAsync(filter, update);
            }
           
           return true;
            }
            catch
            {
                return false;
            }
        }

        public void EliminarPartido(Partido pa)
        {
            _partidos.DeleteOne(partido => partido.Id == pa.Id);
        }

        public void EliminarPartido(string id)
        {
            _partidos.DeleteOne(partido => partido.Id == id);
        }

public async Task<List<Object>> DevuelvoListPartidosAndroid()
{

    List<Partido> part = await _partidos.Find<Partido>(x => true).ToListAsync();
    List<EstadisticasEquipoPartido> estEqPar = await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(x => true).ToListAsync();
    List<Equipo> equi = await _equipos.Find<Equipo>(x => true).ToListAsync();
var res1= ( from p in part
               join est in estEqPar on p.Id equals est.IdPartido
               join e in equi on est.IdEquipo equals e.Id
               where p.equipos[0].Id.Equals(est.IdEquipo)
               select new
               {
                   idPartido=p.Id,
                   equipo= e.NombreEquipo,
                   estadio=p.estadio,
                   puntos=est.Puntos,
                   fecha=p.fecha.ToShortDateString(),
                   hora=p.fecha.ToShortTimeString(),
                   estado=((EstadoPartido)p.estado).ToString()
                   //equipo2= p.equipos[1].NombreEquipo,                
               }         
    );

var res2=(from p in part
               join est in estEqPar on p.Id equals est.IdPartido
               join e in equi on est.IdEquipo equals e.Id
               where p.equipos[1].Id.Equals(est.IdEquipo)
               select new
               {
                   idPartido=p.Id,
                   equipo= e.NombreEquipo,
                   estadio=p.estadio,
                   puntos=est.Puntos,
                   fecha=p.fecha.ToShortDateString(),
                   hora=p.fecha.ToShortTimeString(),                
                   estado=((EstadoPartido)p.estado).ToString()
                   
               } );
   
    //String idPartido=res1.

    var res= (from eq1 in res1
              join eq2 in res2 on eq1.idPartido equals eq2.idPartido
              select new
              {
                   idPartido=eq1.idPartido,
                   equipo1= eq1.equipo,
                   equipo2= eq2.equipo,
                   estadio=eq1.estadio,
                   puntosEq1=eq1.puntos,
                   puntosEq2=eq2.puntos,
                   fecha=eq1.fecha,
                   hora=eq1.hora,
                   estado=eq1.estado
              } );

    var resu=res1.Union(res2);
    //var res=new {res1.NombreEquipo,}
    List<Object> dev=new List<Object>();
    //var res=new {};
   // dev.Add(res.ToJson());

   foreach (var par in res)
   {
    //   var idPart=par.idPartido;
     //  do while(par)
     // var res=new{pepe=par.estado};
       dev.Add(par);
   }

   return dev;

    
}


    }
}