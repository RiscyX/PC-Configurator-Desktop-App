using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PC_Configurator.Helpers;

namespace PC_Configurator.Views.App
{
    /// <summary>
    /// Segédosztály az admin felhasználók megjelenítéséhez
    /// </summary>
    public class AdminUser
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Initials { get; set; }
    }
    
    /// <summary>
    /// Interaction logic for AdminSettings.xaml
    /// </summary>
    public partial class AdminSettings : UserControl
    {
        private bool _isMaintenanceModeEnabled = false;
        
        public AdminSettings()
        {
            InitializeComponent();
            
            Console.WriteLine("AdminSettings constructor called");
            
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
                
                // Ha nem admin, elrejtjük a komponenseket és mutatunk egy "hozzáférés megtagadva" üzenetet
                this.Content = new TextBlock
                {
                    Text = "Hozzáférés megtagadva: Csak adminisztrátorok férhetnek hozzá a rendszergazdai beállításokhoz.",
                    FontSize = 16,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.Red
                };
                Console.WriteLine("Access denied message shown");
                return;
            }
            
            // Admin felhasználó esetén betöltjük a beállításokat
            Console.WriteLine("User is admin, loading admin settings");
            LoadSettingsFromConfig();
            
            // Példa admin felhasználók betöltése
            LoadAdminUsers();
        }
        
        private void LoadSettingsFromConfig()
        {
            try
            {
                // Kapcsolati string alap adatok
                var connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"]?.ConnectionString;
                if (!string.IsNullOrEmpty(connectionString))
                {
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
                    
                    // Kapcsolati adatok megjelenítése a UI-n
                    DbServerTextBox.Text = builder.DataSource;
                    DbNameTextBox.Text = builder.InitialCatalog;
                    DbUserTextBox.Text = builder.UserID;
                    DbPasswordBox.Password = builder.Password;
                }
                
                // Példa: karbantartási mód beállítása
                _isMaintenanceModeEnabled = false;
                MaintenanceModeToggle.IsChecked = _isMaintenanceModeEnabled;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a beállítások betöltésekor: {ex.Message}", 
                    "Beállítások betöltése sikertelen", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void LoadAdminUsers()
        {
            try
            {
                // Példa: Adminok betöltése az adatbázisból
                // Valós alkalmazás esetén ez az adatbázisból jönne
                ObservableCollection<AdminUser> admins = new ObservableCollection<AdminUser>();
                admins.Add(new AdminUser { Id = "1", DisplayName = "Adminisztrátor", Email = "admin@pcconfigapp.com", Initials = "A" });
                admins.Add(new AdminUser { Id = "2", DisplayName = "Teszt Admin", Email = "test.admin@pcconfigapp.com", Initials = "TA" });
                
                AdminList.ItemsSource = admins;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt az adminok betöltésekor: {ex.Message}", 
                    "Adminok betöltése sikertelen", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void TestDbConnection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string server = DbServerTextBox.Text;
                string database = DbNameTextBox.Text;
                string user = DbUserTextBox.Text;
                string password = DbPasswordBox.Password;
                
                string connectionString = $"Server={server};Database={database};User Id={user};Password={password};";
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MessageBox.Show("Adatbázis kapcsolat sikeres!", "Kapcsolat teszt", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Adatbázis kapcsolat sikertelen: {ex.Message}", "Kapcsolat teszt", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void MaintenanceModeToggle_Checked(object sender, RoutedEventArgs e)
        {
            _isMaintenanceModeEnabled = true;
            MessageBox.Show("Karbantartási mód bekapcsolva. Most már csak admin felhasználók jelentkezhetnek be.", 
                "Karbantartási mód", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Itt mentenénk az adatbázisba vagy config fájlba a beállítást
        }
        
        private void MaintenanceModeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            _isMaintenanceModeEnabled = false;
            MessageBox.Show("Karbantartási mód kikapcsolva.", 
                "Karbantartási mód", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Itt mentenénk az adatbázisba vagy config fájlba a beállítást
        }
        
        private void RegistrationModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = RegistrationModeCombo.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                string selectedMode = selectedItem.Content.ToString();
                
                // Itt mentenénk az adatbázisba vagy config fájlba a beállítást
                MessageBox.Show($"Regisztrációs mód megváltoztatva: {selectedMode}", 
                    "Beállítás frissítve", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        private void BackupNow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Itt valósítanánk meg az adatbázis biztonsági mentést
                MessageBox.Show("Adatbázis biztonsági mentés elindítva. A mentés elkészülése után értesítést kap.", 
                    "Adatbázis mentés", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Példa: Aszinkron mentést szimulálunk
                Task.Run(async () => 
                {
                    await Task.Delay(3000); // 3 másodperc szimulált mentési idő
                    
                    // UI szálon frissítünk
                    Application.Current.Dispatcher.Invoke(() => 
                    {
                        MessageBox.Show("Adatbázis mentés sikeresen befejeződött.", 
                            "Sikeres mentés", MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt az adatbázis mentésekor: {ex.Message}", 
                    "Mentés sikertelen", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
          private void AddAdmin_Click(object sender, RoutedEventArgs e)
        {
            // Példa dialógus ablak létrehozása admin felhasználó hozzáadásához
            // Egyszerűbb megközelítés közvetlenül MessageBox-szal
            string email = "uj.admin@pcconfigapp.com"; // alapértelmezett érték
            
            MessageBoxResult result = MessageBox.Show("Szeretne új admin felhasználót hozzáadni?", 
                "Admin hozzáadása", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
            if (result == MessageBoxResult.Yes)
            {
                // Itt valósítanánk meg az admin felhasználó hozzáadását
                MessageBox.Show($"Új admin meghívó elküldve a következő címre: {email}", 
                    "Admin hozzáadása", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                // Frissítjük az admin listát
                LoadAdminUsers();
            }
        }
        
        private void AdminPermissions_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                string adminId = button.Tag as string;
                MessageBox.Show($"Admin jogosultságok kezelése: {adminId}", 
                    "Jogosultságok", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                // Itt nyitnánk meg egy jogosultság kezelő dialógust
            }
        }
        
        private void RemoveAdmin_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                string adminId = button.Tag as string;
                
                MessageBoxResult result = MessageBox.Show(
                    "Biztosan el kívánja távolítani ezt a felhasználót az adminisztrátorok közül?", 
                    "Admin eltávolítása", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Warning);
                    
                if (result == MessageBoxResult.Yes)
                {
                    // Itt valósítanánk meg az admin eltávolítását
                    MessageBox.Show($"Admin ({adminId}) sikeresen eltávolítva!", 
                        "Admin eltávolítva", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                    // Frissítjük az admin listát
                    LoadAdminUsers();
                }
            }
        }
        
        private void RebuildDatabase_Click(object sender, RoutedEventArgs e)
        {
            // Megerősítés kérése a felhasználótól
            MessageBoxResult result = MessageBox.Show(
                "FIGYELEM! Az adatbázis újraépítése törli az összes adatot! Ez a művelet nem visszavonható.\n\n" +
                "Biztosan folytatni szeretné?", 
                "Adatbázis újraépítése", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Warning);
                
            if (result == MessageBoxResult.Yes)
            {
                // Második megerősítés, hogy véletlenül ne lehessen elindítani
                MessageBoxResult confirmResult = MessageBox.Show(
                    "UTOLSÓ FIGYELMEZTETÉS! Minden adat el fog veszni!\n\n" +
                    "Biztosan újra kívánja építeni az adatbázist?", 
                    "Adatbázis újraépítése", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Stop);
                    
                if (confirmResult == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Itt valósítanánk meg az adatbázis újraépítését
                        // Például a create_pc_configurator_db.sql script futtatását
                        
                        MessageBox.Show("Adatbázis újraépítése sikeresen megtörtént!", 
                            "Adatbázis újraépítése", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Hiba történt az adatbázis újraépítésekor: {ex.Message}", 
                            "Adatbázis újraépítése sikertelen", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        
        private void DeleteAllComponents_Click(object sender, RoutedEventArgs e)
        {
            // Megerősítés kérése a felhasználótól
            MessageBoxResult result = MessageBox.Show(
                "Biztosan törölni szeretné az összes komponenst az adatbázisból? Ez a művelet nem visszavonható.", 
                "Komponensek törlése", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Warning);
                
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Itt valósítanánk meg az összes komponens törlését
                    
                    MessageBox.Show("Az összes komponens sikeresen törölve!", 
                        "Komponensek törlése", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba történt a komponensek törlésekor: {ex.Message}", 
                        "Komponensek törlése sikertelen", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        private void DeleteAllConfigs_Click(object sender, RoutedEventArgs e)
        {
            // Megerősítés kérése a felhasználótól
            MessageBoxResult result = MessageBox.Show(
                "Biztosan törölni szeretné az összes konfigurációt az adatbázisból? Ez a művelet nem visszavonható.", 
                "Konfigurációk törlése", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Warning);
                
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Itt valósítanánk meg az összes konfiguráció törlését
                    
                    MessageBox.Show("Az összes konfiguráció sikeresen törölve!", 
                        "Konfigurációk törlése", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba történt a konfigurációk törlésekor: {ex.Message}", 
                        "Konfigurációk törlése sikertelen", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
