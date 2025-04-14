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

        // Propiedades para el binding
        public double TotalAbansIva => (double)_cistell.preu_abans_IVA;
        public double TotalIva => (double)_cistell.preu_total_IVA;
        public double TotalPagar => (double)_cistell.preu_total_a_pagar;

        public CarretoView(Cistell cistell)
        {
            InitializeComponent();
            _cistell = cistell;
            DataContext = this; // Establecer el DataContext para los bindings
            CarregarProductes();
        }



        private void CarregarProductes()
        {
           
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
                // 1. Validar que el sender es un botón
                if (!(sender is Button button))
                {
                    MessageBox.Show("Error: Acción inválida");
                    return;
                }

                // 2. Obtener el producto directamente del DataContext (más limpio que usar Tag)
                if (!(button.DataContext is ProducteACistell producte))
                {
                    MessageBox.Show("Error: No se pudo identificar el producto");
                    return;
                }

                // 3. Confirmar con el usuario
                if (MessageBox.Show("¿Seguro que quieres eliminar este producto del carrito?",
                                  "Confirmar eliminación",
                                  MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }

                // 4. Validar que el carrito existe
                if (_cistell == null)
                {
                    MessageBox.Show("Error: Carrito no disponible");
                    return;
                }

                // 5. Eliminar el producto (ya no necesitamos buscar por ID)
                bool eliminado = _cistell.productes.Remove(producte);

                if (!eliminado)
                {
                    MessageBox.Show("Producto no encontrado en el carrito");
                    return;
                }

                // 6. Actualizar en base de datos
                ActualitzarCistellMongoDB();

                // 7. No necesitamos llamar a RecalcularTotals() ni ActualitzarCistell()
                // porque:
                // - El setter de productes ya llama a RecalcularTotals()
                // - INotifyPropertyChanged actualiza automáticamente los bindings

                // 8. Mostrar confirmación
                MessageBox.Show("Producto eliminado correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar producto: {ex}");
                MessageBox.Show($"Error inesperado: {ex.Message}");
            }
        }

        private void EliminarCistellCompletament()
        {
            try
            {
                var database = _client.GetDatabase("Botiga");
                var collection = database.GetCollection<Cistell>("Cistell");
                collection.DeleteOne(c => c._id == _cistell._id);

                // Resetear el carrito local
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
            if (!_cistell.productes.Any())
            {
                MessageBox.Show("El carretó està buit. Afegeix productes abans de pagar.");
                return;
            }

            // Aquí iría la lógica para abrir la ventana de pago
            MessageBox.Show("Redirigint a la pàgina de pagament...");
            // new PagamentView(_cistell).Show();
            this.Close();
        }

        // Método para notificar cambios en las propiedades (necesario para los bindings)
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}