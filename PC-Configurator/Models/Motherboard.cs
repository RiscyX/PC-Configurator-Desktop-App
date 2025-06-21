using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    internal class Motherboard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Chipset { get; set; }
        public string Socket { get; set; }

        public Motherboard() { }
        public Motherboard(int id, string name, string manufacturer, string chipset, string socket)
        {
            Id = id;
            Name = name;
            Manufacturer = manufacturer;
            Chipset = chipset;
            Socket = socket;
        }

        public void SaveToDatabase()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new System.Data.SqlClient.SqlCommand(
                    "INSERT INTO Motherboards (Name, Manufacturer, Chipset, Socket) VALUES (@Name, @Manufacturer, @Chipset, @Socket)", connection))
                {
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Manufacturer", Manufacturer);
                    command.Parameters.AddWithValue("@Chipset", Chipset);
                    command.Parameters.AddWithValue("@Socket", Socket);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
