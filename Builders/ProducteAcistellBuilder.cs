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
        public static ProducteACistell build(Producte prod,String var, String talla,int quantitat, double preuUnitari, double IVA)
        {
            ProducteACistell producte = new ProducteACistell();
            producte.id_producte = prod.Id;
            producte.variant = var;
            producte.talla = talla;
            producte.quantitat = quantitat;
            producte.preu_unitari = preuUnitari;
            producte.preu_total=preuUnitari * quantitat;
            producte.descompte = 0;
            producte.preu_IVA = (producte.preu_total * IVA) / 100;
            producte.preu_final = producte.preu_total - ((producte.preu_total * producte.descompte) / 100);
            producte.preu_total_IVA = producte.preu_final + ((producte.preu_final * IVA) / 100);
            return producte;
        }
    }
}
