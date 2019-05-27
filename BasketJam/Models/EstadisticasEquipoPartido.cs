using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasketJam.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class EstadisticasEquipoPartido
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonRequired]
    [BsonElement("IdEquipo")]
    public string IdEquipo { get; set; }
    
    [BsonRequired]
    [BsonElement("IdPartido")]
    public string IdPartido { get; set; }

    [BsonRequired]
    [BsonElement("Puntos")]
    public int Puntos { get; set; }

       // [BsonRequired]
    [BsonElement("PuntosPrimerCuarto")]
    public int PuntosPrimerCuarto { get; set; }

       // [BsonRequired]
    [BsonElement("PuntosSegundoCuarto")]
    public int PuntosSegundoCuarto { get; set; }

       // [BsonRequired]
    [BsonElement("PuntosTercerCuarto")]
    public int PuntosTercerCuarto { get; set; }

        //[BsonRequired]
    [BsonElement("PuntosCuartoCuarto")]
    public int PuntosCuartoCuarto { get; set; }

    [BsonRequired]
    [BsonElement("TirosLibresExitosos")]
    public int TirosLibresExitosos { get; set; }
    
    [BsonRequired]
    [BsonElement("TotalTirosLibres")]
    public int TotalTirosLibres { get; set; }
    
    [BsonRequired]
    [BsonElement("PorcentajeTirosLibres")]
    public double PorcentajeTirosLibres { get; set; }
    
    [BsonRequired]
    [BsonElement("Canasta2PuntosExitosas")]
    public int Canasta2PuntosExitosas { get; set; }
    
    [BsonRequired]
    [BsonElement("TotalCanastas2Puntos")]
    public int TotalCanastas2Puntos { get; set; }
    
    [BsonRequired]
    [BsonElement("PorcentajeCanastas2Puntos")]
    public double PorcentajeCanastas2Puntos { get; set; }
    
    [BsonRequired]
    [BsonElement("Canasta3PuntosExitosas")]
    public int Canasta3PuntosExitosas { get; set; }

    [BsonRequired]
    [BsonElement("TotalCanastas3Puntos")]
    public int TotalCanastas3Puntos { get; set; }
    
    [BsonRequired]
    [BsonElement("PorcentajeCanastas3Puntos")]
    public double PorcentajeCanastas3Puntos { get; set; }
    
    [BsonRequired]
    [BsonElement("RebotesDefensivos")]
    public int RebotesDefensivos { get; set; }

    [BsonRequired]
    [BsonElement("RebotesOfensivos")]
    public int RebotesOfensivos { get; set; }
  
    [BsonRequired]
    [BsonElement("Faltas")]
    public int Faltas { get; set; }
    
    [BsonRequired]
    [BsonElement("Perdidas")]
    public int Perdidas { get; set; }
    
    [BsonRequired]
    [BsonElement("TiemposMuertos")]
    public int TiemposMuertos { get; set; }

}