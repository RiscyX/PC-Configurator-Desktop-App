using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    internal class RAM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CapacityGB { get; set; }
        public int SpeedMHz { get; set; }
        public string Type { get; set; }

        public RAM() { }
        public RAM(int id, string name, int capacityGB, int speedMHz, string type)
        {
            Id = id;
            Name = name;
            CapacityGB = capacityGB;
            SpeedMHz = speedMHz;
            Type = type;
        }

        public void SaveToDatabase()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new System.Data.SqlClient.SqlCommand(
                    "INSERT INTO RAMs (Name, CapacityGB, SpeedMHz, Type) VALUES (@Name, @CapacityGB, @SpeedMHz, @Type)", connection))
                {
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@CapacityGB", CapacityGB);
                    command.Parameters.AddWithValue("@SpeedMHz", SpeedMHz);
                    command.Parameters.AddWithValue("@Type", Type);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
