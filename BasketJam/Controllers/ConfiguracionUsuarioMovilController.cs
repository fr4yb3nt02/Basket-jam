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
    public class ConfiguracionUsuarioMovilController
    {
        private IConfiguracionUsuarioMovilService _configuracionUsuarioMovilService;

        public ConfiguracionUsuarioMovilController(IConfiguracionUsuarioMovilService configuracionUsuarioMovilService)
        {
            _configuracionUsuarioMovilService = configuracionUsuarioMovilService;
        }

        [AllowAnonymous]
        [HttpPost("ModoficarConfiguraionUsuarioMovil/{id:length(24)}")]
        public async Task<Boolean> ModoficarConfiguraionUsuarioMovil(string idUser,ConfiguracionUsuarioMovil conf)
        {
            try
            {
                Boolean resultado;
                resultado = await _configuracionUsuarioMovilService.ActualizarConfiguracionUsuarioMovil(idUser,conf);
                return (resultado);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [AllowAnonymous]
        [HttpGet("BuscarConfiguracionUsuarioMovil")]
        public async Task<ConfiguracionUsuarioMovil> BuscarConfiguracionUsuarioMovil(string idUser)
        {
           return await _configuracionUsuarioMovilService.BuscarConfiguracionUsuarioMovil(idUser);
        }

        [AllowAnonymous]
        [HttpGet("AgregarEquipoFavorito/{id:length(24)}")]
        public async Task<bool> AgregarEquipoFavorito(string idUser,List<string> equipos)
        {
            try
            {
                await _configuracionUsuarioMovilService.AgregarEquiposFavoritos(idUser, equipos);
                return true;
            }
            catch
            {
                return false;
            }
        }

        [AllowAnonymous]
        [HttpGet("EquipoEsFavorito/{id:length(24)}")]
        public async Task<bool> EquipoEsFavorito(string idUser, string idEquipo)
        {
            try
            {
                return await _configuracionUsuarioMovilService.EquipoEsFavorito(idUser, idEquipo);                
            }
            catch
            {
                return false;
            }
        }
    }
}
