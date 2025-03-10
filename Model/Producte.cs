using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Projecte2.Model
{


    public class Producte
    {
        [BsonId]
        public ObjectId Id { get; set; }  

        [BsonElement("nom")]
        public string nom { get; set; }  

        [BsonElement("categories_id")]
        public List<ObjectId> categoriesId { get; set; }

        [BsonElement("descripció")]
        public string descripcio { get; set; } 

        [BsonElement("foto")]
        public List<string> fotos { get; set; } 

        [BsonElement("variants")]
        public List<Variant> variants { get; set; } 
    }

    public class Variant
    {
        [BsonElement("color")]
        public string Color { get; set; }  

        [BsonElement("talles")]
        public List<Talla> Talles { get; set; }  
    }

    public class Talla
    {
        [BsonElement("_ID_talla")]
        public ObjectId IdTalla { get; set; } 

        [BsonElement("numero")]
        public int Numero { get; set; }  

        [BsonElement("stock")]
        public int Stock { get; set; } 
    }

}
