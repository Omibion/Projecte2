using Microsoft.VisualBasic;
using MongoDB.Driver;
using Projecte2.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Projecte2.Views
{
    public partial class Principal : Window
    {

        private int currentPage = 1;
        private int totalPages; 
        private int resultsPerPage; 
        public ObservableCollection<int> PageNumbers { get; set; }
        MongoClient client = new MongoClient("mongodb://localhost:27017");
        

        public Principal()
        {
           
            InitializeComponent();
            IMongoDatabase database = client.GetDatabase("Botiga");
            IMongoCollection<Producte> collection = database.GetCollection<Producte>("Producte");
            PageNumbers = new ObservableCollection<int>();
            DataContext = this;
            numResults.SelectedIndex = 1; 
            UpdatePagedItems();
        }

        private void PageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && int.TryParse(button.Content.ToString(), out int pageNumber))
            {
                currentPage = pageNumber;
                UpdatePagedItems();
            }
        }

        private void NumResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (numResults.SelectedItem is ComboBoxItem selectedItem && int.TryParse(selectedItem.Content.ToString(), out int numResultsPerPage))
            {
                resultsPerPage = numResultsPerPage;
                currentPage = 1;
                UpdatePagedItems();
            }
        }

        private void UpdatePagedItems()
        {
              
        }

        private List <Producte> getProductesByFilterPaginats(IMongoCollection<Producte> collection, int page, int numResults,String categoria, double preuBaix, double preuAlt, String nom,int ?talla)
        {
            int ini = (page - 1) * numResults;
            int fin = page * numResults;
            var filter = Builders<Producte>.Filter.Empty;

            if (!string.IsNullOrEmpty(categoria))
            {
                filter &= Builders<Producte>.Filter.Eq("Categoria", categoria);
            }

         
            if ((preuBaix > 0 && preuAlt > 0)&&preuBaix<preuAlt)
            {
                filter &= Builders<Producte>.Filter.Gte("Preu", preuBaix) & Builders<Producte>.Filter.Lte("Preu", preuAlt);
            }

       
            if (!string.IsNullOrEmpty(nom))
            {
                filter &= Builders<Producte>.Filter.Regex("Nom", new MongoDB.Bson.BsonRegularExpression(nom, "i"));
            }

           
            if (talla!=null&&talla > 0)
            {
                filter &= Builders<Producte>.Filter.Eq("Talla", talla);
            }

            List<Producte> result = collection.Find(filter)
                                 .Skip(ini) 
                                 .Limit(fin) 
                                 .ToList();
            return result;
        }
        private int calculMaxPages(List <Producte> allProducts)
        {
            int result = allProducts.Count / resultsPerPage;
            if(allProducts.Count % resultsPerPage != 0)
            {
                result++;
            }
            return result;
        }
    }
}
