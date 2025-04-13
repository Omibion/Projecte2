using Microsoft.VisualBasic;
using MongoDB.Bson;
using MongoDB.Driver;
using Projecte2.Filtres;
using Projecte2.Helpers;
using Projecte2.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Projecte2.Views
{
    public partial class Principal : Window
    {
        Usuari user;
        private int currentPage = 1;
        private int totalPages;
        private int resultsPerPage = 10; // Valor por defecto
        public ObservableCollection<int> PageNumbers { get; set; }
        MongoClient client = new MongoClient("mongodb://localhost:27017");
        List<Producte> productes = new List<Producte>();
        Cistell cistell = new Cistell();
        public Principal(Usuari usuari)
        {
            InitializeComponent();
            user = usuari;
            IMongoDatabase database = client.GetDatabase("Botiga");
            IMongoCollection<Producte> collection = database.GetCollection<Producte>("Productes");
            if(cistell.productes==null||cistell.productes.Count == 0)
            {
                ConstruirCistell(database, cistell);
            }
            PageNumbers = new ObservableCollection<int>();
            DataContext = this;
            productes = getAllProducts(collection);
            numResults.SelectedIndex = 1;
            ItemsListView.ItemsSource = ProductPaginator.Paginator(1, resultsPerPage, productes);
            BotoCarreto.Cistell = cistell;
        }
        public Principal(Usuari usuari, Cistell cistella)
        {
            InitializeComponent();
            user = usuari;
            IMongoDatabase database = client.GetDatabase("Botiga");
            IMongoCollection<Producte> collection = database.GetCollection<Producte>("Productes");
            PageNumbers = new ObservableCollection<int>();
            DataContext = this;
            cistell = cistella;
            productes = getAllProducts(collection);
            numResults.SelectedIndex = 1;
            ItemsListView.ItemsSource = ProductPaginator.Paginator(1, resultsPerPage, productes);
            BotoCarreto.Cistell = cistell;
        }

        private void PageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && int.TryParse(button.Content.ToString(), out int pageNumber))
            {
                currentPage = pageNumber;
                ActualizarVista();
            }
        }

        private void NumResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (numResults.SelectedItem is ComboBoxItem selectedItem && int.TryParse(selectedItem.Content.ToString(), out int numResultsPerPage))
            {
                resultsPerPage = numResultsPerPage;
                currentPage = 1;
                ActualizarVista();
            }
        }

        private List<Producte> getProductesByFilterPaginats(IMongoCollection<Producte> collection, int page, int numResults,
            Categoria categoria, double? preuBaix, double? preuAlt, string nom, int? talla)
        {
            List<Producte> result = productes;

            if (categoria != null)
            {
                List<Categoria> categoriaList = categoria.GetSubcategories();
                if (categoriaList.Count == 0)
                {
                    categoriaList.Add(categoria);
                }

                result = ProductFilters.getProductsByCategory(result, categoriaList);
            }

            if (preuBaix != null && preuAlt != null)
            {
                result = ProductFilters.getProductsByPrice(result, (double)preuBaix, (double)preuAlt);
            }

            if (talla != null)
            {
                result = ProductFilters.getProductsByTalla(result, (int)talla);
            }

            if (!string.IsNullOrEmpty(nom))
            {
                result = ProductFilters.getProductsByName(result, nom);
            }

          
            totalPages = calculMaxPages(result);
            UpdatePageNumbers();

            return ProductPaginator.Paginator(page, numResults, result);
        }

        private int calculMaxPages(List<Producte> filteredProducts)
        {
            if (resultsPerPage == 0) return 1;
            int result = filteredProducts.Count / resultsPerPage;
            if (filteredProducts.Count % resultsPerPage != 0)
            {
                result++;
            }
            return result;
        }

        private void UpdatePageNumbers()
        {
            PageNumbers.Clear();
            for (int i = 1; i <= totalPages; i++)
            {
                PageNumbers.Add(i);
            }
        }

        private void ActualizarVista()
        {
            Categoria cat = (Categoria)FiltresControl.CategoriasTreeView.SelectedItem;

            double? preuBaix = FiltresControl.PreuRangeSlider.LowerValue;
            double? preuAlt = FiltresControl.PreuRangeSlider.HigherValue;
            string nom = FiltresControl.NomTextBox.Text;
            int? talla = FiltresControl.TallaComboBox.SelectedIndex >= 0 ? FiltresControl.TallaComboBox.SelectedIndex : (int?)null;

            var productesFiltrats = getProductesByFilterPaginats(
                client.GetDatabase("Botiga").GetCollection<Producte>("Productes"),
                currentPage,
                resultsPerPage,
                cat,
                preuBaix,
                preuAlt,
                nom,
                talla);

            ItemsListView.ItemsSource = productesFiltrats;
        }


        private void Filtre_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                currentPage = 1;

                Categoria cat = (Categoria)FiltresControl.CategoriasTreeView.SelectedItem;

                var preuBaix = FiltresControl.PreuMinim;
                var preuAlt = FiltresControl.PreuMaxim;
                var nom = FiltresControl.Nom;
                int? talla = null;
                if (FiltresControl.Talla != null)
                {
                    talla = FiltresControl.Talla.Value + 10; // truco para no tener que hacer un parseint
                }

                var productesFiltrats = getProductesByFilterPaginats(
                    client.GetDatabase("Botiga").GetCollection<Producte>("Productes"),
                    currentPage,
                    resultsPerPage,
                    cat,
                    preuBaix,
                    preuAlt,
                    nom,
                    talla);

                ItemsListView.ItemsSource = productesFiltrats;

                var productesFiltratsComplets = getProductesByFilterPaginats(
                    client.GetDatabase("Botiga").GetCollection<Producte>("Productes"),
                    1,
                    int.MaxValue,
                    cat,
                    preuBaix,
                    preuAlt,
                    nom,
                    talla);

                totalPages = calculMaxPages(productesFiltratsComplets);
                UpdatePageNumbers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al aplicar filtros: {ex.Message}");
            }
        }


        private List<Producte> getAllProducts(IMongoCollection<Producte> collection)
        {
            return collection.Find(Builders<Producte>.Filter.Empty).ToList();
        }

        private void Netejar_Button_Click(object sender, RoutedEventArgs e)
        {
            FiltresControl.PreuRangeSlider.LowerValue = 0;
            FiltresControl.PreuRangeSlider.HigherValue = 1000;
            FiltresControl.NomTextBox.Text = "";
            FiltresControl.TallaComboBox.SelectedIndex = -1;
            var data = FiltresControl.CategoriasTreeView.ItemsSource;
            FiltresControl.CategoriasTreeView.ItemsSource = null;
            FiltresControl.CategoriasTreeView.ItemsSource = data;

            ItemsListView.ItemsSource = getProductesByFilterPaginats(
                client.GetDatabase("Botiga").GetCollection<Producte>("Productes"),
                1,
                10,
                null,
                0,
                1000,
                "",
                null);
        }

        private void ItemsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemsListView.SelectedItem is Producte productoSeleccionado)
            {
                FichaProducto fichaProducto = new FichaProducto(productoSeleccionado,user,cistell);
                fichaProducto.Show();
                this.Hide();
                ItemsListView.SelectedItem = null;
            }
        }
        private void ConstruirCistell(IMongoDatabase database,Cistell cistell)
        {
            if (cistell == null)
            {
                throw new ArgumentNullException(nameof(cistell), "El objeto cistell no puede ser nulo");
            }
            IMongoCollection<Cistell> collection = database.GetCollection<Cistell>("Cistell");

            cistell.id_usuari = user.Id; 
            cistell._id = ObjectId.GenerateNewId();

        }
    }
}