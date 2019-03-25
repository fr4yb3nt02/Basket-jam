using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Equipo
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonRequired]
    [BsonElement("NombreEquipo")]
    public string NombreEquipo { get; set; }
    
    [BsonRequired]
    [BsonElement("FechaFundacion")]
    public DateTime FechaFundacion { get; set; }

    [BsonRequired()]
    [BsonElement("ColorCaracteristico")]
    public string ColorCaracteristico { get; set; }

    [BsonElement("Categoria")]
    public string Categoria { get; set; }

}