﻿using MongoDB.Bson;
using MongoDB.Driver;
using Projecte2.Builders;
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
            Principal principal = Application.Current.Windows.OfType<Principal>().FirstOrDefault();

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
            if(CmbTallas.SelectedItem != null)
            {
                int stock = int.Parse(StockTextBox.Text);
                if (stock > 0)
                {
                    int talla = (int)CmbTallas.SelectedItem;
                    int variantIndex = VariantCmb.SelectedIndex;
                    int stockVariant = _producte.variants[variantIndex].Talles.First(t => t.Numero == talla).Stock;
                    if (stockVariant > 0)
                    {
                        _producte.variants[variantIndex].Talles.First(t => t.Numero == talla).Stock--;
                        IMongoCollection<Producte> collection = client.GetDatabase("Botiga").GetCollection<Producte>("Productes");
                        var filter = Builders<Producte>.Filter.Eq(p => p.nom, _producte.nom);
                        var update = Builders<Producte>.Update.Set(p => p.variants[variantIndex].Talles.First(t => t.Numero == talla).Stock, stockVariant - 1);
                        if (cistell.productes == null)
                        {
                            cistell.productes = new List<ProducteACistell>();
                        }
                        try
                        {
                            // Verificar que los valores seleccionados no sean nulos
                            if (VariantCmb.SelectedValue == null || CmbTallas.SelectedValue == null || CmbQuantitat.SelectedValue == null)
                            {
                                MessageBox.Show("Por favor, selecciona todas las opciones");
                                return;
                            }
                            int quantitat = int.Parse(CmbQuantitat.SelectedValue.ToString());
                            // Convertir cantidad de forma segura
                            if (quantitat!=null||quantitat<1)
                            {
                                MessageBox.Show("La cantidad seleccionada no es válida");
                                return;
                            }

                            // Verificar que el índice de la variante es válido
                            if (VariantCmb.SelectedIndex < 0 || VariantCmb.SelectedIndex >= _producte.variants.Count)
                            {
                                MessageBox.Show("La variante seleccionada no es válida");
                                return;
                            }

                            // Obtener los valores seleccionados
                            string variant = VariantCmb.SelectedValue.ToString();
                            string tallas = CmbTallas.SelectedValue.ToString();
                            double preu = _producte.variants[VariantCmb.SelectedIndex].Preu;

                            // Construir el producto
                            ProducteACistell producteA = ProducteAcistellBuilder.build(
                                variant,
                                tallas,
                                quantitat,
                                preu,
                                iva_general
                            );
                            cistell.productes.Add(producteA);
                            MessageBox.Show("✅ Producte afegit al cistell.");
                            // Aquí puedes usar producteA...
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error al crear el producto: {ex.Message}");
                            Debug.WriteLine(ex.ToString());
                        }
                        ;
                       
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("❌ No hi ha stock disponible per aquesta talla.");
                    }
                }
            }
            else
            {
                MessageBox.Show("❌ Selecciona una talla.");
            }
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
