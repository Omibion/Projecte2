using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projecte2.Model
{
    class Iva
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("categoria")]
        public string categoria { get; set; }
    }
}
