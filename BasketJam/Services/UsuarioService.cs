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
        //IEnumerable<Usuario> GetAll();
    }

    public class UsuarioService : IUsuarioService
    {

        private readonly IMongoCollection<Usuario> _usuarios;
        
        private readonly AppSettings _appSettings;

        

        /*public UsuarioService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }*/
    public UsuarioService(IOptions<AppSettings> appSettings,IConfiguration config)
        {
            _appSettings = appSettings.Value;
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
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

        public async Task<List<Usuario>> Get()
        {
            return await _usuarios.Find(usuario => true).ToListAsync();
            
        }

        
        public async Task<Usuario> Create(Usuario usuario)
        {

            // validation
            /*/if (string.IsNullOrWhiteSpace(usuario.Password))
                throw new AppException("Por favor ingrese una contraseña.");

            //var user = _usuarios.Find<Usuario>(x => x.NomUser == usuario.NomUser).Any();
            
            //if (user != null)
            if(_usuarios.Find<Usuario>(x => x.NomUser == usuario.NomUser).Any())
                throw new AppException("El usuario \"" + usuario.NomUser + "\" ya existe"); 
                //throw new AppException("User not found");
            if(_usuarios.Find<Usuario>(x => x.NomUser==usuario.NomUser).First()!=null)
              throw new AppException("El usuario \"" + usuario.NomUser + "\" ya existe"); *
            
              

            /*byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt; */
          if(_usuarios.Find<Usuario>(x => x.NomUser==usuario.NomUser).Any())
              throw new AppException("El usuario \"" + usuario.NomUser + "\" ya existe"); 
            
            if(_usuarios.Find<Usuario>(x => x.CI==usuario.CI).Any())
                throw new AppException("Ya existe un usuario con la cédula ingresada.");

            IndexKeysDefinition<Usuario> keys =
            Builders<Usuario>.IndexKeys.Ascending("CI");            
            var options = new CreateIndexOptions { Name = "IndexUniqueCI" , Unique=true};            
            var indexModel = new CreateIndexModel<Usuario>(keys,options);
            await _usuarios.Indexes.CreateOneAsync(indexModel);

            /*var options = new CreateIndexOptions() { Unique = true };
            var field = new StringFieldDefinition<Usuario>("CI");
            var indexDefinition = new IndexKeysDefinitionBuilder<Usuario>().Ascending(field);
            await _usuarios.Indexes.CreateOneAsync(indexDefinition, options);
 */

            await _usuarios.InsertOneAsync(usuario);
            //_context.Users.Add(user);
            //_context.SaveChanges();

            return usuario;
        }
    }
}