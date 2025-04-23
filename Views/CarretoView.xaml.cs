using MongoDB.Bson;
using MongoDB.Driver;
using Projecte2.Model;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for CarretoView.xaml
    /// </summary>
    public partial class CarretoView : Window
    {
        private readonly Cistell _cistell;
        private readonly MongoClient _client = new MongoClient("mongodb://localhost:27017");
        public double TotalAbansIva => (double)_cistell.preu_abans_IVA;
        public double TotalIva => (double)_cistell.preu_total_IVA;
        public double TotalPagar => (double)_cistell.preu_total_a_pagar;
        private readonly Usuari _usuari;
        public CarretoView(Cistell cistell,Usuari usu)
        {
            InitializeComponent();
            _cistell = cistell;
            _usuari = usu;
            DataContext = this; 
            CarregarProductes();
            MostrarPreus(_cistell);
        }
        private void CarregarProductes()
        {
            var database = _client.GetDatabase("Botiga");
            var colProductes = database.GetCollection<BsonDocument>("Productes");

            foreach (var p in _cistell.productes)
            {
                var filtre = Builders<BsonDocument>.Filter.Eq("_id", p.id_producte);
                var doc = colProductes.Find(filtre).FirstOrDefault();

                if (doc != null)
                {
                    p.ProducteNom = doc.GetValue("ProducteNom", "").AsString;
                    p.ProducteFoto = doc.GetValue("ProducteFoto", "").AsString; 
                }
               
            }

            LlistaProductes.ItemsSource = _cistell.productes;
        }


        private void ActualitzarCistell()
        {
            LlistaProductes.ItemsSource = null;
            LlistaProductes.ItemsSource = _cistell.productes;
        }

        private void EliminarProducte_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(sender is Button button))
                {
                    MessageBox.Show("Error: Acción inválida");
                    return;
                }

                if (button.Tag == null)
                {
                    MessageBox.Show("Error: No se pudo identificar el producto");
                    return;
                }

                ObjectId producteId;

                if (button.Tag is ObjectId)
                {
                    producteId = (ObjectId)button.Tag;
                }
                else if (button.Tag is string idString && ObjectId.TryParse(idString, out var parsedId))
                {
                    producteId = parsedId;
                }
                else
                {
                    MessageBox.Show("Error: Formato de ID incorrecto");
                    return;
                }

                if (MessageBox.Show("¿Seguro que quieres eliminar este producto del carrito?",
                                  "Confirmar eliminación",
                                  MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }

                if (_cistell == null)
                {
                    MessageBox.Show("Error: Carrito no disponible");
                    return;
                }

                if (_cistell.productes == null || !_cistell.productes.Any())
                {
                    MessageBox.Show("El carrito está vacío");
                    return;
                }

                var productesEliminats = _cistell.productes.RemoveAll(p => p.id_producte == producteId);

                if (productesEliminats == 0)
                {
                    MessageBox.Show("Producto no encontrado en el carrito");
                    return;
                }


                RecalcularTotals(_cistell);

                ActualitzarCistellMongoDB();

                ActualitzarCistell();
                
               MostrarPreus(_cistell);

                MessageBox.Show("Producto eliminado correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar producto: {ex.ToString()}");

                MessageBox.Show($"Error inesperado: {ex.Message}");
            }
        }

       public void MostrarPreus(Cistell cistell)
        {
            txtTotalIva.Text = null;
            txtTotalSinIVA.Text = null;
            txtTotalPagar.Text = null;

            txtTotalIva.Text = cistell.preu_total_IVA.ToString();
            txtTotalSinIVA.Text = cistell.preu_abans_IVA.ToString();
            txtTotalPagar.Text = cistell.preu_total_a_pagar.ToString();
        }

        private void EliminarCistellCompletament()
        {
            try
            {
                var database = _client.GetDatabase("Botiga");
                var collection = database.GetCollection<Cistell>("Cistell");
                collection.DeleteOne(c => c._id == _cistell._id);

                _cistell.productes.Clear();
                _cistell.preu_abans_IVA = 0;
                _cistell.preu_total_IVA = 0;
                _cistell.preu_total_a_pagar = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar carrito: {ex.Message}");
            }
        }
        public void RecalcularTotals(Cistell cistell)
        {
            cistell.preu_abans_IVA = cistell.productes?.Sum(p => p.preu_total) ?? 0;
            cistell.preu_total_IVA = cistell.productes?.Sum(p => p.preu_IVA) ?? 0;
            cistell.preu_total_a_pagar = cistell.productes?.Sum(p => p.preu_total_IVA) ?? 0;
        }
       

        private void ActualitzarCistellMongoDB()
        {
            var database = _client.GetDatabase("Botiga");
            var collection = database.GetCollection<Cistell>("Cistell");

            var update = Builders<Cistell>.Update
                .Set(c => c.productes, _cistell.productes)
                .Set(c => c.preu_abans_IVA, _cistell.preu_abans_IVA)
                .Set(c => c.preu_total_IVA, _cistell.preu_total_IVA)
                .Set(c => c.preu_total_a_pagar, _cistell.preu_total_a_pagar);

            collection.UpdateOne(c => c._id == _cistell._id, update);
        }

        private void Tornar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Pagar_Click(object sender, RoutedEventArgs e)
        {
            if (_cistell.productes.Count<1)
            {
                MessageBox.Show("El carretó està buit. Afegeix productes abans de pagar.");
                return;
            }

            MessageBox.Show("Redirigint a la pàgina de pagament...");
            PagamentView _pagamentView = new PagamentView(_cistell,_usuari);
            _pagamentView.Show();
            this.Close();
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}