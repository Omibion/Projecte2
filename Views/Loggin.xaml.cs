using MongoDB.Bson;
using MongoDB.Driver;
using Projecte2.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interaction logic for Loggin.xaml
    /// </summary>
    public partial class Loggin : Window
    {
        public Loggin()
        {
            InitializeComponent();
            InsertarUsuari();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string connectionString = "mongodb://localhost:27017";
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase("Botiga");
                var collection = database.GetCollection<Usuari>("Usuari");

                string username = txtUsuario.Text;
                string password = ComputeSha512Hash(txtContrasenya.Password);

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("❌ Usuario o contraseña vacíos.");
                    return;
                }

                var filter = Builders<Usuari>.Filter.Eq(u => u.Username, username);
                var usuario = collection.Find(filter).FirstOrDefault();

                if (usuario != null)
                {
                    if (password == usuario.PasswordEncriptada)
                    {
                        MessageBox.Show($"✅ Bienvenido {username}.");

                        ObjectId userId = usuario.Id;

                        Principal principal = new Principal(usuario);
                        principal.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("❌ Contraseña incorrecta.");
                    }
                }
                else
                {
                    MessageBox.Show("❌ Usuario no encontrado.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error al iniciar sesión: " + ex.Message);
                Debug.WriteLine(ex.Message);
            }
        }

        private void InsertarUsuari()
        {
            try
            {
                string connectionString = "mongodb://localhost:27017";
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase("Botiga");
                var collection = database.GetCollection<Usuari>("Usuari");

                string username = "admin";
                string contrasenya = "admin";
                string hashedPassword = ComputeSha512Hash(contrasenya);

                var filter = Builders<Usuari>.Filter.Eq(u => u.Username, username);
                var existingUser = collection.Find(filter).FirstOrDefault();

                if (existingUser == null)
                {
                    var newUser = new Usuari
                    {
                        Username = username,
                        PasswordEncriptada = hashedPassword,
                        Direccions = new List<Direccio>() 
                    };

                    collection.InsertOne(newUser);
                    MessageBox.Show("✅ Usuario admin insertado correctamente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error al insertar usuario: " + ex.Message);
            }
        }

        private string ComputeSha512Hash(string input)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = sha512.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(sender, e);
            }
        }
    }
}

