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

namespace PC_Configurator.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void ShowPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Logic to handle when the checkbox is checked
            MessageBox.Show("Show password checked.");
        }

        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Logic to handle when the checkbox is unchecked
            MessageBox.Show("Show password unchecked.");
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: bejelentkezési logika
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            if (ValidateInput(email, password))
            {
                string passwordHash = HashPassword(password);
                bool success = CheckCredentials(email, passwordHash);

                if (success)
                {
                    MessageBox.Show("Sikeres bejelentkezés");
                    // TODO: továbbléptetés főképernyőre vagy dashboardra
                }
                else
                {
                    MessageBox.Show("Hibás email vagy jelszó");
                }
            }
        }

        private bool ValidateInput(string email, string password)
        {
            // TODO: bemenet validálása (formátum, hossz, stb.)
            return true;
        }

        private string HashPassword(string password)
        {
            // TODO: jelszó hash-elése
            return password;
        }

        private bool CheckCredentials(string email, string passwordHash)
        {
            // TODO: adatbázisból lekérdezés és összehasonlítás
            return false;
        }
    }
}
