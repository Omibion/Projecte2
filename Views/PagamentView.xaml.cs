using MongoDB.Bson;
using MongoDB.Driver;
using Projecte2.Builders;
using Projecte2.Model;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Projecte2.Views
{
    public partial class PagamentView : Window, INotifyPropertyChanged
    {
        private readonly IMongoCollection<Enviament> _enviamentsCollection;
        IMongoDatabase database;
        private Cistell? _cistell;
        private double _subtotal;
        private double _totalFinal;
        private double _costeEnvio;
        private Usuari usu;

        public double Subtotal
        {
            get => _subtotal;
            set { _subtotal = value; OnPropertyChanged(); ActualizarTotal(); }
        }

        public double TotalFinal
        {
            get => _totalFinal;
            set { _totalFinal = value; OnPropertyChanged(); }
        }

        public double CosteEnvio
        {
            get => _costeEnvio;
            set { _costeEnvio = value; OnPropertyChanged(); ActualizarTotal(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PagamentView(Cistell cistell, Usuari usuari)
        {
            InitializeComponent();
            DataContext = this;

            if (cistell == null || cistell.productes == null || cistell.productes.Count == 0)
            {
                MessageBox.Show("El carrito está vacío", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            usu = usuari;
            _cistell = cistell;
            Subtotal = _cistell.preu_total_a_pagar ?? 0;

            try
            {
                var client = new MongoClient("mongodb://localhost:27017");
                database = client.GetDatabase("Botiga");
                _enviamentsCollection = database.GetCollection<Enviament>("Enviament");
                CargarTiposEnvio();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar con MongoDB: {ex.Message}\nUsando datos locales",
                              "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                CargarDatosLocales();
            }

            this.usu = usu;
        }

        private async void CargarTiposEnvio()
        {
            try
            {
                var envios = await _enviamentsCollection.Find(_ => true).ToListAsync();
                CmbTipoEnvio.ItemsSource = envios.OrderBy(e => e.preu_base);
                CmbTipoEnvio.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar envíos: {ex.Message}\nUsando datos locales",
                              "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                CargarDatosLocales();
            }
        }

        private void CargarDatosLocales()
        {
            CmbTipoEnvio.ItemsSource = new[]
            {
                new Enviament { nom = "Recollida a botiga", preu_base = 0, preu_minim_gratuit = 0 },
                new Enviament { nom = "Enviament estàndard - 48h", preu_base = 4.99, preu_minim_gratuit = 50 },
                new Enviament { nom = "Enviament exprés - 24h", preu_base = 9.99, preu_minim_gratuit = 100 }
            };
            CmbTipoEnvio.SelectedIndex = 0;
        }

        private void ActualizarTotal()
        {
            TotalFinal = Subtotal + CosteEnvio;
        }

        private void CmbTipoEnvio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbTipoEnvio.SelectedItem is Enviament envio)
            {
                CosteEnvio = (Subtotal >= envio.preu_minim_gratuit) ? 0 : envio.preu_base;

                PanelDireccion.Visibility = envio.preu_base > 0 ? Visibility.Visible : Visibility.Collapsed;

                if (Subtotal >= envio.preu_minim_gratuit && envio.preu_base > 0)
                {
                    MessageBox.Show($"¡Envío gratuito! Superas los {envio.preu_minim_gratuit:C2}",
                                  "Envío gratis", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void NumeroTarjeta_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0) && e.Text != " ";
        }


        private void Numero_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        private bool ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(TxtNumeroTarjeta.Text) ||
                TxtNumeroTarjeta.Text.Replace(" ", "").Length != 16)
            {
                MessageBox.Show("Ingrese un número de tarjeta válido (16 dígitos)", "Validación",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(TxtMesCaducidad.Text, out int mes) || mes < 1 || mes > 12 ||
                !int.TryParse(TxtAnoCaducidad.Text, out int ano) || ano < DateTime.Now.Year % 100)
            {
                MessageBox.Show("Fecha de caducidad inválida", "Validación",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtCVV.Text) || TxtCVV.Text.Length != 3)
            {
                MessageBox.Show("CVV debe tener 3 dígitos", "Validación",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtNombreTitular.Text))
            {
                MessageBox.Show("Ingrese el nombre del titular", "Validación",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (PanelDireccion.Visibility == Visibility.Visible)
            {
                if (string.IsNullOrWhiteSpace(TxtCalle.Text) ||
                    string.IsNullOrWhiteSpace(TxtNumero.Text) ||
                    string.IsNullOrWhiteSpace(TxtCiudad.Text) ||
                    string.IsNullOrWhiteSpace(TxtCodigoPostal.Text))
                {
                    MessageBox.Show("Complete todos los campos obligatorios de la dirección",
                                  "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
        }

        private void BtnPagar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarDatos()) return;
            if (string.IsNullOrWhiteSpace(TxtCalleFacturacion.Text) ||
                string.IsNullOrWhiteSpace(TxtNumeroFacturacion.Text) ||
                string.IsNullOrWhiteSpace(TxtCiudadFacturacion.Text) ||
                string.IsNullOrWhiteSpace(TxtProvinciaFacturacion.Text) ||
                string.IsNullOrWhiteSpace(TxtCodigoPostalFacturacion.Text))
            {
                MessageBox.Show("Complete todos los campos de la dirección de facturación.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string calle = TxtCalleFacturacion.Text;
            string numero = TxtNumeroFacturacion.Text;
            string pisoPuerta = TxtPisoPuertaFacturacion.Text;
            string ciudad = TxtCiudadFacturacion.Text;
            string provincia = TxtProvinciaFacturacion.Text;
            string codigoPostal = TxtCodigoPostalFacturacion.Text;

            var envioSeleccionado = CmbTipoEnvio.SelectedItem as Enviament;
            string metodoEnvio = envioSeleccionado?.nom ?? "Recogida en tienda";

            MessageBox.Show($"Pago realizado con éxito!\n\n" +
                          $"Subtotal: {Subtotal:C2}\n" +
                          $"Envío ({metodoEnvio}): {CosteEnvio:C2}\n" +
                          $"TOTAL: {TotalFinal:C2}",
                          "Confirmación", MessageBoxButton.OK, MessageBoxImage.Information);

            Factura f = FacturaBuilder.build(_cistell, usu, TxtProvincia.Text, TxtCodigoPostal.Text, TxtCiudad.Text, TxtCalle.Text, TxtNumero.Text, TxtPisoPuerta.Text,
                calle,numero,pisoPuerta,ciudad,provincia,codigoPostal);
            FacturaBuilder.InsertarFactura(f);
            string jasperServerUrl = "http://localhost:8080/jasperserver";
            string reportPath = "/jasperFactura/Invoice"; 
            string username = "admin";
            string password = "admin";

            byte[] pdfBytes = GenerateJasperReport(jasperServerUrl, reportPath, username, password);
            string tempPdfPath = Path.Combine(Path.GetTempPath(), $"Factura_{f.NumeroFactura}.pdf");
            File.WriteAllBytes(tempPdfPath, pdfBytes);

           
                string email = Txtmail.Text;
                SendEmailWithAttachment(email, tempPdfPath, f.NumeroFactura.ToString());

            IMongoCollection<Cistell> cistellCollection = database.GetCollection<Cistell>("Cistell");
            var filter = Builders<Cistell>.Filter.Eq("_id", _cistell._id);
            cistellCollection.DeleteOne(filter);

            this.Close();
            _cistell = null;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
           
            this.Close();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private byte[] GenerateJasperReport(string serverUrl, string reportPath, string username, string password)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(username, password);

       
                    string url = $"{serverUrl}/rest_v2/reports{reportPath}.pdf";

                    return client.DownloadData(url);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el reporte: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private void SendEmailWithAttachment(string toEmail, string attachmentPath, string invoiceNumber)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("provaprojecte2dholgado@gmail.com");
                mail.To.Add(toEmail);
                mail.Subject = $"Factura #{invoiceNumber}";
                mail.Body = "Adjunto encontrará su factura. Gracias por su compra.";

                Attachment attachment = new Attachment(attachmentPath);
                mail.Attachments.Add(attachment);

                smtp.Port = 587;
                smtp.Credentials = new NetworkCredential("dholgado@milaifontanals.org", "cllp jjgc rfti vurp");
                smtp.EnableSsl = true;

                smtp.Send(mail);

                MessageBox.Show("Factura enviada por correo electrónico", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al enviar el email: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}