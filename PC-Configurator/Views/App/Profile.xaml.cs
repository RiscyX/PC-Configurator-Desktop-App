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

namespace PC_Configurator.Views.App
{
    /// <summary>
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class Profile : UserControl
    {
        private string email;
        private string role;
        
        public Profile(string email = "", string role = "")
        {
            InitializeComponent();
            
            this.email = email;
            this.role = role;
            
            // Set user information
            EmailTextBlock.Text = email;
            RoleTextBlock.Text = TranslateRole(role);
            
            // Set initials based on email
            SetUserInitials(email);
            
            // Set username from email
            UsernameBlock.Text = ExtractUsernameFromEmail(email);
            
            // Set last login to current date/time for demo
            LastLoginTextBlock.Text = DateTime.Now.ToString("yyyy. MMMM dd. HH:mm");
            
            // Set saved configs count (mock data)
            SavedConfigsTextBlock.Text = "3 konfiguráció";
        }
        
        private string TranslateRole(string role)
        {
            if (string.IsNullOrEmpty(role)) return "Felhasználó";
            
            switch (role.ToLower())
            {
                case "admin":
                    return "Rendszergazda";
                case "user":
                    return "Felhasználó";
                default:
                    return role;
            }
        }
        
        private void SetUserInitials(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                UserInitials.Text = "?";
                return;
            }
            
            string username = ExtractUsernameFromEmail(email);
            if (!string.IsNullOrEmpty(username) && username.Length > 0)
            {
                if (username.Length >= 2)
                {
                    UserInitials.Text = username.Substring(0, 2).ToUpper();
                }
                else
                {
                    UserInitials.Text = username.Substring(0, 1).ToUpper();
                }
            }
            else
            {
                UserInitials.Text = "?";
            }
        }
        
        private string ExtractUsernameFromEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return string.Empty;
            
            int atIndex = email.IndexOf('@');
            if (atIndex > 0)
            {
                return email.Substring(0, atIndex);
            }
            
            return email;
        }
        
        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "A jelszóváltoztatás funkció jelenleg fejlesztés alatt áll.", 
                "Értesítés", 
                MessageBoxButton.OK, 
                MessageBoxImage.Information
            );
        }
        
        private void ViewConfigsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Navigate to Configs page
                var mainWindow = Window.GetWindow(this) as Dashboard;
                if (mainWindow != null)
                {
                    var configsPage = new PC_Configurator.Views.App.Configs();
                    mainWindow.MainContentArea.Content = configsPage;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Hiba történt a navigáció során: {ex.Message}", 
                    "Hiba", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error
                );
            }
        }
    }
}
