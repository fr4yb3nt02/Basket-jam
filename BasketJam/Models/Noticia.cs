using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Noticia
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonRequired]
    [BsonElement("Titulo")]
    public string Titulo { get; set; }
    
    [BsonRequired]
    [BsonElement("ContenidoAbreviado")]
    public string ContenidoAbreviado { get; set; }

    [BsonRequired]
    [BsonElement("Contenido")]
    public string Contenido { get; set; }

    [BsonRequired()]
    [BsonElement("Fecha")]
    public DateTime Fecha { get; set; }

}