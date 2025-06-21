using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    internal class PSU
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Wattage { get; set; }
        public string EfficiencyRating { get; set; }

        public PSU() { }
        public PSU(int id, string name, int wattage, string efficiencyRating)
        {
            Id = id;
            Name = name;
            Wattage = wattage;
            EfficiencyRating = efficiencyRating;
        }

        public void SaveToDatabase()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new System.Data.SqlClient.SqlCommand(
                    "INSERT INTO PSUs (Name, Wattage, EfficiencyRating) VALUES (@Name, @Wattage, @EfficiencyRating)", connection))
                {
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Wattage", Wattage);
                    command.Parameters.AddWithValue("@EfficiencyRating", EfficiencyRating);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
