using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using PC_Configurator.Helpers;
namespace PC_Configurator.Views.App
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        private string UserEmail;
        private string UserRole;
        private Button _activeButton = null;
        private Dictionary<Type, string> _pageIcons = new Dictionary<Type, string>();
        private Dictionary<Type, string> _pageTitles = new Dictionary<Type, string>();

        public Dashboard(string userEmail, string userRole)
        {
            InitializeComponent();
            
            // Initialize page icons and titles
            InitializePageMetadata();
            
            // Set user info
            UserEmail = userEmail;
            UserRole = userRole;
            UserEmailText.Text = userEmail;
            UserRoleText.Text = userRole;
            
            // Create menu items based on user role
            AddMenuItems(userRole);
            
            // Set Configs as the default page instead of Profile (which was removed)
            SetActivePage(typeof(PC_Configurator.Views.App.Configs));
        }
        
        // Initialize page icons and titles dictionary
        private void InitializePageMetadata()
        {
            // Icons (Segoe MDL2 Assets)
            _pageIcons.Add(typeof(PC_Configurator.Views.App.AddComponents), "\uE710"); // Add
            _pageIcons.Add(typeof(PC_Configurator.Views.App.Components), "\uE7F4");    // Components
            _pageIcons.Add(typeof(PC_Configurator.Views.App.ConfigBuilder), "\uE90F"); // Build
            _pageIcons.Add(typeof(PC_Configurator.Views.App.Configs), "\uE8B7");      // List
            _pageIcons.Add(typeof(PC_Configurator.Views.App.Profile), "\uE77B");      // Profile
            _pageIcons.Add(typeof(PC_Configurator.Views.App.AdminSettings), "\uE713"); // Settings
            _pageIcons.Add(typeof(PC_Configurator.Views.App.Users), "\uE716");        // Users
            
            // Page Titles
            _pageTitles.Add(typeof(PC_Configurator.Views.App.AddComponents), "Alkatrészek hozzáadása");
            _pageTitles.Add(typeof(PC_Configurator.Views.App.Components), "Alkatrészek");
            _pageTitles.Add(typeof(PC_Configurator.Views.App.ConfigBuilder), "Gépépítés");
            _pageTitles.Add(typeof(PC_Configurator.Views.App.Configs), "Konfigurációk");
            _pageTitles.Add(typeof(PC_Configurator.Views.App.Profile), "Felhasználói fiók");
            _pageTitles.Add(typeof(PC_Configurator.Views.App.AdminSettings), "Rendszergazdai beállítások");
            _pageTitles.Add(typeof(PC_Configurator.Views.App.Users), "Felhasználók kezelése");
        }        private void AddMenuItems(string userRole)
        {
            SidebarMenu.Children.Clear();
            
            // Separator
            SidebarMenu.Children.Add(new Separator { 
                Margin = new Thickness(20, 0, 20, 10),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"))
            });
            
            // Add category label
            AddCategoryLabel("Főmenü");

            // Common menu items
            SidebarMenu.Children.Add(CreateNavButton("Alkatrészek", typeof(PC_Configurator.Views.App.Components), "\uE7F4"));
            SidebarMenu.Children.Add(CreateNavButton("Gépépítés", typeof(PC_Configurator.Views.App.ConfigBuilder), "\uE90F"));
            SidebarMenu.Children.Add(CreateNavButton("Konfigurációk", typeof(PC_Configurator.Views.App.Configs), "\uE8B7"));
            // A "Fiókom" menüpont el lett távolítva a felhasználó kérésére
            
            // Admin-only menu items
            bool isAdmin = userRole.ToLower() == "admin" || PermissionManager.IsCurrentUserAdmin();
            Console.WriteLine($"Dashboard menu check: UserRole={userRole}, IsAdmin from role={userRole.ToLower() == "admin"}, IsAdmin from PermissionManager={PermissionManager.IsCurrentUserAdmin()}, Final IsAdmin={isAdmin}");
            
            if (isAdmin)
            {
                // Add admin category label
                AddCategoryLabel("Adminisztráció");
                
                SidebarMenu.Children.Add(CreateNavButton("Alkatrészek hozzáadása", typeof(PC_Configurator.Views.App.AddComponents), "\uE710"));
                // Rendszergazdai beállítások és felhasználó kezelés el lett távolítva a felhasználó kérésére
            }

            // Add bottom section with separator
            SidebarMenu.Children.Add(new Separator {
                Margin = new Thickness(20, 20, 20, 10),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"))
            });

            // Logout button (always visible)
            var logoutBtn = new Button
            {
                Content = "Kijelentkezés",
                Style = (Style)FindResource("SidebarButton"),
                Margin = new Thickness(0, 0, 0, 0)
            };
            
            // Set logout icon
            logoutBtn.Template = (ControlTemplate)logoutBtn.Style.Setters
                .Cast<Setter>()
                .First(s => s.Property.Name == "Template")
                .Value;
            
            TextBlock iconBlock = logoutBtn.Template.FindName("IconBlock", logoutBtn) as TextBlock;
            if (iconBlock != null)
            {
                iconBlock.Text = "\uE7E8"; // Logout icon
            }
            
            logoutBtn.Click += LogoutButton_Click;
            SidebarMenu.Children.Add(logoutBtn);
        }

        private void AddCategoryLabel(string text)
        {
            var label = new TextBlock
            {
                Text = text.ToUpper(),
                FontSize = 11,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#777777")),
                Margin = new Thickness(20, 10, 0, 8)
            };
            SidebarMenu.Children.Add(label);
        }        private Button CreateNavButton(string text, Type userControlType, string icon)
        {
            var btn = new Button
            {
                Content = text,
                Style = (Style)FindResource("SidebarButton"),
                Tag = userControlType
            };
            
            // Set icon if template has IconBlock
            btn.Template = (ControlTemplate)btn.Style.Setters
                .Cast<Setter>()
                .First(s => s.Property.Name == "Template")
                .Value;
            
            TextBlock iconBlock = btn.Template.FindName("IconBlock", btn) as TextBlock;
            if (iconBlock != null)
            {
                iconBlock.Text = icon;
            }
            
            btn.Click += (s, e) => {
                SetActivePage(userControlType);
            };
            
            return btn;
        }        public void SetActivePage(Type userControlType)
        {
            try
            {
                Console.WriteLine($"SetActivePage: {userControlType.Name}");
                
                // Admin oldalak ellenőrzése navigáció előtt
                bool isAdminPage = 
                    userControlType == typeof(PC_Configurator.Views.App.AddComponents); 
                // AdminSettings és Users oldalak el lettek távolítva a menüből a felhasználó kérésére
                
                bool isAdmin = UserRole.ToLower() == "admin" || PermissionManager.IsCurrentUserAdmin();
                Console.WriteLine($"Navigation check: IsAdminPage={isAdminPage}, UserRole={UserRole}, IsAdmin={isAdmin}");
                
                // Ha admin oldalt próbálunk megnyitni, de nincs jogosultság, átirányítunk a Konfigurációk oldalra
                if (isAdminPage && !isAdmin)
                {
                    Console.WriteLine("Access denied to admin page, redirecting to Configs");
                    MessageBox.Show(
                        "Nincs jogosultsága ehhez az oldalhoz! Ez az oldal csak rendszergazdák számára érhető el.",
                        "Hozzáférés megtagadva",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    
                    userControlType = typeof(PC_Configurator.Views.App.Configs);
                }
                
                // Ha a Profile oldalt próbáljuk megnyitni, irányítsuk át a Konfigurációk oldalra
                if (userControlType == typeof(PC_Configurator.Views.App.Profile))
                {
                    userControlType = typeof(PC_Configurator.Views.App.Configs);
                }
                
                // Create appropriate instance based on type
                object control = null;
                if (userControlType == typeof(PC_Configurator.Views.App.ConfigBuilder))
                {
                    // Lekérdezzük a felhasználó ID-t az adatbázisból
                    int userId = GetUserIdFromEmail(UserEmail);
                    System.Diagnostics.Debug.WriteLine($"ConfigBuilder létrehozása UserId={userId} értékkel");
                    control = new PC_Configurator.Views.App.ConfigBuilder(userId);
                }
                else
                {
                    // Ha a PermissionManager.CurrentUser nincs beállítva, állítsuk be itt
                    if (isAdmin && PermissionManager.CurrentUser == null)
                    {
                        Console.WriteLine("Setting PermissionManager.CurrentUser from Dashboard");
                        PermissionManager.CurrentUser = new PC_Configurator.Models.Users 
                        { 
                            Email = UserEmail, 
                            Role = UserRole
                        };
                    }
                    
                    control = Activator.CreateInstance(userControlType);
                }

                // Update UI
                MainContentArea.Content = control;
                
                // Update page title
                if (_pageTitles.ContainsKey(userControlType))
                {
                    PageTitle.Text = _pageTitles[userControlType];
                }
                
                // Update active button style
                UpdateActiveButton(userControlType);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt az oldal betöltése közben: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }        // Felhasználói azonosító lekérdezése email alapján
        private int GetUserIdFromEmail(string email)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT Id FROM Users WHERE Email = @Email", connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        var result = command.ExecuteScalar();
                        
                        if (result != null && result != DBNull.Value)
                        {
                            int userId = Convert.ToInt32(result);
                            System.Diagnostics.Debug.WriteLine($"Felhasználó ID lekérdezve: {userId} a(z) {email} e-mail címhez");
                            return userId;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Nem található felhasználó a(z) {email} e-mail címmel");
                            return 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a felhasználói ID lekérdezése közben: {ex.Message}");
                return 0;
            }
        }
        
        private void UpdateActiveButton(Type userControlType)
        {
            // Reset all buttons to default style
            foreach (var child in SidebarMenu.Children)
            {
                if (child is Button button)
                {
                    button.Style = (Style)FindResource("SidebarButton");
                }
            }
            
            // Find and set the active button
            foreach (var child in SidebarMenu.Children)
            {
                if (child is Button button && button.Tag is Type tagType)
                {
                    if (tagType == userControlType)
                    {
                        button.Style = (Style)FindResource("ActiveSidebarButton");
                        break;
                    }
                }
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Biztosan ki szeretne jelentkezni?", 
                "Kijelentkezés megerősítése", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);
                
            if (result == MessageBoxResult.Yes)
            {
                var login = new PC_Configurator.Views.User.LoginWindow();
                login.Show();
                this.Close();
            }
        }
        
        // Window control buttons
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
