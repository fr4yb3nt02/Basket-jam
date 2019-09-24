using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasketJam.Models
{
    public class InfoDispositivo
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        [BsonElement("IDDispositivo")]
        public string IDDispositivo { get; set; }

        [BsonRequired]
        [BsonElement("Info")]
        public string Info { get; set; }

    }
}
