using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasketJam.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class VotacionPartido
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonRequired]
    [BsonElement("IdPartido")]
    public string IdPartido { get; set; }

    [BsonRequired]
    [BsonElement("Usuarios")]
    public List<string> Usuarios { get; set; }
    
    [BsonRequired]
    [BsonElement("Contenido_Votacion")]
    public List<ContenidoVotacion> Contenido_Votacion { get; set; }

    public class ContenidoVotacion
    {

    public string idEquipo { get; set; }

    public int votos { get; set; }

    }
}