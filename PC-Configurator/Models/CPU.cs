using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    internal class CPU
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public int Cores { get; set; }
        public int Threads { get; set; }
        public float BaseClockGHz { get; set; }
        public float BoostClockGHz { get; set; }

        public CPU() { }
        public CPU(int id, string name, string manufacturer, int cores, int threads, float baseClockGHz, float boostClockGHz)
        {
            Id = id;
            Name = name;
            Manufacturer = manufacturer;
            Cores = cores;
            Threads = threads;
            BaseClockGHz = baseClockGHz;
            BoostClockGHz = boostClockGHz;
        }

        public void SaveToDatabase()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new System.Data.SqlClient.SqlCommand(
                    "INSERT INTO CPUs (Name, Manufacturer, Cores, Threads, BaseClockGHz, BoostClockGHz) VALUES (@Name, @Manufacturer, @Cores, @Threads, @BaseClockGHz, @BoostClockGHz)", connection))
                {
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Manufacturer", Manufacturer);
                    command.Parameters.AddWithValue("@Cores", Cores);
                    command.Parameters.AddWithValue("@Threads", Threads);
                    command.Parameters.AddWithValue("@BaseClockGHz", BaseClockGHz);
                    command.Parameters.AddWithValue("@BoostClockGHz", BoostClockGHz);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
