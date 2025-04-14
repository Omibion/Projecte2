using Projecte2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projecte2.Builders
{
    public class ProducteAcistellBuilder
    {
        public static ProducteACistell build(Producte prod, string var, string talla, int quantitat, double preuUnitari, double IVA)
        {
            ProducteACistell producte = new ProducteACistell();
            var variant = prod.variants.FirstOrDefault(v => v.Color == var);
            producte.id_producte = variant.Id;
            producte.variant = var;
            producte.talla = talla;
            producte.quantitat = quantitat;
            producte.preu_unitari = Math.Round(preuUnitari, 2);
            producte.preu_total = Math.Round(preuUnitari * quantitat, 2);
            producte.descompte = 0;
            producte.preu_IVA = Math.Round((producte.preu_total * IVA) / 100, 2);
            producte.preu_final = Math.Round(producte.preu_total - ((producte.preu_total * producte.descompte) / 100), 2);
            producte.preu_total_IVA = Math.Round(producte.preu_final + ((producte.preu_final * IVA) / 100), 2);
            return producte;
        }

    }
}
