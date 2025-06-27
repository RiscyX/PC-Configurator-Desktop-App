using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    public class RAM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CapacityGB { get; set; }
        public int SpeedMHz { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public int PowerConsumption { get; set; }

        public RAM() { }
        public RAM(int id, string name, int capacityGB, int speedMHz, string type, decimal price = 0, int powerConsumption = 0)
        {
            Id = id;
            Name = name;
            CapacityGB = capacityGB;
            SpeedMHz = speedMHz;
            Type = type;
            Price = price;
            PowerConsumption = powerConsumption;
        }

        public void SaveToDatabase()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new System.Data.SqlClient.SqlCommand(
                    "INSERT INTO RAMs (Name, CapacityGB, SpeedMHz, Type, Price, PowerConsumption) VALUES (@Name, @CapacityGB, @SpeedMHz, @Type, @Price, @PowerConsumption)", connection))
                {
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@CapacityGB", CapacityGB);
                    command.Parameters.AddWithValue("@SpeedMHz", SpeedMHz);
                    command.Parameters.AddWithValue("@Type", Type);
                    command.Parameters.AddWithValue("@Price", Price > 0 ? (object)Price : DBNull.Value);
                    command.Parameters.AddWithValue("@PowerConsumption", PowerConsumption > 0 ? (object)PowerConsumption : DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
