using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;

namespace PC_Configurator.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastLogin { get; set; }

        // Properties for display
        public string FullName => $"{FirstName} {LastName}";
        public string Initials => (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName)) 
            ? $"{FirstName[0]}{LastName[0]}" 
            : (!string.IsNullOrEmpty(Email) ? Email[0].ToString().ToUpper() : "?");

        // Check if user has admin role
        public bool IsAdmin => Role?.ToLower() == "admin";

        public Users() { }

        public Users(int id, string email, string firstName, string lastName, string role)
        {
            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Role = role;
            Created = DateTime.Now;
        }

        // Get user from database by email
        public static Users GetUserByEmail(string email)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    var cmd = new SqlCommand(
                        "SELECT Id, Email, FirstName, LastName, Role, Created, LastLogin " +
                        "FROM Users WHERE Email = @email", conn);
                    cmd.Parameters.AddWithValue("@email", email);
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Users
                            {
                                Id = reader.GetInt32(0),
                                Email = reader.GetString(1),
                                FirstName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                LastName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                Role = reader.IsDBNull(4) ? "user" : reader.GetString(4),
                                Created = reader.IsDBNull(5) ? DateTime.Now : reader.GetDateTime(5),
                                LastLogin = reader.IsDBNull(6) ? null : (DateTime?)reader.GetDateTime(6)
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user: {ex.Message}");
            }
            
            return null;
        }

        // Update last login time
        public bool UpdateLastLogin()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    var cmd = new SqlCommand(
                        "UPDATE Users SET LastLogin = @lastLogin WHERE Id = @id", conn);
                    cmd.Parameters.AddWithValue("@lastLogin", DateTime.Now);
                    cmd.Parameters.AddWithValue("@id", Id);
                    
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                return false;
            }
        }
    }
}
