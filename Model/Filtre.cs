using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projecte2.Model
{
    public class Filtre
    {
        string categoria { get; set; }
        double? preuBaix { get; set; }
        double? preuAlt { get; set; }
        string nom { get; set; }
        int talla { get; set; }

        public Filtre(string categoria, double? preuBaix, double? preuAlt, string nom, int talla)
        {
            this.categoria = categoria;
            this.preuBaix = preuBaix;
            this.preuAlt = preuAlt;
            this.nom = nom;
            this.talla = talla;
        }
    }
}
