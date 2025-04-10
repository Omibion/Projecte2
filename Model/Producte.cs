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
        public string PrimeraFoto => fotos != null && fotos.Count > 0 ? fotos[0] : "/Assets/placeholder-image.jpg";

    }


    public class Variant
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("color")]
        public string Color { get; set; }

        [BsonElement("preu")]
        public double Preu { get; set; }

        [BsonElement("talles")]
        public List<Talla> Talles { get; set; }  
    }

    public class Talla
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("numero")]
        public int Numero { get; set; }  

        [BsonElement("stock")]
        public int Stock { get; set; } 
    }

   

}
