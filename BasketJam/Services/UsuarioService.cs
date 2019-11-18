using BasketJam.Helper;
using BasketJam.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using WebApi.Helpers;

namespace BasketJam.Services
{
    public interface IUsuarioService
    {
        Task<Usuario> Autenticar(string nomUser, string password);

        Task<Object> Create(Usuario usuario);
        Task<Usuario> Get(string id);

        Task<List<Usuario>> Get();

        bool BuscarUsuarioPorCI(string ci);
        Task<Object> VerificarCuenta(string activationCode);
        void SendPassReset(string emailId);
        void SendPassResetMovil(string emailId, string nuevaPass);
        Task<Boolean> CambiarPassword(string email, string password, string oldPassword);
        void subirImagen(Imagen img);
        Task<Usuario> BuscarUsuarioPorUser(string email);
        void ActualizarUsuario(string id,Usuario usu);



    }

    public class UsuarioService : IUsuarioService
    {

        private readonly IMongoCollection<Usuario> _usuarios;

        private readonly AppSettings _appSettings;

        private IConfiguracionUsuarioMovilService _configuracionUsuarioMovilService;


        string coso;


        public UsuarioService(IOptions<AppSettings> appSettings, IConfiguration config, IConfiguracionUsuarioMovilService configuracionUsuarioMovilService)
        {
            _appSettings = appSettings.Value;
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
            _configuracionUsuarioMovilService = configuracionUsuarioMovilService;




            _usuarios = database.GetCollection<Usuario>("usuarios");
        }


        public async Task<Usuario> Autenticar(string username, string password)
        {
            // Encripto la password con MD5
            byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
            byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
            string encoded = BitConverter.ToString(hash)
               // without dashes
               .Replace("-", string.Empty)
               // make lowercase
               .ToLower();
            var usuario = await _usuarios.Find<Usuario>(x => x.NomUser.Equals(username) && x.Password.Equals(encoded)).FirstOrDefaultAsync();

            if (usuario == null )
            {
                throw new Exception("Nombre de usuario o contraseña incorrecta.");
            }
            //return null;

            if (usuario.EmailValidado == false)
            {
                throw new Exception("Usuario aún no ha sido activado.");
            }



            // Retorno nulo si no encuentro el usuario

            if (!usuario.Password.Equals(encoded))
            {
                throw new Exception("Contraseña incorrecta.");
            }

            // si la autenticación es correcta genero el Token JWT 
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.TopSecret);//+usuario.TipoUsuario.ToString()
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
            var _usuario = await _usuarios.Find<Usuario>(usuario => usuario.Id == id).FirstOrDefaultAsync();

            return _usuario;
        }

