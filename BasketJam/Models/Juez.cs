using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasketJam.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Juez
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonRequired]
    [BsonElement("CI")]
    public int Ci { get; set; }
    
    [BsonRequired]
    [BsonElement("Nombre")]
    public string Nombre { get; set; }

    [BsonRequired()]
    [BsonElement("Apellido")]
    public string Apellido { get; set; }

    [BsonRequired()]
    [BsonElement("TipoJuez")]
    public TipoJuez TipoJuez { get; set; }

}