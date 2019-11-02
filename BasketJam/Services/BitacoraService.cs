using BasketJam.Helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi.Helpers;
using BasketJam.Services;
using BasketJam.Models;
using System.Dynamic;

namespace BasketJam.Services
{
    public interface IBitacoraService
    {
        BitacoraPartido BuscarBitacoraPartido(string idPartido);
        Task<Object> GenerarBitacora(BitacoraPartido bp);

        Task<Object> consultarEstadisticasPeriodo(string idPartido, int periodo);
        Task<List<Object>> mejoresDiezCadaRubro(int rubro, string idTorneo);
    }

    public class BitacoraService : IBitacoraService
    {
        private readonly IMongoCollection<BitacoraPartido> _bitacoraPartido;

        //private readonly IMongoCollection<EstadisticasJugadorPartido> _estadisticasJugadorPartido;

        private readonly IEstadisticasJugadorPartidoService _estadisticasJugadorPartido;
        private readonly IPartidoService _partidoService;

        private readonly IMongoCollection<Partido> _partido;

        private readonly IMongoCollection<Equipo> _equipo;

        private readonly IMongoCollection<Jugador> _jugadores;

        private readonly IMongoCollection<EstadisticasJugadorPartido> _statsJugadorPartidos;


        public BitacoraService(IConfiguration config, IEstadisticasJugadorPartidoService estadisticasJugadorPartido, IPartidoService partidoService)
        {
            var client = new MongoClient(config.GetConnectionString("BasketJam"));
            var database = client.GetDatabase("BasketJam");
            _bitacoraPartido = database.GetCollection<BitacoraPartido>("BitacorasPartidos");
            //_estadisticasJugadorPartido=database.GetCollection<EstadisticasJugadorPartido>("EstadisticasJugadorPartido");
            _partido = database.GetCollection<Partido>("partidos");
            _equipo = database.GetCollection<Equipo>("equipos");
            _jugadores = database.GetCollection<Jugador>("jugadores");
            _statsJugadorPartidos = database.GetCollection<EstadisticasJugadorPartido>("EstadisticasJugadorPartido");
            _estadisticasJugadorPartido = estadisticasJugadorPartido;
            _partidoService = partidoService;

        }

        public BitacoraPartido BuscarBitacoraPartido(string idPartido)
        {
            return _bitacoraPartido.Find<BitacoraPartido>(bp => bp.idPartido == idPartido).FirstOrDefault();
        }

