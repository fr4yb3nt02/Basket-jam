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
        public async Task<ConfiguracionUsuarioMovil> BuscarConfiguracionUsuarioMovil(string id)
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
        [HttpGet("AgregarEquipoFavorito/{id:length(24)}")]
        public async Task<IActionResult> AgregarEquipoFavorito(string idUser, List<string> equipos)
        {
            try
            {
                //  await _configuracionUsuarioMovilService.AgregarEquiposFavoritos(idUser, equipos);
                // return Ok (new { resultado = true });
                return Ok(new { resultado = await _configuracionUsuarioMovilService.AgregarEquiposFavoritos(idUser, equipos) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("EquipoEsFavorito/{id:length(24)}")]
        public async Task<IActionResult> EquipoEsFavorito(string idUser, string idEquipo)
        {
            try
            {
                return Ok(new { resultado = await _configuracionUsuarioMovilService.EquipoEsFavorito(idUser, idEquipo) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
