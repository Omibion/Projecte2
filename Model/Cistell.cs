using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public decimal preu_unitari { get; set; }
        public decimal descompte { get; set; }
        public decimal preu_total { get; set; }
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
        public List<ProducteACistell> productes { get; set; } = new List<ProducteACistell>();

        [BsonElement("preu_abans_IVA")]
        public double preu_abans_IVA { get; set; }  

        [BsonElement("preu_total_IVA")]
        public double preu_total_IVA { get; set; }  

        [BsonElement("preu_total_a_pagar")]
        public double preu_total_a_pagar { get; set; } 
    }
   
}
public class ProducteACistell
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId id_producte { get; set; }

    [BsonElement("variant")]
    public string variant { get; set; }

    [BsonElement("talla")]
    public string talla { get; set; }

    [BsonElement("quantitat")]
    public int quantitat { get; set; }

    [BsonElement("preu_unitari")]
    public double preu_unitari { get; set; }

    [BsonElement("descompte")]
    public double descompte { get; set; }     

    [BsonElement("preu_total")]
    public double preu_total { get; set; }     
}