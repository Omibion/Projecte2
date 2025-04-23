using MongoDB.Bson.Serialization.Attributes;
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

    public class Factura
    {
        [BsonId] 
        public ObjectId Id { get; set; }

        [BsonElement("numero_factura")]
        public int NumeroFactura { get; set; }

        [BsonElement("data")]
        public DateTime data { get; set; } 

        [BsonElement("id_usuari")]
        public Usuari datosCliente { get; set; }  

        [BsonElement("linies")]
        public List<ProducteACistell> linies { get; set; } 

        [BsonElement("preu_abans_IVA")]
        public double preuAbansIVA { get; set; } 

        [BsonElement("preu_total_IVA")]
        public double preuTotalIVA { get; set; } 

        [BsonElement("preu_total_a_pagar")]
        public double preuTotalAPagar { get; set; }  
        [BsonElement("dades_fiscals")]
        public DadesFiscals dadesFiscals { get; set; }  
    public class LiniaFactura
    {
        [BsonElement("_IDproducte")]
        public ObjectId IdProducte { get; set; } 

        [BsonElement("variant")]
        public string variant { get; set; } 

        [BsonElement("talla")]
        public string talla { get; set; }  

        [BsonElement("descripció")]
        public string descripcio { get; set; }  

        [BsonElement("tipus_IVA")]
        public string tipusIVA { get; set; } 

        [BsonElement("percentatge_IVA")]
        public decimal percentatgeIVA { get; set; }  

        [BsonElement("preu_unitari")]
        public decimal preuUnitari { get; set; } 

        [BsonElement("descompte")]
        public decimal descompte { get; set; }  

        [BsonElement("preu_total_IVA")]
        public decimal preuTotalIVA { get; set; }  

        [BsonElement("preu_total_linia")]
        public decimal preuTotalLinia { get; set; }  
    }


public class DadesFiscals
    {
        [BsonId]  
        public ObjectId Id { get; set; }

        [BsonElement("_nom_botiga")]
        public string nomBotiga { get; set; }  

        [BsonElement("pais")]
        public string pais { get; set; } 

        [BsonElement("provincia")]
        public string provincia { get; set; }

        [BsonElement("codi_postal")]
        public string codiPostal { get; set; } 

        [BsonElement("població")]
        public string poblacio { get; set; } 

        [BsonElement("carrer")]
        public string carrer { get; set; } 

        [BsonElement("CIF")]
        public string CIF { get; set; }  

        [BsonElement("e-mail")]
        public string email { get; set; }  

        [BsonElement("telèfon")]
        public string telefon { get; set; } 
    }


}
}
