using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    internal class Storage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int CapacityGB { get; set; }

        public Storage() { }
        public Storage(int id, string name, string type, int capacityGB)
        {
            Id = id;
            Name = name;
            Type = type;
            CapacityGB = capacityGB;
        }

        public void SaveToDatabase()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new System.Data.SqlClient.SqlCommand(
                    "INSERT INTO Storages (Name, Type, CapacityGB) VALUES (@Name, @Type, @CapacityGB)", connection))
                {
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Type", Type);
                    command.Parameters.AddWithValue("@CapacityGB", CapacityGB);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
