using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Configurator.Models
{
    public class ConfigurationModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Price { get; set; }
        public int PerformanceScore { get; set; }
        public List<ComponentInfo> Components { get; set; } = new List<ComponentInfo>();

        public ConfigurationModel()
        {
        }

        public ConfigurationModel(int id, string name, int userId, DateTime createdAt, decimal price, int performanceScore)
        {
            Id = id;
            Name = name;
            UserId = userId;
            CreatedAt = createdAt;
            Price = price;
            PerformanceScore = performanceScore;
            Components = new List<ComponentInfo>();
        }

        public void SaveToDatabase()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Konfiguráció mentése
                        int configId;
                        using (var command = new SqlCommand(
                            @"INSERT INTO Configurations (Name, UserId, CreatedDate, TotalPrice, PerformanceScore) 
                              VALUES (@Name, @UserId, @CreatedDate, @TotalPrice, @PerformanceScore);
                              SELECT CAST(SCOPE_IDENTITY() as int)", connection, transaction))
                        {
                            command.Parameters.AddWithValue("@Name", Name);
                            command.Parameters.AddWithValue("@UserId", UserId);
                            command.Parameters.AddWithValue("@CreatedDate", CreatedAt);
                            command.Parameters.AddWithValue("@TotalPrice", Price);
                            command.Parameters.AddWithValue("@PerformanceScore", PerformanceScore);
                            configId = (int)command.ExecuteScalar();
                        }

                        // Komponensek hozzáadása a konfigurációhoz
                        foreach (var component in Components)
                        {
                            using (var command = new SqlCommand(
                                @"INSERT INTO ConfigurationComponents (ConfigurationId, ComponentType, ComponentId) 
                                  VALUES (@ConfigurationId, @ComponentType, @ComponentId)", connection, transaction))
                            {
                                command.Parameters.AddWithValue("@ConfigurationId", configId);
                                command.Parameters.AddWithValue("@ComponentType", component.Type);
                                command.Parameters.AddWithValue("@ComponentId", component.Id);
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        Id = configId; // Beállítjuk az új ID-t
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw; // Továbbdobjuk a kivételt a hívó számára
                    }
                }
            }
        }

        public static List<ConfigurationModel> LoadUserConfigurations(int userId)
        {
            List<ConfigurationModel> configurations = new List<ConfigurationModel>();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                // Konfiguráció főadatok lekérése
                using (var command = new SqlCommand(
                    @"SELECT Id, Name, UserId, CreatedDate, TotalPrice, PerformanceScore 
                      FROM Configurations WHERE UserId = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ConfigurationModel config = new ConfigurationModel(
                                id: reader.GetInt32(0),
                                name: reader.GetString(1),
                                userId: reader.GetInt32(2),
                                createdAt: reader.GetDateTime(3),
                                price: reader.GetDecimal(4),
                                performanceScore: reader.GetInt32(5)
                            );
                            configurations.Add(config);
                        }
                    }
                }
                
                // Konfiguráció komponensek lekérése
                foreach (var config in configurations)
                {
                    LoadConfigurationComponents(connection, config);
                }
            }
            
            return configurations;
        }

        private static void LoadConfigurationComponents(SqlConnection connection, ConfigurationModel config)
        {
            // Itt valós lekérdezést kellene használni, amely minden komponens típushoz lekéri az adatokat
            // Ez egy egyszerűsített példa, ami a közvetlen kapcsolatot mutatja
            using (var command = new SqlCommand(
                @"SELECT cc.ComponentType, cc.ComponentId, 
                  CASE 
                    WHEN cc.ComponentType = 'CPU' THEN c.Name
                    WHEN cc.ComponentType = 'GPU' THEN g.Name
                    WHEN cc.ComponentType = 'RAM' THEN r.Name
                    WHEN cc.ComponentType = 'Storage' THEN s.Name
                    WHEN cc.ComponentType = 'Motherboard' THEN m.Name
                    WHEN cc.ComponentType = 'PSU' THEN p.Name
                    WHEN cc.ComponentType = 'Case' THEN ca.Name
                  END as Name,
                  CASE 
                    WHEN cc.ComponentType = 'CPU' THEN CONCAT(c.Cores, ' mag / ', c.Threads, ' szál, ', c.BaseClockGHz, ' GHz')
                    WHEN cc.ComponentType = 'GPU' THEN CONCAT(g.Memory, 'GB ', g.MemoryType)
                    WHEN cc.ComponentType = 'RAM' THEN CONCAT(r.CapacityGB, 'GB ', r.Type, ', ', r.SpeedMHz, ' MHz')
                    WHEN cc.ComponentType = 'Storage' THEN CONCAT(s.Capacity, 'GB ', s.Type)
                    WHEN cc.ComponentType = 'Motherboard' THEN CONCAT(m.Chipset, ', ', m.Socket)
                    WHEN cc.ComponentType = 'PSU' THEN CONCAT(p.Wattage, 'W, ', p.Efficiency)
                    WHEN cc.ComponentType = 'Case' THEN CONCAT(ca.FormFactor, ', ', ca.Color)
                  END as Details,
                  COALESCE(c.Price, g.Price, r.Price, s.Price, m.Price, p.Price, ca.Price, 0) as Price,
                  COALESCE(c.PowerConsumption, g.PowerConsumption, r.PowerConsumption, s.PowerConsumption, 
                           m.PowerConsumption, 0, 0) as Power
                FROM ConfigurationComponents cc
                LEFT JOIN CPUs c ON cc.ComponentType = 'CPU' AND cc.ComponentId = c.Id
                LEFT JOIN GPUs g ON cc.ComponentType = 'GPU' AND cc.ComponentId = g.Id
                LEFT JOIN RAMs r ON cc.ComponentType = 'RAM' AND cc.ComponentId = r.Id
                LEFT JOIN Storages s ON cc.ComponentType = 'Storage' AND cc.ComponentId = s.Id
                LEFT JOIN Motherboards m ON cc.ComponentType = 'Motherboard' AND cc.ComponentId = m.Id
                LEFT JOIN PSUs p ON cc.ComponentType = 'PSU' AND cc.ComponentId = p.Id
                LEFT JOIN Cases ca ON cc.ComponentType = 'Case' AND cc.ComponentId = ca.Id
                WHERE cc.ConfigurationId = @ConfigId", connection))
            {
                command.Parameters.AddWithValue("@ConfigId", config.Id);
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string componentType = reader.GetString(0);
                        int componentId = reader.GetInt32(1);
                        string name = reader.GetString(2);
                        string details = reader.GetString(3);
                        decimal price = reader.GetDecimal(4);
                        int power = reader.GetInt32(5);
                        
                        config.Components.Add(new ComponentInfo(
                            id: componentId,
                            name: name,
                            details: details,
                            price: price,
                            power: power,
                            type: componentType
                        ));
                    }
                }
            }
        }
    }
}