        public async Task<Object> GenerarBitacora(BitacoraPartido bp)
        {
            try
            {
                BitacoraPartido bitacoraPartido = BuscarBitacoraPartido(bp.idPartido);
                Partido par = await _partido.Find<Partido>(a => a.Id == bp.idPartido).FirstOrDefaultAsync();
                EstadisticasJugadorPartido ejb = new EstadisticasJugadorPartido();
                List<Coordenada> coordenadas = new List<Coordenada>();
                EstadisticasJugadorPartido ejb2;

                if (bitacoraPartido == null)
                {

                    await _bitacoraPartido.InsertOneAsync(bp);
                    // await _estadisticasJugadorPartido.CargarEstadistica(bp);
                    //await _estadisticasEquipoPartidoService.CargarEstadistica(ejp.IdPartido,equipo.IdEquipo,ptos);
                    //return(cargoStatDesdeBitacora(bp));
                    //return true;
                    foreach (BitacoraPartido.BitacoraTimeLine b in bp.bitacoraTimeLine)
                    {
                        ejb = new EstadisticasJugadorPartido();
                        ejb2 = _estadisticasJugadorPartido.BuscarEstadisticasJugadorPartido(bp.idPartido, b.idJugador);
                        if (ejb2 != null)
                            ejb.Id = ejb2.Id;
                        ejb.IdJugador = b.idJugador;
                        ejb.IdPartido = bp.idPartido;

                        if (b.Accion == (TipoAccion)0)
                        {
                            ejb.TirosLibresIntentados = 1;
                        }
                        if (b.Accion == (TipoAccion)1)
                        {
                            ejb.Puntos = 1;
                            ejb.TirosLibresConvertidos = 1;
                            ejb.TirosLibresIntentados = 1;
                        }
                        if (b.Accion == (TipoAccion)2)
                        {
                            ejb.DosPuntosIntentados = 1;
                        }
                        if (b.Accion == (TipoAccion)3)
                        {
                            ejb.Puntos = 2;
                            ejb.DosPuntosConvertidos = 1;
                            ejb.DosPuntosIntentados = 1;
                        }
                        if (b.Accion == (TipoAccion)4)
                        {
                            ejb.TresPuntosIntentados = 1;
                        }
                        if (b.Accion == (TipoAccion)5)
                        {
                            ejb.Puntos = 3;
                            ejb.TresPuntosConvertidos = 1;
                            ejb.TresPuntosIntentados = 1;
                        }
                        if (b.Accion == (TipoAccion)6)
                            ejb.RebotesOfensivos = 1;
                        if (b.Accion == (TipoAccion)7)
                            ejb.RebotesDefensivos = 1;
                        if (b.Accion == (TipoAccion)8)
                            ejb.Bloqueos = 1;
                        if (b.Accion == (TipoAccion)9)
                            ejb.Asistencias = 1;
                        if (b.Accion == (TipoAccion)10)
                            ejb.Perdidas = 1;
                        if (b.Accion == (TipoAccion)11)
                            ejb.Recuperos = 1;
                        if (b.Accion == (TipoAccion)12)
                            ejb.FaltasPersonales = 1;
                        if (b.Accion == (TipoAccion)13)
                            ejb.FaltasAntideportivas = 1;
                        if (b.Accion == (TipoAccion)14)
                            ejb.FaltasCometidas = 1;
                        if (b.Accion == (TipoAccion)18)
                            ejb.FaltasTecnicas = 1;

                        if (b.CoordenadasAcciones != null)
                        {
                            coordenadas.Add(b.CoordenadasAcciones);
                            ejb.CoordenadasAcciones = coordenadas;
                        }



                        await _estadisticasJugadorPartido.CargarEstadistica(ejb);
                        //return true;
                    }
                    return true;

                }
                else
                {

                    var filter = Builders<BitacoraPartido>
                    .Filter.Eq(e => e.idPartido, bp.idPartido);

                    Equipo equipoDeJugador;
                    Jugador jugador;

                    foreach (BitacoraPartido.BitacoraTimeLine b in bp.bitacoraTimeLine)
                    {
                        ejb = new EstadisticasJugadorPartido();
                        var update = Builders<BitacoraPartido>.Update
                                    .Push<BitacoraPartido.BitacoraTimeLine>(e => e.bitacoraTimeLine, b);
                        await _bitacoraPartido.FindOneAndUpdateAsync(filter, update);

                        if (b.Accion == (TipoAccion)16 || b.Accion == (TipoAccion)15)
                        {
                            bool esTitular;
                            jugador = await _jugadores.Find<Jugador>(j => j.Id == b.idJugador).FirstOrDefaultAsync();
                            equipoDeJugador = await _equipo.Find<Equipo>(e => e.Id == jugador.IdEquipo).FirstOrDefaultAsync();

                            var equipoJugadorIndex = await _partido
                             .Find(p => p.Id == bp.idPartido)
                             .Project(p => p.EquipoJugador.FindIndex(t => t.idEquipo == equipoDeJugador.Id))
                             .SingleOrDefaultAsync();

                            foreach (EquipoJugador.JugadorEquipo je in par.EquipoJugador[equipoJugadorIndex].jugadorEquipo)
                            {
                                int indexJugador = par.EquipoJugador[equipoJugadorIndex].jugadorEquipo.IndexOf(je);
                                if (je.idJugador == b.idJugador)
                                {
                                    if (b.Accion == (TipoAccion)16)
                                        esTitular = false;
                                    else
                                        esTitular = true;
                                    var UpdateDefinitionBuilder = Builders<Partido>.Update.Set(p => p.EquipoJugador[equipoJugadorIndex].jugadorEquipo[indexJugador].esTitular, esTitular);

                                    await _partido.UpdateOneAsync(p => p.Id == bp.idPartido, UpdateDefinitionBuilder);
                                }

                            }

                        }
                        ejb2 = _estadisticasJugadorPartido.BuscarEstadisticasJugadorPartido(bp.idPartido, b.idJugador);
                        if (ejb2 != null)
                            ejb.Id = ejb2.Id;
                        ejb.IdJugador = b.idJugador;
                        ejb.IdPartido = bp.idPartido;
                        if (b.Accion == (TipoAccion)0)
                        {
                            ejb.TirosLibresIntentados = 1;
                        }
                        if (b.Accion == (TipoAccion)1)
                        {
                            ejb.Puntos = 1;
                            ejb.TirosLibresConvertidos = 1;
                            ejb.TirosLibresIntentados = 1;
                        }
                        if (b.Accion == (TipoAccion)2)
                        {
                            ejb.DosPuntosIntentados = 1;
                        }
                        if (b.Accion == (TipoAccion)3)
                        {
                            ejb.Puntos = 2;
                            ejb.DosPuntosConvertidos = 1;
                            ejb.DosPuntosIntentados = 1;
                        }
                        if (b.Accion == (TipoAccion)4)
                        {
                            ejb.TresPuntosIntentados = 1;
                        }
                        if (b.Accion == (TipoAccion)5)
                        {
                            ejb.Puntos = 3;
                            ejb.TresPuntosConvertidos = 1;
                            ejb.TresPuntosIntentados = 1;
                        }
                        if (b.Accion == (TipoAccion)6)
                            ejb.RebotesOfensivos = 1;
                        if (b.Accion == (TipoAccion)7)
                            ejb.RebotesDefensivos = 1;
                        if (b.Accion == (TipoAccion)8)
                            ejb.Bloqueos = 1;
                        if (b.Accion == (TipoAccion)9)
                            ejb.Asistencias = 1;
                        if (b.Accion == (TipoAccion)10)
                            ejb.Perdidas = 1;
                        if (b.Accion == (TipoAccion)11)
                            ejb.Recuperos = 1;
                        if (b.Accion == (TipoAccion)12)
                            ejb.FaltasPersonales = 1;
                        if (b.Accion == (TipoAccion)13)
                            ejb.FaltasAntideportivas = 1;
                        if (b.Accion == (TipoAccion)14)
                            ejb.FaltasCometidas = 1;
                        if (b.Accion == (TipoAccion)18)
                            ejb.FaltasTecnicas = 1;

                        if (b.CoordenadasAcciones != null)
                        {
                            coordenadas.Add(b.CoordenadasAcciones);
                            ejb.CoordenadasAcciones = coordenadas;
                        }

                        await _estadisticasJugadorPartido.CargarEstadistica(ejb);
                        //Object eep = await _partidoService.ConsultaDetallesPartido(bp.idPartido);


                        // return true;
                    }
                    //cargoStatDesdeBitacora(bp);
                    //return true;
                    return await _partidoService.ConsultarHeaderPartido(bp.idPartido);

                }
                /*foreach(BitacoraPartido.BitacoraTimeLine b in bp.bitacoraTimeLine)
                {
                  ejb.IdJugador=b.idJugador;
                  ejb.IdPartido=bp.idPartido;
                  if(b.Accion==(TipoAccion)0)
                  {
                    ejb.TirosLibresIntentados=1;
                  }
                  if(b.Accion==(TipoAccion)1)
                  {
                    ejb.Puntos=1;
                    ejb.TirosLibresConvertidos=1;
                  }
                  if(b.Accion==(TipoAccion)2)
                  {
                    ejb.DosPuntosIntentados=1;
                  }
                  if(b.Accion==(TipoAccion)3)
                  {
                    ejb.Puntos=2;
                    ejb.DosPuntosConvertidos=1;
                  }
                  if(b.Accion==(TipoAccion)4)
                  {
                    ejb.TresPuntosIntentados=1;
                  }
                  if(b.Accion==(TipoAccion)5)
                  {
                    ejb.Puntos=3;
                    ejb.TresPuntosConvertidos=1;
                  }
                  if(b.Accion==(TipoAccion)6)
                    ejb.RebotesOfensivos=1;
                  if(b.Accion==(TipoAccion)7)
                    ejb.RebotesDefensivos=1;
                  if(b.Accion==(TipoAccion)8)
                    ejb.Bloqueos=1;
                  if(b.Accion==(TipoAccion)9)
                    ejb.Asistencias=1;
                  if(b.Accion==(TipoAccion)10)
                    ejb.Perdidas=1;
                  if(b.Accion==(TipoAccion)11)
                    ejb.Recuperos=1;
                  if(b.Accion==(TipoAccion)12)
                    ejb.FaltasPersonales=1;
                  if(b.Accion==(TipoAccion)13)
                    ejb.FaltasRecibidas=1;
                  if(b.Accion==(TipoAccion)14)
                    ejb.FaltasCometidas=1;

                  coordenadas.Add(b.CoordenadasAcciones);

                  await _estadisticasJugadorPartido.CargarEstadistica(ejb);
                  return true; */

            }

            catch
            {
                return false;
            }

        }

