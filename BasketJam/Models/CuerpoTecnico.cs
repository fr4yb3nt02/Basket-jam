using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasketJam.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class CuerpoTecnico : MiembroDeEquipo
{
    [BsonRequired]    
    [BsonElement("Cargo")]
    public CargoCuerpoTecnico Cargo { get; set; }
    
}