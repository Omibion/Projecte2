using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projecte2.Model
{
    public class Categoria
    {
        [BsonId]  
        public ObjectId Id { get; set; } 

        [BsonElement("nom")] 
        public string nom { get; set; } 

        [BsonElement("id_categoria_pare")]
        public ObjectId? IdCategoriaPare { get; set; } 

        [BsonElement("categoria")]
        public List<Categoria> subcategorias { get; set; }  
    }
}
