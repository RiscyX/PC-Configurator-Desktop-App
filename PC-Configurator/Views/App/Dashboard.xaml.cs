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

using PC_Configurator.Views.App;
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
            AddMenuItems(userRole);
            // Set Profile as the default page
            MainContentArea.Content = new PC_Configurator.Views.App.Profile(userEmail, userRole);
            // Store for navigation if needed
            this.UserEmail = userEmail;
            this.UserRole = userRole;
        }

        private string UserEmail;
        private string UserRole;


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

            // Admin-only menu items
            if (userRole == "admin")
            {
                SidebarMenu.Children.Add(CreateNavButton("Alkatrészek hozzáadása", typeof(PC_Configurator.Views.App.AddComponents)));
                SidebarMenu.Children.Add(CreateNavButton("Admin beállítások", typeof(PC_Configurator.Views.App.AdminSettings)));
            }
            // Common menu items
            SidebarMenu.Children.Add(CreateNavButton("Gépépítés", typeof(PC_Configurator.Views.App.ConfigBuilder)));
            SidebarMenu.Children.Add(CreateNavButton("Konfigurációk", typeof(PC_Configurator.Views.App.Configs)));
            SidebarMenu.Children.Add(CreateNavButton("Fiókom", typeof(PC_Configurator.Views.App.Profile)));

            // Logout button (always visible)
            var logoutBtn = CreateSidebarButton("Kijelentkezés");
            logoutBtn.Click += LogoutButton_Click;
            SidebarMenu.Children.Add(logoutBtn);
        }

        private Button CreateNavButton(string text, System.Type userControlType)
        {
            var btn = new Button
            {
                Content = text,
                Style = (Style)FindResource("SidebarButton"),
                Margin = new Thickness(0, 8, 0, 0)
            };
            btn.Click += (s, e) =>
            {
                if (userControlType == typeof(PC_Configurator.Views.App.Profile))
                {
                    var profile = new PC_Configurator.Views.App.Profile(this.UserEmail, this.UserRole);
                    MainContentArea.Content = profile;
                }
                else
                {
                    var control = (UserControl)Activator.CreateInstance(userControlType);
                    MainContentArea.Content = control;
                }
            };
            return btn;
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


        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var login = new PC_Configurator.Views.LoginWindow();
            login.Show();
            this.Close();
        }
    }
}
