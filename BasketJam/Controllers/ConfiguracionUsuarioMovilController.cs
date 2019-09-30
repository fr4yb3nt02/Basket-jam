using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BasketJam.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using System;
using BasketJam.Models;


namespace BasketJam.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfiguracionUsuarioMovilController : ControllerBase
    {
        private IConfiguracionUsuarioMovilService _configuracionUsuarioMovilService;

        public ConfiguracionUsuarioMovilController(IConfiguracionUsuarioMovilService configuracionUsuarioMovilService)
        {
            _configuracionUsuarioMovilService = configuracionUsuarioMovilService;
        }

        [AllowAnonymous]
        [HttpPut("ModoficarConfiguracionUsuarioMovil/{id:length(24)}")]
        public async Task<Object> ModoficarConfiguracionUsuarioMovil(string id, ConfiguracionUsuarioMovil conf)
        {
            try
            {
                Boolean resultado;
                resultado = await _configuracionUsuarioMovilService.ActualizarConfiguracionUsuarioMovil(id, conf);
                return (new { result = resultado });
            }
            catch (Exception ex)
            {
                return (new { result = false });
            }
        }

        [AllowAnonymous]
        [HttpGet("BuscarConfiguracionUsuarioMovil/{id:length(24)}")]
        public async Task<Object> BuscarConfiguracionUsuarioMovil(string id)
        {
            try
            {

                return await _configuracionUsuarioMovilService.BuscarConfiguracionUsuarioMovil(id);
            }

            catch (Exception ex)
            {
                throw new Exception("Se ha producido un error: " + ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPut("AgregarEquipoFavorito/{id:length(24)}")]
        public async Task<IActionResult> AgregarEquipoFavorito(string id, string equipo)
        {
            try
            {
                //  await _configuracionUsuarioMovilService.AgregarEquiposFavoritos(idUser, equipos);
                // return Ok (new { resultado = true });
                return Ok(new { resultado = await _configuracionUsuarioMovilService.AgregarEquiposFavoritos(id, equipo) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPut("QuitarEquipoFavorito/{id:length(24)}")]
        public async Task<IActionResult> QuitarEquipoFavorito(string id, string equipo)
        {
            try
            {
                //  await _configuracionUsuarioMovilService.AgregarEquiposFavoritos(idUser, equipos);
                // return Ok (new { resultado = true });
                return Ok(new { resultado = await _configuracionUsuarioMovilService.QuitarEquipooFavoritos(id, equipo) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("EquipoEsFavorito/")]
        public async Task<IActionResult> EquipoEsFavorito(string idUsuario, string idEquipo)
        {
            try
            {
                return Ok(new { resultado = await _configuracionUsuarioMovilService.EquipoEsFavorito(idUsuario, idEquipo) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("ListarEquiposFavoritos/{id:length(24)}")]
        public async Task<IActionResult> ListarEquiposFavoritos(string id)
        {
            try
            {
                return Ok( await _configuracionUsuarioMovilService.ListarEquiposFavoritos(id) );
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
