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
    public interface IBitacoraService
    {
        BitacoraPartido BuscarBitacoraPartido(string idPartido);
        Task<Boolean> GenerarBitacora(BitacoraPartido bp);
    }

    public class BitacoraService : IBitacoraService
{
        private readonly IMongoCollection<BitacoraPartido> _bitacoraPartido;      

        private readonly IMongoCollection<EstadisticasJugadorPartido> _estadisticasJugadorPartido;

        public BitacoraService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
             _bitacoraPartido=database.GetCollection<BitacoraPartido>("BitacorasPartidos");
            _estadisticasJugadorPartido=database.GetCollection<EstadisticasJugadorPartido>("EstadisticasJugadorPartido");

        }

        public  BitacoraPartido BuscarBitacoraPartido(string idPartido)
        {
            return  _bitacoraPartido.Find<BitacoraPartido>(bp => bp.idPartido == idPartido).FirstOrDefault();
        }

        public async Task<Boolean> GenerarBitacora(BitacoraPartido bp)
        {
            try
            {
            BitacoraPartido bitacoraPartido=BuscarBitacoraPartido(bp.idPartido);
            
            if(bitacoraPartido==null)
            {
                
                 await _bitacoraPartido.InsertOneAsync(bp);
                // await _estadisticasJugadorPartido.CargarEstadistica(bp);
                 //await _estadisticasEquipoPartidoService.CargarEstadistica(ejp.IdPartido,equipo.IdEquipo,ptos);
                 return true;
                 
            }
            else
            {
             var filter = Builders<BitacoraPartido>
             .Filter.Eq(e => e.idPartido, bp.idPartido);

            foreach(BitacoraPartido.BitacoraTimeLine b in bp.bitacoraTimeLine)
            {
            var update = Builders<BitacoraPartido>.Update
                        .Push<BitacoraPartido.BitacoraTimeLine>(e => e.bitacoraTimeLine, b);
            await  _bitacoraPartido.FindOneAndUpdateAsync(filter, update);
            }
                return true;
            }
            }
            catch
            {
                return false;
            }
            
        }

    }
}