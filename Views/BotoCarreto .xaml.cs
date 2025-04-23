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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projecte2.Views
{
    /// <summary>
    /// Interaction logic for BotoCarreto.xaml
    /// </summary>
    public partial class BotoCarreto : UserControl
    {
      
        public static readonly DependencyProperty CistellProperty =
            DependencyProperty.Register("Cistell", typeof(Cistell), typeof(BotoCarreto));

        public Cistell Cistell
        {
            get => (Cistell)GetValue(CistellProperty);
            set => SetValue(CistellProperty, value);
        }
        public static readonly DependencyProperty UsuariProperty =
                    DependencyProperty.Register("Usuari", typeof(Usuari), typeof(BotoCarreto));
        public Usuari Usuari
        {
            get => (Usuari)GetValue(UsuariProperty);
            set => SetValue(UsuariProperty, value);
        }

        public BotoCarreto()
        {
            InitializeComponent();
        }

        private void BtnCarreto_Click(object sender, RoutedEventArgs e)
        {
            if (Cistell == null)
            {
                MessageBox.Show("Error: No s'ha trobat el carretó.");
                return;
            }
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("Botiga");
            var cistellCollection = database.GetCollection<Cistell>("Cistell");
            var filter = Builders<Cistell>.Filter.Eq("_id", Cistell._id);
            var cistellExistente = cistellCollection.Find(filter).FirstOrDefault();

            if (cistellExistente == null)
            {
                MessageBox.Show("Error: No s'ha trobat el carretó.");
                return;
            }

            if (Usuari == null)
            {
                MessageBox.Show("Error: No s'ha trobat l'usuari.");
                return;
            }
            var carretoWindow = new CarretoView(Cistell, Usuari);
            carretoWindow.Show();
        }


    }
}
