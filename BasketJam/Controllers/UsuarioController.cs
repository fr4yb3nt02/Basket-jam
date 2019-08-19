using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using System;
using WebApi.Helpers;
using BasketJam.Models;

namespace BasketJam.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private IUsuarioService _usuarioService;
  
        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        

        [AllowAnonymous]
        [HttpPost("registrar")]
        public async Task<ActionResult<Usuario>> Create(Usuario usuario)
        {
            try{
            if (string.IsNullOrWhiteSpace(usuario.Password))
                return BadRequest("Por favor ingrese una contraseña.");

            //var user = _usuarios.Find<Usuario>(x => x.NomUser == usuario.NomUser).Any();
            
            //if (user != null)
        //    var usuarios=await _usuarioService.Get();
            //if(_usuarioService.Get().Find(x => x.NomUser == usuario.NomUser).Any())
            // return BadRequest("El usuario \"" + usuario.NomUser + "\" ya existe"); 
//                _usuarios.Find<Usuario>(x => x.NomUser == usuario.NomUser).Any())
  

            await _usuarioService.Create(usuario);


                return CreatedAtRoute("GetUsuario", new { id = usuario.Id.ToString() }, usuario);
            }
            catch(AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<Usuario>>> Get()
        {
   
            
            return await _usuarioService.Get();
        }

        [HttpGet("{id:length(24)}", Name = "GetUsuario")]
 
        public async Task<ActionResult<Usuario>> Get(string id)
        {
            var usuario = await _usuarioService.Get(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }
//[FromBody]

      [AllowAnonymous]
        [HttpPost("autenticar")]
        public async Task<IActionResult> Autenticar([FromBody]Usuario userParam)
        {
            var user = await _usuarioService.Autenticar(userParam.NomUser, userParam.Password);

            if (user == null)
                return BadRequest(new {result=false, message = "Nombre de usuario o contraseña incorrecta" });

            return Ok(new {result=true, user.Token });
        }

[AllowAnonymous]
[HttpGet("VerifCi")]
//[AcceptVerbs("Get" , "Post")]
public  IActionResult VerificarCI(string ci)
{
    if (!_usuarioService.BuscarUsuarioPorCI(ci))
    {
        return Ok($"CI {ci} is already in use.");
    }

    return Ok(true);
}

    }
}