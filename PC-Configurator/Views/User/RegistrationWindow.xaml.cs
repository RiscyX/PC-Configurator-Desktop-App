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
    /// <summary>
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }
        private void ShowPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // TODO: jelszó megjelenítése
        }

        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // TODO: jelszó elrejtése
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (ValidateInput(email, password, confirmPassword))
            {
                string passwordHash = HashPassword(password);
                bool success = RegisterUser(email, passwordHash);

                if (success)
                {
                    MessageBox.Show("Sikeres regisztráció");
                    // TODO: Továbblépés vagy ablak bezárása
                }
                else
                {
                    MessageBox.Show("A regisztráció sikertelen. Az email már használatban van?");
                }
            }
        }

        private bool ValidateInput(string email, string password, string confirmPassword)
        {
            // TODO: validációs logika (pl. jelszó megegyezés, email formátum)
            return true;
        }

        private string HashPassword(string password)
        {
            // TODO: hash készítése a jelszóból
            return password;
        }

        private bool RegisterUser(string email, string passwordHash)
        {
            // TODO: új felhasználó beszúrása az adatbázisba
            return true;
        }
    }
}
