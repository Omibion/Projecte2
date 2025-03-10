using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projecte2.Model
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.Collections.Generic;

    public class Usuari
    {
        [BsonId] 
        public ObjectId Id { get; set; } 

        [BsonElement("_login")]
        public string login { get; set; } 

        [BsonElement("password")]
        public string passwordEncriptada { get; set; } 

        [BsonElement("direccions")]
        public List<Direccio> direccions { get; set; } 
    }

    public class Direccio
    {
        [BsonElement("pais")]
        public string pais { get; set; } 

        [BsonElement("provincia")]
        public string provincia { get; set; }  

        [BsonElement("codi_postal")]
        public string codiPostal { get; set; }  

        [BsonElement("poblacio")]
        public string poblacio { get; set; } 

        [BsonElement("carrer")]
        public string carrer { get; set; } 
    }

}
