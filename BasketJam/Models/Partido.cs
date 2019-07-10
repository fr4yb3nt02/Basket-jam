using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasketJam.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Partido
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonRequired]
    [BsonElement("Fecha")]
    public DateTime fecha { get; set; }
    
    [BsonRequired]
    [BsonElement("Estado")]
    public EstadoPartido estado { get; set; }

    [BsonRequired]
    [BsonElement("Equipos")]
    public List<Equipo> equipos { get; set; }

    //[BsonRequired]
    [BsonElement("EquipoJugador")]
    public List<EquipoJugador> EquipoJugador { get; set; }
   
    [BsonRequired]
    [BsonElement("Estadio")]
    public string estadio { get; set; }

    [BsonRequired]
    [BsonElement("Cuarto")]
    public int cuarto { get; set; }

    [BsonRequired]
    [BsonElement("Tiempo")]
    public string Tiempo { get; set; }

    [BsonRequired]
    [BsonElement("Jueces")]
    public List<Juez> jueces { get; set; }
}