﻿using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<Object>> Create(Usuario usuario)
        {
            try {
                if (string.IsNullOrWhiteSpace(usuario.Password))
                    return BadRequest("Por favor ingrese una contraseña.");

                //var user = _usuarios.Find<Usuario>(x => x.NomUser == usuario.NomUser).Any();

                //if (user != null)
                //    var usuarios=await _usuarioService.Get();
                //if(_usuarioService.Get().Find(x => x.NomUser == usuario.NomUser).Any())
                // return BadRequest("El usuario \"" + usuario.NomUser + "\" ya existe"); 
                //                _usuarios.Find<Usuario>(x => x.NomUser == usuario.NomUser).Any())


                return await _usuarioService.Create(usuario);


                //  return CreatedAtRoute("GetUsuario", new { id = usuario.Id.ToString() }, usuario);
            }
            catch (AppException ex)
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
                return BadRequest(new { result = false, message = "Nombre de usuario o contraseña incorrecta" });

            return Ok(new { result = true, user.Token, idUser = user.Id });
        }

        [AllowAnonymous]
        [HttpGet("VerifCi")]
        //[AcceptVerbs("Get" , "Post")]
        public IActionResult VerificarCI(string ci)
        {
            if (!_usuarioService.BuscarUsuarioPorCI(ci))
            {
                return Ok($"CI {ci} is already in use.");
            }

            return Ok(true);
        }
        /* old
        [AllowAnonymous]
        [HttpGet("ActivateAccount/{id}")]
        public async Task<IActionResult> ActivateAccount(string id)
        {
            string str = "";
            RestClient restClient = new RestClient();
             string value = await restClient.RunAsyncGet<string, string>("usuario/VeryFiyAccount", id);
            //string value = "sadf";
            if (value != null)
            {
                str = value;
               // return new { nombre = "potato happy" };
            }
            else
            {
                str = "¡Activación ha fallado!";
               // return new { nombre = "potato sad" };
            }
            return Ok(str);
            //ViewBag.Message = str;
            //return View();

        }
        */
        [AllowAnonymous]
        //[HttpGet("VeryFiyAccount/{id}")]
        [HttpGet("ActivateAccount/{id}")]
        public async Task<Object> VerifyAccount(string id)
        {
            string str = "";
            try
            {
                //  str = objReg.VeryFiyAccount(id);
                return await _usuarioService.VerificarCuenta(id);
               // return new { res = "potato" };
            }
            catch (Exception ex)
            {
                //throw new Exception(new{ StatusCode = HttpStatusCode.BadGateway.ToString(), Razon = ex.Message });
                return ex.Message;
            }

           // return str;
        }

        // If we got this far, something failed, redisplay form
        //  return View(model);
    }

    /*[HttpGet]
    public async Task<ActionResult> ActivateAccount(string id)
    {
        string str = "";
        RestClient restClient = new RestClient();
        string value = await restClient.RunAsyncGet<string, string>("api/UserReg/VeryFiyAccount", id);

        if (value != null)
        {
            str = value;
        }
        else
        {
            str = "Activation falid";
        }

        ViewBag.Message = str;
        return View();

    }*/
}

