using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class EquipoJugador

{ 

    public string idEquipo {get;set;}

    public List<JugadorEquipo> jugadorEquipo {get;set;}

    public class JugadorEquipo
{

    public string idJugador { get; set; }

    public Boolean esCapitan { get; set; }

    public Boolean esTitular { get; set; }

    public Boolean jugando { get; set; }

    public int nroCamiseta { get; set; }
}


}