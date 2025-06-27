using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    public class Storage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int CapacityGB { get; set; }
        public int Capacity { get { return CapacityGB; } }  // Alias for CapacityGB for compatibility
        public decimal Price { get; set; }
        public int PowerConsumption { get; set; }

        public Storage() { }
        public Storage(int id, string name, string type, int capacityGB, decimal price = 0)
        {
            Id = id;
            Name = name;
            Type = type;
            CapacityGB = capacityGB;
            Price = price;
        }

        public void SaveToDatabase()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new System.Data.SqlClient.SqlCommand(
                    "INSERT INTO Storages (Name, Type, CapacityGB, Price) VALUES (@Name, @Type, @CapacityGB, @Price)", connection))
                {
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Type", Type);
                    command.Parameters.AddWithValue("@CapacityGB", CapacityGB);
                    command.Parameters.AddWithValue("@Price", Price);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
