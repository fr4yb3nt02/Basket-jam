using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class TablaDePosiciones
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonRequired]
    [BsonElement("Nombre")]
    public string Nombre { get; set; }
    
    [BsonRequired]
    [BsonElement("Torneo")]
    public Torneo Torneo { get; set; }

    [BsonRequired()]
    [BsonElement("Equipos")]
    public List<Equipo> Equipos { get; set; }

    [BsonRequired]
    [BsonElement("Puntos")]
    public int Puntos { get; set; }

    [BsonRequired()]
    [BsonElement("Posicion")]
    public int Posicion { get; set; }

    [BsonRequired]
    [BsonElement("PartidosGanados")]
    public int PG { get; set; }

    [BsonRequired()]
    [BsonElement("PartidosEmpatados")]
    public int PE { get; set; }

    [BsonRequired()]
    [BsonElement("PartidosPerdidos")]
    public int PP { get; set; }

}