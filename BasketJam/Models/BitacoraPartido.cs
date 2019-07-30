using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class BitacoraPartido 
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonRequired]
    [BsonElement("idPartido")]
    public string idPartido { get; set; }

    [BsonRequired]
    [BsonElement("bitacoraTimeLine")]
    public List<BitacoraTimeLine> bitacoraTimeLine { get; set; }

    
    public class BitacoraTimeLine
    {
   // [BsonRequired()]
   // [BsonElement("idJugador")]
    public string idJugador { get; set; }

   // [BsonRequired]
   // [BsonElement("Accion")]
    public TipoAccion Accion { get; set; }

   // [BsonRequired()]
   // [BsonElement("Cuarto")]
    public int Cuarto { get; set; }

    //[BsonRequired()]
    //[BsonElement("Tiempo")]
    public string Tiempo { get; set; }

    public Coordenada CoordenadasAcciones { get; set; }
}

}