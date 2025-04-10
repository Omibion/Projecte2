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
    /// Interaction logic for ProducteUserControl.xaml
    /// </summary>
    public partial class ProducteUserControl : UserControl
    {
        public static readonly DependencyProperty ProducteProperty =
    DependencyProperty.Register("Producte", typeof(Producte), typeof(ProducteUserControl), new PropertyMetadata(null));

        public Producte Producte
        {
            get { return (Producte)GetValue(ProducteProperty); }
            set { SetValue(ProducteProperty, value); }
        }
        public ProducteUserControl()
        {
            InitializeComponent();
        }
    }
}
