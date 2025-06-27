using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    public class PSU
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Wattage { get; set; }
        public string EfficiencyRating { get; set; }
        public string Efficiency { get { return EfficiencyRating; } }  // Alias for EfficiencyRating for compatibility
        public decimal Price { get; set; }

        public PSU() { }
        public PSU(int id, string name, int wattage, string efficiencyRating, decimal price = 0)
        {
            Id = id;
            Name = name;
            Wattage = wattage;
            EfficiencyRating = efficiencyRating;
            Price = price;
        }

        public void SaveToDatabase()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new System.Data.SqlClient.SqlCommand(
                    "INSERT INTO PSUs (Name, Wattage, EfficiencyRating, Price) VALUES (@Name, @Wattage, @EfficiencyRating, @Price)", connection))
                {
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Wattage", Wattage);
                    command.Parameters.AddWithValue("@EfficiencyRating", EfficiencyRating);
                    command.Parameters.AddWithValue("@Price", Price);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateInDatabase()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new System.Data.SqlClient.SqlCommand(
                    "UPDATE PSUs SET Name = @Name, Wattage = @Wattage, EfficiencyRating = @EfficiencyRating, Price = @Price WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", Id);
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Wattage", Wattage);
                    command.Parameters.AddWithValue("@EfficiencyRating", EfficiencyRating);
                    command.Parameters.AddWithValue("@Price", Price);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
