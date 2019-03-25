using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("register")]
        public ActionResult<Usuario> Create(Usuario usuario)
        {
            _usuarioService.Create(usuario);

            return CreatedAtRoute("GetUsuario", new { id = usuario.Id.ToString() }, usuario);
        }

        [HttpGet]
        public ActionResult<List<Usuario>> Get()
        {
            return _usuarioService.Get();
        }

        [HttpGet("{id:length(24)}", Name = "GetUsuario")]
        public ActionResult<Usuario> Get(string id)
        {
            var usuario = _usuarioService.Get(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }


    /*    [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]Usuario userParam)
        {
            var user = _usuarioService.Authenticate(userParam.NomUser, userParam.Password);

            if (user == null)
                return BadRequest(new { message = "Nombre de usuario o contraseña incorrecta" });

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _usuarioService.GetAll();
            return Ok(users);
        }*/
    }
}