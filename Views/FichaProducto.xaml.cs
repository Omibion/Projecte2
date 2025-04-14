using MongoDB.Bson;
using MongoDB.Driver;
using Projecte2.Builders;
using Projecte2.Helpers;
using Projecte2.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Projecte2.Views
{
    /// <summary>
    /// Interaction logic for FichaProducto.xaml
    /// </summary>
    public partial class FichaProducto : Window
    {
        MongoClient client = new MongoClient("mongodb://localhost:27017");
        private readonly Producte _producte;
        double iva_general;
        double iva_reduit;
        double iva_superreduit;
        Usuari user;
        Cistell cistell;
        public FichaProducto(Producte producte,Usuari usuari, Cistell cistella)
        {
            _producte = producte;
            InitializeComponent();
            user = usuari;
            cistell=cistella;
            IMongoDatabase database = client.GetDatabase("Botiga");
            IMongoCollection<Producte> collection = database.GetCollection<Producte>("Productes");
            IMongoCollection<Iva> collectioniva = database.GetCollection<Iva>("IVA");
            
            carregarIva();
            CarregarProductesEnVista(_producte);
            rellenarComboVariants(_producte);
            VariantCmb.SelectionChanged += (s, e) =>
            {
                if (VariantCmb.SelectedIndex >= 0 && producte.variants != null &&
                    VariantCmb.SelectedIndex < producte.variants.Count)
                {
                    ProductPrice.Text = calculaPreuIva(producte.variants[0].Preu, iva_general).ToString("C");
                    CmbTallas.ItemsSource = producte.variants[VariantCmb.SelectedIndex].Talles.Select(talla => talla.Numero).OrderBy(x => x).ToList();
                    CmbTallas.SelectedIndex = 0; 
                    StockTextBox.Text = producte.variants[VariantCmb.SelectedIndex].Talles.First().Stock.ToString();
                }
            };
            CmbTallas.SelectionChanged += (s, e) =>
            {
                if (CmbTallas.SelectedItem != null)
                {
                    StockTextBox.Text = producte.variants[VariantCmb.SelectedIndex].Talles.First(talla => talla.Numero == (int)CmbTallas.SelectedItem).Stock.ToString();
                }
            };
           
            BotoCarreto.Cistell = cistell;
        }

        private double calculaPreuIva(double preu, double iva)
        {
            return preu + (preu * iva / 100);
        }

        private void CarregarProductesEnVista(Producte producte)
        {
            this.DataContext = producte;
            ProductName.Text = producte.nom;
            ProductDescription.Text = producte.descripcio;
            ProductImage.Source = new BitmapImage(new Uri(producte.PrimeraFoto, UriKind.RelativeOrAbsolute));
            ProductPrice.Text = calculaPreuIva(producte.variants[0].Preu,iva_general).ToString("C");
            StockTextBox.Text = producte.variants[0].Talles[0].Stock.ToString();
            CmbTallas.ItemsSource = producte.variants[0].Talles.Select(talla => talla.Numero).ToList();
            CmbTallas.SelectedIndex = 0; // Selecciona la primera talla por defecto
            //ProductImage1.Source = new BitmapImage(new Uri(producte.fotos[0], UriKind.RelativeOrAbsolute));
            //ProductImage2.Source = new BitmapImage(new Uri(producte.fotos[1], UriKind.RelativeOrAbsolute));
            //ProductImage3.Source = new BitmapImage(new Uri(producte.fotos[2], UriKind.RelativeOrAbsolute));
            //ProductImage4.Source = new BitmapImage(new Uri(producte.fotos[3], UriKind.RelativeOrAbsolute));

        }
        
        private void carregarIva()
        {
            IMongoDatabase database = client.GetDatabase("Botiga");
            IMongoCollection<Iva> collectioniva = database.GetCollection<Iva>("IVA");
            var filter = new BsonDocument { { "tipus", "normal" } };
            Iva iva = collectioniva.Find(filter).FirstOrDefault();
            if (iva != null)
            {
                iva_general = iva.Percentatge;
            }
            
            }

        private void VariantCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VariantCmb.SelectedItem is Variant variantSeleccionada)
            {
                ProductPrice.Text = $"{variantSeleccionada.Preu:C}";
                CmbTallas.Items.Clear(); 
                CmbTallas.ItemsSource = variantSeleccionada.Talles.Select(talla => talla.Numero).ToList();
            }
        }

        private void Torna_Click(object sender, RoutedEventArgs e)
        {
            Principal principal = new Principal(user, cistell);

            if (principal != null)
            {
                principal.Show();
                this.Close();
            }
            else
            {
                new Principal(user).Show();
                this.Close();
            }
        }
        private void rellenarComboVariants(Producte producte)
        {
            if (producte?.variants == null || !producte.variants.Any())
            {
                VariantCmb.ItemsSource = null;
                VariantCmb.IsEnabled = false; 
                return;
            }

           List<Variant> variants = producte.variants.ToList();
            foreach (var variant in variants)
            {
                VariantCmb.Items.Add(variant);
            }
          
            if (variants.Count > 0)
            {
                VariantCmb.SelectedIndex = 0;
            }
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CmbTallas.SelectedItem == null || VariantCmb.SelectedItem == null || CmbQuantitat.SelectedItem == null)
                {
                    MessageBox.Show("❌ Por favor, selecciona todas las opciones (talla, variante y cantidad)");
                    return;
                }

                int talla = (int)CmbTallas.SelectedItem;
                int variantIndex = VariantCmb.SelectedIndex;
                int quantitat = int.Parse(((ComboBoxItem)CmbQuantitat.SelectedItem).Content.ToString());

                if (quantitat < 1)
                {
                    MessageBox.Show("❌ La cantidad debe ser mayor que 0");
                    return;
                }

                var tallaSeleccionada = _producte.variants[variantIndex].Talles.FirstOrDefault(t => t.Numero == talla);
                if (tallaSeleccionada == null || tallaSeleccionada.Stock < quantitat)
                {
                    MessageBox.Show("❌ No hay suficiente stock disponible");
                    return;
                }

                var database = client.GetDatabase("Botiga");
                var collectionCistell = database.GetCollection<Cistell>("Cistell");

                var filterCistellUsuario = Builders<Cistell>.Filter.Eq(c => c.id_usuari, user.Id);
                var cistellExistente = collectionCistell.Find(filterCistellUsuario).FirstOrDefault();

                if (cistellExistente == null)
                {
                    var producteACistell = ProducteAcistellBuilder.build(
                        _producte,
                        _producte.variants[variantIndex].Color,
                        talla.ToString(),
                        quantitat,
                        _producte.variants[variantIndex].Preu,
                        iva_general
                    );

                    var nouCistell = new Cistell
                    {
                        _id = ObjectId.GenerateNewId(),
                        id_usuari = user.Id,
                        productes = new List<ProducteACistell> { producteACistell },
                        preu_abans_IVA = Math.Round(producteACistell.preu_total, 2),
                        preu_total_IVA = Math.Round(producteACistell.preu_total * (iva_general / 100), 2),
                        preu_total_a_pagar = Math.Round(producteACistell.preu_total * (1 + (iva_general / 100)), 2)
                    };
                    collectionCistell.InsertOne(nouCistell);
                    cistell = nouCistell; 
                }
                else
                {
          
                    cistell = cistellExistente;

                    var producteExistente = cistellExistente.productes.FirstOrDefault(p =>
                        p.id_producte == _producte.Id &&
                        p.variant == _producte.variants[variantIndex].Color &&
                        p.talla == talla.ToString());

                    if (producteExistente != null)
                    {
               
                        producteExistente.quantitat += quantitat;
                        producteExistente.preu_total = Math.Round(producteExistente.preu_unitari * producteExistente.quantitat, 2);
                        producteExistente.preu_IVA = Math.Round((producteExistente.preu_total * iva_general) / 100, 2);
                        producteExistente.preu_final = Math.Round(producteExistente.preu_total - ((producteExistente.preu_total * producteExistente.descompte) / 100), 2);
                        producteExistente.preu_total_IVA = Math.Round(producteExistente.preu_final + ((producteExistente.preu_final * iva_general) / 100), 2);
                    }
                    else
                    {
    
                        var producteACistell = ProducteAcistellBuilder.build(
                            _producte,
                            _producte.variants[variantIndex].Color,
                            talla.ToString(),
                            quantitat,
                            _producte.variants[variantIndex].Preu,
                            iva_general
                        );
                        cistellExistente.productes.Add(producteACistell);
                    }

                    cistellExistente.preu_abans_IVA = Math.Round(cistellExistente.productes.Sum(p => p.preu_total), 2);
                    cistellExistente.preu_total_IVA = Math.Round(cistellExistente.productes.Sum(p => p.preu_IVA), 2);
                    cistellExistente.preu_total_a_pagar = Math.Round(cistellExistente.productes.Sum(p => p.preu_total_IVA), 2);

  
                    var update = Builders<Cistell>.Update
                        .Set(c => c.productes, cistellExistente.productes)
                        .Set(c => c.preu_abans_IVA, cistellExistente.preu_abans_IVA)
                        .Set(c => c.preu_total_IVA, cistellExistente.preu_total_IVA)
                        .Set(c => c.preu_total_a_pagar, cistellExistente.preu_total_a_pagar);
                    cistell = cistellExistente;

                    collectionCistell.UpdateOne(filterCistellUsuario, update);
                }

                MessageBox.Show("✅ Producto añadido al carrito");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error: {ex.Message}");
            }
            BotoCarreto.Cistell = cistell;
        }

        private void Desastock_Click(object sender, RoutedEventArgs e)
        {
            int stock = int.Parse(StockTextBox.Text);
            IMongoCollection<Producte> collection;
            int i=CmbTallas.SelectedIndex;
            if (stock > 0)
            {
                collection = client.GetDatabase("Botiga").GetCollection<Producte>("Productes");
                var filter = Builders<Producte>.Filter.Eq(p => p.nom, ProductName.Text);
                var update = Builders<Producte>.Update.Set(p => p.variants[VariantCmb.SelectedIndex].Talles[i].Stock, stock);
                collection.UpdateOne(filter, update);
                StockTextBox.Text = (stock).ToString();
                _producte.variants[VariantCmb.SelectedIndex].Talles[i].Stock = stock;
            }
            
        }
    }
}
