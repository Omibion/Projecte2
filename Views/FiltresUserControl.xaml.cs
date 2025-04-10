using MongoDB.Bson;
using MongoDB.Driver;
using Projecte2.Model;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Projecte2.Views
{
    public partial class FiltresUserControl : UserControl, INotifyPropertyChanged
    {
        private Categoria _selectedCategory;
        private readonly IMongoClient _client;

        public event PropertyChangedEventHandler PropertyChanged;

        public FiltresUserControl()
        {
            InitializeComponent();
            _client = new MongoClient("mongodb://localhost:27017");
            CategoriasTreeView.SelectedItemChanged += CategoriasTreeView_SelectedItemChanged;
            Loaded += FiltresUserControl_Loaded;
            LlenarComboBoxTallas();
        }

        public Categoria CategoriaSeleccionada
        {
            get => _selectedCategory;
            private set
            {
                _selectedCategory = value;
                OnPropertyChanged();
            }
        }

        public double PreuMinim => PreuRangeSlider.LowerValue;
        public double PreuMaxim => PreuRangeSlider.HigherValue;
        public string Nom => NomTextBox.Text;
        public int? Talla => TallaComboBox.SelectedIndex >= 0 ? (int?)TallaComboBox.SelectedIndex : null;

        private async void FiltresUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await CarregarCategories();
        }

        private async Task CarregarCategories()
        {
            try
            {
                var database = _client.GetDatabase("Botiga");
                var collection = database.GetCollection<BsonDocument>("Categories");

                var arrel = await collection.Find(Builders<BsonDocument>.Filter.Eq("nom", "Sabates")).FirstOrDefaultAsync();

                if (arrel != null)
                {
                    var categories = ParseCategories(arrel);
                    CategoriasTreeView.ItemsSource = categories;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando categorías: {ex.Message}");
            }
        }

        private List<Categoria> ParseCategories(BsonDocument node)
        {
            var result = new List<Categoria>();

            if (node.Contains("categoría") && node["categoría"].IsBsonArray)
            {
                foreach (var child in node["categoría"].AsBsonArray)
                {
                    var categoria = new Categoria
                    {
                        Id = child["_id"].AsObjectId,
                        Nom = child["nom"].AsString,
                        IdCategoriaPare = node["_id"].AsObjectId
                    };

                    categoria.Subcategories = ParseCategories(child.AsBsonDocument);
                    result.Add(categoria);
                }
            }

            return result;
        }

        private void CategoriasTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                // Verificar si hay un nuevo elemento seleccionado
                if (e.NewValue is Categoria selectedCategory)
                {
                    _selectedCategory = selectedCategory;
                    OnPropertyChanged(nameof(CategoriaSeleccionada));
                }
                else
                {
                    _selectedCategory = null;
                    OnPropertyChanged(nameof(CategoriaSeleccionada));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al seleccionar categoría: {ex.Message}");
                _selectedCategory = null;
                OnPropertyChanged(nameof(CategoriaSeleccionada));
            }
        }

        private void LlenarComboBoxTallas()
        {
            TallaComboBox.Items.Clear();
            for (int i = 10; i <= 50; i++)
            {
                TallaComboBox.Items.Add(i);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}