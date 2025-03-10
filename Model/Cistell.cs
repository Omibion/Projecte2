using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projecte2.Model
{

    public class Cistell
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("id_usuari")]
        public ObjectId IdUsuari { get; set; }

        [BsonElement("productes")]
        public List<Producte> Productes { get; set; }

    }

    }
