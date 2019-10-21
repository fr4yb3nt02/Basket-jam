using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Torneo
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonRequired]
    [BsonElement("Nombre")]
    public string Nombre { get; set; }
    
    [BsonRequired]
    [BsonElement("Anio")]
    public int Anio { get; set; }

    [BsonRequired()]
    [BsonElement("Equipos")]
    public List<Equipo> Equipos { get; set; }

    [BsonRequired()]
    [BsonElement("Activo")]
    public Boolean Activo { get; set; }

}