using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace PC_Configurator.Views.Forms
{
    public class ChipsetType
    {
        public int Id { get; set; }
        public string ChipsetName { get; set; }
        public string Manufacturer { get; set; }
        public string Generation { get; set; }
        public string Description { get; set; }
    }

    public class SocketType
    {
        public int Id { get; set; }
        public string SocketName { get; set; }
        public string Manufacturer { get; set; }
        public string Generation { get; set; }
        public string Description { get; set; }
    }

    public static class ChipsetHelper
    {
        // Chipset típusok betöltése az adatbázisból
        public static List<ChipsetType> LoadChipsetTypes(string manufacturer = null)
        {
            var chipsets = new List<ChipsetType>();
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            
            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    
                    string query = "SELECT * FROM ChipsetTypes";
                    if (!string.IsNullOrEmpty(manufacturer))
                    {
                        query += " WHERE Manufacturer = @Manufacturer";
                    }
                    
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(manufacturer))
                        {
                            cmd.Parameters.AddWithValue("@Manufacturer", manufacturer);
                        }
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                chipsets.Add(new ChipsetType
                                {
                                    Id = (int)reader["Id"],
                                    ChipsetName = reader["ChipsetName"].ToString(),
                                    Manufacturer = reader["Manufacturer"].ToString(),
                                    Generation = reader["Generation"]?.ToString() ?? string.Empty,
                                    Description = reader["Description"]?.ToString() ?? string.Empty
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a chipset típusok betöltésekor: {ex.Message}");
            }
            
            return chipsets;
        }

        // Socket típusok betöltése az adatbázisból
        public static List<SocketType> LoadSocketTypes(string manufacturer = null)
        {
            var sockets = new List<SocketType>();
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            
            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    
                    string query = "SELECT * FROM SocketTypes";
                    if (!string.IsNullOrEmpty(manufacturer))
                    {
                        query += " WHERE Manufacturer = @Manufacturer";
                    }
                    
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(manufacturer))
                        {
                            cmd.Parameters.AddWithValue("@Manufacturer", manufacturer);
                        }
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                sockets.Add(new SocketType
                                {
                                    Id = (int)reader["Id"],
                                    SocketName = reader["SocketName"].ToString(),
                                    Manufacturer = reader["Manufacturer"].ToString(),
                                    Generation = reader["Generation"]?.ToString() ?? string.Empty,
                                    Description = reader["Description"]?.ToString() ?? string.Empty
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a socket típusok betöltésekor: {ex.Message}");
            }
            
            return sockets;
        }

        // Ellenőrzi, hogy a ChipsetTypes tábla létezik-e
        public static bool CheckChipsetTypesTableExists()
        {
            bool tableExists = false;
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            
            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ChipsetTypes'";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        int count = (int)cmd.ExecuteScalar();
                        tableExists = count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a ChipsetTypes tábla ellenőrzésekor: {ex.Message}");
            }
            
            return tableExists;
        }

        // Fallback chipset lista, ha az adatbázis nem elérhető
        public static List<ChipsetType> GetFallbackChipsets(string manufacturer)
        {
            List<ChipsetType> chipsets = new List<ChipsetType>();
            
            if (manufacturer.Equals("AMD", StringComparison.OrdinalIgnoreCase))
            {
                chipsets.AddRange(new[]
                {
                    new ChipsetType { Id = 1, ChipsetName = "A320", Manufacturer = "AMD", Generation = "Ryzen 1000/2000", Description = "Entry-level" },
                    new ChipsetType { Id = 2, ChipsetName = "B350", Manufacturer = "AMD", Generation = "Ryzen 1000/2000", Description = "Mid-range" },
                    new ChipsetType { Id = 3, ChipsetName = "X370", Manufacturer = "AMD", Generation = "Ryzen 1000/2000", Description = "High-end" },
                    new ChipsetType { Id = 4, ChipsetName = "B450", Manufacturer = "AMD", Generation = "Ryzen 2000/3000", Description = "Mid-range" },
                    new ChipsetType { Id = 5, ChipsetName = "X470", Manufacturer = "AMD", Generation = "Ryzen 2000/3000", Description = "High-end" },
                    new ChipsetType { Id = 6, ChipsetName = "A520", Manufacturer = "AMD", Generation = "Ryzen 3000/5000", Description = "Entry-level" },
                    new ChipsetType { Id = 7, ChipsetName = "B550", Manufacturer = "AMD", Generation = "Ryzen 3000/5000", Description = "Mid-range" },
                    new ChipsetType { Id = 8, ChipsetName = "X570", Manufacturer = "AMD", Generation = "Ryzen 3000/5000", Description = "High-end" },
                    new ChipsetType { Id = 9, ChipsetName = "B650", Manufacturer = "AMD", Generation = "Ryzen 7000", Description = "Mid-range" },
                    new ChipsetType { Id = 10, ChipsetName = "X670", Manufacturer = "AMD", Generation = "Ryzen 7000", Description = "High-end" },
                    new ChipsetType { Id = 11, ChipsetName = "X670E", Manufacturer = "AMD", Generation = "Ryzen 7000", Description = "Extreme" }
                });
            }
            else if (manufacturer.Equals("Intel", StringComparison.OrdinalIgnoreCase))
            {
                chipsets.AddRange(new[]
                {
                    new ChipsetType { Id = 12, ChipsetName = "B365", Manufacturer = "Intel", Generation = "8th/9th Gen", Description = "Mid-range" },
                    new ChipsetType { Id = 13, ChipsetName = "Z390", Manufacturer = "Intel", Generation = "8th/9th Gen", Description = "High-end" },
                    new ChipsetType { Id = 14, ChipsetName = "B460", Manufacturer = "Intel", Generation = "10th Gen", Description = "Mid-range" },
                    new ChipsetType { Id = 15, ChipsetName = "Z490", Manufacturer = "Intel", Generation = "10th Gen", Description = "High-end" },
                    new ChipsetType { Id = 16, ChipsetName = "B560", Manufacturer = "Intel", Generation = "11th Gen", Description = "Mid-range" },
                    new ChipsetType { Id = 17, ChipsetName = "Z590", Manufacturer = "Intel", Generation = "11th Gen", Description = "High-end" },
                    new ChipsetType { Id = 18, ChipsetName = "H610", Manufacturer = "Intel", Generation = "12th/13th Gen", Description = "Entry-level" },
                    new ChipsetType { Id = 19, ChipsetName = "B660", Manufacturer = "Intel", Generation = "12th/13th Gen", Description = "Mid-range" },
                    new ChipsetType { Id = 20, ChipsetName = "H670", Manufacturer = "Intel", Generation = "12th/13th Gen", Description = "Mid-high" },
                    new ChipsetType { Id = 21, ChipsetName = "Z690", Manufacturer = "Intel", Generation = "12th/13th Gen", Description = "High-end" },
                    new ChipsetType { Id = 22, ChipsetName = "B760", Manufacturer = "Intel", Generation = "13th/14th Gen", Description = "Mid-range" },
                    new ChipsetType { Id = 23, ChipsetName = "Z790", Manufacturer = "Intel", Generation = "13th/14th Gen", Description = "High-end" }
                });
            }
            
            return chipsets;
        }
    }
}
