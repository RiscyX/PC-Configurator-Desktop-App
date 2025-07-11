using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    public class Case
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FormFactor { get; set; }
        public string Color { get; set; } = "Black";  // Default value
        public decimal Price { get; set; }
        public string Manufacturer { get; set; }
        public string SupportedFormFactors { get; set; }
        
        public Case() { }
        public Case(int id, string name, string formFactor, string color = "Black", decimal price = 0)
        {
            Id = id;
            Name = name;
            FormFactor = formFactor;
            Color = color;
            Price = price;
        }

        public void SaveToDatabase()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new System.Data.SqlClient.SqlCommand(
                    "INSERT INTO Cases (Name, FormFactor, Color, Price) " +
                    "VALUES (@Name, @FormFactor, @Color, @Price)", connection))
                {
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@FormFactor", FormFactor);
                    command.Parameters.AddWithValue("@Color", Color);
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
                    "UPDATE Cases SET Name = @Name, FormFactor = @FormFactor, Color = @Color, " +
                    "Price = @Price WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", Id);
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@FormFactor", FormFactor);
                    command.Parameters.AddWithValue("@Color", Color);
                    command.Parameters.AddWithValue("@Price", Price);
                    command.ExecuteNonQuery();
                }
            }
        }
        
        /// <summary>
        /// Ellenőrzi, hogy a ház támogatja-e az adott formfaktort
        /// </summary>
        /// <param name="formFactor">Az ellenőrizendő formfaktor</param>
        /// <returns>True, ha a ház támogatja a megadott formfaktort</returns>
        public bool SupportsFormFactor(string formFactor)
        {
            // Alapértelmezetten támogat minden formfaktort (egyszerűsített verzió)
            return true;
        }
    }
}
