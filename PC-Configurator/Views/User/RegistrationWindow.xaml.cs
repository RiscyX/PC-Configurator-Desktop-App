using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace PC_Configurator.Views.User
{
    public partial class RegistrationWindow : Window
    {
        private readonly string _connStr;

        public RegistrationWindow()
        {
            InitializeComponent();
            _connStr = ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            
            // Beállítjuk az ablak kezdeti fókuszát
            Loaded += (s, e) => EmailTextBox.Focus();
        }
        
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Lehetővé teszi az ablak mozgatását
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }        // Reveal password by swapping visibility between PasswordBox and overlay TextBox
        private void ShowPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Text = PasswordBox.Password;
            PasswordTextBox.Visibility = Visibility.Visible;
            PasswordBox.Visibility = Visibility.Collapsed;
            PasswordTextBox.Focus();
            PasswordTextBox.SelectionStart = PasswordTextBox.Text.Length;

            ConfirmPasswordTextBox.Text = ConfirmPasswordBox.Password;
            ConfirmPasswordTextBox.Visibility = Visibility.Visible;
            ConfirmPasswordBox.Visibility = Visibility.Collapsed;
            
            // Debug üzenet a beírt szöveg ellenőrzéséhez
            Console.WriteLine($"Jelszó: {PasswordTextBox.Text}, Megerősítés: {ConfirmPasswordTextBox.Text}");
        }

        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = PasswordTextBox.Text;
            PasswordBox.Visibility = Visibility.Visible;
            PasswordTextBox.Visibility = Visibility.Collapsed;

            ConfirmPasswordBox.Password = ConfirmPasswordTextBox.Text;
            ConfirmPasswordBox.Visibility = Visibility.Visible;
            ConfirmPasswordTextBox.Visibility = Visibility.Collapsed;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = ShowPasswordCheckBox.IsChecked == true ? PasswordTextBox.Text : PasswordBox.Password;
            string confirmPassword = ShowPasswordCheckBox.IsChecked == true ? ConfirmPasswordTextBox.Text : ConfirmPasswordBox.Password;

            if (!ValidateInput(email, password, confirmPassword))
                return;

            string passwordHash = HashPassword(password);
            bool success = RegisterUser(email, passwordHash);

            if (success)
            {
                MessageBox.Show("Sikeres regisztráció! Kérlek jelentkezz be.", "Regisztráció", MessageBoxButton.OK, MessageBoxImage.Information);
                var loginWin = new LoginWindow();
                
                // Módosítás: A Show() helyett ShowDialog() használata
                // És a Close() metódus hívása előtt
                loginWin.Owner = this.Owner;
                this.Close();
                loginWin.ShowDialog();
            }
            else
            {
                MessageBox.Show("A regisztráció sikertelen. Az email már használatban van?", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private bool ValidateInput(string email, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Kérlek töltsd ki az emailt és a jelszót!", "Hiányzó adatok", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!email.Contains("@"))
            {
                MessageBox.Show("Érvénytelen email formátum!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (password != confirmPassword)
            {
                MessageBox.Show("A jelszavak nem egyeznek!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (password.Length < 6)
            {
                MessageBox.Show("A jelszó legalább 6 karakter legyen!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }

        private bool RegisterUser(string email, string passwordHash)
        {
            try
            {
                using (var conn = new SqlConnection(_connStr))
                {
                    conn.Open();
                    var cmd = new SqlCommand(
                        "INSERT INTO Users (Email, PasswordHash, Role) VALUES (@e, @p, 'user')", conn);
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@p", passwordHash);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                // Unique constraint violation
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void BackToLoginButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWin = new LoginWindow();
            loginWin.Owner = this.Owner;
            this.Close();
            loginWin.ShowDialog();
        }
    }
}
