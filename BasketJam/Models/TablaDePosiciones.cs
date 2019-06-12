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

    [BsonRequired]
    [BsonElement("EquipoTablaPosicion")]
    public EquipoTablaPosicion EquipoTablaPosicion { get; set; }
   

}