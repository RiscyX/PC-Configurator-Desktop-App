using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PC_Configurator.Models; // Hozzáadva a SqlDataReaderExtensions használatához

namespace PC_Configurator.Helpers
{
    /// <summary>
    /// A Chipset típusok betöltéséért és kezeléséért felelős segédosztály
    /// </summary>
    public class ChipsetType
    {
        public int Id { get; set; }
        public string ChipsetName { get; set; }
        public string Manufacturer { get; set; }
        public string Generation { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// A Socket típusok betöltéséért és kezeléséért felelős segédosztály
    /// </summary>
    public class SocketType
    {
        public int Id { get; set; }
        public string SocketName { get; set; }
        public string Manufacturer { get; set; }
        public string Generation { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// Chipset és Socket adatok kezeléséért felelős segédosztály
    /// </summary>
    public static class ChipsetHelper
    {
        /// <summary>
        /// Ellenőrzi, hogy létezik-e a ChipsetTypes tábla az adatbázisban
        /// </summary>
        public static bool CheckChipsetTypesTableExists()
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(
                        "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ChipsetTypes'", connection))
                    {
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a ChipsetTypes tábla ellenőrzésekor: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Betölti a chipset típusokat az adatbázisból a megadott gyártó szerint szűrve
        /// Ha a tábla nem létezik vagy hiba történik, fallback értékeket használ
        /// </summary>
        public static List<ChipsetType> LoadChipsetTypes(string manufacturer)
        {
            List<ChipsetType> result = new List<ChipsetType>();

            try
            {
                System.Diagnostics.Debug.WriteLine($"Chipset típusok betöltésének indítása: {manufacturer}");
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    System.Diagnostics.Debug.WriteLine("Kapcsolati string hiányzik. Fallback értékek használata.");
                    return GetFallbackChipsets(manufacturer);
                }
                
                using (var connection = new SqlConnection(connectionString))
                {
                    try 
                    {
                        connection.Open();
                        System.Diagnostics.Debug.WriteLine("Adatbázis kapcsolat sikeresen megnyitva.");
                        
                        // Ellenőrizzük, hogy létezik-e a ChipsetTypes tábla
                        bool chipsetTableExists = false;
                        using (var checkTableCommand = new SqlCommand(
                            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ChipsetTypes'", connection))
                        {
                            chipsetTableExists = (int)checkTableCommand.ExecuteScalar() > 0;
                        }
                        
                        if (!chipsetTableExists)
                        {
                            System.Diagnostics.Debug.WriteLine("A ChipsetTypes tábla nem létezik. Fallback értékek használata.");
                            return GetFallbackChipsets(manufacturer);
                        }
                        
                        // Ellenőrizzük a tábla tartalmát
                        int chipsetCount = 0;
                        using (var checkContentCommand = new SqlCommand(
                            "SELECT COUNT(*) FROM ChipsetTypes WHERE Manufacturer = @Manufacturer", connection))
                        {
                            checkContentCommand.Parameters.AddWithValue("@Manufacturer", manufacturer);
                            chipsetCount = (int)checkContentCommand.ExecuteScalar();
                        }
                        
                        if (chipsetCount == 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"Nem találtunk {manufacturer} gyártójú chipseteket. Fallback értékek használata.");
                            return GetFallbackChipsets(manufacturer);
                        }
                        
                        // Először betöltjük az összes chipsetet
                        result = LoadAllChipsetTypes(manufacturer, connection);
                        
                        if (result.Count == 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"Nem sikerült betölteni a chipseteket a {manufacturer} gyártótól. Fallback értékek használata.");
                            return GetFallbackChipsets(manufacturer);
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"Sikeresen betöltve {result.Count} chipset a(z) {manufacturer} gyártótól.");
                        foreach (var chipset in result.Take(3))
                        {
                            System.Diagnostics.Debug.WriteLine($" - {chipset.ChipsetName} (ID: {chipset.Id})");
                        }
                        
                        return result;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Hiba az adatbázis kapcsolat használatakor: {ex.Message}");
                        return GetFallbackChipsets(manufacturer);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a ChipsetTypes betöltésekor: {ex.Message}");
                return GetFallbackChipsets(manufacturer);
            }
        }
        
        /// <summary>
        /// Betölti az összes chipset típust egy adott gyártótól
        /// </summary>
        private static List<ChipsetType> LoadAllChipsetTypes(string manufacturer, SqlConnection connection)
        {
            List<ChipsetType> result = new List<ChipsetType>();
            
            try
            {
                // Már nyitott kapcsolatot használunk
                using (var command = new SqlCommand(
                    "SELECT * FROM ChipsetTypes WHERE Manufacturer = @Manufacturer ORDER BY ChipsetName", connection))
                {
                    command.Parameters.AddWithValue("@Manufacturer", manufacturer);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var chipset = new ChipsetType
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                ChipsetName = reader["ChipsetName"].ToString(),
                                Manufacturer = reader["Manufacturer"].ToString(),
                                Generation = "",
                                Description = ""
                            };

                            // Biztonságos mód a mezők ellenőrzésére HasColumn nélkül
                            try
                            {
                                chipset.Generation = reader["Generation"] != DBNull.Value ? reader["Generation"].ToString() : "";
                            }
                            catch (IndexOutOfRangeException)
                            {
                                // Generation oszlop nem létezik
                                System.Diagnostics.Debug.WriteLine("Generation oszlop nem található");
                            }
                            
                            try
                            {
                                chipset.Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : "";
                            }
                            catch (IndexOutOfRangeException)
                            {
                                // Description oszlop nem létezik
                                System.Diagnostics.Debug.WriteLine("Description oszlop nem található");
                            }
                            
                            try
                            {
                                // Ha van Features oszlop, azt használjuk a Description-ben
                                chipset.Description = reader["Features"] != DBNull.Value ? reader["Features"].ToString() : chipset.Description;
                            }
                            catch (IndexOutOfRangeException)
                            {
                                // Features oszlop nem létezik
                                System.Diagnostics.Debug.WriteLine("Features oszlop nem található");
                            }
                                
                            result.Add(chipset);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba az összes ChipsetType betöltésekor: {ex.Message}");
                // Itt nem adunk vissza fallback értéket, mivel ez egy belső metódus
            }
            
            return result;
        }

        /// <summary>
        /// Betölti a socket típusokat az adatbázisból
        /// Ha a tábla nem létezik vagy hiba történik, fallback értékeket használ
        /// </summary>
        public static List<SocketType> LoadSocketTypes()
        {
            List<SocketType> result = new List<SocketType>();

            try
            {
                System.Diagnostics.Debug.WriteLine("Socket típusok betöltésének indítása...");
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    System.Diagnostics.Debug.WriteLine("Kapcsolati string hiányzik. Fallback értékek használata.");
                    return GetFallbackSocketTypes();
                }
                
                using (var connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        System.Diagnostics.Debug.WriteLine("Adatbázis kapcsolat sikeresen megnyitva socket típusok betöltéséhez.");
                        
                        // Ellenőrizzük, hogy létezik-e a SocketTypes tábla
                        bool socketTableExists = false;
                        using (var checkTableCommand = new SqlCommand(
                            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SocketTypes'", connection))
                        {
                            socketTableExists = (int)checkTableCommand.ExecuteScalar() > 0;
                        }
                        
                        if (!socketTableExists)
                        {
                            System.Diagnostics.Debug.WriteLine("A SocketTypes tábla nem létezik. Fallback értékek használata.");
                            return GetFallbackSocketTypes();
                        }
                        
                        // Ellenőrizzük a tábla tartalmát
                        int socketCount = 0;
                        using (var checkContentCommand = new SqlCommand(
                            "SELECT COUNT(*) FROM SocketTypes", connection))
                        {
                            socketCount = (int)checkContentCommand.ExecuteScalar();
                        }
                        
                        if (socketCount == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("Nem találtunk socket típusokat. Fallback értékek használata.");
                            return GetFallbackSocketTypes();
                        }
                        
                        // Betöltjük az összes socket típust
                        result = LoadAllSocketTypes(connection);
                        
                        if (result.Count == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("Nem sikerült betölteni a socket típusokat. Fallback értékek használata.");
                            return GetFallbackSocketTypes();
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"Sikeresen betöltve {result.Count} socket típus.");
                        foreach (var socket in result.Take(3))
                        {
                            System.Diagnostics.Debug.WriteLine($" - {socket.SocketName} (Gyártó: {socket.Manufacturer}, ID: {socket.Id})");
                        }
                        
                        return result;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Hiba az adatbázis használatakor socket betöltéskor: {ex.Message}");
                        return GetFallbackSocketTypes();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a SocketTypes betöltésekor: {ex.Message}");
                return GetFallbackSocketTypes();
            }
        }
        
        /// <summary>
        /// Betölti az összes socket típust
        /// </summary>
        private static List<SocketType> LoadAllSocketTypes(SqlConnection connection)
        {
            List<SocketType> result = new List<SocketType>();
            
            try
            {
                // Már nyitott kapcsolatot használunk
                using (var command = new SqlCommand(
                    "SELECT * FROM SocketTypes ORDER BY Manufacturer, SocketName", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var socketType = new SocketType
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                SocketName = reader["SocketName"].ToString(),
                                Manufacturer = reader["Manufacturer"].ToString(),
                                Generation = "",
                                Description = ""
                            };
                            
                            // Biztonságos mód a mezők ellenőrzésére HasColumn nélkül
                            try
                            {
                                socketType.Generation = reader["Generation"] != DBNull.Value ? reader["Generation"].ToString() : "";
                            }
                            catch (IndexOutOfRangeException)
                            {
                                // Generation oszlop nem létezik
                                System.Diagnostics.Debug.WriteLine("SocketTypes: Generation oszlop nem található");
                            }
                            
                            try
                            {
                                socketType.Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : "";
                            }
                            catch (IndexOutOfRangeException)
                            {
                                // Description oszlop nem létezik
                                System.Diagnostics.Debug.WriteLine("SocketTypes: Description oszlop nem található");
                            }
                            
                            result.Add(socketType);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba az összes SocketType betöltésekor: {ex.Message}");
                // Itt nem adunk vissza fallback értéket, mivel ez egy belső metódus
            }
            
            return result;
        }

        /// <summary>
        /// Fallback chipset értékek, ha az adatbázis nem elérhető
        /// </summary>
        public static List<ChipsetType> GetFallbackChipsets(string manufacturer)
        {
            var result = new List<ChipsetType>();
            int id = 1;

            if (manufacturer == "AMD")
            {
                // AM4 Chipsets
                result.Add(new ChipsetType { Id = id++, ChipsetName = "A320", Manufacturer = "AMD", Generation = "Ryzen 1000, 2000, 3000", Description = "Entry-level chipset for AM4" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "B350", Manufacturer = "AMD", Generation = "Ryzen 1000, 2000, 3000", Description = "Mid-range chipset for AM4" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "B450", Manufacturer = "AMD", Generation = "Ryzen 1000, 2000, 3000, 5000", Description = "Mid-range chipset for AM4" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "X370", Manufacturer = "AMD", Generation = "Ryzen 1000, 2000, 3000", Description = "High-end chipset for AM4" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "X470", Manufacturer = "AMD", Generation = "Ryzen 1000, 2000, 3000, 5000", Description = "High-end chipset for AM4" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "B550", Manufacturer = "AMD", Generation = "Ryzen 3000, 5000", Description = "Mid-range chipset for AM4 with PCIe 4.0" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "X570", Manufacturer = "AMD", Generation = "Ryzen 3000, 5000", Description = "High-end chipset for AM4 with PCIe 4.0" });

                // AM5 Chipsets
                result.Add(new ChipsetType { Id = id++, ChipsetName = "B650", Manufacturer = "AMD", Generation = "Ryzen 7000", Description = "Mid-range chipset for AM5" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "X670", Manufacturer = "AMD", Generation = "Ryzen 7000", Description = "High-end chipset for AM5" });
            }
            else if (manufacturer == "Intel")
            {
                // LGA1151 Chipsets
                result.Add(new ChipsetType { Id = id++, ChipsetName = "H310", Manufacturer = "Intel", Generation = "8th and 9th Gen", Description = "Entry-level chipset for Coffee Lake" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "B360", Manufacturer = "Intel", Generation = "8th and 9th Gen", Description = "Mid-range chipset for Coffee Lake" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "B365", Manufacturer = "Intel", Generation = "8th and 9th Gen", Description = "Mid-range chipset for Coffee Lake" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "H370", Manufacturer = "Intel", Generation = "8th and 9th Gen", Description = "Mid-range chipset for Coffee Lake" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "Z370", Manufacturer = "Intel", Generation = "8th Gen", Description = "High-end chipset for Coffee Lake" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "Z390", Manufacturer = "Intel", Generation = "8th and 9th Gen", Description = "High-end chipset for Coffee Lake refresh" });

                // LGA1200 Chipsets
                result.Add(new ChipsetType { Id = id++, ChipsetName = "H410", Manufacturer = "Intel", Generation = "10th Gen", Description = "Entry-level chipset for Comet Lake" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "B460", Manufacturer = "Intel", Generation = "10th Gen", Description = "Mid-range chipset for Comet Lake" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "H470", Manufacturer = "Intel", Generation = "10th Gen", Description = "Mid-range chipset for Comet Lake" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "Z490", Manufacturer = "Intel", Generation = "10th Gen", Description = "High-end chipset for Comet Lake" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "H510", Manufacturer = "Intel", Generation = "11th Gen", Description = "Entry-level chipset for Rocket Lake" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "B560", Manufacturer = "Intel", Generation = "11th Gen", Description = "Mid-range chipset for Rocket Lake" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "H570", Manufacturer = "Intel", Generation = "11th Gen", Description = "Mid-range chipset for Rocket Lake" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "Z590", Manufacturer = "Intel", Generation = "11th Gen", Description = "High-end chipset for Rocket Lake" });

                // LGA1700 Chipsets
                result.Add(new ChipsetType { Id = id++, ChipsetName = "H610", Manufacturer = "Intel", Generation = "12th and 13th Gen", Description = "Entry-level chipset for Alder Lake and Raptor Lake" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "B660", Manufacturer = "Intel", Generation = "12th and 13th Gen", Description = "Mid-range chipset for Alder Lake and Raptor Lake" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "H670", Manufacturer = "Intel", Generation = "12th and 13th Gen", Description = "Mid-range chipset for Alder Lake and Raptor Lake" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "Z690", Manufacturer = "Intel", Generation = "12th and 13th Gen", Description = "High-end chipset for Alder Lake and Raptor Lake" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "B760", Manufacturer = "Intel", Generation = "13th Gen", Description = "Mid-range chipset for Raptor Lake" });
                result.Add(new ChipsetType { Id = id++, ChipsetName = "Z790", Manufacturer = "Intel", Generation = "13th Gen", Description = "High-end chipset for Raptor Lake" });
            }

            System.Diagnostics.Debug.WriteLine($"Fallback chipset értékek betöltve {manufacturer} gyártóhoz: {result.Count} db");
            return result;
        }

        /// <summary>
        /// Fallback socket értékek, ha az adatbázis nem elérhető
        /// </summary>
        public static List<SocketType> GetFallbackSocketTypes()
        {
            var result = new List<SocketType>();
            int id = 1;

            // AMD Socket Types
            result.Add(new SocketType { Id = id++, SocketName = "AM4", Manufacturer = "AMD", Generation = "Ryzen 1000, 2000, 3000, 5000", Description = "Socket for Ryzen CPUs" });
            result.Add(new SocketType { Id = id++, SocketName = "AM5", Manufacturer = "AMD", Generation = "Ryzen 7000", Description = "Socket for Ryzen 7000 series and newer" });
            result.Add(new SocketType { Id = id++, SocketName = "TR4", Manufacturer = "AMD", Generation = "Threadripper 1000, 2000", Description = "Socket for Threadripper CPUs" });
            result.Add(new SocketType { Id = id++, SocketName = "sTRX4", Manufacturer = "AMD", Generation = "Threadripper 3000", Description = "Socket for Threadripper 3000 series" });

            // Intel Socket Types
            result.Add(new SocketType { Id = id++, SocketName = "LGA1151", Manufacturer = "Intel", Generation = "8th and 9th Gen", Description = "Socket for Coffee Lake CPUs" });
            result.Add(new SocketType { Id = id++, SocketName = "LGA1200", Manufacturer = "Intel", Generation = "10th and 11th Gen", Description = "Socket for Comet Lake and Rocket Lake CPUs" });
            result.Add(new SocketType { Id = id++, SocketName = "LGA1700", Manufacturer = "Intel", Generation = "12th and 13th Gen", Description = "Socket for Alder Lake and Raptor Lake CPUs" });
            result.Add(new SocketType { Id = id++, SocketName = "LGA2066", Manufacturer = "Intel", Generation = "Core X-series", Description = "Socket for High-End Desktop (HEDT) CPUs" });

            System.Diagnostics.Debug.WriteLine($"Fallback socket értékek betöltve: {result.Count} db");
            return result;
        }
    }
}
