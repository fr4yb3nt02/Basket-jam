using BasketJam.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi.Helpers;
using BasketJam.Models;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Dynamic;

namespace BasketJam.Services
{
    public interface IPartidoService
    {

        void ActualizarPartido(string id, Partido pa);
        void EliminarPartido(string id);
        void ActualizarTiempoPartido(string id, string tiempo);
        void ActualizarEstadoPartido(string id, string tiempo);
        Task<List<Partido>> ListarPartidos();
        Task<Partido> BuscarPartido(string id);
        Task<Partido> CrearPartido(Partido equipo);
        Task<Boolean> AgregarJuezPartida(string id, List<Juez> jueces);
        Task<List<Partido>> ListarPartidosPorFecha(DateTime fecha);
        Task<List<Object>> DevuelvoListPartidosAndroid();
        Task<Object> ConsultarHeaderPartido(string idPartido);
        Task<Object> ConsultaDetallesPartido(string idPartido);
        Task<Object> UltimosEventosEquipo(string idPartido);
        Task<Object> ListarEquipoJugador(string idPartido);
        Task<Boolean> AgregarJugadoresAPartido(string id, List<EquipoJugador> jugadores);
        Task<List<Object>> ListarPartidosProgOJug();
        Task<List<Object>> ListarPartidosPorEstado(int estado);
        Task<List<Object>> ListarJugadoresEquiposPartido(string idPartido);
        Task<List<Object>> ListarEstadios();
        Task<List<Object>> FixtureTodosLosEquipos();
        Task<List<Object>> FixturePorEquipo(string idEquipo);
        Task<List<Object>> EstadisticasJugsEquipoPartido(string idPartido, string idEquipo);
        Task<Object> DevolverEstadoPartido(string idPartido);
        Task<List<Partido>> ListarPartidosSinJueces();
        void ActualizarCuartoPartido(string id, int cuarto);
        //Task<List<String>> DevuelvoListPartidosAndroid();
    }

    public class PartidoService : IPartidoService
    {
        private readonly IMongoCollection<Partido> _partidos;
        private readonly IMongoCollection<Equipo> _equipos;
        private readonly IMongoCollection<CuerpoTecnico> _cuerpoTecnico;
        private readonly IMongoCollection<Jugador> _jugadores;
        private readonly IMongoCollection<EstadisticasEquipoPartido> _estadisticasEquipoPartido;
        private readonly IMongoCollection<EstadisticasJugadorPartido> _estadisticasJugadorPartido;
        private readonly IMongoCollection<ConfiguracionUsuarioMovil> _configuracionUsuarioMovil;
        private readonly IMongoCollection<BitacoraPartido> _bitacoraPartido;
        private IVotacionPartidoService _votacionPartidoService;
        private readonly IMongoCollection<TablaDePosiciones> _tablaDePosiciones;
        private string urlNotifications = "https://exp.host/--/api/v2/push/send";


        // private IEstadisticasEquipoPartidoService _estadisticasEquipoPartidoService;

        public PartidoService(IConfiguration config, IVotacionPartidoService votacionPartidoService)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
            _partidos = database.GetCollection<Partido>("partidos");
            _equipos = database.GetCollection<Equipo>("equipos");
            _jugadores = database.GetCollection<Jugador>("jugadores");
            _cuerpoTecnico = database.GetCollection<CuerpoTecnico>("cuerpoTecnico");
            _estadisticasEquipoPartido = database.GetCollection<EstadisticasEquipoPartido>("EstadisticasEquipoPartido");
            _votacionPartidoService = votacionPartidoService;
            _estadisticasJugadorPartido = database.GetCollection<EstadisticasJugadorPartido>("EstadisticasJugadorPartido");
            _bitacoraPartido= database.GetCollection<BitacoraPartido>("BitacorasPartidos");
            _configuracionUsuarioMovil = database.GetCollection<ConfiguracionUsuarioMovil>("configuracionUsuarioMovil");
            _tablaDePosiciones = database.GetCollection<TablaDePosiciones>("tablaDePosiciones");
            //  _estadisticasEquipoPartidoService = estadisticasEquipoPartidoService;



        }

        public async Task<List<Partido>> ListarPartidos()
        {
            return await _partidos.Find(partido => true).ToListAsync();
        }

        public async Task<Object> ListarEquipoJugador(string idPartido)
        {
            /*    try
        { */
            //List<Partido> part = await _partidos.Find<Partido>(x => true).ToListAsync();
            //List<EstadisticasEquipoPartido> estEqPar = await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(x => true).ToListAsync();
            //List<Equipo> equi = await _equipos.Find<Equipo>(x => true).ToListAsync();

            Partido p = await _partidos.Find<Partido>(partido => partido.Id == idPartido).FirstOrDefaultAsync();

            List<EquipoJugador> equiposJugadores = new List<EquipoJugador>();
            List<Object> jugadores = new List<Object>();
            List<Object> jugEq = new List<Object>();
            Jugador j;
            CuerpoTecnico tecnico;
            int puntosEquipoPartido = 0;
            EstadisticasEquipoPartido estEqPar = new EstadisticasEquipoPartido();
            BitacoraPartido bp = new BitacoraPartido();
            dynamic part;

            bp = await _bitacoraPartido.Find<BitacoraPartido>(bpp => bpp.idPartido.Equals(idPartido)).FirstOrDefaultAsync();
            
            string tiempo = "10:00";
            if(bp!=null)
            {
                //tiempo = bp.bitacoraTimeLine.Last().Tiempo;
                List<BitacoraPartido.BitacoraTimeLine> btl = bp.bitacoraTimeLine.OrderBy(b => b.Cuarto).ToList();
                tiempo = btl.Last().Tiempo;
            }
            int periodo = p.cuarto;

            
            

            foreach (Equipo e in p.equipos)
            {
                string id = "0";
                if (id != e.Id)
                {
                    id = e.Id;
                    jugadores = new List<Object>();
                    puntosEquipoPartido = 0;
                    if (_estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(a => a.IdEquipo.Equals(e.Id) && a.IdPartido.Equals(p.Id)).Any())
                    {
                        estEqPar = _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(a => a.IdEquipo.Equals(e.Id) && a.IdPartido.Equals(p.Id)).FirstOrDefault();
                        puntosEquipoPartido = estEqPar.Puntos;
                    }


                    foreach (EquipoJugador ej in p.EquipoJugador)
                    {
                        int cantJugadoresConvocados;
                        cantJugadoresConvocados = ej.jugadorEquipo.Count;




                        foreach (EquipoJugador.JugadorEquipo je in ej.jugadorEquipo)
                        {
                            j = await _jugadores.Find<Jugador>(x => x.Id == je.idJugador && x.IdEquipo == id).FirstOrDefaultAsync();
                            if (j != null)
                            {
                                jugadores.Add(new
                                {
                                    idJugador = j.Id,
                                    Nombre = j.Nombre,
                                    Apellido = j.Apellido,
                                    Posicion = j.Posicion,
                                    Camiseta = je.nroCamiseta,
                                    EsTitular = je.esTitular,
                                    EsCapitan = je.esCapitan,
                                    FotoJugador =j.UrlFoto
                                });
                            }
                        }
                        //equiposJugadores.Add(ej);
                    }
                    tecnico = await _cuerpoTecnico.Find<CuerpoTecnico>(x => x.IdEquipo == e.Id && x.Cargo == (CargoCuerpoTecnico)0).FirstOrDefaultAsync();
                    jugEq.Add(new
                    {
                        Equipo = e.Id,
                        NombreEquipo = e.NombreEquipo,
                        FotoEquipo = e.UrlFoto,
                        PuntosEnPartido = puntosEquipoPartido,
                        Entrenador = tecnico.Nombre + " " + tecnico.Apellido,
                        IdEntrenador = tecnico.Id,
                        jugadores
                    });
                }
            }

            part = new
            {
                //tiempo = tiempo,
                tiempo=p.Tiempo,
                periodo = periodo,
                jugEq=jugEq
            };
            return part;

        }