        private Boolean cargoStatDesdeBitacora(BitacoraPartido bp)
        {
            try
            {
                EstadisticasJugadorPartido ejb = new EstadisticasJugadorPartido();
                List<Coordenada> coordenadas = new List<Coordenada>();
                foreach (BitacoraPartido.BitacoraTimeLine b in bp.bitacoraTimeLine)
                {
                    ejb.IdJugador = b.idJugador;
                    ejb.IdPartido = bp.idPartido;
                    if (b.Accion == (TipoAccion)0)
                    {
                        ejb.TirosLibresIntentados = 1;
                    }
                    if (b.Accion == (TipoAccion)1)
                    {
                        ejb.Puntos = 1;
                        ejb.TirosLibresIntentados = 1;
                        ejb.TirosLibresConvertidos = 1;
                    }
                    if (b.Accion == (TipoAccion)2)
                    {
                        ejb.DosPuntosIntentados = 1;
                    }
                    if (b.Accion == (TipoAccion)3)
                    {
                        ejb.Puntos = 2;
                        ejb.DosPuntosIntentados = 1;
                        ejb.DosPuntosConvertidos = 1;
                    }
                    if (b.Accion == (TipoAccion)4)
                    {
                        ejb.TresPuntosIntentados = 1;
                    }
                    if (b.Accion == (TipoAccion)5)
                    {
                        ejb.Puntos = 3;
                        ejb.TresPuntosIntentados = 1;
                        ejb.TresPuntosConvertidos = 1;
                    }
                    if (b.Accion == (TipoAccion)6)
                        ejb.RebotesOfensivos = 1;
                    if (b.Accion == (TipoAccion)7)
                        ejb.RebotesDefensivos = 1;
                    if (b.Accion == (TipoAccion)8)
                        ejb.Bloqueos = 1;
                    if (b.Accion == (TipoAccion)9)
                        ejb.Asistencias = 1;
                    if (b.Accion == (TipoAccion)10)
                        ejb.Perdidas = 1;
                    if (b.Accion == (TipoAccion)11)
                        ejb.Recuperos = 1;
                    if (b.Accion == (TipoAccion)12)
                        ejb.FaltasPersonales = 1;
                    if (b.Accion == (TipoAccion)13)
                        ejb.FaltasAntideportivas = 1;
                    if (b.Accion == (TipoAccion)14)
                        ejb.FaltasCometidas = 1;
                    if (b.Accion == (TipoAccion)18)
                        ejb.FaltasTecnicas = 1;

                    coordenadas.Add(b.CoordenadasAcciones);

                    _estadisticasJugadorPartido.CargarEstadistica(ejb);



                }
                return true;
            }
            catch
            {
                return false;
            }

        }

