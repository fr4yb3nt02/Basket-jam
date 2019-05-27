using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasketJam.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

public class Usuario
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonRequired]
    //[Required(ErrorMessage = "Por favor ingrese un a cédula")]
    [BsonElement("CI")]
    [Remote(action: "VerificarCI", controller: "Usuario")]
    public string CI { get; set; }

    [BsonRequired]
    //[Required(ErrorMessage = "Por favor ingrese un nombre de usuario")]
    [StringLength(20)]
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