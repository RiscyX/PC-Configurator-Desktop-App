using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PC_Configurator.Views.User
{
    public partial class LoginWindow : Window
    {
        private readonly string _connStr;

        public LoginWindow()
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
        }        private void ShowPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Jelszó megjelenítése
            PasswordVisibleBox.Text = PasswordBox.Password;
            PasswordBox.Visibility = Visibility.Collapsed;
            PasswordVisibleBox.Visibility = Visibility.Visible;
            PasswordVisibleBox.Focus();
            PasswordVisibleBox.SelectionStart = PasswordVisibleBox.Text.Length;
            
            // Debug üzenet a beírt szöveg ellenőrzéséhez
            Console.WriteLine($"Jelszó: {PasswordVisibleBox.Text}");
        }

        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Jelszó elrejtése
            PasswordBox.Password = PasswordVisibleBox.Text;
            PasswordVisibleBox.Visibility = Visibility.Collapsed;
            PasswordBox.Visibility = Visibility.Visible;
            PasswordBox.Focus();
        }private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = ShowPasswordCheckBox.IsChecked == true ? PasswordVisibleBox.Text : PasswordBox.Password;

            if (!ValidateInput(email, password))
                return;

            string passwordHash = HashPassword(password);            if (CheckCredentials(email, passwordHash))
            {
                string role = GetUserRole(email);
                Console.WriteLine($"User login successful: {email}, Role: {role}");
                
                // Set current user in PermissionManager
                if (PC_Configurator.Helpers.PermissionManager.SetCurrentUser(email))
                {
                    Console.WriteLine("PermissionManager.CurrentUser set successfully");
                }
                else
                {
                    // Ha a PermissionManager nem tudta beállítani a felhasználót, közvetlenül állítsuk be a szerepkört
                    Console.WriteLine("Setting CurrentUser failed, using direct role setting");
                    PC_Configurator.Models.Users user = new PC_Configurator.Models.Users
                    {
                        Email = email,
                        Role = role
                    };
                    PC_Configurator.Helpers.PermissionManager.CurrentUser = user;
                }
                
                var dashboard = new App.Dashboard(email, role);
                dashboard.WindowState = WindowState.Maximized;
                this.Close();
                dashboard.ShowDialog();
            }
            else
            {
                MessageBox.Show("Hibás email vagy jelszó");
            }
        }

        private string GetUserRole(string email)
        {
            try
            {
                using (var conn = new SqlConnection(_connStr))
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT Role FROM Users WHERE Email = @e", conn);
                    cmd.Parameters.AddWithValue("@e", email);
                    var result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "user";
                }
            }
            catch
            {
                return "user";
            }
        }

        private bool ValidateInput(string email, string password)
        {
            // TODO: Add real validation
            return !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password);
        }

        private string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }

        private bool CheckCredentials(string email, string passwordHash)
        {
            try
            {
                using (var conn = new SqlConnection(_connStr))
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Email = @e AND PasswordHash = @p", conn);
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@p", passwordHash);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void GoToRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var regWin = new RegistrationWindow();
            regWin.Owner = this.Owner;
            regWin.Show();
            this.Close();
        }

        private void EmailTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }
    }
}
