using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using System;
using WebApi.Helpers;
using BasketJam.Models;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Security.Cryptography;

namespace BasketJam.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private IUsuarioService _usuarioService;
        private object usuario;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }



        [AllowAnonymous]
        [HttpPost("registrar")]
        public async Task<ActionResult<Object>> Create(Usuario usuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuario.Password))
                    return BadRequest("Por favor ingrese una contraseña.");

                return await _usuarioService.Create(usuario);
                

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }

        //[AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<Usuario>>> Get()
        {
            try
            {

                return await _usuarioService.Get();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }

        [HttpGet("{id:length(24)}", Name = "GetUsuario")]
        public async Task<ActionResult<Usuario>> Get(string id)
        {
            try
            {
                var usuario = await _usuarioService.Get(id);

                if (usuario == null)
                {
                    return NotFound();
                }

                return usuario;
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }
        //[FromBody]

        [AllowAnonymous]
        [HttpPost("autenticar")]
        public async Task<IActionResult> Autenticar([FromBody]Usuario userParam)
        {
            try
            {
                var user = await _usuarioService.Autenticar(userParam.NomUser, userParam.Password);

               /* if (user == null)
                    return BadRequest(new { result = false, message = "Nombre de usuario o contraseña incorrecta" });*/

                byte[] encodedRol = new UTF8Encoding().GetBytes(user.TipoUsuario.ToString());
                byte[] hashRol = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedRol);
                string encodedd = BitConverter.ToString(hashRol)
                   // without dashes
                   .Replace("-", string.Empty)
                   // make lowercase
                   .ToLower();

                return Ok(new { result = true, user.Token, idUser = user.Id, rolUsuario = encodedd, email = user.NomUser });
            }
            catch (Exception ex)
            {
                return BadRequest(
                
                    new { result = false, message = ex.Message }
                    //result = ex.Message
                    );
            }
        }

        [AllowAnonymous]
        [HttpGet("VerifCi")]
        //[AcceptVerbs("Get" , "Post")]
        public IActionResult VerificarCI(string ci)
        {
            try
            {
                if (!_usuarioService.BuscarUsuarioPorCI(ci))
                {
                    return Ok($"CI {ci} is already in use.");
                }

                return Ok(new { Resultado = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }
        }

        [AllowAnonymous]
        //[HttpGet("VeryFiyAccount/{id}")]
        [HttpGet("ActivateAccount/{id}")]
        public async Task<Object> VerifyAccount(string id)
        {
            string str = "";
            try
            {

                await _usuarioService.VerificarCuenta(id);
                return Redirect("http://www.basketjam.com");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [AllowAnonymous]
        //[HttpGet("VeryFiyAccount/{id}")]
        [HttpPost("ResetearPassword/")]
        public IActionResult ResetearContraseña(string email)
        {
            string str = "";
            try
            {

                _usuarioService.SendPassReset(email);
                return Ok(new { resultado = true });

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }

        }


        [AllowAnonymous]
        //[HttpGet("VeryFiyAccount/{id}")]
        [HttpPost("ResetearPasswordMovil/")]
        public IActionResult ResetearPasswordMovil(string email, string pass)
        {
            string str = "";
            try
            {

                _usuarioService.SendPassResetMovil(email, pass);
                return Ok(new { resultado = true });
                //return Redirect("http://www.basketjam.com");

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }

        }


        [AllowAnonymous]
        //[HttpGet("VeryFiyAccount/{id}")]
        [HttpPut("CambiarPassowrd/")]
        public async Task<IActionResult> CambiarPassowrd(string email, string password, string oldPassword)
        {
            try
            {

                await _usuarioService.CambiarPassword(email, password, oldPassword);
                return Ok(new { resultado = true });
                //return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }

        }

        [AllowAnonymous]
        //[HttpGet("VeryFiyAccount/{id}")]
        [HttpPut("CambiarPasswordMovil/")]
        public async Task<IActionResult> CambiarPasswordMovil(string email, string password)
        {
            try
            {
                Usuario u = await _usuarioService.BuscarUsuarioPorUser(email);
                await _usuarioService.CambiarPassword(email, password, u.Password);
                return Redirect("http://www.basketjam.com");
                //return Ok(new { resultado = true });
                //return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "Se ha producido un error: " + ex.Message
                });
            }

        }

        //[AllowAnonymous]
        [HttpPost("subirFoto/")]
        public IActionResult subirFoto(Imagen img)
        {
            try
            {
                _usuarioService.subirImagen(img);
                return Ok(new { Resultado = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Usuario usu)
        {
            try
            {
                var user = _usuarioService.BuscarUsuarioPorUser(usu.NomUser);
                
                if (user == null)
                {
                    return NotFound(new { Error = "No se ha encontrado el usuario." });
                }

                _usuarioService.ActualizarUsuario(id, usu);

                return Ok(new { Resultado = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            try
            {
                /*var equipo = _usuarioService.BuscarEquipo(id);

                if (equipo == null)
                {
                    return NotFound(new { Error = "No se ha encontrado el equipo." });
                }*/

                _usuarioService.EliminarUsuario(id);

                return Ok(new { Resultado = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Se ha producido un error: " + ex.Message });
            }
        }
    }
}

