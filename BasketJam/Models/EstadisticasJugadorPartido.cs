using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasketJam.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class EstadisticasJugadorPartido
{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonRequired]
        [BsonElement("IdJugador")]
        public string IdJugador { get; set; }
        
        [BsonRequired]
        [BsonElement("IdPartido")]
        public string IdPartido { get; set; }

        [BsonRequired]
        [BsonElement("Puntos")]
        public int Puntos { get; set; }

        [BsonRequired]
        [BsonElement("Eficiencia")]
        public int Eficiencia { get; set; }

        [BsonRequired]
        [BsonElement("TresPuntosConvertidos")]
        public int TresPuntosConvertidos { get; set; }

        [BsonRequired]
        [BsonElement("TresPuntosIntentados")]
        public int TresPuntosIntentados { get; set; }

        [BsonRequired]
        [BsonElement("TresPuntosPorcentaje")]
        public double TresPuntosPorcentaje { get; set; }

        [BsonRequired]
        [BsonElement("DosPuntosConvertidos")]
        public int DosPuntosConvertidos { get; set; }

        [BsonRequired]
        [BsonElement("DosPuntosIntentados")]
        public int DosPuntosIntentados { get; set; }

        [BsonRequired]
        [BsonElement("DosPuntosPorcentaje")]
        public double DosPuntosPorcentaje { get; set; }

        [BsonRequired]
        [BsonElement("TirosLibresConvertidos")]
        public int TirosLibresConvertidos { get; set; }

        [BsonRequired]
        [BsonElement("TirosLibresIntentados")]
        public int TirosLibresIntentados { get; set; }

        [BsonRequired]
        [BsonElement("TirosLibresPorcentaje")]
        public double TirosLibresPorcentaje { get; set; }

        [BsonRequired]
        [BsonElement("RebotesOfensivos")]
        public int RebotesOfensivos { get; set; }

        [BsonRequired]
        [BsonElement("RebotesDefensivos")]
        public int RebotesDefensivos { get; set; }

        [BsonRequired]
        [BsonElement("RebotesTotales")]
        public int RebotesTotales { get; set; }

        [BsonRequired]
        [BsonElement("Bloqueos")]
        public int Bloqueos { get; set; }

        [BsonRequired]
        [BsonElement("Asistencias")]
        public int Asistencias { get; set; }

        [BsonRequired]
        [BsonElement("Perdidas")]
        public int Perdidas { get; set; }

        [BsonRequired]
        [BsonElement("Recuperos")]
        public int Recuperos { get; set; }

        [BsonRequired]
        [BsonElement("FaltasPersonales")]
        public int FaltasPersonales { get; set; }

        [BsonRequired]
        [BsonElement("FaltasAntideportivas")]
        public int FaltasAntideportivas { get; set; }

        [BsonRequired]
        [BsonElement("FaltasTecnicas")]
        public int FaltasTecnicas { get; set; }

        [BsonRequired]
        [BsonElement("FaltasCometidas")]
        public int FaltasCometidas { get; set; }

        [BsonRequired]
        [BsonElement("CoordenadasAcciones")]
        public List<Coordenada> CoordenadasAcciones { get; set; }
    

}