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

            // Abre la ventana del carrito con el Cistell pasado manualmente
            //var carretoWindow = new CarretoView(Cistell);
            //carretoWindow.Show();
        }
    }
}
