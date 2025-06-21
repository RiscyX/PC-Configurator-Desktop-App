using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    internal class Case
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FormFactor { get; set; }
        
        public Case() { }
        public Case(int id, string name, string formFactor)
        {
            Id = id;
            Name = name;
            FormFactor = formFactor;
        }

        public void SaveToDatabase()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new System.Data.SqlClient.SqlCommand(
                    "INSERT INTO Cases (Name, FormFactor) VALUES (@Name, @FormFactor)", connection))
                {
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@FormFactor", FormFactor);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
