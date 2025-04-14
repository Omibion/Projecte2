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
    }

    public class Cistell : INotifyPropertyChanged
    {
        // Propiedades SIN notificación (no necesitan binding con la UI)
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }

        [BsonElement("id_usuari")]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId id_usuari { get; set; }

        // Propiedades CON notificación (vinculadas a la UI)
        private List<ProducteACistell> _productes = new List<ProducteACistell>();
        private double? _preu_abans_IVA = 0;
        private double? _preu_total_IVA = 0;
        private double? _preu_total_a_pagar = 0;

        [BsonElement("productes")]
        public List<ProducteACistell> productes
        {
            get => _productes;
            set
            {
                if (_productes != value)
                {
                    _productes = value;
                    OnPropertyChanged();
                    RecalcularTotals(); 
                }
            }
        }

        [BsonElement("preu_abans_IVA")]
        public double? preu_abans_IVA
        {
            get => _preu_abans_IVA;
            set
            {
                if (_preu_abans_IVA != value)
                {
                    _preu_abans_IVA = value;
                    OnPropertyChanged();
                }
            }
        }

        [BsonElement("preu_total_IVA")]
        public double? preu_total_IVA
        {
            get => _preu_total_IVA;
            set
            {
                if (_preu_total_IVA != value)
                {
                    _preu_total_IVA = value;
                    OnPropertyChanged();
                }
            }
        }

        [BsonElement("preu_total_a_pagar")]
        public double? preu_total_a_pagar
        {
            get => _preu_total_a_pagar;
            set
            {
                if (_preu_total_a_pagar != value)
                {
                    _preu_total_a_pagar = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RecalcularTotals()
        {
            preu_abans_IVA = productes?.Sum(p => p.preu_total) ?? 0;
            preu_total_IVA = productes?.Sum(p => p.preu_IVA) ?? 0;
            preu_total_a_pagar = productes?.Sum(p => p.preu_total_IVA) ?? 0;
        }
    }
}