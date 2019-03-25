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
    public interface IUsuarioService
    {
      //  Usuario Authenticate(string nomUser, string password);
        //List<Usuario> GetAll();
        Usuario Create(Usuario usuario);
        Usuario Get(string id);

        List<Usuario> Get();
        //IEnumerable<Usuario> GetAll();
    }

    public class UsuarioService : IUsuarioService
    {

        private readonly IMongoCollection<Usuario> _usuarios;
        

        //private readonly DataContext _dataContext=new DataContext(IConfiguration config);

        private readonly DataContext _dataContext;
        /*public UsuarioService(DataContext dataContext) {
        _dataContext = dataContext;
        }*/
        

        public UsuarioService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
             _usuarios=database.GetCollection<Usuario>("usuarios");

        }
   /*     public UsuarioService(IConfiguration config)
        {
               var _dataContext=new DataContext(config);
               _usuarios=_dataContext.MongoDatabase.GetCollection<Usuario>("usuarios");
        } */

       // [Route("buscarEmpleado/{id}")]
  
        public Usuario Get(string id)
        {
            return _usuarios.Find<Usuario>(usuario => usuario.Id == id).FirstOrDefault();
        }

        public List<Usuario> Get()
        {
            return _usuarios.Find(usuario => true).ToList();
        }

        
        public Usuario Create(Usuario usuario)
        {

            // validation
            if (string.IsNullOrWhiteSpace(usuario.Password))
                throw new AppException("Por favor ingrese una contraseña.");

            //var user = _usuarios.Find<Usuario>(x => x.NomUser == usuario.NomUser).Any();
            
            //if (user != null)
            if(_usuarios.Find<Usuario>(x => x.NomUser == usuario.NomUser).Any())
                throw new AppException("El usuario \"" + usuario.NomUser + "\" ya existe"); 
                //throw new AppException("User not found");
            /*/if(_usuarios.Find<Usuario>(x => x.NomUser==usuario.NomUser).First()!=null)
              throw new AppException("El usuario \"" + usuario.NomUser + "\" ya existe"); *
            
              

            /*byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt; */

            _usuarios.InsertOne(usuario);
            //_context.Users.Add(user);
            //_context.SaveChanges();

            return usuario;
        }

        /*public Usuario Create(Usuario usuario)
        {
          //  _dataContext.MongoDatabase.GetCollection<Usuario>("usuarios").InsertOne(usuario);
           _usuarios.InsertOne(usuario);
            return usuario;
        }*/

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /*private List<Usuario> _users = new List<Usuario>
        {
            new Usuario { Id = 1, Nombre = "Test", Apellido = "User", NomUser = "test", Password = "test" }
        };*/

        /*public List<Usuario> GetAll()
        {
            return _usuarios.Find(usuario => true).ToList();
        }
        private readonly AppSettings _appSettings;

        public UsuarioService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public Usuario Authenticate(string nomUser, string password)
        {
            List<Usuario> users=GetAll();
            var user = users.SingleOrDefault(x => x.NomUser == nomUser && x.Password == password);

            // devuelve null si no se encuentra el usuario
            if (user == null)
                return null;

            // la autenticación fué exitosa , por lo que devuelve un token JWT            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.TopSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // elimina la contraseña antes de retornar            
            user.Password = null;

            return user;
        }*/

     /*   public IEnumerable<Usuario> GetAll()
        {
            List<Usuario> users=GetAllUsers();
            // return users without passwords
            return users.Select(x => {
                x.Password = null;
                return x;
            });
        }*/
    }
}