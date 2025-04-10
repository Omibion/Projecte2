using Projecte2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projecte2.Helpers
{
    class ProductPaginator
    {
        public static List <Producte> Paginator(int page, int NumResults, List<Producte> allProducts)
        { 
            return allProducts.Skip((page - 1) * NumResults).Take(NumResults).ToList();
        }
    }
}