        public async Task<Object> consultarEstadisticasPeriodo(string idPartido, int    periodo)
        {
            try
            {

                int libresIntentEq1 = 0, libresAcertadosEq1 = 0, dosIntentEq1 = 0, dosAcertadosEq1 = 0, tresIntentEq1 = 0, tresAcertadosEq1 = 0, campoIntentEq1 = 0, campoAcertadosEq1 = 0, rebotesOfensivosEq1 = 0, rebotesDefensivosEq1 = 0, faltasEq1 = 0, perdidasEq1 = 0, tiempoMuertosEq1 = 0
                , libresIntentEq2 = 0, libresAcertadosEq2 = 0, dosIntentEq2 = 0, dosAcertadosEq2 = 0, tresIntentEq2 = 0, tresAcertadosEq2 = 0, campoIntentEq2 = 0, campoAcertadosEq2 = 0, rebotesOfensivosEq2 = 0, rebotesDefensivosEq2 = 0, faltasEq2 = 0, perdidasEq2 = 0, tiempoMuertosEq2 = 0;
                double libresPorcentajeEq1 = 0, dosPorcentajeEq1 = 0, tresPorcentajeEq1 = 0, campoPorcentajeEq1 = 0, libresPorcentajeEq2 = 0, dosPorcentajeEq2 = 0, tresPorcentajeEq2 = 0, campoPorcentajeEq2 = 0;




                Partido part = await _partido.Find<Partido>(x => x.Id == idPartido).FirstOrDefaultAsync();
                //List<EquipoJugador.JugadorEquipo> je1,je2;
                List<BitacoraPartido.BitacoraTimeLine> btl1 = new List<BitacoraPartido.BitacoraTimeLine>();
                List<BitacoraPartido.BitacoraTimeLine> btl2 = new List<BitacoraPartido.BitacoraTimeLine>();


                BitacoraPartido estEqPar1 = await _bitacoraPartido.Find<BitacoraPartido>(x => x.idPartido.Equals(idPartido)).FirstOrDefaultAsync();
                Equipo equipoDeJugador;
                Jugador jugador;
                foreach (BitacoraPartido.BitacoraTimeLine btl in estEqPar1.bitacoraTimeLine)
                {
                    jugador = await _jugadores.Find<Jugador>(j => j.Id == btl.idJugador).FirstOrDefaultAsync();
                    equipoDeJugador = await _equipo.Find<Equipo>(e => e.Id == jugador.IdEquipo).FirstOrDefaultAsync();
                    if (equipoDeJugador.Id == part.equipos[0].Id)
                        btl1.Add(btl);
                    else
                        btl2.Add(btl);

                }
                //EstadisticasEquipoPartido estEqPar2 = await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(x => x.IdPartido == idPartido && x.IdEquipo == part.equipos[1].Id).FirstOrDefaultAsync();

                foreach (BitacoraPartido.BitacoraTimeLine a in btl1)
                {
                    if(a.Cuarto==periodo || 0 == periodo)
                    { 
                    if (a.Accion == (TipoAccion)0)
                        libresIntentEq1++;
                    if (a.Accion == (TipoAccion)1)
                    {
                        libresAcertadosEq1++;
                        libresIntentEq1++;
                    }
                    if (a.Accion == (TipoAccion)2)
                    {
                        dosIntentEq1++;
                        campoIntentEq1++;
                    }
                    if (a.Accion == (TipoAccion)3)
                    {
                        dosAcertadosEq1++;
                        dosIntentEq1++;
                        campoIntentEq1++;
                        campoAcertadosEq1++;
                    }
                    if (a.Accion == (TipoAccion)4)
                    {
                        tresIntentEq1++;
                       campoIntentEq1++;
                        }
                        if (a.Accion == (TipoAccion)5)
                        {
                            tresAcertadosEq1++;
                            tresIntentEq1++;
                            campoAcertadosEq1++;
                            campoIntentEq1++;

                        }
                    if (a.Accion == (TipoAccion)6)
                        rebotesOfensivosEq1++;
                    if (a.Accion == (TipoAccion)7)
                        rebotesDefensivosEq1++;
                        //if (a.Accion == (TipoAccion)13 || a.Accion == (TipoAccion)14)
                    if (a.Accion == (TipoAccion)14) //|| a.Accion == (TipoAccion)13 || a.Accion == (TipoAccion)18)
                        faltasEq1++;
                    if (a.Accion == (TipoAccion)10)
                        perdidasEq1++;
                    if (a.Accion == (TipoAccion)17)
                        tiempoMuertosEq1++;
                    }
                }

                foreach (BitacoraPartido.BitacoraTimeLine a in btl2)
                {
                    if (a.Cuarto == periodo || 0 == periodo)
                    {
                        if (a.Accion == (TipoAccion)0)
                            libresIntentEq2++;
                        if (a.Accion == (TipoAccion)1)
                        {
                            libresAcertadosEq2++;
                            libresIntentEq2++;
                        }
                        if (a.Accion == (TipoAccion)2)
                        {
                            dosIntentEq2++;
                            campoIntentEq2++; 
                        }
                        if (a.Accion == (TipoAccion)3)
                        {
                            dosAcertadosEq2++;
                            dosIntentEq2++;
                            campoAcertadosEq2++;
                            campoIntentEq2++;
                        }
                        if (a.Accion == (TipoAccion)4)
                        {
                            tresIntentEq2++;
                            campoIntentEq2++;
                        }
                        if (a.Accion == (TipoAccion)5)
                        {
                            tresAcertadosEq2++;
                            tresIntentEq2++;
                            campoAcertadosEq2++;
                            campoIntentEq2++;
                        }
                        if (a.Accion == (TipoAccion)6)
                            rebotesOfensivosEq2++;
                        if (a.Accion == (TipoAccion)7)
                            rebotesDefensivosEq2++;
                        if (a.Accion == (TipoAccion)14) //|| a.Accion == (TipoAccion)13 || a.Accion == (TipoAccion)18)
                            faltasEq2++;
                        if (a.Accion == (TipoAccion)10)
                            perdidasEq2++;
                        if (a.Accion == (TipoAccion)17)
                            tiempoMuertosEq2++;
                    }
                }

                if (libresIntentEq1 != 0)
                    libresPorcentajeEq1 = (libresAcertadosEq1 * 100) / libresIntentEq1;
                if (dosIntentEq1 != 0)
                    dosPorcentajeEq1 = (dosAcertadosEq1 * 100) / dosIntentEq1;
                if (campoIntentEq1 != 0)
                    campoPorcentajeEq1 = (campoAcertadosEq1 * 100) / campoIntentEq1;
                if (libresIntentEq2 != 0)
                    libresPorcentajeEq2 = (libresAcertadosEq2 * 100) / libresIntentEq2;
                if (dosIntentEq2 != 0)
                    dosPorcentajeEq2 = (dosAcertadosEq2 * 100) / dosIntentEq2;
                if (campoIntentEq2 != 0)
                    campoPorcentajeEq2 = (campoAcertadosEq2 * 100) / campoIntentEq2;
                if (tresIntentEq1 != 0)
                    tresPorcentajeEq1 = (tresAcertadosEq1 * 100) / tresIntentEq1;
                if (tresIntentEq2 != 0)
                    tresPorcentajeEq2 = (tresAcertadosEq2 * 100) / tresIntentEq2;


                var det = new
                {
                    idPartido = part.Id,
                    cuarto = periodo,
                    libIntentEq1 = libresIntentEq1,
                    libAcertadosEq1 = libresAcertadosEq1,
                    librePorcentajeEq1 = libresPorcentajeEq1,
                    dosInEqu1 = dosIntentEq1,
                    dosAcertEq1 = dosAcertadosEq1,
                    dosPorcentajEq1 = dosPorcentajeEq1,
                    tresIntEq1 = tresIntentEq1,
                    tresAcertEq1 = tresAcertadosEq1,
                    tresPorcentajeEq1 = tresPorcentajeEq1,
                    campIntentEq1 = campoIntentEq1,
                    campAcertEq1 = campoAcertadosEq1,
                    campoPorcentajEq1 = campoPorcentajeEq1,
                    rebotesOfEq1 = rebotesOfensivosEq1,
                    rebotesDefEq1 = rebotesDefensivosEq1,
                    faltsEq1 = faltasEq1,
                    perdEq1 = perdidasEq1,
                    tiempoDeadEq1 = tiempoMuertosEq1,
                    libIntentEq2 = libresIntentEq2,
                    libAcertadosEq2 = libresAcertadosEq2,
                    libresPorcentajEq2 = libresPorcentajeEq2,
                    dosInEqu2 = dosIntentEq2,
                    dosAcertEq2 = dosAcertadosEq2,
                    dosPorcentajEq2 = dosPorcentajeEq2,
                    tresIntEq2 = tresIntentEq2,
                    tresAcertEq2 = tresAcertadosEq2,
                    tresPorcentajeEq2 = tresPorcentajeEq2,
                    campIntentEq2 = campoIntentEq2,
                    campAcertEq2 = campoAcertadosEq2,
                    campoPorcentajEq2 = campoPorcentajeEq2,
                    rebotesOfEq2 = rebotesOfensivosEq2,
                    rebotesDefEq2 = rebotesDefensivosEq2,
                    faltsEq2 = faltasEq2,
                    perdEq2 = perdidasEq2,
                    tiempoDeadEq2 = tiempoMuertosEq2
                };


                return det;
            }
            catch (Exception ex)
            {
                return new { ERROR = ex.Message };
            }

        }

