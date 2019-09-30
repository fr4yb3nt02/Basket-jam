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
        Task<List<Object>> ListarEquipoJugador(string idPartido);
        Task<Boolean> AgregarJugadoresAPartido(string id, List<EquipoJugador> jugadores);
        Task<List<Object>> ListarPartidosProgOJug();
        Task<List<Object>> ListarPartidosPorEstado(int estado);
        Task<List<Object>> ListarJugadoresEquiposPartido(string idPartido);
        Task<List<Object>> ListarEstadios();
        Task<List<Object>> FixtureTodosLosEquipos();
        Task<List<Object>> FixturePorEquipo(string idEquipo);
        //Task<List<String>> DevuelvoListPartidosAndroid();
    }

    public class PartidoService : IPartidoService
    {
        private readonly IMongoCollection<Partido> _partidos;
        private readonly IMongoCollection<Equipo> _equipos;
        private readonly IMongoCollection<CuerpoTecnico> _cuerpoTecnico;
        private readonly IMongoCollection<Jugador> _jugadores;
        private readonly IMongoCollection<EstadisticasEquipoPartido> _estadisticasEquipoPartido;
        private IVotacionPartidoService _votacionPartidoService;
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
            //  _estadisticasEquipoPartidoService = estadisticasEquipoPartidoService;



        }

        public async Task<List<Partido>> ListarPartidos()
        {
            return await _partidos.Find(partido => true).ToListAsync();
        }

        public async Task<List<Object>> ListarEquipoJugador(string idPartido)
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
                                    FotoJugador = "https://res.cloudinary.com/dregj5syg/image/upload/v1567135827/Jugadores/" + j.Id
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
                        FotoEquipo = "https://res.cloudinary.com/dregj5syg/image/upload/v1567135827/Equipos/" + e.Id,
                        PuntosEnPartido = puntosEquipoPartido,
                        Entrenador = tecnico.Nombre + " " + tecnico.Apellido,
                        IdEntrenador = tecnico.Id,
                        jugadores
                    });
                }
            }
            return jugEq;

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
                List<Partido> parts = await _partidos.Find<Partido>(partido => partido.estado != (EstadoPartido)3).ToListAsync();
                foreach (Partido p in parts)
                {
                    Equipo Eq1 = await _equipos.Find<Equipo>(x => x.Id == p.equipos[0].Id).FirstOrDefaultAsync();
                    Equipo Eq2 = await _equipos.Find<Equipo>(x => x.Id == p.equipos[1].Id).FirstOrDefaultAsync();

                    dev.Add(new
                    {
                        idPartido = p.Id,
                        idEquipo1 = Eq1.Id,
                        idEquipo2 = Eq2.Id,
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
                List<Partido> parts = await _partidos.Find<Partido>(partido => partido.estado == (EstadoPartido)estado).ToListAsync();
                foreach (Partido p in parts)
                {
                    Equipo Eq1 = await _equipos.Find<Equipo>(x => x.Id == p.equipos[0].Id).FirstOrDefaultAsync();
                    Equipo Eq2 = await _equipos.Find<Equipo>(x => x.Id == p.equipos[1].Id).FirstOrDefaultAsync();

                    dev.Add(new
                    {
                        idPartido = p.Id,
                        idEquipo1 = Eq1.Id,
                        idEquipo2 = Eq2.Id,
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
                List<Partido> parts = await _partidos.Find<Partido>(partido => partido.estado == (EstadoPartido)0  && partido.fecha>=DateTime.Now).ToListAsync();
                foreach (Partido p in parts)
                {
                    Equipo Eq1 = await _equipos.Find<Equipo>(x => x.Id == p.equipos[0].Id).FirstOrDefaultAsync();
                    Equipo Eq2 = await _equipos.Find<Equipo>(x => x.Id == p.equipos[1].Id).FirstOrDefaultAsync();

                    dev.Add(new
                    {
                        idPartido = p.Id,
                        idEquipo1 = Eq1.Id,
                        nombreEquipo1=Eq1.NombreEquipo,
                        banderaEquipo1= HelperCloudinary.cloudUrl + "Equipos/" + Eq1.Id,
                        idEquipo2 = Eq2.Id,
                        nombreEquipo2=Eq2.NombreEquipo,
                        banderaEquipo2 = HelperCloudinary.cloudUrl + "Equipos/" + Eq2.Id,
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
                List<Partido> parts = await _partidos.Find<Partido>(partido => partido.estado == (EstadoPartido)0 && partido.fecha >= DateTime.Now).ToListAsync();
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
                        banderaEquipo1 = HelperCloudinary.cloudUrl + "Equipos/" + Eq1.Id,
                        idEquipo2 = Eq2.Id,
                        nombreEquipo2 = Eq2.NombreEquipo,
                        banderaEquipo2 = HelperCloudinary.cloudUrl + "Equipos/" + Eq2.Id,
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
            _partidos.ReplaceOne(partido => partido.Id == id, pa);
        }

        public async void ActualizarEstadoPartido(string id, string tiempo)
        {
            try
            {
                Partido p = await _partidos.Find<Partido>(pa => pa.Id == id).FirstOrDefaultAsync();
                if (p.estado == (EstadoPartido)0 || (p.estado == (EstadoPartido)2 & tiempo == "10:00"))
                {
                    await _partidos.UpdateOneAsync(
                                    pa => pa.Id.Equals(id),
                                    Builders<Partido>.Update.
                                    Set(b => b.estado, (EstadoPartido)1));
                }
                if (p.estado == (EstadoPartido)1 & tiempo == "00:00" & p.cuarto != 4)
                {
                    await _partidos.UpdateOneAsync(
                                                        pa => pa.Id.Equals(id),
                                                        Builders<Partido>.Update.
                                                        Set(b => b.estado, (EstadoPartido)2));
                }
                if (p.estado == (EstadoPartido)1 & tiempo == "00:00" & p.cuarto == 4)
                {
                    await _partidos.UpdateOneAsync(
                                                        pa => pa.Id.Equals(id),
                                                        Builders<Partido>.Update.
                                                        Set(b => b.estado, (EstadoPartido)3));
                }
            }
            catch
            {
                throw new Exception("Error al cambiar de estado.");
            }
        }

        public async void ActualizarTiempoPartido(string id, string tiempo)
        {

            await _partidos.UpdateOneAsync(
                a => a.Id.Equals(id),// Filtros para encontrar al jugador y partido correcto
               Builders<Partido>.Update
               .Set(b => b.Tiempo, tiempo));

        }

        public async Task<Boolean> AgregarJuezPartida(string id, List<Juez> jueces)
        {
            try
            {
                //_partidos.ReplaceOne(partido => partido.Id == id, pa);
                Partido p = await _partidos.Find<Partido>(pa => pa.Id == id).FirstOrDefaultAsync();

                var filter = Builders<Partido>
                 .Filter.Eq(e => e.Id, id);

                foreach (Juez j in jueces)
                {
                    Boolean yaExisteJuez = p.jueces.Any(ju => ju == j);
                    if (yaExisteJuez == false)
                    {
                        var update = Builders<Partido>.Update
                        .Push<Juez>(e => e.jueces, j);
                        await _partidos.FindOneAndUpdateAsync(filter, update);
                    }
                }

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

                    if (equipoJugadorIndex == -1)
                    {
                        var update = Builders<Partido>.Update
                            .Push<EquipoJugador>(e => e.EquipoJugador, j);
                        await _partidos.FindOneAndUpdateAsync(filter, update);

                    }
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
                        }
                    }

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
                    tiempo = p.Tiempo,
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
                    fotoEquipo1 = HelperCloudinary.cloudUrl + "Equipos/" + p.equipos[0].Id,
                    fotoEquipo2 = HelperCloudinary.cloudUrl + "Equipos/" + p.equipos[1].Id,
                    equipo1 = p.equipos[0].NombreEquipo,
                    equipo2 = p.equipos[1].NombreEquipo,
                    ptosequipo1 = puntosEq1,
                    ptosequipo2 = puntosEq2,
                    cuartoenjuego = p.cuarto,
                    tiempoDeJuego = p.Tiempo,
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
                List<Partido> part = await _partidos.Find<Partido>(x => true).ToListAsync();
                List<EstadisticasEquipoPartido> estEqPar = await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(x => true).ToListAsync();
                List<Equipo> equi = await _equipos.Find<Equipo>(x => true).ToListAsync();

                var partidos = (from p in part
                                join e in equi on p.equipos[0].Id equals e.Id
                                join e2 in equi on p.equipos[1].Id equals e2.Id
                                join est1 in estEqPar on e.Id equals est1.IdEquipo
                                join est2 in estEqPar on e2.Id equals est2.IdEquipo
                                where p.estado.Equals((EstadoPartido)3) && (p.equipos[0].Id.Equals(idPartido) || p.equipos[1].Id.Equals(idPartido))
                                orderby p.fecha descending
                                select new
                                {
                                    idPartido = p.Id,
                                    equipo1 = e.NombreEquipo,
                                    equipo2 = e2.NombreEquipo,
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
                            posicion = j.Posicion
                        });
                    }
                    devv.Add(new
                    {
                        equipo = e.Id,
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
                List<Object> estadios = new List<object>();
                List<Equipo> equi = await _equipos.Find<Equipo>(x => true).ToListAsync();
                foreach (Equipo e in equi)
                {
                    estadios.Add(e.Estadio);
                }
                return estadios;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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