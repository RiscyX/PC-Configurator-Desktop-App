using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using PC_Configurator.Models;

namespace PC_Configurator.Helpers
{
    /// <summary>
    /// Helper class for managing permissions in the application
    /// </summary>
    public static class PermissionManager
    {
        // Constants for permission types
        public const string ADMIN_ROLE = "admin";
        public const string USER_ROLE = "user";

        // Store the current user details (can be accessed throughout the application)
        private static Users _currentUser;
        public static Users CurrentUser 
        { 
            get { return _currentUser; } 
            set { _currentUser = value; }
        }

        /// <summary>
        /// Set the current user based on email
        /// </summary>
        /// <param name="email">User's email</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool SetCurrentUser(string email)
        {
            try
            {
                _currentUser = Users.GetUserByEmail(email);
                if (_currentUser != null)
                {
                    Console.WriteLine($"Current user set: {_currentUser.Email}, Role: {_currentUser.Role}, IsAdmin: {_currentUser.IsAdmin}");
                    return true;
                }
                Console.WriteLine("Failed to set current user: User not found");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting current user: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Check if the current user is an admin
        /// </summary>
        /// <returns>True if user is admin, false otherwise</returns>
        public static bool IsCurrentUserAdmin()
        {
            bool isAdmin = _currentUser != null && _currentUser.IsAdmin;
            Console.WriteLine($"IsCurrentUserAdmin check: {isAdmin}, CurrentUser: {_currentUser?.Email ?? "null"}, Role: {_currentUser?.Role ?? "null"}");
            
            // Fallback közvetlen adatbázis-ellenőrzés, ha a felhasználó objektum nem tartalmaz jogosultságot
            if (_currentUser != null && string.IsNullOrEmpty(_currentUser.Role))
            {
                isAdmin = IsEmailAdmin(_currentUser.Email);
                Console.WriteLine($"Fallback admin check: {isAdmin}");
            }
            
            return isAdmin;
        }

        /// <summary>
        /// Check if the given email is associated with an admin account
        /// </summary>
        /// <param name="email">User email to check</param>
        /// <returns>True if admin, false if not or if error occurs</returns>
        public static bool IsEmailAdmin(string email)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT Role FROM Users WHERE Email = @e", conn);
                    cmd.Parameters.AddWithValue("@e", email);
                    var result = cmd.ExecuteScalar()?.ToString() ?? "";
                    return result.ToLower() == ADMIN_ROLE;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking admin: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Check if current user has admin permissions and show error if not
        /// </summary>
        /// <returns>True if user has admin access, false otherwise</returns>
        public static bool EnsureAdminAccess()
        {
            if (!IsCurrentUserAdmin())
            {
                MessageBox.Show(
                    "Nincs jogosultsága ehhez a művelethez! Ez a funkció csak rendszergazdák számára érhető el.",
                    "Hozzáférés megtagadva",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return false;
            }
            return true;
        }

        /// <summary>
        /// Apply permission checks to a UserControl and redirect to Dashboard if access denied
        /// </summary>
        /// <param name="control">The UserControl to check permissions for</param>
        /// <param name="requireAdmin">Whether admin rights are required</param>
        /// <returns>True if access is allowed, false otherwise</returns>
        public static bool ApplyPermissionCheck(UserControl control, bool requireAdmin)
        {
            // Logolás a könnyebb hibakereséshez
            Console.WriteLine($"ApplyPermissionCheck: control={control?.GetType().Name}, requireAdmin={requireAdmin}");
            Console.WriteLine($"CurrentUser: {(_currentUser != null ? $"{_currentUser.Email}, Role={_currentUser.Role}" : "null")}");
                
            // Ha nincs szükség admin jogra, akkor mindenképp engedélyezzük
            if (!requireAdmin)
            {
                Console.WriteLine("Admin rights not required, access granted.");
                return true;
            }
            
            // Első körben ellenőrizzük a PermissionManager beállításait
            bool isAdmin = IsCurrentUserAdmin();
            Console.WriteLine($"IsCurrentUserAdmin result: {isAdmin}");
            
            // Ha a PermissionManager szerint admin, engedélyezzük
            if (isAdmin)
            {
                Console.WriteLine("User is admin, access granted.");
                return true;
            }
            
            // Második körben ellenőrizzük a Dashboard-ban beállított szerepkört
            // (ez a kiegészítő ellenőrzés a hibák elkerülése végett)
            Window parentWindow = Window.GetWindow(control);
            if (parentWindow != null && parentWindow is Views.App.Dashboard dashboard)
            {
                var dashboardType = dashboard.GetType();
                var userRoleField = dashboardType.GetField("UserRole", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (userRoleField != null)
                {
                    string userRole = userRoleField.GetValue(dashboard) as string;
                    Console.WriteLine($"Dashboard.UserRole: {userRole}");
                    
                    if (userRole?.ToLower() == ADMIN_ROLE)
                    {
                        Console.WriteLine("Dashboard shows user is admin, access granted despite PermissionManager.");
                        // Frissítsük a PermissionManager-t, hogy legközelebb ne kelljen ezt az útvonalat használni
                        if (_currentUser != null && string.IsNullOrEmpty(_currentUser.Role))
                        {
                            _currentUser.Role = ADMIN_ROLE;
                        }
                        return true;
                    }
                }
            }
            
            // Ha nem admin, nem engedélyezzük a hozzáférést
            MessageBox.Show(
                "Nincs jogosultsága ehhez az oldalhoz! Ez az oldal csak rendszergazdák számára érhető el.",
                "Hozzáférés megtagadva",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
            
            // Átirányítjuk a Profile oldalra
            if (parentWindow != null && parentWindow is Views.App.Dashboard dashboard2)
            {
                dashboard2.SetActivePage(typeof(PC_Configurator.Views.App.Profile));
            }
            
            return false;
        }

        /// <summary>
        /// Set visibility of UI elements based on user permissions 
        /// </summary>
        /// <param name="element">The UI element to modify</param>
        /// <param name="requireAdmin">Whether admin rights are required to see the element</param>
        public static void SetElementVisibility(UIElement element, bool requireAdmin)
        {
            if (element == null) return;
            
            if (requireAdmin && !IsCurrentUserAdmin())
            {
                element.Visibility = Visibility.Collapsed;
            }
            else
            {
                element.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Property to check if the current user is an admin
        /// </summary>
        public static bool IsAdmin
        {
            get { return IsCurrentUserAdmin(); }
        }
    }
}
