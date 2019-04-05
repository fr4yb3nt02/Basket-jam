using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasketJam.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Usuario
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonRequired]
    [BsonElement("CI")]
    public string CI { get; set; }

    [BsonRequired]
    [BsonElement("NombreUser")]
    public string NomUser { get; set; }
    
    [BsonRequired]
    [BsonElement("Password")]
    public string Password { get; set; }

    [BsonRequired()]
    [BsonElement("Nombre")]
    public string Nombre { get; set; }

    [BsonElement("Apellido")]
    public string Apellido { get; set; }

    [BsonElement("TipoUsuario")]
    public TipoUsuario TipoUsuario { get; set; }

    public string Token { get; set; }
    
}