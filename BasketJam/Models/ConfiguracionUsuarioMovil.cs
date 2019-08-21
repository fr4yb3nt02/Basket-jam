using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasketJam.Models
{
    public class ConfiguracionUsuarioMovil
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        [BsonElement("Usuario")]
        public string Usuario { get; set; }

        [BsonRequired]
        [BsonElement("NotificacionEquiposFavoritos")]
        public Boolean NotificacionEquiposFavoritos { get; set; }

        [BsonRequired]
        [BsonElement("EquiposFavoritos")]
        public List<String> EquiposFavoritos { get; set; }

        [BsonRequired]
        [BsonElement("NotificacionTodosLosPartidos")]
        public Boolean NotificacionTodosLosPartidos { get; set; }

        [BsonRequired]
        [BsonElement("NotificacionInicioPartido")]
        public Boolean NotificacionInicioPartido { get; set; }

        [BsonRequired]
        [BsonElement("NotificacionFinPartido")]
        public Boolean NotificacionFinPartido { get; set; }

    }
}
