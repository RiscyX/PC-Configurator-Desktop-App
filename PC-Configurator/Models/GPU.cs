using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    public class GPU
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public int MemoryGB { get; set; }
        public int Memory { get { return MemoryGB; } }  // Alias for MemoryGB for compatibility
        public string MemoryType { get; set; } = "GDDR6";  // Default value
        public decimal Price { get; set; }
        public int PowerConsumption { get; set; }

        public GPU() { }
        public GPU(int id, string name, string manufacturer, int memoryGB, string memoryType = "GDDR6", decimal price = 0, int powerConsumption = 0)
        {
            Id = id;
            Name = name;
            MemoryType = memoryType;
            Price = price;
            PowerConsumption = powerConsumption;
            Manufacturer = manufacturer;
            MemoryGB = memoryGB;
        }

        public void SaveToDatabase()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new System.Data.SqlClient.SqlCommand(
                    "INSERT INTO GPUs (Name, Manufacturer, MemoryGB) VALUES (@Name, @Manufacturer, @MemoryGB)", connection))
                {
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Manufacturer", Manufacturer);
                    command.Parameters.AddWithValue("@MemoryGB", MemoryGB);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