        public async Task<List<Object>> mejoresDiezCadaRubro(int rubro, string idTorneo)
        {
            try
            {

                // List<Partido> partidosPorTorneo = await _partido.Find<Partido>(p => p.IdTorneo == idTorneo && p.estado == (EstadoPartido)3).ToListAsync();
                //List<Partido> partidosPorTorneo = await _partido.Find<Partido>(p => p.estado == (EstadoPartido)3).ToListAsync();
                //    List<EstadisticasJugadorPartido> estJugPartido = await _statsJugadorPartidos.Find<EstadisticasJugadorPartido>(e => true).ToListAsync();
                //  List<Jugador> jugTorneo = await _jugadores.Find<Jugador>(j => true).ToListAsync();
                List<Jugador> jugTorneo = new List<Jugador>();
                List<Equipo> equi = await _equipo.Find<Equipo>(x => true).ToListAsync();
                List<Object> datos = new List<Object>();
                List<Object> datosLinq = new List<Object>();
                List<Jugador> jugadoresDeTorneo = new List<Jugador>();
                List<Equipo> equiposTorneo = new List<Equipo>();
                List<EquipoJugador.JugadorEquipo> jugadoresEquiposTorneo = new List<EquipoJugador.JugadorEquipo>();
                List<EstadisticasJugadorPartido> statsJugadoresPartidosTorneo = new List<EstadisticasJugadorPartido>();
                
                /***********************************************/
               // List<Partido> partidosPorTorneo = await _partido.Find<Partido>(p => p.estado == (EstadoPartido)3).ToListAsync();
                List<Partido> partidosPorTorneo = await _partido.Find<Partido>(p => p.IdTorneo == idTorneo && p.estado == (EstadoPartido)3).ToListAsync();
                //  List<EstadisticasEquipoPartido> estEqPar = await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(x => true).ToListAsync();
                // List<Equipo> equi = await _equipos.Find<Equipo>(x => true).ToListAsync();
                Object devv = new Object();

                /*foreach(Partido p in part)
                {*/
                foreach (Partido p in partidosPorTorneo)
                {
                    foreach (EquipoJugador.JugadorEquipo je in p.EquipoJugador[0].jugadorEquipo)
                    {
                        EstadisticasJugadorPartido stat = await _statsJugadorPartidos.Find<EstadisticasJugadorPartido>(x => x.IdPartido == p.Id && x.IdJugador == je.idJugador).FirstOrDefaultAsync();
                        if (stat != null)
                            statsJugadoresPartidosTorneo.Add(stat);
                    }
                    foreach (EquipoJugador.JugadorEquipo je in p.EquipoJugador[1].jugadorEquipo)
                    {
                        EstadisticasJugadorPartido stat = await _statsJugadorPartidos.Find<EstadisticasJugadorPartido>(x => x.IdPartido == p.Id && x.IdJugador == je.idJugador).FirstOrDefaultAsync();
                        if(stat!=null)
                        statsJugadoresPartidosTorneo.Add(stat);
                    }
                    foreach (EstadisticasJugadorPartido ejp in statsJugadoresPartidosTorneo)
                    {
                        // statsJugadoresPartidosTorneo.Add(ejp);
                        Jugador jug = await _jugadores.Find<Jugador>(j => j.Id == ejp.IdJugador).FirstOrDefaultAsync();
                        if (!jugTorneo.Any(jee => jee.Id == jug.Id))
                        {
                            jugTorneo.Add(jug);
                        }
                    }
                    foreach (Equipo e in p.equipos)
                    {
                        if (!equiposTorneo.Any(eqt => eqt.Id == e.Id))
                        {
                            equiposTorneo.Add(e);
                        }
                    }
                    {

                    }
                }
                    /* EstadisticasJugadorPartido estJug = await _estadisticasEquipoPartido.Find<EstadisticasEquipoPartido>(x => x.IdPartido == p.Id && x.IdEquipo == p.equipos[1].Id).FirstOrDefaultAsync();
                     int puntosEq1 = 0, puntosEq2 = 0;
                     if (estEqPar1 != null)
                         puntosEq1 = estEqPar1.Puntos;
                     if (estEqPar2 != null)
                         puntosEq2 = estEqPar2.Puntos;*/
                    if (rubro == 0)
                    {
                    int counter = 1;
                    var cosos = (from statJugP in statsJugadoresPartidosTorneo
                                     join jugTor in jugTorneo on statJugP.IdJugador equals jugTor.Id
                                     join eqt in equiposTorneo on jugTor.IdEquipo equals eqt.Id
                                     orderby statJugP.Puntos descending
                                     group new { statJugP, jugTor,eqt } by new { jugTor.Id , eqt.NombreEquipo, jugTor.Nombre, jugTor.Apellido } into jugGroup
                                     //group statJugP by statJugP.IdJugador into jugGroup
                                     select new
                                     {
                                         posicion = counter++,
                                         jugador =jugGroup.Key.Id,
                                         nombreJugador= jugGroup.Key.Nombre+" "+jugGroup.Key.Apellido,
                                         foto = HelperCloudinary.cloudUrl + "Jugadores/" + jugGroup.Key.Id,
                                         equipo = jugGroup.Key.NombreEquipo,
                                         puntos = jugGroup.Sum(x => x.statJugP.Puntos),
                                         //puntos = jugGroup.Sum(x => x.statJugP.Puntos),
                                     }
).Take(10).OrderByDescending(x => x.puntos);

                       // datos = cosos;
                        foreach(var v in cosos)
                            {

                        datos.Add(v);                        
                        }                    

                }
                if (rubro == 1)
                {
                    int counter = 1;
                    var cosos = (from statJugP in statsJugadoresPartidosTorneo
                                 join jugTor in jugTorneo on statJugP.IdJugador equals jugTor.Id
                                 join eqt in equiposTorneo on jugTor.IdEquipo equals eqt.Id
                                 orderby statJugP.TirosLibresConvertidos descending
                                 group new { statJugP, jugTor, eqt } by new { jugTor.Id, eqt.NombreEquipo, jugTor.Nombre, jugTor.Apellido } into jugGroup
                                 //group statJugP by statJugP.IdJugador into jugGroup
                                 select new
                                 {
                                     posicion = counter++,
                                     jugador = jugGroup.Key.Id,
                                     nombreJugador = jugGroup.Key.Nombre + " " + jugGroup.Key.Apellido,
                                     foto = HelperCloudinary.cloudUrl + "Jugadores/" + jugGroup.Key.Id,
                                     equipo = jugGroup.Key.NombreEquipo,
                                     puntos = jugGroup.Sum(x => x.statJugP.TirosLibresConvertidos),
                                     //puntos = jugGroup.Sum(x => x.statJugP.Puntos),
                                 }
).Take(10).OrderByDescending(x => x.puntos); 

                    // datos = cosos;
                    foreach (Object v in cosos)
                    {

                        datos.Add(v);
                    }

                }

                if (rubro == 2)
                {
                    int counter = 1;
                    var cosos = (from statJugP in statsJugadoresPartidosTorneo
                                 join jugTor in jugTorneo on statJugP.IdJugador equals jugTor.Id
                                 join eqt in equiposTorneo on jugTor.IdEquipo equals eqt.Id
                                 orderby statJugP.DosPuntosConvertidos descending
                                 group new { statJugP, jugTor, eqt } by new { jugTor.Id, eqt.NombreEquipo, jugTor.Nombre, jugTor.Apellido } into jugGroup
                                 //group statJugP by statJugP.IdJugador into jugGroup
                                 select new
                                 {
                                     posicion = counter++,
                                     jugador = jugGroup.Key.Id,
                                     nombreJugador = jugGroup.Key.Nombre + " " + jugGroup.Key.Apellido,
                                     foto = HelperCloudinary.cloudUrl + "Jugadores/" + jugGroup.Key.Id,
                                     equipo = jugGroup.Key.NombreEquipo,
                                     puntos = jugGroup.Sum(x => x.statJugP.DosPuntosConvertidos),
                                     //puntos = jugGroup.Sum(x => x.statJugP.Puntos),
                                 }
).Take(10).OrderByDescending(x => x.puntos); ;

                    // datos = cosos;
                    foreach (Object v in cosos)
                    {

                        datos.Add(v);
                    }

                }
                if (rubro == 3)
                {
                    int counter = 1;
                    var cosos = (from statJugP in statsJugadoresPartidosTorneo
                                 join jugTor in jugTorneo on statJugP.IdJugador equals jugTor.Id
                                 join eqt in equiposTorneo on jugTor.IdEquipo equals eqt.Id
                                 orderby statJugP.TresPuntosConvertidos descending
                                 group new { statJugP, jugTor, eqt } by new { jugTor.Id, eqt.NombreEquipo, jugTor.Nombre, jugTor.Apellido } into jugGroup
                                 //group statJugP by statJugP.IdJugador into jugGroup
                                 select new
                                 {
                                     posicion = counter++,
                                     jugador = jugGroup.Key.Id,
                                     nombreJugador = jugGroup.Key.Nombre + " " + jugGroup.Key.Apellido,
                                     foto = HelperCloudinary.cloudUrl + "Jugadores/" + jugGroup.Key.Id,
                                     equipo = jugGroup.Key.NombreEquipo,
                                     puntos = jugGroup.Sum(x => x.statJugP.TresPuntosConvertidos),
                                     //puntos = jugGroup.Sum(x => x.statJugP.Puntos),
                                 }
).Take(10).OrderByDescending(x => x.puntos); ;

                    // datos = cosos;
                    foreach (Object v in cosos)
                    {

                        datos.Add(v);
                    }

                }

                if (rubro >=4 && rubro <= 14)
                {

                    int counter = 1;
                    List<string> idJugadores = new List<string>();
                    List<string> idPartidos = new List<string>();
                    foreach (Partido p in partidosPorTorneo)
                    {
                        if(!idPartidos.Contains(p.Id))
                        {
                            idPartidos.Add(p.Id);
                        }
                        foreach (EquipoJugador.JugadorEquipo je in p.EquipoJugador[0].jugadorEquipo)
                        {
                            Jugador j = await _jugadores.Find<Jugador>(x => x.Id == je.idJugador).FirstOrDefaultAsync();
                            if (!idJugadores.Contains(j.Id))
                            {
                                idJugadores.Add(j.Id);
                            }
                        }
                        foreach (EquipoJugador.JugadorEquipo je in p.EquipoJugador[1].jugadorEquipo)
                        {
                            Jugador j = await _jugadores.Find<Jugador>(x => x.Id == je.idJugador).FirstOrDefaultAsync();
                            if (!idJugadores.Contains(j.Id))
                            {
                                idJugadores.Add(j.Id);
                            }
                        }
                    }
                        List<Jugador> jugadores = await _jugadores.Find<Jugador>(x => idJugadores.Contains(x.Id)).ToListAsync();

                    List<ExpandoObject> statsAAnalizar = new List<ExpandoObject>();
                    
                    List<Object> cosoADevolver = new List<object>();
   
                    
                    foreach (Jugador j in jugadores)
                    {
                        Equipo e = await _equipo.Find<Equipo>(eq => eq.Id.Equals(j.IdEquipo)).FirstOrDefaultAsync();
                        dynamic statEjp = new ExpandoObject();
                        statEjp.idJugador = j.Id;
                        statEjp.nombreJugador = j.Nombre + " " + j.Apellido;
                        statEjp.foto = HelperCloudinary.cloudUrl + "Jugadores/" + j.Id;
                        statEjp.equipo = e.NombreEquipo;
                        statEjp.puntos = 0.0;
                        //statEjp.partidos = 0;
                        //int paraPorcentaje = 0;
                        long partidosJugadosPorJugador= await _partido.Find<Partido>(pp => (pp.equipos[0].Id.Equals(j.IdEquipo) || pp.equipos[1].Id.Equals(j.IdEquipo)) && pp.estado.Equals((EstadoPartido)3)).CountDocumentsAsync();
                        statEjp.partidos = partidosJugadosPorJugador;
                        List<EstadisticasJugadorPartido> estJugPar = await _statsJugadorPartidos.Find<EstadisticasJugadorPartido>(es => es.IdJugador == j.Id && idPartidos.Contains(es.IdPartido)).ToListAsync();
                        foreach (EstadisticasJugadorPartido ejp in estJugPar)
                        {
                            //rubro dynamic statEjp = new Object();
                            if (rubro == 4)
                                statEjp.puntos = statEjp.puntos + ejp.Puntos;
                            if (rubro == 5)
                                statEjp.puntos = statEjp.puntos+ejp.RebotesDefensivos + ejp.RebotesOfensivos;
                            if (rubro == 6)
                                statEjp.puntos = statEjp.puntos + ejp.RebotesOfensivos;
                            if (rubro == 7)
                                statEjp.puntos = statEjp.puntos + ejp.RebotesDefensivos;
                            if (rubro == 8)
                                statEjp.puntos = statEjp.puntos + ejp.Recuperos;
                            if (rubro == 9 )
                                statEjp.puntos = statEjp.puntos + ejp.TresPuntosConvertidos;
                            if (rubro == 10)
                                statEjp.puntos = statEjp.puntos + ejp.DosPuntosConvertidos;
                            if (rubro == 11)
                                statEjp.puntos = statEjp.puntos + ejp.TirosLibresConvertidos;
                            if (rubro == 12)
                                statEjp.puntos = statEjp.puntos + ejp.TresPuntosPorcentaje;
                            if (rubro == 13)
                                statEjp.puntos = statEjp.puntos + ejp.TirosLibresPorcentaje;
                            if (rubro == 14)
                                statEjp.puntos = statEjp.puntos + ejp.DosPuntosPorcentaje;


                        }
                        /*if (rubro != 12 && rubro != 13 && rubro != 14)
                        {*/

                            Double pts = Math.Round(statEjp.puntos / partidosJugadosPorJugador,2);
                            statEjp.puntos = pts;
                            
                        //}
                        statsAAnalizar.Add(statEjp);
                    }

                    var apa = (from jugGroup in statsAAnalizar.Cast<dynamic>()
                               orderby jugGroup.puntos descending
                               select new
                               {
                                   posicion = counter++,
                                   jugador = jugGroup.idJugador,
                                   nombreJugador = jugGroup.nombreJugador,
                                   foto = jugGroup.foto,
                                   equipo = jugGroup.equipo,
                                   puntos = jugGroup.puntos,
                               }
                               
                               ).Take(10);


                    foreach(Object o in apa)
                    {
                        cosoADevolver.Add(o);
                    }
                    return cosoADevolver;
                    /*List<Jugador> jugDistintos = new List<Jugador>();

                    foreach(Jugador j in jugadores)
                    {
                        if(!jugDistintos.Contains(j))
                        {
                            jugDistintos.Add(j);
                        }
                    }*/



                }
                /***********************************************/
                /*       
        puntosPromedio,
        rebotesTotalesPromedio,
        rebotesOfensivosPromedio,
        rebotesDefensivosPromedio,
        recuperosPromedio,
        tresPuntosConvertidosPromedio,
        tirosLibresConvertidosPromedio,
        dosPuntosConvertidos,
        tresPuntosPorcentaje,
        tirosLibresPorcentaje,
        dosPuntosPorcentaje,
        minutosPromedio,
        tresPuntosConvertidos,
        tirosLibresConvertidos*/


                return datos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
 