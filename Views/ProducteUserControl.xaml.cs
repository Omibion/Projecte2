using Projecte2.Model;
using System.Windows;
using System.Windows.Controls;

namespace Projecte2.Views
{
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