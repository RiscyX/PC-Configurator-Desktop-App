using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    internal class GPU
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public int MemoryGB { get; set; }

        public GPU() { }
        public GPU(int id, string name, string manufacturer, int memoryGB)
        {
            Id = id;
            Name = name;
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
