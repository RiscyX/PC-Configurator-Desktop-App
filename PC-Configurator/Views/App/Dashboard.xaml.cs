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
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {



        public Dashboard(string userEmail, string userRole)
        {
            InitializeComponent();
            EmailTextBlock.Text = userEmail;
            RoleTextBlock.Text = $"Szerepkör: {userRole}";
            AddMenuItems(userRole);
        }

        private void AddMenuItems(string userRole)
        {
            SidebarMenu.Children.Clear();
            var title = new TextBlock {
                Text = "PC Konfigurátor",
                FontSize = 22,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A90E2")),
                Margin = new Thickness(0,0,0,32),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            SidebarMenu.Children.Add(title);

            // Common menu items
            SidebarMenu.Children.Add(CreateSidebarButton("Gépépítés"));
            SidebarMenu.Children.Add(CreateSidebarButton("Konfigurációk"));
            SidebarMenu.Children.Add(CreateSidebarButton("Fiókom"));

            // Admin-only menu items
            if (userRole == "admin")
            {
                SidebarMenu.Children.Add(CreateSidebarButton("Felhasználók kezelése"));
                SidebarMenu.Children.Add(CreateSidebarButton("Admin beállítások"));
            }

            // Logout button (always visible)
            var logoutBtn = CreateSidebarButton("Kijelentkezés");
            logoutBtn.Click += LogoutButton_Click;
            SidebarMenu.Children.Add(logoutBtn);
        }

        private Button CreateSidebarButton(string text)
        {
            return new Button
            {
                Content = text,
                Style = (Style)FindResource("SidebarButton"),
                Margin = new Thickness(0, 8, 0, 0)
            };
        }

        public Dashboard(string userEmail) : this(userEmail, "user")
        {
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var login = new PC_Configurator.Views.LoginWindow();
            login.Show();
            this.Close();
        }
    }
}
