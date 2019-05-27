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
    public interface IEquipoService
    {
        Task<List<Equipo>> ListarEquipos();
        Task<Equipo> BuscarEquipo(string id);
        Task<Equipo> CrearEquipo(Equipo equipo);
        void ActualizarEquipo(string id, Equipo eq);
        void EliminarEquipo(string id);

        Task<List<Jugador>> ListarJugadoresEquipo(string id);
    }

    public class EquipoService : IEquipoService
{
        private readonly IMongoCollection<Equipo> _equipos;
        private readonly IMongoCollection<Jugador> _jugadores;        
        //private readonly IMongoCollection<Estadio> _estadios;
        public EquipoService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
             _equipos=database.GetCollection<Equipo>("equipos");
             _jugadores=database.GetCollection<Jugador>("jugadores");
            // _estadios=database.GetCollection<Estadio>("estadios");

        }
        public async Task<List<Equipo>> ListarEquipos()
        {
            
            return await _equipos.Find(equipo => true).ToListAsync();
        }

                public async Task<List<Jugador>> ListarJugadoresEquipo(string id)
        {
            
            return await _jugadores.Find<Jugador>(e=>e.IdEquipo==id).ToListAsync();
        }

        public async Task<Equipo> BuscarEquipo(string id)
        {   
            return await _equipos.Find<Equipo>(equipo => equipo.Id == id).FirstOrDefaultAsync(); 
            //return await _equipos.Find<Equipo>(equipo => equipo.Id == id).Project(y => y.Select(y => y.direccion)).FirstOrDefaultAsync();
        }

  /*      public void AgregarJugadorAEquipo(string equipoId,Jugador nuevoJugador)
{
    var filter = Builders<Equipo>.Filter.And(
                 Builders<Equipo>.Filter.Where(x => x.Id == equipoId), 
                 Builders<Equipo>.Filter.Eq("jugadores.Id", nuevoJugador.Id));
    var update = Builders<Product>.Update.Push("jugadores.$.SubCategories", newSubCategory);
    await collection.FindOneAndUpdateAsync(filter, update);
}
*/
       /*/ public async Task<Equipo> CrearEquipo(Equipo equipo,Estadio estadio)
        {
            await _estadios.InsertOneAsync(estadio);
            equipo.Estadio.Id=estadio.Id;
             await _equipos.InsertOneAsync(equipo);
                 
            return equipo;
        }*/
        public async Task<Equipo> CrearEquipo(Equipo equipo)
        {

             await _equipos.InsertOneAsync(equipo);
                 
            return equipo;
        }

        public void ActualizarEquipo(string id, Equipo eq)
        {
            _equipos.ReplaceOne(equipo => equipo.Id == id, eq);
        }

        public void EliminarEquipo(Equipo eq)
        {
            _equipos.DeleteOne(equipo => equipo.Id == eq.Id);
        }

        public void EliminarEquipo(string id)
        {
            _equipos.DeleteOne(equipo => equipo.Id == id);
        }
    }
}