        public bool BuscarUsuarioPorCI(string ci)
        {
            var _usuario = _usuarios.Find<Usuario>(usuario => usuario.CI == ci).FirstOrDefaultAsync();
            if (_usuario != null)
            {
                return true;
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

        public async Task<Usuario> BuscarUsuarioPorUser(string email)
        {
            return await _usuarios.Find<Usuario>(usuario => usuario.NomUser.Equals(email)).FirstOrDefaultAsync();

        }

        public async Task<Boolean> CambiarPassword(string email, string password, string oldPassword)
        {
            try
            {
                if (password.Length < 5 || password.Length > 20)
                    throw new Exception("La contraseña debe tener como mínimo 5 caracateres , y 50 como máximo.");

                // Encripto la password con MD5
                //byte[] encodedPassword1 = new UTF8Encoding().GetBytes(oldPassword);
                //byte[] hash1 = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword1);
                //string encoded1 = BitConverter.ToString(hash1)
                   // without dashes
                   //.Replace("-", string.Empty)
                   // make lowercase
                   //.ToLower();

                Usuario u = await _usuarios.Find<Usuario>(us => us.NomUser.Equals(email) && us.Password.Equals(oldPassword)).FirstOrDefaultAsync();
                if (u != null)
                {
                    // Encripto la password con MD5
                    byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
                    byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                    string encoded = BitConverter.ToString(hash)
                       // without dashes
                       .Replace("-", string.Empty)
                       // make lowercase
                       .ToLower();
                    var UpdateDefinitionBuilder = Builders<Usuario>.Update.Set(user => user.Password, encoded);

                    await _usuarios.FindOneAndUpdateAsync(us => us.NomUser.Equals(email), UpdateDefinitionBuilder);

                    return true;
                }
                else
                {
                    throw new Exception("La contraseña anterior no coincide con el usuario.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        public async Task<Object> Create(Usuario usuario)
        {

                try
            {
                /*Controles del model*/
                if (usuario.NomUser.Length < 5 || usuario.NomUser.Length > 50)
                    throw new Exception("El nombre de usuario debe tener como mínimo 5 caracateres , y 50 como máximo.");
                if (usuario.Password.Length < 5 || usuario.Password.Length > 20)
                    throw new Exception("La contraseña debe tener como mínimo 5 caracateres , y 50 como máximo.");
                if (usuario.Nombre.Length > 20)
                    throw new Exception("El nombre debe tener 20 caracteres como máximo.");
                if (usuario.Apellido.Length > 20)
                    throw new Exception("El apellido debe tener 20 caracteres como máximo.");

                /*Controles del model*/

                string host = "54.208.166.6";
                string scheme = "http";
                string port = "";
                /*string host = "localhost";
                string scheme = "http";
                string port = "5001";*/

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

                usuario.CodigoAutenticacion = Guid.NewGuid().ToString();

                // Encripto la password con MD5
                byte[] encodedPassword = new UTF8Encoding().GetBytes(usuario.Password);
                byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                string encoded = BitConverter.ToString(hash)
                   // without dashes
                   .Replace("-", string.Empty)
                   // make lowercase
                   .ToLower();
                usuario.Password = encoded;

                await _usuarios.InsertOneAsync(usuario);

                if (usuario.TipoUsuario == (TipoUsuario)2)
                {
                    ConfiguracionUsuarioMovil unaConf = new ConfiguracionUsuarioMovil();
                    unaConf.EquiposFavoritos = new List<String>();
                    unaConf.NotificacionEquiposFavoritos = false;
                    unaConf.NotificacionFinPartido = false;
                    unaConf.NotificacionInicioPartido = false;
                    unaConf.NotificacionTodosLosPartidos = false;
                    unaConf.Usuario = usuario.Id;
                    await _configuracionUsuarioMovilService.CrearConfiguracionUsuarioMovil(unaConf);
                }

                SendVerificationLinkEmail(usuario.NomUser, usuario.CodigoAutenticacion.ToString(), scheme, host, port);
                string mensaje = "El registro se ha realizado correctamente ,se ha enviado un link de activación a tu mail: " + usuario.NomUser;
                return new { result = true, mensaje = mensaje };

                //return usuario;
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey && ex.Message.Contains("IndexUniqueCI") && usuario.TipoUsuario != (TipoUsuario)2)
                    return (new { result = false, mensaje = "Ya existe un usuario con la C.I ingresada." });
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey && ex.Message.Contains("IndexUniqueCI") && usuario.TipoUsuario.Equals((TipoUsuario)2))
                    return (new { result = false, mensaje = "Ya existe un usuario el mail ingresado." });
                if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey && ex.Message.Contains("IndexUniqueNombreUser"))
                    //throw new AppException("Ya existe un usuario con el nombre de usuario ingresado.");
                    return (new { result = false, mensaje = "Ya existe un usuario con el nombre de usuario ingresado." });
                else
                    throw ex;

            }
        }

        private void SendVerificationLinkEmail(string emailId, string codigoActivacion, string scheme, string host, string port)
        {
            //var varifyUrl = scheme + "://" + host + ":" + port + "/usuario/ActivateAccount/" + codigoActivacion;//esto es para pruebas locales
            var varifyUrl = scheme + "://" + host + "/usuario/ActivateAccount/" + codigoActivacion;
            var fromMail = new MailAddress("basketjam2019@gmail.com", "Basket Jam Team");
            var toMail = new MailAddress(emailId);
            var frontEmailPassowrd = "BasketJam2019";
            string subject = "¡Tu cuenta en BasketJam ha sido creada exitosamente!";
            string body = "<br/><br/>Estamos encantados de informarte que tu cuenta " +
        " fué creada exitosamente. Por favor haz click en el link debajo para verificar tu cuenta" +
        " <br/><br/><a href='" + varifyUrl + "'>" + varifyUrl + "</a> ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromMail.Address, frontEmailPassowrd)

            };
            using (var message = new MailMessage(fromMail, toMail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }


        public void SendPassReset(string emailId)
        {
            string host = "54.208.166.6";
            string scheme = "http";
            string port = "";

            BasketJam.Models.RandomNumberGenerator generator = new BasketJam.Models.RandomNumberGenerator();
            string pass = generator.RandomPassword();

            // Encripto la password con MD5
            byte[] encodedPassword = new UTF8Encoding().GetBytes(pass);
            byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
            string encoded = BitConverter.ToString(hash)
               // without dashes
               .Replace("-", string.Empty)
               // make lowercase
               .ToLower();

            var UpdateDefinitionBuilder = Builders<Usuario>.Update.Set(use => use.Password, encoded);

            _usuarios.UpdateOneAsync(u => u.NomUser == emailId, UpdateDefinitionBuilder);

            // var varifyUrl = scheme + "://" + host + "/usuario/resetearContraseña/?" + emailId;
            //var varifyUrl= "http://basketjam.s3.us-east-2.amazonaws.com/Bjam/restarurarContrase%C3%B1a.html"+"?mail="+emailId;
            var fromMail = new MailAddress("basketjam2019@gmail.com", "Basket Jam Team");
            var toMail = new MailAddress(emailId);
            var frontEmailPassowrd = "BasketJam2019";
            string subject = "¡Se ha reseteado tu contraseña!";
            string body = "<br/><br/>Se le ha asignado la siguiente contraseña: <b>" + pass + "</b>" +
        " <br/><br/>por favor loguearse en Basket Jam con su usuario y contraseña proporcionada y cambiarla por una nueva. ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromMail.Address, frontEmailPassowrd)

            };
            using (var message = new MailMessage(fromMail, toMail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        public void SendPassResetMovil(string emailId, string nuevaPass)
        {
            string host = "54.208.166.6";
            string scheme = "http";
            string port = "";

            if (nuevaPass.Length < 5 || nuevaPass.Length > 20)
                throw new Exception("La contraseña debe tener como mínimo 5 caracateres , y 50 como máximo.");

            var UpdateDefinitionBuilder = Builders<Usuario>.Update.Set(use => use.Password, null);

            _usuarios.UpdateOneAsync(u => u.NomUser == emailId, UpdateDefinitionBuilder);

            var varifyUrl = "http://54.208.166.6/usuario/CambiarPasswordMovil" + "?email=" + emailId + "&password=" + nuevaPass;
            var fromMail = new MailAddress("basketjam2019@gmail.com", "Basket Jam Team");
            var toMail = new MailAddress(emailId);
            var frontEmailPassowrd = "BasketJam2019";
            string subject = "¡Se ha reseteado tu contraseña!";
            string body = "<br/><br/>Para aceptar la contraseña insertada desde la app de BasketJam haga click en el link debajo. " +
        " <br/><br/><a href='" + varifyUrl + "'>" + varifyUrl + "</a> ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromMail.Address, frontEmailPassowrd)

            };
            using (var message = new MailMessage(fromMail, toMail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }


        public async Task<Object> VerificarCuenta(string activationCode)
        {

            try
            {
                string str = "";


                var usuario = await _usuarios.Find<Usuario>(x => x.CodigoAutenticacion == activationCode).FirstOrDefaultAsync();
                if (usuario != null)
                {
                    await _usuarios.UpdateOneAsync(
                                     us => us.CodigoAutenticacion.Equals(activationCode),
                                     Builders<Usuario>.Update.
                                     Set(b => b.EmailValidado, true));


                    str = "Estimado usuario , su e-mail ha sido activado correctamente , ahora puede acceder a BasketJam con su cuenta";
                    
                    return new { result = true, mensaje = str };
                    // return usuario;
                }
                else
                {
                    str = "Estimado usuario , su e-mail no ha podido ser activado.";
                    return new { result = false, mensaje = str };
                }

                //  return str;
            }
            catch
            {
                return new { result = false, mensaje = "Se ha producido un error inesperado." };
            }

        }

        public void subirImagen(Imagen img)
        {
            try
            {
                string claseImagen = "Usuarios";
                ImagenService.subirImagen(img, claseImagen);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public void ActualizarUsuario(string id,Usuario usu)
        {
            try
            {
                /*Controles del model*/
                if (usu.Nombre.Length > 20)
                    throw new Exception("El nombre debe tener 20 caracteres como máximo.");
                if (usu.Apellido.Length > 20)
                    throw new Exception("El apellido debe tener 20 caracteres como máximo.");

                /*Controles del model*/
                _usuarios.ReplaceOne(equipo => equipo.Id.Equals(id), usu);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}