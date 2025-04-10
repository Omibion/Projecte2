using MongoDB.Bson;
using Projecte2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projecte2.Filtres
{
    class ProductFilters
    {
        public static List<Producte> getProductsByCategory(List<Producte> productes, List<Categoria> categoriaList)
        {
            List<Producte> productesFiltrats = new List<Producte>();
            foreach (Producte producte in productes)
            { 
                foreach (ObjectId categoriaId in producte.categoriesId)
                {
                    foreach (Categoria categoria in categoriaList)
                    {
                        if (categoriaId == categoria.Id)
                        {
                            productesFiltrats.Add(producte);
                        }
                    }
                }
            }
            return productesFiltrats;
        }
        public static List<Producte> getProductsByPrice(List<Producte> productes, double preuBaix, double preuAlt)
        {
            List<Producte> productesFiltrats = new List<Producte>();
            foreach (Producte producte in productes)
            {
                bool matches = false;
                foreach (Variant variant in producte.variants)
                {
                    if (variant.Preu >= preuBaix && variant.Preu <= preuAlt)
                    {
                        matches =true;
                    }
                }
                if (matches)
                {
                    productesFiltrats.Add(producte);
                }   
            }
            return productesFiltrats;
        }
        public static List<Producte> getProductsByTalla(List<Producte> productes, int talla)
        {
            
            List<Producte> productesFiltrats = new List<Producte>();
            foreach (Producte producte in productes)
            {
                Boolean matches = false;
                foreach (Variant variant in producte.variants)
                {
                    foreach (Talla tallaVariant in variant.Talles)
                    {
                        if (tallaVariant.Numero == talla)
                        {
                            matches = true;
                        }
                    }
                }
                if (matches)
                {
                    productesFiltrats.Add(producte);
                }
            }
            return productesFiltrats;
        }
        public static List<Producte> getProductsByName(List<Producte> productes, string nom)
        {
            List<Producte> productesFiltrats = new List<Producte>();
            foreach (Producte producte in productes)
            {
                if (producte.nom.ToLower().Contains(nom.ToLower()))
                {
                    productesFiltrats.Add(producte);
                }
            }
            return productesFiltrats;
        }
    }
}