        public async Task<List<Partido>> ListarPartidosPorFecha(DateTime fecha)
        {
            return await _partidos.Find(partido => partido.fecha == fecha).ToListAsync();
        }

        public async Task<List<Object>> ListarPartidosProgOJug()
        {
            try
            {
                List<Object> dev = new List<Object>();
                List<Partido> parts = await _partidos.Find<Partido>(partido => partido.estado != (EstadoPartido)3 && partido.estado != (EstadoPartido)4).ToListAsync();
                foreach (Partido p in parts)
                {
                    Equipo Eq1 = await _equipos.Find<Equipo>(x => x.Id == p.equipos[0].Id).FirstOrDefaultAsync();
                    Equipo Eq2 = await _equipos.Find<Equipo>(x => x.Id == p.equipos[1].Id).FirstOrDefaultAsync();

                    dev.Add(new
                    {
                        idPartido = p.Id,
                        idEquipo1 = Eq1.Id,
                        fotoEq1 = Eq1.UrlFoto,
                        idEquipo2 = Eq2.Id,
                        fotoEq2 = Eq2.UrlFoto,
                        estadio = p.estadio,
                        categoria = Eq1.Categoria,
                        equipo1 = Eq1.NombreEquipo,
                        equipo2 = Eq2.NombreEquipo,
                        fecha = p.fecha.ToString("dd/MM/yyyy"),
                        hora = p.fecha.ToShortTimeString(),
                        estado = p.estado
                    });
                    //dev.Add(det);
                };


                return dev;
            }
            catch (Exception ex)
            {
                List<Object> dev = new List<Object>();
                dev.Add(new { Error = ex.Message });
                return dev;
            }
        }

        public async Task<List<Object>> ListarPartidosPorEstado(int estado)
        {
            try
            {
                List<Object> dev = new List<Object>();
                List<Partido> parts = new List<Partido>();
                if(estado==1)
                     parts = await _partidos.Find<Partido>(partido => partido.estado == (EstadoPartido)estado || partido.estado == (EstadoPartido)2 ).ToListAsync();
                else
                     parts = await _partidos.Find<Partido>(partido => partido.estado == (EstadoPartido)estado).ToListAsync();

                foreach (Partido p in parts)
                {
                    Equipo Eq1 = await _equipos.Find<Equipo>(x => x.Id == p.equipos[0].Id).FirstOrDefaultAsync();
                    Equipo Eq2 = await _equipos.Find<Equipo>(x => x.Id == p.equipos[1].Id).FirstOrDefaultAsync();

                    dev.Add(new
                    {
                        idPartido = p.Id,
                        idTorneo=p.IdTorneo,
                        idEquipo1 = Eq1.Id,
                        fotoEq1=Eq1.UrlFoto,
                        idEquipo2 = Eq2.Id,
                        fotoEq2=Eq2.UrlFoto,
                        estadio = p.estadio,
                        categoria = Eq1.Categoria,
                        equipo1 = Eq1.NombreEquipo,
                        equipo2 = Eq2.NombreEquipo,
                        fecha = p.fecha.ToString("dd/MM/yyyy"),
                        hora = p.fecha.ToShortTimeString(),
                        estado = p.estado
                    });
                    //dev.Add(det);
                };


                return dev;
            }
            catch (Exception ex)
            {
                List<Object> dev = new List<Object>();
                dev.Add(new { Error = ex.Message });
                return dev;
            }
        }

        public async Task<List<Object>> FixtureTodosLosEquipos()
        {
            try
            {
                List<Object> dev = new List<Object>();
                List<Partido> parts = await _partidos.Find<Partido>(partido => (partido.estado == (EstadoPartido)0 || partido.estado == (EstadoPartido)4) && partido.fecha>=DateTime.Now).ToListAsync();
                foreach (Partido p in parts)
                {
                    Equipo Eq1 = await _equipos.Find<Equipo>(x => x.Id == p.equipos[0].Id).FirstOrDefaultAsync();
                    Equipo Eq2 = await _equipos.Find<Equipo>(x => x.Id == p.equipos[1].Id).FirstOrDefaultAsync();

                    dev.Add(new
                    {
                        idPartido = p.Id,
                        idEquipo1 = Eq1.Id,
                        nombreEquipo1=Eq1.NombreEquipo,
                        banderaEquipo1= Eq1.UrlFoto,
                        idEquipo2 = Eq2.Id,
                        nombreEquipo2=Eq2.NombreEquipo,
                        banderaEquipo2 = Eq2.UrlFoto,
                        estadio = p.estadio,
                        categoria = Eq1.Categoria,
                        fecha = p.fecha.ToString("dd/MM/yyyy"),
                        hora = p.fecha.ToShortTimeString(),                        
                    });
                    //dev.Add(det);
                };


                return dev;
            }
            catch (Exception ex)
            {
                List<Object> dev = new List<Object>();
                dev.Add(new { Error = ex.Message });
                return dev;
            }
        }

        public async Task<List<Object>> FixturePorEquipo(string idEquipo)
        {
            try
            {
                List<Object> dev = new List<Object>();
                List<Partido> parts = await _partidos.Find<Partido>(partido => (partido.estado == (EstadoPartido)0|| partido.estado == (EstadoPartido)4) && partido.fecha >= DateTime.Now).ToListAsync();
                foreach (Partido p in parts)
                {
                    if(p.equipos[0].Id.Equals(idEquipo)|| p.equipos[1].Id.Equals(idEquipo))
                    { 
                    Equipo Eq1 = await _equipos.Find<Equipo>(x => x.Id == p.equipos[0].Id).FirstOrDefaultAsync();
                    Equipo Eq2 = await _equipos.Find<Equipo>(x => x.Id == p.equipos[1].Id).FirstOrDefaultAsync();

                    dev.Add(new
                    {
                        idPartido = p.Id,
                        idEquipo1 = Eq1.Id,
                        nombreEquipo1 = Eq1.NombreEquipo,
                        banderaEquipo1 = Eq1.UrlFoto,
                        idEquipo2 = Eq2.Id,
                        nombreEquipo2 = Eq2.NombreEquipo,
                        banderaEquipo2 = Eq2.UrlFoto,
                        estadio = p.estadio,
                        categoria = Eq1.Categoria,
                        fecha = p.fecha.ToString("dd/MM/yyyy"),
                        hora = p.fecha.ToShortTimeString(),
                    });
                    //dev.Add(det);
                };
                }

                return dev;
            }
            catch (Exception ex)
            {
                List<Object> dev = new List<Object>();
                dev.Add(new { Error = ex.Message });
                return dev;
            }
        }

