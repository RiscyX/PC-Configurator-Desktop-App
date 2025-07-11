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
        public int ChipsetTypeId { get; set; }  // Módosítva: string helyett int
        public int SocketTypeId { get; set; }
        public string Socket { get; set; }      // Megtartva kompatibilitási okokból
        public int MemorySlots { get; set; }
        public int MaxMemoryGB { get; set; }
        public decimal Price { get; set; }
        public int PowerConsumption { get; set; }
        public string FormFactor { get; set; }
        public string RamType { get; set; }     // Új: RAM típus (DDR4, DDR5)
        public int PCIeSlots { get; set; }      // Új: PCIe slotok száma
        public int SATAPorts { get; set; }      // Új: SATA portok száma
        public int M2Slots { get; set; }        // Új: M.2 slotok száma
        public bool WiFi { get; set; }          // Új: Van-e WiFi
        public bool Bluetooth { get; set; }     // Új: Van-e Bluetooth

        public string Chipset { get; set; } // Kompatibilitási okokból megtartva

        public Motherboard() { }
        
        public Motherboard(int id, string name, string manufacturer, int chipsetTypeId, int socketTypeId)
        {
            Id = id;
            Name = name;
            Manufacturer = manufacturer;
            ChipsetTypeId = chipsetTypeId;
            SocketTypeId = socketTypeId;
            PowerConsumption = 30; // Alapértelmezett érték
        }

        // Régi konstruktor a kompatibilitás miatt
        public Motherboard(int id, string name, string manufacturer, string chipset, string socket) 
            : this(id, name, manufacturer, 1, 1) // Alapértelmezett chipset és socket ID
        {
            Chipset = chipset; // Régi Chipset mező kompatibilitásért
            Socket = socket;   // Régi Socket mező kompatibilitásért
        }

        public void SaveToDatabase()
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    using (var command = new System.Data.SqlClient.SqlCommand(
                        "INSERT INTO Motherboards (Name, Manufacturer, ChipsetTypeId, SocketTypeId, MemorySlots, MaxMemoryGB, " +
                        "FormFactor, Price, PowerConsumption) " + 
                        "VALUES (@Name, @Manufacturer, @ChipsetTypeId, @SocketTypeId, @MemorySlots, @MaxMemoryGB, " +
                        "@FormFactor, @Price, @PowerConsumption); " +
                        "SELECT SCOPE_IDENTITY();", connection))
                    {
                        // Alap paraméterek
                        command.Parameters.AddWithValue("@Name", Name);
                        command.Parameters.AddWithValue("@Manufacturer", Manufacturer);
                        command.Parameters.AddWithValue("@ChipsetTypeId", ChipsetTypeId);
                        command.Parameters.AddWithValue("@SocketTypeId", SocketTypeId);
                        
                        // Memória információk
                        command.Parameters.AddWithValue("@MemorySlots", MemorySlots > 0 ? MemorySlots : 4); // Alapértelmezett: 4 slot
                        command.Parameters.AddWithValue("@MaxMemoryGB", MaxMemoryGB > 0 ? MaxMemoryGB : 128); // Alapértelmezett: 128GB
                        
                        // Egyéb paraméterek
                        command.Parameters.AddWithValue("@FormFactor", !string.IsNullOrEmpty(FormFactor) ? FormFactor : "ATX"); // Alapértelmezett: ATX
                        command.Parameters.AddWithValue("@Price", Price);
                        command.Parameters.AddWithValue("@PowerConsumption", PowerConsumption > 0 ? PowerConsumption : 30); // Alapértelmezett: 30W
                        
                        // Lekérjük a beillesztett rekord ID-ját
                        var result = command.ExecuteScalar();
                        if (result != null)
                        {
                            Id = Convert.ToInt32(result);
                            System.Diagnostics.Debug.WriteLine($"Új alaplap létrehozva, ID: {Id}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba az alaplap mentésekor: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Belső hiba: {ex.InnerException.Message}");
                }
                throw; // Továbbadjuk a hibát, hogy a hívó kezelni tudja
            }
        }

        public void UpdateInDatabase()
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new System.Data.SqlClient.SqlCommand(
                        "UPDATE Motherboards SET Name = @Name, Manufacturer = @Manufacturer, ChipsetTypeId = @ChipsetTypeId, " +
                        "SocketTypeId = @SocketTypeId, MemorySlots = @MemorySlots, MaxMemoryGB = @MaxMemoryGB, " +
                        "FormFactor = @FormFactor, Price = @Price, PowerConsumption = @PowerConsumption " +
                        "WHERE Id = @Id", connection))
                    {
                        // Alap paraméterek
                        command.Parameters.AddWithValue("@Id", Id);
                        command.Parameters.AddWithValue("@Name", Name);
                        command.Parameters.AddWithValue("@Manufacturer", Manufacturer);
                        command.Parameters.AddWithValue("@ChipsetTypeId", ChipsetTypeId);
                        command.Parameters.AddWithValue("@SocketTypeId", SocketTypeId);
                        
                        // Memória információk
                        command.Parameters.AddWithValue("@MemorySlots", MemorySlots);
                        command.Parameters.AddWithValue("@MaxMemoryGB", MaxMemoryGB);
                        
                        // Egyéb paraméterek
                        command.Parameters.AddWithValue("@FormFactor", FormFactor);
                        command.Parameters.AddWithValue("@Price", Price);
                        command.Parameters.AddWithValue("@PowerConsumption", PowerConsumption);
                        
                        int rowsAffected = command.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine($"Alaplap frissítve, érintett sorok száma: {rowsAffected}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba az alaplap frissítésekor: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Belső hiba: {ex.InnerException.Message}");
                }
                throw; // Továbbadjuk a hibát, hogy a hívó kezelni tudja
            }
        }
        
        // Statikus metódus az alaplap betöltéséhez ID alapján
        public static Motherboard LoadFromDatabase(int id)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    // Ellenőrizzük, hogy létezik-e a Motherboards tábla
                    bool tableExists = false;
                    using (var cmd = new System.Data.SqlClient.SqlCommand(
                        "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Motherboards'", connection))
                    {
                        tableExists = ((int)cmd.ExecuteScalar()) > 0;
                    }

                    if (!tableExists)
                    {
                        System.Diagnostics.Debug.WriteLine("A Motherboards tábla nem létezik az adatbázisban!");
                        return null;
                    }
                    
                    // Egyszerű lekérdezés használata a biztos működés érdekében
                    string query = "SELECT * FROM Motherboards WHERE Id = @Id";
                    
                    using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var motherboard = new Motherboard();
                                
                                // Alapvető adatok beállítása biztonságosan
                                motherboard.Id = id;
                                
                                try { motherboard.Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : "Ismeretlen"; } 
                                catch (IndexOutOfRangeException) { motherboard.Name = "Ismeretlen"; }
                                
                                try { motherboard.Manufacturer = reader["Manufacturer"] != DBNull.Value ? reader["Manufacturer"].ToString() : "Ismeretlen"; }
                                catch (IndexOutOfRangeException) { motherboard.Manufacturer = "Ismeretlen"; }
                                
                                try { motherboard.ChipsetTypeId = reader["ChipsetTypeId"] != DBNull.Value ? Convert.ToInt32(reader["ChipsetTypeId"]) : 1; }
                                catch (IndexOutOfRangeException) { motherboard.ChipsetTypeId = 1; }
                                
                                try { motherboard.SocketTypeId = reader["SocketTypeId"] != DBNull.Value ? Convert.ToInt32(reader["SocketTypeId"]) : 1; }
                                catch (IndexOutOfRangeException) { motherboard.SocketTypeId = 1; }
                                
                                // Socket információ - ellenőrizzük a Socket oszlopot
                                try
                                {
                                    if (reader["Socket"] != DBNull.Value)
                                        motherboard.Socket = reader["Socket"].ToString();
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    motherboard.Socket = "Ismeretlen";
                                }
                                
                                // Opcionális mezők betöltése alapértelmezett értékekkel
                                try { motherboard.MemorySlots = reader["MemorySlots"] != DBNull.Value ? Convert.ToInt32(reader["MemorySlots"]) : 4; }
                                catch (IndexOutOfRangeException) { motherboard.MemorySlots = 4; }
                                
                                try { motherboard.MaxMemoryGB = reader["MaxMemoryGB"] != DBNull.Value ? Convert.ToInt32(reader["MaxMemoryGB"]) : 128; }
                                catch (IndexOutOfRangeException) { motherboard.MaxMemoryGB = 128; }
                                
                                try { motherboard.FormFactor = reader["FormFactor"] != DBNull.Value ? reader["FormFactor"].ToString() : "ATX"; }
                                catch (IndexOutOfRangeException) { motherboard.FormFactor = "ATX"; }
                                
                                try { motherboard.Price = reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : 0; }
                                catch (IndexOutOfRangeException) { motherboard.Price = 0; }
                                
                                // PowerConsumption beállítása alapértelmezett értékkel
                                try { motherboard.PowerConsumption = reader["PowerConsumption"] != DBNull.Value ? Convert.ToInt32(reader["PowerConsumption"]) : 30; }
                                catch (IndexOutOfRangeException) { motherboard.PowerConsumption = 30; }
                                
                                // Chipset neve a kompatibilitás miatt
                                try 
                                { 
                                    if (reader["Chipset"] != DBNull.Value)
                                        motherboard.Chipset = reader["Chipset"].ToString(); 
                                }
                                catch (IndexOutOfRangeException) 
                                { 
                                    // Ha nincs Chipset oszlop, próbáljunk keresni egy ChipsetType-ot az ID alapján
                                    motherboard.Chipset = "Ismeretlen"; 
                                }
                                
                                return motherboard;
                            }
                        }
                    }
                }
                
                return null; // Nem található ilyen ID-jú alaplap
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba az alaplap betöltésekor (ID: {id}): {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Belső hiba: {ex.InnerException.Message}");
                }
                
                // Hiba esetén visszaadunk egy alapértelmezett alaplapot
                var defaultMotherboard = new Motherboard
                {
                    Id = id,
                    Name = "Alaplap adatok nem elérhetők",
                    Manufacturer = "Ismeretlen",
                    Chipset = "Ismeretlen",
                    Socket = "Ismeretlen",
                    ChipsetTypeId = 1,
                    SocketTypeId = 1,
                    FormFactor = "ATX",
                    Price = 0,
                    PowerConsumption = 30,
                    MemorySlots = 4,
                    MaxMemoryGB = 128
                };
                
                return defaultMotherboard;
            }
        }
    }
}
