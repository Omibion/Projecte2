using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Projecte2.Model
{

    public class ProducteACistell
    {
        public ObjectId id_producte { get; set; }
        public string variant { get; set; }
        public string talla { get; set; }
        public int quantitat { get; set; }
        public double preu_unitari { get; set; }
        public double descompte { get; set; }
        public double preu_total { get; set; }
        public double preu_IVA { get; set; }
        public double preu_total_IVA { get; set; }  
        public double preu_final { get; set; }
   
        public string ProducteNom { get; set; }

      
        public string ProducteFoto { get; set; }
    }

    public class Cistell 
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }

        [BsonElement("id_usuari")]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId id_usuari { get; set; }

        [BsonElement("productes")]
        public List<ProducteACistell>? productes { get; set; } = new List<ProducteACistell>();

        [BsonElement("preu_abans_IVA")]
        public double? preu_abans_IVA { get; set; } = 0;

        [BsonElement("preu_total_IVA")]
        public double? preu_total_IVA { get; set; } = 0;

        [BsonElement("preu_total_a_pagar")]
        public double? preu_total_a_pagar { get; set; } = 0;

    
        
    }
   
}
