using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    //[StringLength(20, ErrorMessage = "El nombre debe tener 20 caracteres como máximo.")]
    public string NombreEquipo { get; set; }
    
    [BsonRequired]
    [BsonElement("FechaFundacion")]
    public DateTime FechaFundacion { get; set; }

    [BsonRequired()]
    [BsonElement("ColorCaracteristico")]
    public List<string> ColorCaracteristico { get; set; }

    [BsonElement("Categoria")]
    public string Categoria { get; set; }

   // [BsonElement("Estadio")]
    public Estadio Estadio { get; set; }

    [BsonElement("Activo")]
    public Boolean Activo { get; set; }

    [BsonRequired()]
    [BsonElement("UrlFoto")]
    public string UrlFoto { get; set; }

    /* [BsonRequired]
     [BsonElement("CuerpoTecnico")]
     public List<CuerpoTecnico> CuerpoTecnico { get; set; }

     [BsonRequired()]
     [BsonElement("Jugadores")]
     public List<Jugador> Jugadores { get; set; }*/

}