        public async Task<Partido> BuscarPartido(string id)
        {
            return await _partidos.Find<Partido>(partido => partido.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Partido> CrearPartido(Partido partido)
        {

            if(partido.fecha<DateTime.Now)
            {
                throw new Exception("No puede ingresar un partido con fecha anterior a la actual.");
            }

            partido.EquipoJugador = new List<EquipoJugador>();
            partido.jueces = new List<Juez>();
            await _partidos.InsertOneAsync(partido);

            VotacionPartido vp = new VotacionPartido();
            vp.Contenido_Votacion = new List<VotacionPartido.ContenidoVotacion>();
            vp.IdPartido = partido.Id;
            List<String> usuarios = new List<string>();
            vp.Usuarios = usuarios;
            VotacionPartido.ContenidoVotacion cv1 = new VotacionPartido.ContenidoVotacion();
            VotacionPartido.ContenidoVotacion cv2 = new VotacionPartido.ContenidoVotacion();
            cv1.idEquipo = partido.equipos[0].Id;
            cv1.votos = 0;
            cv2.idEquipo = partido.equipos[1].Id;
            cv2.votos = 0;

            vp.Contenido_Votacion.Add(cv1);
            vp.Contenido_Votacion.Add(cv2);

            await _votacionPartidoService.votarEquipoPartido(vp);

            return partido;
        }

        public void ActualizarPartido(string id, Partido pa)
        {
            if (pa.fecha < DateTime.Now)
            {
                throw new Exception("No puede ingresar un partido con fecha anterior a la actual.");
            }
            _partidos.ReplaceOne(partido => partido.Id == id, pa);
        }

        public async void ActualizarEstadoPartido(string id, string tiempo)
        {
            try
            {
                var client = new RestSharp.RestClient(urlNotifications);
                var request = new RestRequest(Method.POST);
                JArray a = new JArray();

                Partido p = BuscarPartido(id).Result;
                TablaDePosiciones tp = await _tablaDePosiciones.Find<TablaDePosiciones>(t => t.IdTorneo.Equals(p.IdTorneo)).FirstOrDefaultAsync();
                List<TablaDePosiciones.EquipoTablaPosicion> etp = new List<TablaDePosiciones.EquipoTablaPosicion>();

                //Partido p = await _partidos.Find<Partido>(pa => pa.Id.Equals(id)).FirstOrDefaultAsync();
                if (p.estado ==(EstadoPartido)5)
                {
                    await _partidos.UpdateOneAsync(
                                                        pa => pa.Id.Equals(id),
                                                        Builders<Partido>.Update.
                                                        Set(b => b.estado, (EstadoPartido)0));
                }
                if (p.estado == (EstadoPartido)0 || (p.estado == (EstadoPartido)2 & tiempo == "10:00"))
                {                    

                    if(p.estado == (EstadoPartido)0)
                    {
                        //List<Notificacion> n = new List<Notificacion>();
                        JArray n = new JArray();
                        /*List<ConfiguracionUsuarioMovil> cumEf = new List<ConfiguracionUsuarioMovil>();
                        List<ConfiguracionUsuarioMovil> cum = new List<ConfiguracionUsuarioMovil>();*/

                        List<ConfiguracionUsuarioMovil> cumEf = await _configuracionUsuarioMovil.Find<ConfiguracionUsuarioMovil>(cf => cf.NotificacionEquiposFavoritos == true && (cf.EquiposFavoritos.Contains(p.equipos[0].Id) || cf.EquiposFavoritos.Contains(p.equipos[1].Id))).ToListAsync();
                        List<ConfiguracionUsuarioMovil> cum = await  _configuracionUsuarioMovil.Find<ConfiguracionUsuarioMovil>(cf => cf.NotificacionInicioPartido == true).ToListAsync();

                        foreach (ConfiguracionUsuarioMovil c in cumEf)
                        {
                            dynamic no = new ExpandoObject();
                            no.to = c.Token;
                            no.sound = "default";
                            no.title = "!Tu equipo favorito está jugando!☺☺☺";
                            no.body = "¡Ha dado comienzo al partido entre " + p.equipos[0].NombreEquipo + " y " + p.equipos[1].NombreEquipo + "!";
                            a.Add(JToken.FromObject(no));
                        }
                        foreach (ConfiguracionUsuarioMovil c in cum)
                        {
                            if(!cumEf.Contains(c) && c.NotificacionInicioPartido==true)
                            {
                            dynamic no = new ExpandoObject();
                            no.to = c.Token;
                            no.sound = "default";
                            no.title="Comienzo de partido.";
                            no.body="¡Ha dado comienzo al partido entre "+p.equipos[0].NombreEquipo + " y " + p.equipos[1].NombreEquipo+"!";
                                a.Add(JToken.FromObject(no));
                            }
                        }

                        //var Body = new Object();
                        request.AddParameter("application/json", a, ParameterType.RequestBody);
                        //request.AddJsonBody(a);
                        IRestResponse response = client.Execute(request);
                        var content = response.Content;



                    }

                    await _partidos.UpdateOneAsync(
                                    pa => pa.Id.Equals(id),
                                    Builders<Partido>.Update.
                                    Set(b => b.estado, (EstadoPartido)1));
                }
                if (p.estado == (EstadoPartido)1 & tiempo == "00:00" & p.cuarto != 4)
                {
                    int cuarto = p.cuarto + 1;
                    await _partidos.UpdateOneAsync(
                                                        pa => pa.Id.Equals(id),
                                                        Builders<Partido>.Update.
                                                        Set(b => b.estado, (EstadoPartido)2)
                                                        .Set(b => b.cuarto, cuarto));
                }
                if (p.estado == (EstadoPartido)2 & tiempo != "00:00")
                {
                    await _partidos.UpdateOneAsync(
                                                        pa => pa.Id.Equals(id),
                                                        Builders<Partido>.Update.
                                                        Set(b => b.estado, (EstadoPartido)1));                                                        
                }
                if (p.estado == (EstadoPartido)1 & tiempo == "00:00" & p.cuarto == 4)
                {
                    //List<Notificacion> n = new List<Notificacion>();
                    JArray n = new JArray();
                    //List<ConfiguracionUsuarioMovil> cumEf = new List<ConfiguracionUsuarioMovil>();
                    //List<ConfiguracionUsuarioMovil> cum = new List<ConfiguracionUsuarioMovil>();
                    EstadisticasEquipoPartido est1 =await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(e => e.IdPartido.Equals(p.Id) && e.IdEquipo.Equals(p.equipos[0].Id)).FirstOrDefaultAsync();
                    EstadisticasEquipoPartido est2 =await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(e => e.IdPartido.Equals(p.Id) && e.IdEquipo.Equals(p.equipos[1].Id)).FirstOrDefaultAsync();
                    


                    List<ConfiguracionUsuarioMovil> cumEf = await _configuracionUsuarioMovil.Find<ConfiguracionUsuarioMovil>(cf => cf.NotificacionEquiposFavoritos == true && (cf.EquiposFavoritos.Contains(p.equipos[0].Id) || cf.EquiposFavoritos.Contains(p.equipos[1].Id))).ToListAsync();
                    List<ConfiguracionUsuarioMovil> cum = await _configuracionUsuarioMovil.Find<ConfiguracionUsuarioMovil>(cf => cf.NotificacionFinPartido == true).ToListAsync();

                    foreach (ConfiguracionUsuarioMovil c in cumEf)
                    {
                        dynamic no = new ExpandoObject();
                        no.to = c.Token;
                        no.sound = "default";
                        no.title = "¡Ha finalizado el partido entre " + p.equipos[0].NombreEquipo + " y " + p.equipos[0].NombreEquipo + "!";
                        if(est1.Puntos>est2.Puntos)
                        no.body = "El equipo de "+p.equipos[0].NombreEquipo +" se impuso a "+p.equipos[1].NombreEquipo+" por "+est1.Puntos+" puntos a "+est2.Puntos;
                        else
                            no.body = "El equipo de " + p.equipos[1].NombreEquipo + " se impuso a " + p.equipos[0].NombreEquipo + " por " + est2.Puntos + " puntos a " + est1.Puntos;
                        a.Add(JToken.FromObject(no));
                    }
                    foreach (ConfiguracionUsuarioMovil c in cum)
                    {
                        if (!cumEf.Contains(c) && c.NotificacionFinPartido == true)
                        {
                            dynamic no = new ExpandoObject();
                            no.to = c.Token;
                            no.sound = "default";
                            no.title = "¡Ha finalizado el partido entre " + p.equipos[0].NombreEquipo + " y " + p.equipos[1].NombreEquipo + "!";
                            if (est1.Puntos > est2.Puntos)
                                no.body = "El equipo de " + p.equipos[0].NombreEquipo + " se impuso a " + p.equipos[1].NombreEquipo + " por " + est1.Puntos + " puntos a " + est2.Puntos;
                            else
                                no.body = "El equipo de " + p.equipos[1].NombreEquipo + " se impuso a " + p.equipos[0].NombreEquipo + " por " + est2.Puntos + " puntos a " + est1.Puntos;
                            a.Add(JToken.FromObject(no));
                        }
                    }

                    request.AddParameter("application/json", a, ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                    var content = response.Content;
                    
                    await _partidos.UpdateOneAsync(
                                                        pa => pa.Id.Equals(id),
                                                        Builders<Partido>.Update.
                                                        Set(b => b.estado, (EstadoPartido)3));

                    EstadisticasEquipoPartido est3 = await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(e => e.IdPartido.Equals(p.Id) && e.IdEquipo.Equals(p.equipos[0].Id)).FirstOrDefaultAsync();
                    EstadisticasEquipoPartido est4 = await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(e => e.IdPartido.Equals(p.Id) && e.IdEquipo.Equals(p.equipos[1].Id)).FirstOrDefaultAsync();

                    foreach (Equipo e in p.equipos)
                    {

                        TablaDePosiciones.EquipoTablaPosicion et = tp.EquiposTablaPosicion.Find(aj => aj.idEquipo.Equals(e.Id));
                        if (e.Id.Equals(est3.IdEquipo))
                        {

                            if (est3.Puntos > est4.Puntos)
                            {
                                et.Puntos = et.Puntos + 2;
                                et.PG++;
                            }
                            if (est3.Puntos < est4.Puntos)
                            {
                                et.PP++;
                                et.Puntos = et.Puntos + 1;
                            }
                            et.PF = et.PF + est3.Puntos;
                            et.PC = et.PC + est4.Puntos;
                            et.DIF = et.PF - et.PC;



                        }
                        if (e.Id.Equals(est4.IdEquipo))
                        {

                            if (est4.Puntos > est3.Puntos)
                            {
                                et.Puntos = et.Puntos + 2;
                                et.PG++;
                            }
                            if (est4.Puntos < est3.Puntos)
                            {
                                et.PP++;
                                et.Puntos = et.Puntos + 1;
                            }
                            et.PF = et.PF + est4.Puntos;
                            et.PC = et.PC + est3.Puntos;
                            et.DIF = et.PF - et.PC;
                        }

                        var equipoTablaIndex = await _tablaDePosiciones
                                       .Find(pa => pa.Id == tp.Id)
                                       .Project(pa => pa.EquiposTablaPosicion.FindIndex(t => t.idEquipo.Equals(e.Id)))
                                       .SingleOrDefaultAsync();

                        var UpdateDefinitionBuilder = Builders<TablaDePosiciones>.Update.Set(ta => ta.EquiposTablaPosicion[equipoTablaIndex], et);

                        await _tablaDePosiciones.UpdateOneAsync(pe => pe.Id.Equals(tp.Id), UpdateDefinitionBuilder);

                    }


                    tp.EquiposTablaPosicion.OrderByDescending(b => new { b.Puntos, b.DIF, b.PF, b.PC });
                    List<TablaDePosiciones.EquipoTablaPosicion> equiposParaMod = new List<TablaDePosiciones.EquipoTablaPosicion>();
                    int posicion = 0;
                    foreach (TablaDePosiciones.EquipoTablaPosicion etpp in tp.EquiposTablaPosicion)
                    {
                        if (posicion == 0)
                        {
                            etpp.Posicion = 1;
                            posicion = posicion + 1;
                            equiposParaMod.Add(etpp);
                        }
                        else
                        {
                            posicion = posicion + 1;
                            etpp.Posicion = posicion;                            
                            equiposParaMod.Add(etpp);
                        }


                    }
                    await _tablaDePosiciones.UpdateOneAsync(
                                    tap => tap.Id.Equals(tp.Id),
                                    Builders<TablaDePosiciones>.Update.
                                    Set(b => b.EquiposTablaPosicion, equiposParaMod));
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error al cambiar de estado: "+ex.Message);
            }
        }

        public async void ActualizarTiempoPartido(string id, string tiempo)
        {

            try
            { 

            await _partidos.UpdateOneAsync(
                 p => p.Id.Equals(id),
                 Builders<Partido>.Update.
                 Set(b => b.Tiempo, tiempo));
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async void ActualizarCuartoPartido(string id, int cuarto)
        {



            await _partidos.UpdateOneAsync(
                 p => p.Id.Equals(id),
                 Builders<Partido>.Update.
                 Set(b => b.cuarto, cuarto));

        }

        public async Task<Boolean> AgregarJuezPartida(string id, List<Juez> jueces)
        {
            try
            {
                //_partidos.ReplaceOne(partido => partido.Id == id, pa);
                Partido p = await _partidos.Find<Partido>(pa => pa.Id == id).FirstOrDefaultAsync();

                var filter = Builders<Partido>
                 .Filter.Eq(e => e.Id, id);


                await _partidos.UpdateOneAsync(
                                                      pa => pa.Id.Equals(id),
                                                      Builders<Partido>.Update.
                                                      Set(b => b.jueces, jueces));

                //await _partidos.FindOneAndUpdateAsync(filter, update);


                //Agrega individualmente
                /*foreach (Juez j in jueces)
                {
                    Boolean yaExisteJuez = p.jueces.Any(ju => ju == j);
                    if (yaExisteJuez == false)
                    {
                        var update = Builders<Partido>.Update
                        .Push<Juez>(e => e.jueces, j);
                        await _partidos.FindOneAndUpdateAsync(filter, update);
                    }
                }*/

                await _partidos.UpdateOneAsync(
                                                      pa => pa.Id.Equals(id),
                                                      Builders<Partido>.Update.
                                                      Set(b => b.estado, (EstadoPartido)0));

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Boolean> AgregarJugadoresAPartido(string id, List<EquipoJugador> jugadores)
        {
            try
            {
                Partido partido = await BuscarPartido(id);
                //List<Equipo> equi = new List<Equipo>();
                Equipo equipoDeJugador;
                Jugador jugador;
                var filter = Builders<Partido>
             .Filter.Eq(e => e.Id, id);
                //EquipoJugador existe ;

                List<EquipoJugador> je = new List<EquipoJugador>();

                await _partidos.UpdateOneAsync(
                                                      pa => pa.Id.Equals(id),
                                                      Builders<Partido>.Update.
                                                      Set(b => b.EquipoJugador, je));


                /*await _partidos.UpdateOneAsync(
                                                      pa => pa.Id.Equals(id),
                                                      Builders<Partido>.Update.
                                                      Set(b => b.EquipoJugador[1], null));*/

                foreach (EquipoJugador j in jugadores)
                {
                    //jugador = await _jugadores.Find<Jugador>(ju => ju.Id == j.jugadorEquipo).FirstOrDefaultAsync();
                    equipoDeJugador = await _equipos.Find<Equipo>(e => e.Id == j.idEquipo).FirstOrDefaultAsync();
                    //for(int x;x < partido.EquipoJugador.Length )
                    //existe= await _partidos.Find(e => e.EquipoJugador.);

                    var equipoJugadorIndex = await _partidos
                  .Find(p => p.Id == id)
                  .Project(p => p.EquipoJugador.FindIndex(t => t.idEquipo == equipoDeJugador.Id))
                  .SingleOrDefaultAsync();

                   // if (equipoJugadorIndex == -1)
                   // {
                        var update = Builders<Partido>.Update
                            .Push<EquipoJugador>(e => e.EquipoJugador, j);
                        await _partidos.FindOneAndUpdateAsync(filter, update);

                    /*}
                    else
                    {
                        foreach (EquipoJugador.JugadorEquipo je in j.jugadorEquipo)
                        {
                            Boolean yaExisteJugador = j.jugadorEquipo.Any(p => p.idJugador == je.idJugador);
                            if (yaExisteJugador == false)
                            {
                                var update = Builders<Partido>.Update
                                .Push<EquipoJugador.JugadorEquipo>(e => e.EquipoJugador[equipoJugadorIndex].jugadorEquipo, je);
                                await _partidos.FindOneAndUpdateAsync(filter, update);
                            }
                        }*/
                    //}

                    /*  await _partidos.UpdateOneAsync(
                       a => a.Id.Equals(id),// Filtros para encontrar al jugador y partido correcto
                      Builders<Partido>.Update
                      .Push<EquipoJugador>(b => b.EquipoJugador,j));*/
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void EliminarPartido(Partido pa)
        {
            _partidos.DeleteOne(partido => partido.Id == pa.Id);
        }

        public void EliminarPartido(string id)
        {
            _partidos.DeleteOne(partido => partido.Id == id);
        }

        public async Task<List<Object>> DevuelvoListPartidosAndroid()
        {

            List<Partido> part = await _partidos.Find<Partido>(x => true).ToListAsync();
            List<EstadisticasEquipoPartido> estEqPar = await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(x => true).ToListAsync();
            List<Equipo> equi = await _equipos.Find<Equipo>(x => true).ToListAsync();
            List<Object> devv = new List<Object>();

            foreach (Partido p in part)
            {

               BitacoraPartido bp = await _bitacoraPartido.Find<BitacoraPartido>(bpp => bpp.idPartido.Equals(p.Id)).FirstOrDefaultAsync();

                string tiempo = "10:00";
                if (bp != null)
                {
                    //tiempo = bp.bitacoraTimeLine.Last().Tiempo;
                    List<BitacoraPartido.BitacoraTimeLine> btl = bp.bitacoraTimeLine.OrderBy(b => b.Cuarto).ToList();
                    tiempo = btl.Last().Tiempo;
                }

                EstadisticasEquipoPartido estEqPar1 = await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(x => x.IdPartido == p.Id && x.IdEquipo == p.equipos[0].Id).FirstOrDefaultAsync();
                EstadisticasEquipoPartido estEqPar2 = await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(x => x.IdPartido == p.Id && x.IdEquipo == p.equipos[1].Id).FirstOrDefaultAsync();
                int puntosEq1 = 0, puntosEq2 = 0;
                if (estEqPar1 != null)
                    puntosEq1 = estEqPar1.Puntos;
                if (estEqPar2 != null)
                    puntosEq2 = estEqPar2.Puntos;


                devv.Add(new
                {
                    idPartido = p.Id,
                    idEquipo1 = p.equipos[0].Id,
                    idEquipo2 = p.equipos[1].Id,
                    equipo1 = p.equipos[0].NombreEquipo,
                    equipo2 = p.equipos[1].NombreEquipo,
                    estadio = p.estadio,
                    puntosEq1 = puntosEq1,
                    puntosEq2 = puntosEq2,
                    fecha = p.fecha.ToString("dd/MM/yyyy"),
                    hora = p.fecha.ToShortTimeString(),
                    estado = ((EstadoPartido)p.estado).ToString(),
                    cuarto = p.cuarto,
                    tiempo = tiempo,
                });
            }

            return devv;

        }

        public async Task<Object> ConsultarHeaderPartido(string idPartido)
        {
            try
            {
                Partido p = await _partidos.Find<Partido>(x => x.Id == idPartido).FirstOrDefaultAsync();
                //  List<EstadisticasEquipoPartido> estEqPar = await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(x => true).ToListAsync();
                // List<Equipo> equi = await _equipos.Find<Equipo>(x => true).ToListAsync();
                Object devv = new Object();

               BitacoraPartido bp = await _bitacoraPartido.Find<BitacoraPartido>(bpp => bpp.idPartido.Equals(idPartido)).FirstOrDefaultAsync();
                string tiempo = "10:00";
                if (bp != null)
                {
                    //tiempo = bp.bitacoraTimeLine.Last().Tiempo;
                    List<BitacoraPartido.BitacoraTimeLine> btl = bp.bitacoraTimeLine.OrderBy(b => b.Cuarto).ToList();
                    tiempo = btl.Last().Tiempo;
                }

                /*foreach(Partido p in part)
                {*/
                EstadisticasEquipoPartido estEqPar1 = await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(x => x.IdPartido == p.Id && x.IdEquipo == p.equipos[0].Id).FirstOrDefaultAsync();
                EstadisticasEquipoPartido estEqPar2 = await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(x => x.IdPartido == p.Id && x.IdEquipo == p.equipos[1].Id).FirstOrDefaultAsync();
                int puntosEq1 = 0, puntosEq2 = 0;
                if (estEqPar1 != null)
                    puntosEq1 = estEqPar1.Puntos;
                if (estEqPar2 != null)
                    puntosEq2 = estEqPar2.Puntos;

                devv = (new
                {
                    
                    idPartido = p.Id,
                    idEquipo1 = p.equipos[0].Id,
                    idEquipo2 = p.equipos[1].Id,
                    /*fotoEquipo1 = HelperCloudinary.cloudUrl + "Equipos/" + p.equipos[0].Id,
                    fotoEquipo2 = HelperCloudinary.cloudUrl + "Equipos/" + p.equipos[1].Id,*/
                    fotoEquipo1 = p.equipos[0].UrlFoto,
                    fotoEquipo2 = p.equipos[1].UrlFoto,
                    equipo1 = p.equipos[0].NombreEquipo,
                    equipo2 = p.equipos[1].NombreEquipo,
                    ptosequipo1 = puntosEq1,
                    ptosequipo2 = puntosEq2,
                    cuartoenjuego = p.cuarto,
                    tiempoDeJuego = tiempo,
                    Estadio = p.estadio,
                    fecha = p.fecha.ToString("dd/MM/yyyy"),
                    statuspartido = ((EstadoPartido)p.estado).ToString()
                });

                return devv;

            }
            catch
            {
                return new { ERROR = "No se encuentran estadísticas para el partido." };
            }

        }

        public async Task<Object> ConsultaDetallesPartido(string idPartido)
        {
            try
            {

                Partido part = await _partidos.Find<Partido>(x => x.Id == idPartido).FirstOrDefaultAsync();
                EstadisticasEquipoPartido estEqPar1 = await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(x => x.IdPartido == idPartido && x.IdEquipo == part.equipos[0].Id).FirstOrDefaultAsync();
                EstadisticasEquipoPartido estEqPar2 = await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(x => x.IdPartido == idPartido && x.IdEquipo == part.equipos[1].Id).FirstOrDefaultAsync();

                int PuntosPrimerCuartoEq1, PuntosSegundoCuartoEq1, PuntosTercerCuartoEq1, PuntosCuartoCuartoEq1, PuntosOverTimeEq1 = 0;
                int PuntosPrimerCuartoEq2, PuntosSegundoCuartoEq2, PuntosTercerCuartoEq2, PuntosCuartoCuartoEq2, PuntosOverTimeEq2;

                if (estEqPar1 == null)
                {
                    PuntosPrimerCuartoEq1 = 0;
                    PuntosSegundoCuartoEq1 = 0;
                    PuntosTercerCuartoEq1 = 0;
                    PuntosCuartoCuartoEq1 = 0;
                    PuntosOverTimeEq1 = 0;
                }
                else
                {
                    PuntosPrimerCuartoEq1 = estEqPar1.PuntosPrimerCuarto;
                    PuntosSegundoCuartoEq1 = estEqPar1.PuntosSegundoCuarto;
                    PuntosTercerCuartoEq1 = estEqPar1.PuntosTercerCuarto;
                    PuntosCuartoCuartoEq1 = estEqPar1.PuntosCuartoCuarto;
                    PuntosOverTimeEq1 = estEqPar1.PuntosOverTime;
                }
                if (estEqPar2 == null)
                {
                    PuntosPrimerCuartoEq2 = 0;
                    PuntosSegundoCuartoEq2 = 0;
                    PuntosTercerCuartoEq2 = 0;
                    PuntosCuartoCuartoEq2 = 0;
                    PuntosOverTimeEq2 = 0;
                }
                else
                {
                    PuntosPrimerCuartoEq2 = estEqPar2.PuntosPrimerCuarto;
                    PuntosSegundoCuartoEq2 = estEqPar2.PuntosSegundoCuarto;
                    PuntosTercerCuartoEq2 = estEqPar2.PuntosTercerCuarto;
                    PuntosCuartoCuartoEq2 = estEqPar2.PuntosCuartoCuarto;
                    PuntosOverTimeEq2 = estEqPar2.PuntosOverTime;
                }


                var det = new
                {
                    idPartido = part.Id,
                    estadio = part.estadio,
                    ptosPrimerCuartoEq1 = PuntosPrimerCuartoEq1,
                    ptosSegundoCuartoEq1 = PuntosSegundoCuartoEq1,
                    ptosTercerCuartoEq1 = PuntosTercerCuartoEq1,
                    ptosCuartoCuartoEq1 = PuntosCuartoCuartoEq1,
                    ptosOverTimeEq1 = PuntosOverTimeEq1,
                    ptosPrimerCuartoEq2 = PuntosPrimerCuartoEq2,
                    ptosSegundoCuartoEq2 = PuntosSegundoCuartoEq2,
                    ptosTercerCuartoEq2 = PuntosTercerCuartoEq2,
                    ptosCuartoCuartoEq2 = PuntosCuartoCuartoEq2,
                    ptosOverTimeEq2 = PuntosOverTimeEq2,
                    arbitro1 = part.jueces[0].Nombre + " " + part.jueces[0].Apellido,
                    arbitro2 = part.jueces[1].Nombre + " " + part.jueces[1].Apellido,
                    arbitro3 = part.jueces[2].Nombre + " " + part.jueces[2].Apellido,
                    statuspartido = ((EstadoPartido)part.estado).ToString()
                };


                return det;
            }
            catch (Exception ex)
            {
                return new { ERROR = ex.Message };
            }
        }

        public async Task<Object> UltimosEventosEquipo(string idPartido)
        {
            try
            {
                List<Partido> part = await _partidos.Find<Partido>(x => x.estado.Equals((EstadoPartido)3) && (x.equipos[0].Id.Equals(idPartido) || x.equipos[1].Id.Equals(idPartido))).ToListAsync();
                List<EstadisticasEquipoPartido> estEq1 = new List<EstadisticasEquipoPartido>();
                List<EstadisticasEquipoPartido> estEq2 = new List<EstadisticasEquipoPartido>();
                foreach(Partido p in part)
                {
                    estEq1.Add(await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(x => x.IdPartido.Equals(p.Id) && x.IdEquipo.Equals(p.equipos[0].Id)).FirstOrDefaultAsync());
                    estEq2.Add(await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(x => x.IdPartido.Equals(p.Id) && x.IdEquipo.Equals(p.equipos[1].Id)).FirstOrDefaultAsync());
                }

                List<EstadisticasEquipoPartido> estEqPar = await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(x => true).ToListAsync();
                List<Equipo> equi = await _equipos.Find<Equipo>(x => true).ToListAsync();

                var partidos = (from p in part
                               // join e in equi on p.equipos[0].Id equals e.Id
                               // join e2 in equi on p.equipos[1].Id equals e2.Id
                                join est1 in estEq1 on p.Id equals est1.IdPartido
                                join est2 in estEq2 on p.Id equals est2.IdPartido
                              //  where (p.equipos[0].Id.Equals(idPartido) || p.equipos[1].Id.Equals(idPartido))
                                orderby p.fecha descending
                                select new
                                {
                                    idPartido = p.Id,
                                    equipo1 = p.equipos[0].NombreEquipo,
                                    equipo2 = p.equipos[1].NombreEquipo,
                                    ptosequipo1 = est1.Puntos,
                                    ptosequipo2 = est2.Puntos,
                                    fecha = p.fecha.ToString("dd/MM/yyyy"),
                                    resultado = (p.equipos[0].Id.Equals(idPartido) && est1.Puntos > est2.Puntos || p.equipos[1].Id.Equals(idPartido) && est2.Puntos > est1.Puntos) ? "WIN" : "LOSE"
                                }
                   ).Take(5);

                List<Object> dev = new List<Object>();

                foreach (var par in partidos)
                {

                    dev.Add(par);
                }

                return dev;
            }
            catch
            {
                return new { ERROR = "No se encuentran estadísticas para el partido." };
            }
        }

        /* public async Task<List<Object>> ListarJugadoresEquiposPartido(string idPartido)
         {
             try
             {

                 Partido part = await _partidos.Find<Partido>(x => x.Id == idPartido).FirstOrDefaultAsync();
                // List<EstadisticasEquipoPartido> estEqPar = await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(x => true).ToListAsync();
                 List<Equipo> equi = await _equipos.Find<Equipo>(x => true).ToListAsync();
                 List<Object> listaJugadores = new List<Object>();
                 List<Object> devv = new List<Object>();
                 List<Jugador> jugEqPart;
                 List<EquipoJugador.JugadorEquipo> jugEq = new List<EquipoJugador.JugadorEquipo>();

                 dynamic jugadoresEquipoPartido = new System.Dynamic.ExpandoObject();
                 jugadoresEquipoPartido.nombreEq1 = part.equipos[0].NombreEquipo;
                 jugadoresEquipoPartido.nombreEq2 = part.equipos[1].NombreEquipo;
                 jugadoresEquipoPartido.cantJugadoresConvocadosEq1 = part.EquipoJugador[0].jugadorEquipo.Count;
                 jugadoresEquipoPartido.cantJugadoresConvocadosEq2 = part.EquipoJugador[1].jugadorEquipo.Count;


                 foreach (Equipo e in part.equipos)
                 {
                     var equipoJugadorIndex = await _partidos
                        .Find(p => p.Id == idPartido)
                        .Project(p => p.EquipoJugador.FindIndex(t => t.idEquipo == e.Id))
                        .SingleOrDefaultAsync();
                     jugEq = part.EquipoJugador[equipoJugadorIndex].jugadorEquipo;

                     jugEqPart = new List<Jugador>(); ;
                     listaJugadores = new List<Object>();
                     int cantJugadoresConvocados;
                     cantJugadoresConvocados= part.EquipoJugador[equipoJugadorIndex].jugadorEquipo.Count;
                     foreach (EquipoJugador.JugadorEquipo je in jugEq)
                     { 
                   //  jugEqPart = await _jugadores.Find<Jugador>(x => x.IdEquipo == e.Id).ToListAsync();
                     Jugador jug = await _jugadores.Find<Jugador>(x => x.Id == je.idJugador).SingleOrDefaultAsync();

                   //      foreach (Jugador j in jugEqPart)
                    // {

                         listaJugadores.Add(new
                         {
                             idJugador = jug.Id,
                             nombre = jug.Nombre,
                             apellido = jug.Apellido,
                             posicion = jug.Posicion,
                             esTitular=je.esTitular
                         });
                     }
                   //  }
                     devv.Add(new
                     {
                         equipo = e.Id,
                         nombreEquipo=e.NombreEquipo,
                         cantJugadoresConvocados= cantJugadoresConvocados,
                         jugadores = listaJugadores
                     });

                 }

                 return devv;
             }
             catch (Exception ex)
             {
                 throw new Exception(ex.Message);
             }
         }*/
        public async Task<List<Object>> ListarJugadoresEquiposPartido(string idPartido)
        {
            try
            {

                Partido part = await _partidos.Find<Partido>(x => x.Id == idPartido).FirstOrDefaultAsync();
                List<EstadisticasEquipoPartido> estEqPar = await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(x => true).ToListAsync();
                List<Equipo> equi = await _equipos.Find<Equipo>(x => true).ToListAsync();
                List<Object> listaJugadores = new List<Object>();
                List<Object> devv = new List<Object>();
                List<Jugador> jugEqPart;

                foreach (Equipo e in part.equipos)
                {
                    jugEqPart = new List<Jugador>(); ;
                    listaJugadores = new List<Object>();
                    jugEqPart = await _jugadores.Find<Jugador>(x => x.IdEquipo == e.Id).ToListAsync();
                    foreach (Jugador j in jugEqPart)
                    {

                        listaJugadores.Add(new
                        {
                            idJugadir = j.Id,
                            nombre = j.Nombre,
                            apellido = j.Apellido,
                            posicion = j.Posicion,
                            foto=j.UrlFoto
                        });
                    }
                    devv.Add(new
                    {
                        equipo = e.Id,
                        fotoEquipo=e.UrlFoto,
                        nombreEquipo=e.NombreEquipo,
                        jugadores = listaJugadores
                    });

                }

                return devv;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Object>> ListarEstadios()
        {
            try
            {
                List<Object> estadios = new List<Object>();
                List<Equipo> equi = await _equipos.Find<Equipo>(x => true).ToListAsync();
                foreach (Equipo e in equi)
                {
                    if(!estadios.Contains(e) && e.Estadio!=null)
                    estadios.Add(e.Estadio);
                }
                return estadios;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Object>> EstadisticasJugsEquipoPartido(string idPartido,string idEquipo)
        {
            try
            {

                Partido part = await _partidos.Find<Partido>(x => x.Id == idPartido).FirstOrDefaultAsync();
                List<Jugador> jugadoresEquipo= await _jugadores.Find<Jugador>(x => x.IdEquipo == idEquipo).ToListAsync();

                List<dynamic> listReturn = new List<dynamic>();

                foreach(Jugador j in jugadoresEquipo)
                {
                    Equipo equipoDeJugador;
                    Jugador jugador;

                    bool esTitular;
                    jugador = await _jugadores.Find<Jugador>(ju => ju.Id == j.Id).FirstOrDefaultAsync();
                    equipoDeJugador = await _equipos.Find<Equipo>(e => e.Id == jugador.IdEquipo).FirstOrDefaultAsync();

                    var equipoJugadorIndex = await _partidos
                     .Find(p => p.Id == part.Id)
                     .Project(p => p.EquipoJugador.FindIndex(t => t.idEquipo == equipoDeJugador.Id))
                     .SingleOrDefaultAsync();


                    var jugadorEquipoIndex = await _partidos
                     .Find(p => p.Id == part.Id)
                     .Project(p => p.EquipoJugador[equipoJugadorIndex].jugadorEquipo.FindIndex(t => t.idJugador == j.Id))
                     .SingleOrDefaultAsync();


                    EquipoJugador.JugadorEquipo je = part.EquipoJugador[equipoJugadorIndex].jugadorEquipo[jugadorEquipoIndex];
                    //int indexJugador = part.EquipoJugador[equipoJugadorIndex].jugadorEquipo.IndexOf(je);

                    EstadisticasJugadorPartido ejp = await _estadisticasJugadorPartido.Find<EstadisticasJugadorPartido>(p => p.IdPartido.Equals(idPartido) && p.IdJugador.Equals(j.Id)).FirstOrDefaultAsync();
                    if(ejp!=null)
                    {
                        var det = new
                        {
                            idJugador = j.Id,
                            nombre = j.Nombre,
                            apellido = j.Apellido,
                            FotoJugador = j.UrlFoto,
                            numeroCamiseta = j.NumeroCamiseta,
                            puntos = ejp.Puntos,
                            tresPuntosConvertidos = ejp.TresPuntosConvertidos,
                            tresPuntosIntentados = ejp.TresPuntosIntentados,
                            porcentaje3Puntos = ejp.TresPuntosPorcentaje,
                            dosPuntosConvertidos = ejp.DosPuntosConvertidos,
                            dosPuntosIntentados = ejp.DosPuntosIntentados,
                            dosPuntosPorcentaje = ejp.DosPuntosPorcentaje,
                            libresConvertidos = ejp.TirosLibresConvertidos,
                            libresIntentados = ejp.TirosLibresIntentados,
                            porcentajeLibres = ejp.TirosLibresPorcentaje,
                            rebotesOfensivos = ejp.RebotesOfensivos,
                            rebotesDefensivos = ejp.RebotesDefensivos,
                            rebotesTotales = ejp.RebotesTotales,
                            asistencias = ejp.Asistencias,
                            recuperos = ejp.Recuperos,
                            faltasPersonales = ejp.FaltasPersonales,
                            faltasAntideportivas = ejp.FaltasAntideportivas,
                            faltasTecnicas = ejp.FaltasTecnicas,
                            jugando = je.esTitular
                    };
                        listReturn.Add(det);
                    }
                    else
                    {
                        var det = new
                        {
                            idJugador = j.Id,
                            nombre = j.Nombre,
                            apellido = j.Apellido,
                            FotoJugador = j.UrlFoto,
                            numeroCamiseta = j.NumeroCamiseta,
                            puntos = 0,
                            tresPuntosConvertidos = 0,
                            tresPuntosIntentados = 0,
                            porcentaje3Puntos = 0,
                            dosPuntosConvertidos = 0,
                            dosPuntosIntentados = 0,
                            dosPuntosPorcentaje = 0,
                            libresConvertidos = 0,
                            libresIntentados = 0,
                            porcentajeLibres = 0,
                            rebotesOfensivos = 0,
                            rebotesDefensivos = 0,
                            rebotesTotales = 0,
                            asistencias = 0,
                            recuperos = 0,
                            faltasPersonales = 0,
                            faltastAntideportivas=0,
                            faltasTecnicas=0
                        };
                        listReturn.Add(det);
                    }

                    
                }



                return listReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Object> DevolverEstadoPartido(string idPartido)
        {
            try { 
            Partido p= await _partidos.Find<Partido>(partido => partido.Id == idPartido).FirstOrDefaultAsync();
            if (p != null)
                return new { estado = p.estado};
            else
                return new { error = "No existe el partido con el ID ingresado." };
            }
            catch(Exception ex)
            {
                return new { error = "Se ha producido un error: "+ex.Message };
            }
        }

        public async Task<List<Partido>> ListarPartidosSinJueces()
        {
            List<Partido> parts = await _partidos.Find<Partido>(partido => partido.estado == (EstadoPartido)4).ToListAsync();
            return parts;
        }



        /*public async Task<Object> ConsultaDetallesPartido(string idPartido)
        {
            try
            {

            Partido part = await _partidos.Find<Partido>(x => x.Id == idPartido).FirstOrDefaultAsync();
            EstadisticasEquipoPartido estEqPar1 = await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(x => x.IdPartido == idPartido && x.IdEquipo == part.equipos[0].Id).FirstOrDefaultAsync();
            EstadisticasEquipoPartido estEqPar2 = await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(x => x.IdPartido == idPartido && x.IdEquipo == part.equipos[1].Id).FirstOrDefaultAsync();

            var det=new { idPartido=part.Id,
                           estadio=part.estadio,
                           ptosPrimerCuartoEq1=estEqPar1.PuntosPrimerCuarto,
                           ptosSegundoCuartoEq1=estEqPar1.PuntosSegundoCuarto,
                           ptosTercerCuartoEq1=estEqPar1.PuntosTercerCuarto,
                           ptosCuartoCuartoEq1=estEqPar1.PuntosCuartoCuarto,
                           ptosOverTimeEq1= estEqPar1.PuntosOverTime,
                           ptosPrimerCuartoEq2=estEqPar2.PuntosPrimerCuarto,
                           ptosSegundoCuartoEq2=estEqPar2.PuntosSegundoCuarto,
                           ptosTercerCuartoEq2=estEqPar2.PuntosTercerCuarto,
                           ptosCuartoCuartoEq2=estEqPar2.PuntosCuartoCuarto,
                           ptosOverTimeEq2= estEqPar2.PuntosOverTime,                   
                           arbitro1=part.jueces[0].Nombre+" "+part.jueces[0].Apellido,
                           arbitro2=part.jueces[1].Nombre+" "+part.jueces[1].Apellido,
                           arbitro3=part.jueces[2].Nombre+" "+part.jueces[2].Apellido,
                           statuspartido=((EstadoPartido)part.estado).ToString()   };


            return det;
            }
            catch(Exception ex)
        {
            return new {ERROR=ex.Message};
        }
        } */

    }
}
