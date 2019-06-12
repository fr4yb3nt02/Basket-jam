using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class EquipoTablaPosicion

{ 
    //[BsonRequired()]
    //[BsonElement("Equipos")]
    public string idEquipo { get; set; }

    //[BsonRequired]
    //[BsonElement("Puntos")]
    public int Puntos { get; set; }

    //[BsonRequired()]
    //[BsonElement("Posicion")]
    public int Posicion { get; set; }

    //[BsonRequired]
    //[BsonElement("PartidosGanados")]
    public int PG { get; set; }

    //[BsonRequired()]
    //[BsonElement("PartidosEmpatados")]
    public int PE { get; set; }

    //[BsonRequired()]
    //[BsonElement("PartidosPerdidos")]
    public int PP { get; set; }

    public int DIF { get; set; }
}