using Projecte2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projecte2.Helpers
{
    class CalculPreusCistellHelper
    {
        public static double CalculaPreuAbansIvaCistell(List<ProducteACistell> productes)
        {
            double preuTotal = 0;
            foreach (var producte in productes)
            {
                preuTotal += producte.preu_total;
            }
            return preuTotal;
        }
        public static double CalculaPreuTotalIvaCistell(List<ProducteACistell> productes)
        {
            double preuTotal = 0;
            foreach (var producte in productes)
            {
                preuTotal += producte.preu_IVA;
            }
            return preuTotal;
        }
        public static double CalculaPreuTotalCistell(List<ProducteACistell> productes)
        {
            double preuTotal = 0;
            foreach (var producte in productes)
            {
                preuTotal += producte.preu_total_IVA;
            }
            return preuTotal;
        }
    }
}
