using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class CuerpoTecnico : MiembroDeEquipo
{
    [BsonRequired]    
    [BsonElement("Cargo")]
    public bool Cargo { get; set; }
    
}