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
    //[Required(ErrorMessage = "Es necesario ingresar una C.I.")]
    [StringLength(8, MinimumLength = 7, ErrorMessage = "La C.I debe tener como mínimo 7 caracateres , y 8 como máximo.")]
    public string CI { get; set; }

    [BsonRequired]
    //[Required(ErrorMessage = "Por favor ingrese un nombre de usuario")]
    [StringLength(20 , MinimumLength =5,ErrorMessage = "El nombre de usuario debe tener como mínimo 5 caracateres , y 20 como máximo.")]
   // [Required(ErrorMessage = "Es necesario ingresar un nombre de usuario.")]
    [BsonElement("NombreUser")]
    public string NomUser { get; set; }
    
    [BsonRequired]
    [BsonElement("Password")]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "La contraseña debe tener como mínimo 5 caracateres , y 50 como máximo.")]
    [Required(ErrorMessage = "Es necesario ingresar una contraseña.")]
    public string Password { get; set; }

    [BsonRequired()]
    [BsonElement("Nombre")]
    [StringLength(20, ErrorMessage = "El nombre debe tener 20 caracteres como máximo.")]
    //[Required(ErrorMessage = "Es necesario ingresar un nombre.")]
    public string Nombre { get; set; }

    [BsonElement("Apellido")]
    [StringLength(20, ErrorMessage = "El apellido debe tener 20 caracteres como máximo.")]
   // [Required(ErrorMessage ="Es necesario ingresar un apellido.")]
    public string Apellido { get; set; }

    [BsonElement("TipoUsuario")]
    public TipoUsuario TipoUsuario { get; set; }

    public string Token { get; set; }
    
}