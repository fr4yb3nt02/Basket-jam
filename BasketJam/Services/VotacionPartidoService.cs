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
    public interface IVotacionPartidoService
    {
        Task<VotacionPartido> BuscarVotacionPartido(string id);
        Task<Boolean> votarEquipoPartido(VotacionPartido votacionPartido);
        Task<Boolean> usuarioYaVoto(string usuario, string idPartido);
    }

    public class VotacionPartidoService : IVotacionPartidoService
{
        private readonly IMongoCollection<VotacionPartido> _votacionPartido;      

        public VotacionPartidoService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
            _votacionPartido=database.GetCollection<VotacionPartido>("votacionPartido");
        }


        public  async Task<Boolean> votarEquipoPartido(VotacionPartido votacionPartido)
        {
            try{
                VotacionPartido vp= await BuscarVotacionPartido(votacionPartido.IdPartido);
        if(vp != null)
        {
                var equipoVotadoIndex = await _votacionPartido
                    .Find(p => p.IdPartido == votacionPartido.IdPartido)
                    .Project(p => p.Contenido_Votacion.FindIndex(t => t.idEquipo == votacionPartido.Contenido_Votacion[0].idEquipo))
                    .SingleOrDefaultAsync();
               
               if(vp.Usuarios.Any(v => v == votacionPartido.Usuarios[0]))
               {
                   return false;
               }
               else
               {
                              int votos =vp.Contenido_Votacion[equipoVotadoIndex].votos;

             var UpdateDefinitionBuilder = Builders<VotacionPartido>.Update.Set(p => p.Contenido_Votacion[equipoVotadoIndex].votos, votos+1).Push(p => p.Usuarios,votacionPartido.Usuarios[0]);

                    await _votacionPartido.UpdateOneAsync(p=> p.IdPartido == vp.IdPartido, UpdateDefinitionBuilder); 
                    return true;
               }

    
        }
        else
        {
            await _votacionPartido.InsertOneAsync(votacionPartido);
            return true;
        }

            }
            catch
            {
                return false;
            }
            
        }

        public async Task<Boolean> usuarioYaVoto(string usuario,string idPartido)
        {
            try
            {
                VotacionPartido vp = await BuscarVotacionPartido(idPartido);
                if (vp != null)
                {
                    if (vp.Usuarios.Any(v => v == usuario))
                    {
                        return true;
                    }
                    else
                        return false;
                }
                else
                return false;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<VotacionPartido> BuscarVotacionPartido(string id)
        {
            return await _votacionPartido.Find<VotacionPartido>(VotacionPartido => VotacionPartido.IdPartido == id).FirstOrDefaultAsync();
        }
       
        }
}