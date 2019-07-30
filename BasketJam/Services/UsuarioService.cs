using BasketJam.Helper;
using Microsoft.AspNetCore.Mvc;
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
    public interface IUsuarioService
    {
        Task<Usuario> Autenticar(string nomUser, string password);
        //List<Usuario> GetAll();
        Task<Usuario> Create(Usuario usuario);
        Task<Usuario> Get(string id);

        Task<List<Usuario>> Get();

        bool BuscarUsuarioPorCI(string ci);
        //IEnumerable<Usuario> GetAll();
    }

    public class UsuarioService : IUsuarioService
    {

        private readonly IMongoCollection<Usuario> _usuarios;
        
        private readonly AppSettings _appSettings;

        
        string coso;
        /*public UsuarioService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }*/
    public UsuarioService(IOptions<AppSettings> appSettings,IConfiguration config)
        {
            _appSettings = appSettings.Value;
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");

//var client = new MongoClient("mongodb+srv://fr4yb3nt02:Emiliano49110131@basketjam-ajrid.azure.mongodb.net/test?retryWrites=true");
//var database = client.GetDatabase("test");


             _usuarios=database.GetCollection<Usuario>("usuarios");


          /*   _usuarios=database.GetCollection<Usuario>("usuarios")
                .Indexes
                .CreateOneAsync(Builders<Usuario>
                                    .IndexKeys
                                    .Ascending(item => item.CI));*/
 /*
             var notificationLogBuilder = Builders<Usuario>.IndexKeys;
            var indexModel = new CreateIndexModel<Usuario>(notificationLogBuilder.Ascending(x => x.CI));
         IMongoCollection.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken).ConfigureAwait(false);    
 */
        }
        
        
        public async Task<Usuario> Autenticar(string username, string password)
        {            
            var usuario = await _usuarios.Find<Usuario>(x => x.NomUser == username && x.Password == password).FirstOrDefaultAsync();

            // Retorno nulo si no encuentro el usuario
            if (usuario == null)
                return null;

            // si la autenticación es correcta genero el Token JWT 
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.TopSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, usuario.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            usuario.Token = tokenHandler.WriteToken(token);            

            return usuario;
        }
  
        public async Task<Usuario> Get(string id)
        {
           var _usuario =await  _usuarios.Find<Usuario>(usuario => usuario.Id == id).FirstOrDefaultAsync();

           return _usuario;
        }

       public bool BuscarUsuarioPorCI(string ci)
        {
           var _usuario =  _usuarios.Find<Usuario>(usuario => usuario.CI == ci).FirstOrDefaultAsync();
           // return  _usuarios.Find<Usuario>(usuario => usuario.CI == ci).FirstOrDefaultAsync();
           if(_usuario!=null)
           {
           return  true ;
           }
           else 
           {
           return false;
           }

        }

        public async Task<List<Usuario>> Get()
        {
            return await _usuarios.Find(usuario => true).ToListAsync();
            
        }

        
        public async Task<Usuario> Create(Usuario usuario)
        {
            try
            {
                /*Inicio creación de índices*/
                IndexKeysDefinition<Usuario> keysNomUser =
               Builders<Usuario>.IndexKeys.Ascending("NombreUser");
                    var optionsNomUser = new CreateIndexOptions { Name = "IndexUniqueNombreUser", Unique = true };
                    var indexModelNomUser = new CreateIndexModel<Usuario>(keysNomUser, optionsNomUser);
                    await _usuarios.Indexes.CreateOneAsync(indexModelNomUser);

                IndexKeysDefinition<Usuario> keysCi =
                   Builders<Usuario>.IndexKeys.Ascending("CI");
                   var optionsCi = new CreateIndexOptions { Name = "IndexUniqueCI", Unique = true };
                   var indexModelCi = new CreateIndexModel<Usuario>(keysCi, optionsCi);
                   await _usuarios.Indexes.CreateOneAsync(indexModelCi);
                /*Fin creación de índices*/


                await _usuarios.InsertOneAsync(usuario);

                return usuario;
            }
            catch(MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey && ex.Message.Contains("IndexUniqueCI"))
                     throw new AppException("Ya existe un usuario con la C.I ingresada.");
               else if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey && ex.Message.Contains("IndexUniqueNombreUser"))
                        throw new AppException("Ya existe un usuario con el nombre de usuario ingresado.");
                else
                    throw ex;
           
            }
        }
    }
}