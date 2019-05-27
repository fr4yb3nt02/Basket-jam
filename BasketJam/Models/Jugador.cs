using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Jugador : MiembroDeEquipo
{
    [BsonRequired]
    [BsonElement("Capitan")]
    public bool Capitan { get; set; }
    
    [BsonRequired]
    [BsonElement("Posicion")]
    public string Posicion { get; set; }

    [BsonRequired()]
    [BsonElement("NumeroCamiseta")]
    public int NumeroCamiseta { get; set; }

    [BsonRequired()]
    [BsonElement("Altura")]
    public double Altura { get; set; }

    [BsonRequired()]
    [BsonElement("Peso")]
    public double Peso { get; set; }




}