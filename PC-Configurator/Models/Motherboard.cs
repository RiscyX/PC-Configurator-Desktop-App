using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    public class Motherboard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Chipset { get; set; }
        public string Socket { get; set; }
        public int SocketTypeId { get; set; }
        public int MaxMemoryGB { get; set; }
        public decimal Price { get; set; }
        public int PowerConsumption { get; set; }
        public string FormFactor { get; set; }

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
                    "INSERT INTO Motherboards (Name, Manufacturer, Chipset, Socket, FormFactor, Price, PowerConsumption) " + 
                    "VALUES (@Name, @Manufacturer, @Chipset, @Socket, @FormFactor, @Price, @PowerConsumption)", connection))
                {
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Manufacturer", Manufacturer);
                    command.Parameters.AddWithValue("@Chipset", Chipset);
                    command.Parameters.AddWithValue("@Socket", Socket);
                    command.Parameters.AddWithValue("@FormFactor", FormFactor);
                    command.Parameters.AddWithValue("@Price", Price);
                    command.Parameters.AddWithValue("@PowerConsumption", PowerConsumption);
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
                    "UPDATE Motherboards SET Name = @Name, Manufacturer = @Manufacturer, Chipset = @Chipset, " +
                    "Socket = @Socket, FormFactor = @FormFactor, Price = @Price, PowerConsumption = @PowerConsumption " +
                    "WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", Id);
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Manufacturer", Manufacturer);
                    command.Parameters.AddWithValue("@Chipset", Chipset);
                    command.Parameters.AddWithValue("@Socket", Socket);
                    command.Parameters.AddWithValue("@FormFactor", FormFactor);
                    command.Parameters.AddWithValue("@Price", Price);
                    command.Parameters.AddWithValue("@PowerConsumption", PowerConsumption);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
