using MongoDB.Driver;
using Projecte2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using static Projecte2.Model.Factura;
using static System.Runtime.InteropServices.JavaScript.JSType;
using String = System.String;

namespace Projecte2.Builders
{
    class FacturaBuilder
    {
        public static Factura build(Cistell cistell, Usuari usu, String provincia, String codiP, String poblacio, String carrer, String numero, String pisPorta,
           String callefac, String numerofac, String pisoPuertafac, String ciudadfac, String provinciafac, String codigoPostalfac)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("Botiga");
            var fcollection = database.GetCollection<Factura>("Factura");
            var dcollection = database.GetCollection<DadesFiscals>("Dades_Fiscals");

            usu = buildAderça(usu, provincia, codiP, poblacio, carrer, numero, pisPorta,callefac,numerofac,pisoPuertafac,ciudadfac,provinciafac, codigoPostalfac);
            Factura f = new Factura();
            f.NumeroFactura = generarNumeroFactura(ObtenerUltimoNumeroFactura(fcollection));
            f.data = DateTime.Now;
            f.dadesFiscals = dadesBuild(dcollection);
            f.datosCliente = usu;
            f.linies = cistell.productes;
            f.preuAbansIVA = cistell.preu_abans_IVA ?? 0;
            f.preuTotalIVA = cistell.preu_total_IVA ?? 0;
            f.preuTotalAPagar = cistell.preu_total_a_pagar ?? 0;
            return f;
        }

        public static Usuari buildAderça(Usuari usu, String provincia, String codiP, String poblacio, String carrer, String numero, String pisPorta,
           String carrerFac, String numerofac, String pisportafac,   String poblaciofac, String provinciafac, String codiPfac)
        {
            Direccio d = new Direccio();
            d.pais = "Espanya";
            d.provincia = provincia;
            d.codiPostal = codiP;
            d.poblacio = poblacio;
            d.carrer = carrer;
            if (numero != null)
            {
                d.numero = numero;
            }
            else
            {
                d.numero = "S/N";
            }
            if (pisPorta != null)
            {
                d.pisPorta = pisPorta;
            }
            else
            {
                d.pisPorta = "-";
            }

            
            usu.direccio = d;
            Direccio dfac = new Direccio();
            dfac.pais = "Espanya";
            dfac.provincia = provinciafac;
            dfac.codiPostal = codiPfac;
            dfac.poblacio = poblaciofac;
            dfac.carrer = carrerFac;
            if (numerofac != null)
            {
                dfac.numero = numerofac;
            }
            else
            {
                dfac.numero = "S/N";
            }
            if (pisportafac != null)
            {
                dfac.pisPorta = pisportafac;
            }
            else
            {
                dfac.pisPorta = "-";
            }
            usu.direccioFacturacio = dfac;

            return usu;
        }
        public static int generarNumeroFactura(int ultimNumero)
        {
            int numeroFactura = ultimNumero + 1;
            return numeroFactura;
        }
        private static int ObtenerUltimoNumeroFactura(IMongoCollection<Factura> collection)
        {
            var filtro = Builders<Factura>.Filter.Exists(f => f.NumeroFactura);
            var factura = collection.Find(filtro)
                                            .SortByDescending(f => f.NumeroFactura)
                                            .FirstOrDefault();


            if (factura == null)
            {
                return 1000;
            }

            return factura.NumeroFactura;
        }

        private static DadesFiscals dadesBuild(IMongoCollection<DadesFiscals> collection)
        {
            var dadesFiscals = collection.Find(_ => true).ToList();
            if (dadesFiscals.Count > 0)
            {
                return dadesFiscals[0];
            }
            else
            {
                return null;
            }
        }
        public static void InsertarFactura(Factura factura)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("Botiga");
            var collection = database.GetCollection<Factura>("Factura");
            collection.InsertOne(factura);
        }
    }
}
