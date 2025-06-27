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
using PC_Configurator.Helpers;

namespace PC_Configurator.Views.App
{
    /// <summary>
    /// Interaction logic for AddComponents.xaml
    /// </summary>
    public partial class AddComponents : UserControl
    {
        public AddComponents()
        {
            // Inicializáljuk a komponenseket
            InitializeComponent();
            
            Console.WriteLine("AddComponents constructor called");
            
            // Közvetlenül ellenőrizzük, hogy admin-e, még a PermissionManager előtt
            bool isAdmin = false;
            
            // Ellenőrizzük a PermissionManager-ben
            if (PermissionManager.CurrentUser != null) 
            {
                isAdmin = PermissionManager.IsCurrentUserAdmin();
                Console.WriteLine($"Direct check: CurrentUser={PermissionManager.CurrentUser.Email}, Role={PermissionManager.CurrentUser.Role}, IsAdmin={isAdmin}");
            }
            else
            {
                Console.WriteLine("PermissionManager.CurrentUser is null, checking Dashboard.UserRole");
                
                // Ha a PermissionManager nem segít, próbáljuk meg a Dashboard.UserRole-t ellenőrizni
                Window parentWindow = Window.GetWindow(this);
                if (parentWindow != null && parentWindow is Dashboard dashboard)
                {
                    var dashboardType = dashboard.GetType();
                    var userRoleField = dashboardType.GetField("UserRole", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (userRoleField != null)
                    {
                        string userRole = userRoleField.GetValue(dashboard) as string;
                        Console.WriteLine($"Dashboard.UserRole: {userRole}");
                        isAdmin = userRole?.ToLower() == "admin";
                    }
                }
            }
            
            // Ha nem admin, akkor is ellenőrizzük a PermissionManager-rel, de csak a logolás miatt
            if (!isAdmin)
            {
                PermissionManager.ApplyPermissionCheck(this, true);
                
                // Ha nem admin, elrejtjük a komponenseket
                FormHost.Visibility = Visibility.Collapsed;
                
                // Mutatunk egy "hozzáférés megtagadva" üzenetet
                var textBlock = new TextBlock
                {
                    Text = "Hozzáférés megtagadva: Csak adminisztrátorok férhetnek hozzá az alkatrészek hozzáadásához.",
                    FontSize = 16,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.Red
                };
                
                this.Content = textBlock;
                Console.WriteLine("Access denied message shown");
            }
            else
            {
                Console.WriteLine("User is admin, showing AddComponents page");
            }
        }

        private void AddCpuButton_Click(object sender, RoutedEventArgs e)
        {
            FormHost.Content = new PC_Configurator.Views.Forms.CPU();
        }

        private void AddGpuButton_Click(object sender, RoutedEventArgs e)
        {
            FormHost.Content = new PC_Configurator.Views.Forms.GPU();
        }

        private void AddMotherboardButton_Click(object sender, RoutedEventArgs e)
        {
            FormHost.Content = new PC_Configurator.Views.Forms.Motherboard();
        }

        private void AddRamButton_Click(object sender, RoutedEventArgs e)
        {
            FormHost.Content = new PC_Configurator.Views.Forms.RAM();
        }

        private void AddStorageButton_Click(object sender, RoutedEventArgs e)
        {
            FormHost.Content = new PC_Configurator.Views.Forms.Storage();
        }

        private void AddPsuButton_Click(object sender, RoutedEventArgs e)
        {
            FormHost.Content = new PC_Configurator.Views.Forms.PSU();
        }

        private void AddCaseButton_Click(object sender, RoutedEventArgs e)
        {
            FormHost.Content = new PC_Configurator.Views.Forms.Case();
        }
    }
}
