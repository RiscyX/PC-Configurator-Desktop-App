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
using PC_Configurator.Models;

namespace PC_Configurator.Views.App
{
    /// <summary>
    /// Class to display users in the UI
    /// </summary>
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Initials { get; set; }
        public DateTime Created { get; set; }
        public string CreatedString => Created.ToString("yyyy.MM.dd");
        public DateTime? LastLogin { get; set; }
        public string LastLoginString => LastLogin.HasValue ? LastLogin.Value.ToString("yyyy.MM.dd HH:mm") : "Soha";
        public bool IsAdmin => Role?.ToLower() == "admin";
    }

    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : UserControl
    {
        private readonly string _connStr;
        private ObservableCollection<UserViewModel> _users;
        
        public Users()
        {
            InitializeComponent();
            
            _connStr = ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            _users = new ObservableCollection<UserViewModel>();
            
            Console.WriteLine("Users constructor called");
            
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
                    Text = "Hozzáférés megtagadva: Csak adminisztrátorok férhetnek hozzá a felhasználók kezeléséhez.",
                    FontSize = 16,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.Red
                };
                Console.WriteLine("Access denied message shown");
                return;
            }
            
            // If admin, load users
            Console.WriteLine("User is admin, loading users data");
            LoadUsers();
        }
        
        private void LoadUsers()
        {
            try
            {
                _users.Clear();
                
                using (var conn = new SqlConnection(_connStr))
                {
                    conn.Open();
                    var cmd = new SqlCommand(
                        "SELECT Id, Email, FirstName, LastName, Role, Created, LastLogin " +
                        "FROM Users ORDER BY Email", conn);
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string firstName = reader.IsDBNull(2) ? "" : reader.GetString(2);
                            string lastName = reader.IsDBNull(3) ? "" : reader.GetString(3);
                            string fullName = string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName) 
                                ? "N/A" 
                                : $"{firstName} {lastName}".Trim();
                                
                            string initials = (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName)) 
                                ? $"{firstName[0]}{lastName[0]}" 
                                : "?";
                            
                            _users.Add(new UserViewModel
                            {
                                Id = reader.GetInt32(0),
                                Email = reader.GetString(1),
                                FullName = fullName,
                                Role = reader.IsDBNull(4) ? "user" : reader.GetString(4),
                                Initials = initials,
                                Created = reader.IsDBNull(5) ? DateTime.Now : reader.GetDateTime(5),
                                LastLogin = reader.IsDBNull(6) ? null : (DateTime?)reader.GetDateTime(6)
                            });
                        }
                    }
                }
                
                UsersListView.ItemsSource = _users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a felhasználók betöltésekor: {ex.Message}", 
                    "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void ChangeRole_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is UserViewModel user)
            {
                string newRole = user.IsAdmin ? "user" : "admin";
                
                MessageBoxResult result = MessageBox.Show(
                    $"Biztosan megváltoztatja {user.Email} felhasználó jogosultságát?\n\n" +
                    $"Jelenlegi: {user.Role}\n" +
                    $"Új: {newRole}", 
                    "Jogosultság módosítása", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var conn = new SqlConnection(_connStr))
                        {
                            conn.Open();
                            var cmd = new SqlCommand("UPDATE Users SET Role = @role WHERE Id = @id", conn);
                            cmd.Parameters.AddWithValue("@role", newRole);
                            cmd.Parameters.AddWithValue("@id", user.Id);
                            
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Jogosultság sikeresen frissítve!", "Siker", 
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                                
                                // Reload users to reflect changes
                                LoadUsers();
                            }
                            else
                            {
                                MessageBox.Show("Nem sikerült frissíteni a jogosultságot.", "Hiba", 
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Hiba történt a jogosultság módosításakor: {ex.Message}", 
                            "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        
        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is UserViewModel user)
            {
                // Prevent deleting your own account
                if (user.Email == PermissionManager.CurrentUser?.Email)
                {
                    MessageBox.Show("Nem tudja törölni a saját fiókját!", 
                        "Művelet megtagadva", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // Confirm deletion
                MessageBoxResult result = MessageBox.Show(
                    $"Biztosan törölni szeretné ezt a felhasználót?\n\n" +
                    $"Email: {user.Email}\n" +
                    $"Név: {user.FullName}\n\n" +
                    "Ez a művelet nem visszavonható!", 
                    "Felhasználó törlése", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Warning);
                
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var conn = new SqlConnection(_connStr))
                        {
                            conn.Open();
                            var cmd = new SqlCommand("DELETE FROM Users WHERE Id = @id", conn);
                            cmd.Parameters.AddWithValue("@id", user.Id);
                            
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Felhasználó sikeresen törölve!", "Siker", 
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                                
                                // Reload users to reflect changes
                                LoadUsers();
                            }
                            else
                            {
                                MessageBox.Show("Nem sikerült törölni a felhasználót.", "Hiba", 
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Hiba történt a felhasználó törlésekor: {ex.Message}", 
                            "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }
    }
}
