using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Configuration;
using PC_Configurator.Models;
using PC_Configurator.Helpers;
using Configuration = PC_Configurator.Models.ConfigurationModel;

namespace PC_Configurator.Views.App
{
    /// <summary>
    /// Interaction logic for ConfigBuilder.xaml
    /// </summary>
    public partial class ConfigBuilder : UserControl, INotifyPropertyChanged
    {
        // Aktuális felhasználó azonosítója
        private int _currentUserId;
        
        // Kiválasztott komponensek
        private Dictionary<string, ComponentInfo> _selectedComponents = new Dictionary<string, ComponentInfo>();

        // Componenet model structs for storing selected items
        private class ComponentItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Details { get; set; }
            public decimal Price { get; set; }
            public int PowerConsumption { get; set; }
        }
        
        // Property changed implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        // Property for storing total price
        private decimal _totalPrice = 0;
        public decimal TotalPrice 
        { 
            get { return _totalPrice; } 
            set 
            { 
                if (_totalPrice != value)
                {
                    _totalPrice = value;
                    TotalPriceBlock.Text = $"{_totalPrice:N0} Ft";
                    NotifyPropertyChanged(nameof(TotalPrice));
                }
            }
        }
        
        // Property for storing total power consumption
        private int _totalPower = 0;
        public int TotalPower 
        { 
            get { return _totalPower; } 
            set 
            { 
                if (_totalPower != value)
                {
                    _totalPower = value;
                    PowerConsumptionBlock.Text = $"{_totalPower} W";
                    NotifyPropertyChanged(nameof(TotalPower));
                }
            }
        }
        
        // Store the selected components
        private Dictionary<string, ComponentItem> selectedComponents = new Dictionary<string, ComponentItem>();
        
        // Dictionary of component cards by type
        private Dictionary<string, Border> componentCards = new Dictionary<string, Border>();
        
        // Dictionary of component name blocks by type
        private Dictionary<string, TextBlock> componentNameBlocks = new Dictionary<string, TextBlock>();
        
        // Dictionary of component detail blocks by type
        private Dictionary<string, TextBlock> componentDetailBlocks = new Dictionary<string, TextBlock>();
        
        // Dictionary for tracking compatibility
        private Dictionary<string, bool> componentCompatibility = new Dictionary<string, bool>();
        
        // Currently selected component type (for showing selection dialog)
        private string currentComponentType = "";
        
        // Paraméter nélküli konstruktor a navigációhoz
        public ConfigBuilder() : this(0)
        {
            // Minden inicializálást a paraméteres konstruktor végez 0 paraméterrel
        }
        
        public ConfigBuilder(int userId = 0)
        {
            InitializeComponent();
            
            // Debug üzenet a konstruktorból
            System.Diagnostics.Debug.WriteLine($"ConfigBuilder konstruktor meghívva - userId: {userId}");
            
            // Beállítjuk az aktuális felhasználó azonosítóját
            _currentUserId = userId;
            
            // Sorrendben inicializáljuk a különböző adatokat
            // 1. Alapszótárak inicializálása
            InitializeDictionaries();
            
            // 2. Komponens UI elemek szótárainak inicializálása
            SetupComponentDictionaries();
            
            // 3. Betöltjük a Components osztálytól érkező alkatrészeket
            LoadComponentsFromStore();
            
            // 4. Frissítjük az árakat és egyéb adatokat
            UpdateTotals();
            
            // 5. Adatbázis ellenőrzése a háttérben (csak debug céllal)
            Task.Run(() => CheckDatabaseTables());
        }
        
        // Segédfüggvény az adatbázis táblák ellenőrzéséhez
        private async Task CheckDatabaseTables()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Adatbázis táblák ellenőrzése...");
                
                // Ellenőrizzük a Motherboards és SocketTypes táblákat
                bool motherboardsExist = await CheckIfTableExistsAsync("Motherboards");
                bool socketTypesExist = await CheckIfTableExistsAsync("SocketTypes");
                bool motherboardHasSocketTypeId = await CheckIfTableColumnExistsAsync("Motherboards", "SocketTypeId");
                
                System.Diagnostics.Debug.WriteLine($"Táblák ellenőrzése: Motherboards létezik: {motherboardsExist}, " +
                                              $"SocketTypes létezik: {socketTypesExist}, " +
                                              $"Motherboards.SocketTypeId létezik: {motherboardHasSocketTypeId}");
                
                // Ha létezik a Motherboards tábla, ellenőrizzük a rekordok számát
                if (motherboardsExist)
                {
                    int count = await GetTableRowCountAsync("Motherboards");
                    System.Diagnostics.Debug.WriteLine($"A Motherboards táblában {count} rekord található.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba az adatbázis ellenőrzésekor: {ex.Message}");
            }
        }
        
        // Aszinkron függvény a táblák létezésének ellenőrzésére
        private async Task<bool> CheckIfTableExistsAsync(string tableName)
        {
            try
            {
                string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                if (string.IsNullOrEmpty(connStr))
                {
                    return false;
                }
                
                using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                {
                    await conn.OpenAsync();
                    
                    string query = @"
                        SELECT COUNT(*)
                        FROM INFORMATION_SCHEMA.TABLES
                        WHERE TABLE_NAME = @TableName";
                    
                    using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TableName", tableName);
                        int count = (int)await cmd.ExecuteScalarAsync();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a tábla létezésének ellenőrzésekor ({tableName}): {ex.Message}");
                return false;
            }
        }
        
        // Aszinkron függvény az oszlop létezésének ellenőrzésére
        private async Task<bool> CheckIfTableColumnExistsAsync(string tableName, string columnName)
        {
            try
            {
                string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                if (string.IsNullOrEmpty(connStr))
                {
                    return false;
                }
                
                using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                {
                    await conn.OpenAsync();
                    
                    string query = @"
                        SELECT COUNT(*)
                        FROM INFORMATION_SCHEMA.COLUMNS
                        WHERE TABLE_NAME = @TableName
                        AND COLUMN_NAME = @ColumnName";
                    
                    using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TableName", tableName);
                        cmd.Parameters.AddWithValue("@ColumnName", columnName);
                        int count = (int)await cmd.ExecuteScalarAsync();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba az oszlop létezésének ellenőrzésekor ({tableName}.{columnName}): {ex.Message}");
                return false;
            }
        }
        
        // Aszinkron függvény a táblában lévő sorok számának lekérdezéséhez
        private async Task<int> GetTableRowCountAsync(string tableName)
        {
            try
            {
                string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                if (string.IsNullOrEmpty(connStr))
                {
                    return 0;
                }
                
                using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                {
                    await conn.OpenAsync();
                    
                    string query = $"SELECT COUNT(*) FROM {tableName}";
                    
                    using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                    {
                        return (int)await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a sorok számának lekérdezésekor ({tableName}): {ex.Message}");
                return 0;
            }
        }
        
        // Ellenőrizzük, hogy létezik-e a megadott nézet
        private bool CheckIfViewExists(string viewName)
        {
            try
            {
                string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                if (string.IsNullOrEmpty(connStr))
                {
                    return false;
                }
                
                using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                {
                    conn.Open();
                    
                    string query = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.VIEWS 
                        WHERE TABLE_NAME = @ViewName";
                    
                    using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ViewName", viewName);
                        int count = (int)cmd.ExecuteScalar();
                        
                        System.Diagnostics.Debug.WriteLine($"A {viewName} nézet létezik: {count > 0}");
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a nézet létezésének ellenőrzésekor ({viewName}): {ex.Message}");
                return false;
            }
        }
        
        // Set up mock data for demo purposes
        private void SetupMockData()
        {
            // This is just a demo implementation - would be replaced by real data
            // Set up some demo components
            SelectComponent("CPU", 2, "AMD Ryzen 7 5800X", "8 mag / 16 szál, 3.8 GHz (4.7 GHz)", 89900M, 105);
            SelectComponent("GPU", 3, "NVIDIA RTX 3070", "8 GB GDDR6", 159900M, 220);
            
            // Update compatibility and performance metrics
            UpdateCompatibilityStatus();
            UpdatePerformanceRating();
        }
        
        private void ComponentCard_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag != null)
            {
                string componentType = border.Tag.ToString();
                currentComponentType = componentType;
                
                ShowComponentSelectionDialog(componentType);
            }
        }
        
        private void ShowComponentSelectionDialog(string componentType)
        {
            try
            {
                // Adatbázisból betöltjük az összes ilyen típusú komponenst és kiválaszthatóvá tesszük
                List<ComponentInfo> components = LoadComponentsFromDatabase(componentType);
                
                if (components != null && components.Count > 0)
                {
                    // Itt egy valós dialógusablak nyílik meg a komponensek listázásával
                    var selectedComponent = ShowComponentListDialog(componentType, components);
                    
                    if (selectedComponent != null)
                    {
                        // A kiválasztott komponens hozzáadása a konfigurációhoz
                        SelectComponent(
                            componentType,
                            selectedComponent.Id,
                            selectedComponent.Name,
                            selectedComponent.Details,
                            selectedComponent.Price,
                            selectedComponent.Power
                        );
                        
                        // Update compatibility after selection
                        UpdateCompatibilityStatus();
                        UpdatePerformanceRating();
                    }
                }
                else
                {
                    // Ha nincs komponens az adatbázisban, jelezzük a felhasználónak
                    MessageBox.Show($"Nem találhatók {componentType} komponensek az adatbázisban.", 
                                  "Komponens választó", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Komponensek betöltése");
                MessageBox.Show($"Hiba történt a {componentType} komponensek betöltése közben.", 
                              "Betöltési hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void SelectComponent(string type, int id, string name, string details, decimal price, int power)
        {
            // Store the selected component
            selectedComponents[type] = new ComponentItem
            {
                Id = id,
                Name = name,
                Details = details,
                Price = price,
                PowerConsumption = power
            };
            
            // Update the UI
            if (componentNameBlocks.ContainsKey(type))
            {
                componentNameBlocks[type].Text = name;
            }
            
            if (componentDetailBlocks.ContainsKey(type))
            {
                componentDetailBlocks[type].Text = details;
            }
            
            // Mark the card as selected
            if (componentCards.ContainsKey(type))
            {
                componentCards[type].Style = (Style)FindResource("ComponentCardSelected");
            }
            
            // Update the total price and power
            UpdateTotals();
        }

        private void UpdateCompatibilityStatus()
        {
            List<string> incompatibilities = new List<string>();
            bool allCompatible = true;
            
            // Ellenőrizzük az alaplap és CPU kompatibilitást
            if (selectedComponents.ContainsKey("CPU") && selectedComponents.ContainsKey("Motherboard"))
            {
                bool cpuMbCompatible = CheckCPUMotherboardCompatibility();
                if (!cpuMbCompatible)
                {
                    incompatibilities.Add("CPU és alaplap");
                    allCompatible = false;
                }
            }
            
            // Ellenőrizzük az RAM és alaplap kompatibilitást
            if (selectedComponents.ContainsKey("RAM") && selectedComponents.ContainsKey("Motherboard"))
            {
                bool ramMbCompatible = CheckRAMMotherboardCompatibility();
                if (!ramMbCompatible)
                {
                    incompatibilities.Add("RAM és alaplap");
                    allCompatible = false;
                }
            }
            
            // Update the compatibility message
            if (selectedComponents.ContainsKey("CPU") && selectedComponents.ContainsKey("Motherboard"))
            {
                if (allCompatible)
                {
                    CompatibilityMessage.Text = "Az összes kiválasztott komponens kompatibilis egymással.";
                    CompatibilityMessage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"));
                }
                else
                {
                    CompatibilityMessage.Text = $"Kompatibilitási probléma! Ellenőrizd a következőket: {string.Join(", ", incompatibilities)}";
                    CompatibilityMessage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F44336"));
                }
            }
            else
            {
                CompatibilityMessage.Text = "Válassz CPU-t és Alaplapot a kompatibilitás ellenőrzéséhez.";
                CompatibilityMessage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#AAAAAA"));
            }
        }
        
        private void UpdatePerformanceRating()
        {
            // Simple performance rating calculation based on selected components
            int performanceScore = 0;
            string recommendedUse = "Általános felhasználás";
            
            // Calculate performance score based on components (mock implementation)
            if (selectedComponents.ContainsKey("CPU") && selectedComponents["CPU"] != null)
            {
                performanceScore += 25;  // CPU adds 25 points
            }
            
            if (selectedComponents.ContainsKey("GPU") && selectedComponents["GPU"] != null)
            {
                performanceScore += 35;  // GPU adds 35 points
            }
            
            if (selectedComponents.ContainsKey("RAM") && selectedComponents["RAM"] != null)
            {
                performanceScore += 15;  // RAM adds 15 points
            }
            
            if (selectedComponents.ContainsKey("Storage") && selectedComponents["Storage"] != null)
            {
                performanceScore += 10;  // Storage adds 10 points
            }
            
            // Set recommended use based on performance score
            if (performanceScore >= 60)
            {
                recommendedUse = "Játék és professzionális felhasználás";
            }
            else if (performanceScore >= 40)
            {
                recommendedUse = "Modern játékok és multimédia";
            }
            else if (performanceScore >= 20)
            {
                recommendedUse = "Irodai munka és alapvető multimédia";
            }
            
            // Update the UI
            PerformanceRating.Text = $"{performanceScore}/100";
            RecommendedUseBlock.Text = recommendedUse;
            
            // Update the progress bar
            var progressBar = PerformanceRating.Parent as Grid;
            if (progressBar != null && progressBar.Children.Count > 0 && progressBar.Children[0] is ProgressBar)
            {
                (progressBar.Children[0] as ProgressBar).Value = performanceScore;
            }
        }
        
        private void SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            if (selectedComponents.Count > 0)
            {
                // Konfiguráció név bekérése a felhasználótól
                string configName = PromptForConfigurationName();
                
                if (string.IsNullOrEmpty(configName))
                {
                    return; // Ha a felhasználó mégsem ad nevet, akkor kilépünk
                }
                
                // Komponensek konvertálása a megfelelő modell objektumokra
                SaveConfiguration(configName);
            }
            else
            {
                MessageBox.Show("Nem menthető üres konfiguráció. Kérlek, válassz komponenseket!", 
                              "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        
        private void ClearConfig_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.Yes;
            
            if (selectedComponents.Count > 0)
            {
                result = MessageBox.Show("Biztosan törölni szeretnéd az összes kiválasztott komponenst?", 
                                        "Konfiguráció törlése", MessageBoxButton.YesNo, MessageBoxImage.Question);
            }
            
            if (result == MessageBoxResult.Yes)
            {
                ClearConfig();
            }
        }
        
        public void ClearConfig()
        {
            // Clear the selected components
            selectedComponents.Clear();
            
            // Reset the UI
            foreach (var type in componentNameBlocks.Keys)
            {
                componentNameBlocks[type].Text = $"Válassz {TranslateComponentType(type).ToLower()}";
                componentDetailBlocks[type].Text = "Nincs kiválasztva";
                componentCards[type].Style = (Style)FindResource("ComponentCard");
            }
            
            // Reset total price and power
            TotalPrice = 0;
            TotalPower = 0;
            
            // Reset compatibility message
            CompatibilityMessage.Text = "Válassz komponenseket a kompatibilitás ellenőrzéséhez";
            CompatibilityMessage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#AAAAAA"));
            
            // Reset performance rating
            UpdatePerformanceRating();
        }
        
        private void SetupComponentDictionaries()
        {
            // Initialize component cards dictionary if it's not already initialized
            if (componentCards == null || componentCards.Count == 0)
            {
                componentCards = new Dictionary<string, Border>
                {
                    { "CPU", CPUCard },
                    { "Motherboard", MotherboardCard },
                    { "RAM", RAMCard },
                    { "GPU", GPUCard },
                    { "Storage", StorageCard },
                    { "PSU", PSUCard },
                    { "Case", CaseCard }
                };
            }
            
            // Check if the dictionaries are already initialized before adding items
            if (!componentNameBlocks.ContainsKey("CPU"))
            {
                componentNameBlocks["CPU"] = CPUNameBlock;
                componentNameBlocks["GPU"] = GPUNameBlock;
                componentNameBlocks["RAM"] = RAMNameBlock;
                componentNameBlocks["Storage"] = StorageNameBlock;
                componentNameBlocks["Motherboard"] = MotherboardNameBlock;
                componentNameBlocks["PSU"] = PSUNameBlock;
                componentNameBlocks["Case"] = CaseNameBlock;
            }
            
            if (!componentDetailBlocks.ContainsKey("CPU"))
            {
                componentDetailBlocks["CPU"] = CPUDetailBlock;
                componentDetailBlocks["GPU"] = GPUDetailBlock;
                componentDetailBlocks["RAM"] = RAMDetailBlock;
                componentDetailBlocks["Storage"] = StorageDetailBlock;
                componentDetailBlocks["Motherboard"] = MotherboardDetailBlock;
                componentDetailBlocks["PSU"] = PSUDetailBlock;
                componentDetailBlocks["Case"] = CaseDetailBlock;
            }
            
            // Initialize compatibility dictionary if needed
            if (!componentCompatibility.ContainsKey("CPU"))
            {
                componentCompatibility["CPU"] = false;
                componentCompatibility["Motherboard"] = false;
                componentCompatibility["RAM"] = false;
            }
        }
        
        private void InitializeDictionaries()
        {
            // Initialize _selectedComponents if needed
            if (_selectedComponents == null || _selectedComponents.Count == 0)
            {
                _selectedComponents = new Dictionary<string, ComponentInfo>
                {
                    { "CPU", null },
                    { "Motherboard", null },
                    { "RAM", null },
                    { "GPU", null },
                    { "Storage", null },
                    { "PSU", null },
                    { "Case", null }
                };
            }

            // Initialize dictionaries only if they haven't been initialized yet
            if (componentNameBlocks == null)
            {
                componentNameBlocks = new Dictionary<string, TextBlock>();
            }

            if (componentDetailBlocks == null)
            {
                componentDetailBlocks = new Dictionary<string, TextBlock>();
            }

            // Initialize selected components too
            if (selectedComponents == null)
            {
                selectedComponents = new Dictionary<string, ComponentItem>();
            }
            
            // Initialize compatibility dictionary
            if (componentCompatibility == null)
            {
                componentCompatibility = new Dictionary<string, bool>();
            }

            // Always update UI elements
            TotalPriceBlock.Text = "0 Ft";
            PerformanceScoreBlock.Text = "0/100";
            PerformanceBar.Value = 0;
        }
        
        // Becsült árak komponens típus alapján
        private decimal GetEstimatedPrice(string componentType)
        {
            switch (componentType)
            {
                case "CPU": return 85000M;
                case "GPU": return 150000M;
                case "RAM": return 35000M;
                case "Storage": return 30000M;
                case "Motherboard": return 45000M;
                case "PSU": return 25000M;
                case "Case": return 20000M;
                default: return 0M;
            }
        }
        
        // Becsült teljesítményfelvétel komponens típus alapján
        private int GetEstimatedPower(string componentType)
        {
            switch (componentType)
            {
                case "CPU": return 95;
                case "GPU": return 220;
                case "RAM": return 10;
                case "Storage": return 5;
                case "Motherboard": return 20;
                case "PSU": return 0;
                case "Case": return 0;
                default: return 0;
            }
        }
        
        private string TranslateComponentType(string type)
        {
            switch (type)
            {
                case "CPU": return "processzort";
                case "GPU": return "videókártyát";
                case "RAM": return "memóriát";
                case "Storage": return "tárhelyet";
                case "Motherboard": return "alaplapot";
                case "PSU": return "tápegységet";
                case "Case": return "számítógépházat";
                default: return type;
            }
        }
        
        // CPU és alaplap kompatibilitás ellenőrzése
        private bool CheckCPUMotherboardCompatibility()
        {
            try
            {
                int cpuId = selectedComponents["CPU"].Id;
                int motherboardId = selectedComponents["Motherboard"].Id;
                
                string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                {
                    conn.Open();
                    // Ellenőrizzük, hogy a CPU és az Alaplap socket-je egyezik-e
                    string query = @"
                    SELECT COUNT(*) FROM CPUs c 
                    JOIN Motherboards m ON c.SocketTypeId = m.SocketTypeId
                    WHERE c.Id = @CpuId AND m.Id = @MbId";
                    
                    using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CpuId", cpuId);
                        cmd.Parameters.AddWithValue("@MbId", motherboardId);
                        
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception)
            {
                // Ha bármilyen hiba történik, feltételezzük, hogy nem kompatibilis
                return false;
            }
        }
        
        // RAM és alaplap kompatibilitás ellenőrzése
        private bool CheckRAMMotherboardCompatibility()
        {
            try
            {
                int ramId = selectedComponents["RAM"].Id;
                int motherboardId = selectedComponents["Motherboard"].Id;
                
                string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                {
                    conn.Open();
                    // Itt ellenőrizhetjük például, hogy az alaplap támogatja-e a RAM típusát vagy órajel-sebességét
                    // Egyszerűsítésként feltételezzük, hogy az Alaplap MaxMemoryGB értékének nagyobbnak kell lennie, mint a RAM kapacitása
                    string query = @"
                    SELECT mb.MaxMemoryGB, r.CapacityGB
                    FROM Motherboards mb, RAMs r
                    WHERE mb.Id = @MbId AND r.Id = @RamId";
                    
                    using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MbId", motherboardId);
                        cmd.Parameters.AddWithValue("@RamId", ramId);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int maxMemoryGB = reader.GetInt32(0);
                                int ramCapacityGB = reader.GetInt32(1);
                                
                                // Ellenőrizzük, hogy az alaplap támogatja-e a RAM kapacitását
                                return ramCapacityGB <= maxMemoryGB;
                            }
                        }
                    }
                }
                
                return false; // Ha nem találtuk az adatokat, akkor feltételezzük, hogy nem kompatibilis
            }
            catch (Exception)
            {
                // Ha bármilyen hiba történik, feltételezzük, hogy nem kompatibilis
                return false;
            }
        }

        private string PromptForConfigurationName()
        {
            // Egyszerű dialógus ablak a konfiguráció nevének bekéréséhez
            string defaultName = $"Saját konfiguráció - {DateTime.Now:yyyy.MM.dd}";
            
            // Dialógus létrehozása
            Window dialog = new Window
            {
                Title = "Konfiguráció mentése",
                Width = 400,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.ToolWindow
            };
            
            // Grid panel létrehozása a dialógus tartalmához
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.Margin = new Thickness(20);
            
            // Cím szöveg
            TextBlock title = new TextBlock
            {
                Text = "Add meg a konfiguráció nevét:",
                Margin = new Thickness(0, 0, 0, 10),
                FontSize = 16
            };
            Grid.SetRow(title, 0);
            
            // TextBox a név megadásához
            TextBox nameTextBox = new TextBox
            {
                Text = defaultName,
                Padding = new Thickness(8),
                Margin = new Thickness(0, 0, 0, 20),
                FontSize = 14
            };
            Grid.SetRow(nameTextBox, 1);
            
            // Gombsor
            StackPanel buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            
            Button cancelButton = new Button
            {
                Content = "Mégsem",
                Padding = new Thickness(20, 8, 20, 8),
                Margin = new Thickness(0, 0, 10, 0)
            };
            
            Button saveButton = new Button
            {
                Content = "Mentés",
                Padding = new Thickness(20, 8, 20, 8),
                IsDefault = true
            };
            
            buttonPanel.Children.Add(cancelButton);
            buttonPanel.Children.Add(saveButton);
            Grid.SetRow(buttonPanel, 2);
            
            // Elemek hozzáadása a gridhez
            grid.Children.Add(title);
            grid.Children.Add(nameTextBox);
            grid.Children.Add(buttonPanel);
            
            // Grid beállítása a dialógus tartalmának
            dialog.Content = grid;
            
            // Visszatérési érték tárolására
            string result = null;
            
            // Gombok eseménykezelői
            cancelButton.Click += (s, e) =>
            {
                dialog.DialogResult = false;
            };
            
            saveButton.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(nameTextBox.Text))
                {
                    result = nameTextBox.Text;
                    dialog.DialogResult = true;
                }
                else
                {
                    MessageBox.Show("A konfiguráció neve nem lehet üres!", 
                                  "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            };
            
            // Dialógus megjelenítése
            bool? dialogResult = dialog.ShowDialog();
            
            // Visszatérés a felhasználó által megadott névvel, vagy null ha mégsem mentett
            return dialogResult == true ? result : null;
        }
        
        public void SaveConfiguration(string name)
        {
            try
            {
                // Ellenőrizzük, hogy van-e legalább egy komponens kiválasztva
                if (!selectedComponents.Any(c => c.Value != null))
                {
                    MessageBox.Show("Legalább egy komponenst ki kell választani a konfiguráció mentéséhez!",
                                  "Hiányzó Komponensek", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var config = new ConfigurationModel
                {
                    Name = name,
                    UserId = _currentUserId,
                    CreatedAt = DateTime.Now,
                    Price = TotalPrice,
                    PerformanceScore = CalculatePerformanceScore(),
                    Components = new List<ComponentInfo>()
                };
                
                // ID-k beállítása és komponensek hozzáadása a konfigurációhoz
                foreach (var componentPair in selectedComponents)
                {
                    string type = componentPair.Key;
                    var component = componentPair.Value;
                    
                    if (component != null)
                    {
                        // ID beállítása a típusnak megfelelő tulajdonságban
                        switch (type)
                        {
                            case "CPU":
                                config.CPUId = component.Id;
                                break;
                            case "GPU":
                                config.GPUId = component.Id;
                                break;
                            case "RAM":
                                config.RAMId = component.Id;
                                break;
                            case "Storage":
                                config.StorageId = component.Id;
                                break;
                            case "Motherboard":
                                config.MotherboardId = component.Id;
                                break;
                            case "PSU":
                                config.PSUId = component.Id;
                                break;
                            case "Case":
                                config.CaseId = component.Id;
                                break;
                        }
                        
                        // Komponens hozzáadása a listához is (UI megjelenítéshez)
                        var componentInfo = new ComponentInfo
                        {
                            Id = component.Id,
                            Name = component.Name,
                            Details = component.Details,
                            Price = component.Price,
                            Power = component.PowerConsumption,
                            Type = type
                        };
                        
                        config.Components.Add(componentInfo);
                    }
                }

                // Komponensek ellenőrzése a mentés előtt
                bool componentsValid = ValidateComponents(config);
                if (!componentsValid)
                {
                    return; // Ha a komponensek érvénytelenek, nem megyünk tovább
                }

                // Konfiguráció mentése az adatbázisba
                System.Diagnostics.Debug.WriteLine($"Konfiguráció mentése: {name}, komponensek száma: {config.Components.Count}");
                bool success = config.SaveToDatabase();
                
                if (success)
                {
                    MessageBox.Show($"A konfiguráció sikeresen mentésre került '{name}' néven!",
                                  "Sikeres Mentés", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Hiba esetén részletes információkat írunk ki
                    System.Diagnostics.Debug.WriteLine("Hiba a konfiguráció mentése közben:");
                    
                    // Komponensek kiírása debug célokra
                    DumpComponentIds(config);
                    
                    MessageBox.Show("Hiba történt a konfiguráció mentése közben.\n\nEllenőrizze, hogy minden kiválasztott komponens megfelelően van rögzítve az adatbázisban.",
                                  "Mentési Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a konfiguráció mentése közben: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Belső hiba: {ex.InnerException.Message}");
                }
                ErrorHandler.HandleDatabaseError(ex, "mentés", "konfiguráció");
            }
        }
        
        // Komponensek ellenőrzése mentés előtt
        private bool ValidateComponents(ConfigurationModel config)
        {
            // Komponens ID-k érvényességének ellenőrzése
            foreach (var component in config.Components)
            {
                if (component.Id <= 0)
                {
                    MessageBox.Show($"A(z) {component.Type} komponens nem rendelkezik érvényes azonosítóval. Kérjük, válasszon másik komponenst.",
                                  "Érvénytelen Komponens", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                
                // Ellenőrizzük, hogy a komponens létezik-e az adatbázisban
                if (!CheckComponentExistence(component.Type, component.Id))
                {
                    MessageBox.Show($"A(z) {component.Name} ({component.Type}) nem található az adatbázisban vagy hiányos adatokkal rendelkezik. Kérjük, válasszon másik komponenst.",
                                  "Hiányzó Komponens", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            return true;
        }
        
        // Komponens létezésének ellenőrzése az adatbázisban
        private bool CheckComponentExistence(string componentType, int id)
        {
            try
            {
                string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                string tableName = GetTableNameByType(componentType);
                
                using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                {
                    conn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand($"SELECT COUNT(*) FROM {tableName} WHERE Id = @Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a komponens létezésének ellenőrzésekor: {ex.Message}");
                return false;
            }
        }
        
        // Komponens ID-k kiírása hibakereséshez
        private void DumpComponentIds(ConfigurationModel config)
        {
            System.Diagnostics.Debug.WriteLine($"Konfiguráció ID: {config.Id}");
            System.Diagnostics.Debug.WriteLine($"CPU ID: {config.CPUId}");
            System.Diagnostics.Debug.WriteLine($"GPU ID: {config.GPUId}");
            System.Diagnostics.Debug.WriteLine($"RAM ID: {config.RAMId}");
            System.Diagnostics.Debug.WriteLine($"Storage ID: {config.StorageId}");
            System.Diagnostics.Debug.WriteLine($"Motherboard ID: {config.MotherboardId}");
            System.Diagnostics.Debug.WriteLine($"PSU ID: {config.PSUId}");
            System.Diagnostics.Debug.WriteLine($"Case ID: {config.CaseId}");
            
            System.Diagnostics.Debug.WriteLine("Komponens lista:");
            foreach (var component in config.Components)
            {
                System.Diagnostics.Debug.WriteLine($"  {component.Type}: ID={component.Id}, Name={component.Name}");
            }
        }
        
        public void LoadConfiguration(int configId)
        {
            try
            {
                // Közvetlen betöltés a ConfigurationModel.LoadFromDatabase metódussal
                var config = ConfigurationModel.LoadFromDatabase(configId);
                
                if (config != null)
                {
                    // Komponensek betöltése objektumként
                    config.LoadComponents();
                    
                    LoadConfiguration(config);
                }
                else
                {
                    MessageBox.Show("A konfiguráció nem található.",
                                  "Betöltési Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a konfiguráció betöltése közben: {ex.Message}");
                ErrorHandler.HandleError(ex, "Konfiguráció betöltése");
            }
        }

        public void LoadConfiguration(ConfigurationModel config)
        {
            try
            {
                if (config == null)
                {
                    MessageBox.Show("A konfiguráció betöltése sikertelen: Hiányzó adatok",
                                  "Betöltési Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Tisztítjuk a jelenlegi komponenseket
                ClearConfig();

                // Komponensek betöltése
                if (config.CPU != null)
                {
                    SelectComponent(
                        "CPU",
                        config.CPU.Id,
                        config.CPU.Name,
                        $"{config.CPU.Cores} mag / {config.CPU.Threads} szál, {config.CPU.BaseClockGHz} GHz",
                        config.CPU.Price,
                        config.CPU.PowerConsumption
                    );
                }
                
                if (config.GPU != null)
                {
                    SelectComponent(
                        "GPU",
                        config.GPU.Id,
                        config.GPU.Name,
                        $"{config.GPU.Memory}GB {config.GPU.MemoryType}",
                        config.GPU.Price,
                        config.GPU.PowerConsumption
                    );
                }
                
                if (config.RAM != null)
                {
                    SelectComponent(
                        "RAM",
                        config.RAM.Id,
                        config.RAM.Name,
                        $"{config.RAM.CapacityGB}GB {config.RAM.Type}, {config.RAM.SpeedMHz} MHz",
                        config.RAM.Price,
                        config.RAM.PowerConsumption
                    );
                }
                
                if (config.Storage != null)
                {
                    SelectComponent(
                        "Storage",
                        config.Storage.Id,
                        config.Storage.Name,
                        $"{config.Storage.Capacity}GB {config.Storage.Type}",
                        config.Storage.Price,
                        config.Storage.PowerConsumption
                    );
                }
                
                if (config.Motherboard != null)
                {
                    SelectComponent(
                        "Motherboard",
                        config.Motherboard.Id,
                        config.Motherboard.Name,
                        $"{config.Motherboard.Chipset}, {config.Motherboard.Socket}",
                        config.Motherboard.Price,
                        config.Motherboard.PowerConsumption
                    );
                }
                
                if (config.PSU != null)
                {
                    SelectComponent(
                        "PSU",
                        config.PSU.Id,
                        config.PSU.Name,
                        $"{config.PSU.Wattage}W, {config.PSU.Efficiency}",
                        config.PSU.Price,
                        0 // Tápegységnek nincs fogyasztása a számítógép szempontjából
                    );
                }
                
                if (config.Case != null)
                {
                    SelectComponent(
                        "Case",
                        config.Case.Id,
                        config.Case.Name,
                        $"{config.Case.FormFactor}, {config.Case.Color}",
                        config.Case.Price,
                        0 // Háznak nincs fogyasztása a számítógép szempontjából
                    );
                }
                
                // Alternatív betöltés, ha nincsenek konkrét komponens objektumok, csak ComponentInfo
                if (config.Components != null && config.Components.Count > 0 && 
                    !(config.CPU != null || config.GPU != null || config.RAM != null || 
                      config.Storage != null || config.Motherboard != null || 
                      config.PSU != null || config.Case != null))
                {
                    foreach (var component in config.Components)
                    {
                        if (component != null && !string.IsNullOrEmpty(component.Type))
                        {
                            SelectComponent(
                                component.Type,
                                component.Id,
                                component.Name,
                                component.Details,
                                component.Price,
                                component.Power
                            );
                        }
                    }
                }

                // Frissítjük a teljes árat és teljesítményfelvételt
                UpdateTotals();
                
                // Frissítjük a kompatibilitást és teljesítményt
                UpdateCompatibilityStatus();
                UpdatePerformanceRating();
                
                MessageBox.Show($"A(z) '{config.Name}' konfiguráció sikeresen betöltve!", 
                              "Sikeres Betöltés", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a konfiguráció betöltése közben: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Belső hiba: {ex.InnerException.Message}");
                }
                ErrorHandler.HandleError(ex, "Konfiguráció betöltése");
            }
        }
        
        private string DetermineComponentType(ComponentInfo component)
        {
            // Itt a komponens nevét vagy más tulajdonságait használhatjuk a típus meghatározásához
            // Egyszerűsített példa - valós alkalmazásban ez összetettebb lenne
            
            string name = component.Name.ToLowerInvariant();
            
            if (name.Contains("cpu") || name.Contains("processor") || name.Contains("processzor"))
                return "CPU";
                
            if (name.Contains("gpu") || name.Contains("graphic") || name.Contains("grafik"))
                return "GPU";
                
            if (name.Contains("ram") || name.Contains("memory") || name.Contains("memória"))
                return "RAM";
                
            if (name.Contains("hdd") || name.Contains("ssd") || name.Contains("nvme") || name.Contains("storage") || name.Contains("tárhely"))
                return "Storage";
                
            if (name.Contains("motherboard") || name.Contains("alaplap"))
                return "Motherboard";
                
            if (name.Contains("psu") || name.Contains("power") || name.Contains("táp"))
                return "PSU";
                
            if (name.Contains("case") || name.Contains("ház"))
                return "Case";
                
            // Ha nem tudjuk meghatározni a típust
            return null;
        }
        

        public void AddComponent(ComponentInfo component)
        {
            if (component == null) return;

            string componentType = GetComponentTypeFromInfo(component);
            bool isCompatible = CheckComponentCompatibility(componentType, component);

            if (!isCompatible)
            {
                MessageBox.Show("Ez a komponens nem kompatibilis a jelenlegi konfigurációval!",
                              "Kompatibilitási Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ComponentInfo tárolása a _selectedComponents szótárban
            _selectedComponents[componentType] = component;

            // Emellett frissítjük a régi selectedComponents szótárat is a kompatibilitás miatt
            selectedComponents[componentType] = new ComponentItem 
            { 
                Id = component.Id,
                Name = component.Name,
                Details = component.Details,
                Price = component.Price,
                PowerConsumption = component.Power
            };
            
            // UI frissítése
            UpdateComponentDisplay();
        }
        
        private string GetComponentTypeFromInfo(ComponentInfo component)
        {
            // Itt megpróbáljuk kitalálni a komponens típusát
            // Egyelőre egyszerű string illesztéssel
            if (component.GetType().Name.Contains("CPU"))
                return "CPU";
            else if (component.GetType().Name.Contains("GPU"))
                return "GPU";
            else if (component.GetType().Name.Contains("RAM"))
                return "RAM";
            else if (component.GetType().Name.Contains("Storage"))
                return "Storage";
            else if (component.GetType().Name.Contains("Motherboard"))
                return "Motherboard";
            else if (component.GetType().Name.Contains("PSU"))
                return "PSU";
            else if (component.GetType().Name.Contains("Case"))
                return "Case";
            
            // Ha nem tudtuk meghatározni, akkor a nevéből próbáljuk
            return DetermineComponentType(component);
        }
        
        private bool CheckComponentCompatibility(string componentType, ComponentInfo newComponent)
        {
            try
            {
                // CPU és Motherboard kompatibilitás ellenőrzése
                if (componentType == "CPU")
                {
                    int cpuSocketTypeId = 0;
                    // Itt közvetlenül a ComponentInfo-ból próbáljuk kinyerni a socket ID-t
                    // Mivel ez nem közvetlenül elérhető, használjunk egy helper metódust vagy adatbázis lekérdezést
                    
                    // Példa adatbázis lekérdezésre:
                    string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                    using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                    {
                        conn.Open();
                        using (var cmd = new System.Data.SqlClient.SqlCommand("SELECT SocketTypeId FROM CPUs WHERE Id = @Id", conn))
                        {
                            cmd.Parameters.AddWithValue("@Id", newComponent.Id);
                            var result = cmd.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                                cpuSocketTypeId = Convert.ToInt32(result);
                        }
                    }
                    
                    if (_selectedComponents.ContainsKey("Motherboard") && _selectedComponents["Motherboard"] != null)
                    {
                        int mbSocketTypeId = 0;
                        // Hasonló lekérdezés az alaplap socket ID-jához
                        using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                        {
                            conn.Open();
                            using (var cmd = new System.Data.SqlClient.SqlCommand("SELECT SocketTypeId FROM Motherboards WHERE Id = @Id", conn))
                            {
                                cmd.Parameters.AddWithValue("@Id", _selectedComponents["Motherboard"].Id);
                                var result = cmd.ExecuteScalar();
                                if (result != null && result != DBNull.Value)
                                    mbSocketTypeId = Convert.ToInt32(result);
                            }
                        }
                        
                        return cpuSocketTypeId == mbSocketTypeId;
                    }
                }
                else if (componentType == "Motherboard")
                {
                    int mbSocketTypeId = 0;
                    // Az alaplap socket ID lekérdezése
                    string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                    using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                    {
                        conn.Open();
                        using (var cmd = new System.Data.SqlClient.SqlCommand("SELECT SocketTypeId FROM Motherboards WHERE Id = @Id", conn))
                        {
                            cmd.Parameters.AddWithValue("@Id", newComponent.Id);
                            var result = cmd.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                                mbSocketTypeId = Convert.ToInt32(result);
                        }
                    }
                    
                    if (_selectedComponents.ContainsKey("CPU") && _selectedComponents["CPU"] != null)
                    {
                        int cpuSocketTypeId = 0;
                        // CPU socket ID lekérdezése
                        using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                        {
                            conn.Open();
                            using (var cmd = new System.Data.SqlClient.SqlCommand("SELECT SocketTypeId FROM CPUs WHERE Id = @Id", conn))
                            {
                                cmd.Parameters.AddWithValue("@Id", _selectedComponents["CPU"].Id);
                                var result = cmd.ExecuteScalar();
                                if (result != null && result != DBNull.Value)
                                    cpuSocketTypeId = Convert.ToInt32(result);
                            }
                        }
                        
                        return cpuSocketTypeId == mbSocketTypeId;
                    }
                }
                else if (componentType == "RAM")
                {
                    int ramCapacityGB = 0;
                    // RAM kapacitás lekérdezése
                    string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                    using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                    {
                        conn.Open();
                        using (var cmd = new System.Data.SqlClient.SqlCommand("SELECT CapacityGB FROM RAMs WHERE Id = @Id", conn))
                        {
                            cmd.Parameters.AddWithValue("@Id", newComponent.Id);
                            var result = cmd.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                                ramCapacityGB = Convert.ToInt32(result);
                        }
                    }
                    
                    if (_selectedComponents.ContainsKey("Motherboard") && _selectedComponents["Motherboard"] != null)
                    {
                        int maxMemoryGB = 0;
                        // Alaplap max memória lekérdezése
                        using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                        {
                            conn.Open();
                            using (var cmd = new System.Data.SqlClient.SqlCommand("SELECT MaxMemoryGB FROM Motherboards WHERE Id = @Id", conn))
                            {
                                cmd.Parameters.AddWithValue("@Id", _selectedComponents["Motherboard"].Id);
                                var result = cmd.ExecuteScalar();
                                if (result != null && result != DBNull.Value)
                                    maxMemoryGB = Convert.ToInt32(result);
                            }
                        }
                        
                        return ramCapacityGB <= maxMemoryGB;
                    }
                }
                
                // Alapértelmezetten kompatibilis
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Kompatibilitás ellenőrzés", false);
                return false;
            }
        }
        
        private void UpdateComponentDisplay()
        {
            try
            {
                foreach (var component in _selectedComponents)
                {
                    string componentType = component.Key;
                    ComponentInfo componentInfo = component.Value;
                    
                    if (componentNameBlocks.ContainsKey(componentType) && componentInfo != null)
                    {
                        componentNameBlocks[componentType].Text = componentInfo.Name ?? "Nincs kiválasztva";
                        if (componentDetailBlocks.ContainsKey(componentType))
                        {
                            componentDetailBlocks[componentType].Text = componentInfo.Details ?? "";
                        }
                        
                        if (componentCards.ContainsKey(componentType))
                        {
                            componentCards[componentType].Style = (Style)FindResource("ComponentCardSelected");
                        }
                    }
                }
                
                UpdateTotalPrice();
                UpdatePerformanceScore();
                UpdateCompatibilityStatus();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Komponensek megjelenítése");
            }
        }
        
        private void UpdateTotalPrice()
        {
            try
            {
                decimal total = 0;
                foreach (var component in _selectedComponents.Values)
                {
                    if (component != null)
                    {
                        total += component.Price;
                    }
                }
                TotalPrice = total;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Teljes ár frissítése");
            }
        }
        
        private void UpdatePerformanceScore()
        {
            try
            {
                // Use the CalculatePerformanceScore method
                var score = CalculatePerformanceScore();
                
                // Update UI elements directly
                PerformanceBar.Value = score;
                PerformanceScoreBlock.Text = $"{score}/100";
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Teljesítménypont frissítése");
            }
        }

        private void UpdateTotals()
        {
            decimal totalPrice = 0;
            int totalPower = 0;

            // Számoljuk a teljes árat és energiafogyasztást a kiválasztott komponensekből
            foreach (var component in selectedComponents)
            {
                if (component.Value != null)
                {
                    totalPrice += component.Value.Price;
                    totalPower += component.Value.PowerConsumption;
                }
            }

            // Frissítjük a UI-t
            TotalPrice = totalPrice;
            TotalPower = totalPower;
        }
        
        // Komponensek betöltése az adatbázisból típus szerint
        private List<ComponentInfo> LoadComponentsFromDatabase(string componentType)
        {
            List<ComponentInfo> components = new List<ComponentInfo>();
            string connStr = "";
            
            try
            {
                // Adatbázis kapcsolat string lekérése
                connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                if (string.IsNullOrEmpty(connStr))
                {
                    throw new InvalidOperationException("Az adatbázis kapcsolat string nincs konfigurálva.");
                }
                
                // Táblanév lekérése a komponens típus alapján
                string tableName = GetTableNameByType(componentType);
                if (string.IsNullOrEmpty(tableName))
                {
                    throw new InvalidOperationException($"Ismeretlen komponens típus: {componentType}");
                }
                
                // Lekérdezés meghatározása a komponens típus alapján
                string query;
                if (componentType == "Motherboard")
                {
                    // Ellenőrizzük, hogy létezik-e a MotherboardView nézet
                    bool viewExists = CheckIfViewExists("MotherboardView");
                    
                    if (viewExists)
                    {
                        // Ha létezik a nézet, használjuk azt
                        System.Diagnostics.Debug.WriteLine("MotherboardView nézet használata az alaplapok lekérdezéséhez.");
                        query = "SELECT * FROM MotherboardView";
                    }
                    else
                    {
                        // Ha nem létezik a nézet, visszatérünk a régi JOIN lekérdezéshez
                        System.Diagnostics.Debug.WriteLine("MotherboardView nézet nem létezik. JOIN lekérdezés használata.");
                        
                        // Ellenőrizzük, hogy létezik-e a SocketTypes tábla és a SocketTypeId mező
                        bool socketTypeExists = CheckIfTableColumnExists("SocketTypes", "Id");
                        bool motherboardHasSocketTypeId = CheckIfTableColumnExists("Motherboards", "SocketTypeId");
                        
                        if (socketTypeExists && motherboardHasSocketTypeId)
                        {
                            query = $"SELECT m.*, s.SocketName, s.Manufacturer as SocketManufacturer FROM {tableName} m LEFT JOIN SocketTypes s ON m.SocketTypeId = s.Id";
                        }
                        else
                        {
                            // Ha nem létezik a SocketTypes tábla vagy a SocketTypeId mező, használjuk az egyszerű lekérdezést
                            System.Diagnostics.Debug.WriteLine("A SocketTypes tábla vagy a SocketTypeId mező nem létezik. Egyszerű lekérdezés használata.");
                            query = $"SELECT * FROM {tableName}";
                        }
                    }
                }
                else
                {
                    query = $"SELECT * FROM {tableName}";
                }
                
                using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                {
                    conn.Open();
                    System.Diagnostics.Debug.WriteLine($"Adatbázis kapcsolat nyitva: {conn.State}, Lekérdezés: {query}");
                    
                    using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            System.Diagnostics.Debug.WriteLine($"Lekérdezés futtatva, oszlopok száma: {reader.FieldCount}");
                            
                            // Oszlopnevek kiírása a debugoláshoz
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                System.Diagnostics.Debug.WriteLine($"Oszlop {i}: {reader.GetName(i)}");
                            }
                            
                            // Adatok olvasása
                            int recordCount = 0;
                            while (reader.Read())
                            {
                                recordCount++;
                                try
                                {
                                    var component = new ComponentInfo
                                    {
                                        Id = (int)reader["Id"],
                                        Name = reader["Name"].ToString(),
                                        Type = componentType,
                                        // További tulajdonságok betöltése a típustól függően
                                        Details = CreateDetailsString(reader, componentType),
                                        Price = reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : 0,
                                        Power = EstimatePowerConsumption(reader, componentType)
                                    };
                                    
                                    components.Add(component);
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Hiba az adatok olvasásakor a {recordCount}. rekordnál: {ex.Message}");
                                }
                            }
                            
                            System.Diagnostics.Debug.WriteLine($"Összesen {recordCount} rekord beolvasva, {components.Count} komponens létrehozva");
                        }
                    }
                }
                
                if (components.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Nem sikerült adatokat betölteni a(z) {tableName} táblából.");
                }
                
                return components;
            }
            catch (Exception ex)
            {
                string errorMessage = $"Hiba a komponensek betöltésekor ({componentType}): {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $" - {ex.InnerException.Message}";
                }
                
                System.Diagnostics.Debug.WriteLine(errorMessage);
                System.Diagnostics.Debug.WriteLine($"Connection string: {connStr}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Az alkalmazás hibakezelőjével jelezzük a problémát
                ErrorHandler.HandleError(ex, $"Komponensek betöltése ({componentType})", false);
                
                return new List<ComponentInfo>(); // Üres lista hiba esetén, nem tesztadatok
            }
        }
        
        // Dialógusablak megjelenítése a komponensek listázásához és kiválasztásához
        private ComponentInfo ShowComponentListDialog(string componentType, List<ComponentInfo> components)
        {
            // Dialógus ablak létrehozása
            Window dialog = new Window
            {
                Title = $"{componentType} kiválasztása",
                Width = 600,
                Height = 500,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.CanResize,
                WindowStyle = WindowStyle.ToolWindow
            };
            
            // Fő grid létrehozása
            Grid mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50, GridUnitType.Pixel) });
            mainGrid.Margin = new Thickness(15);
            
            // ListView a komponensek megjelenítéséhez
            ListView componentListView = new ListView();
            componentListView.SelectionMode = SelectionMode.Single;
            componentListView.Margin = new Thickness(0, 0, 0, 10);
            
            // GridView oszlopok beállítása
            GridView gridView = new GridView();
            
            // Komponens neve
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Név",
                Width = 250,
                DisplayMemberBinding = new Binding("Name")
            });
            
            // Komponens részletei
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Részletek",
                Width = 250,
                DisplayMemberBinding = new Binding("Details")
            });
            
            // Komponens ára
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Ár",
                Width = 80,
                DisplayMemberBinding = new Binding("Price") { StringFormat = "{0:N0} Ft" }
            });
            
            componentListView.View = gridView;
            componentListView.ItemsSource = components;
            
            // Komponensek hozzáadása a fő gridhez
            Grid.SetRow(componentListView, 0);
            mainGrid.Children.Add(componentListView);
            
            // Gomb panel létrehozása
            StackPanel buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            
            Button cancelButton = new Button
            {
                Content = "Mégsem",
                Padding = new Thickness(20, 5, 20, 5),
                Margin = new Thickness(0, 0, 10, 0)
            };
            
            Button selectButton = new Button
            {
                Content = "Kiválasztás",
                Padding = new Thickness(20, 5, 20, 5),
                IsDefault = true,
                IsEnabled = false // Kezdetben inaktív, amíg nem választunk ki komponenst
            };
            
            // Gombok hozzáadása a panelhez
            buttonPanel.Children.Add(cancelButton);
            buttonPanel.Children.Add(selectButton);
            
            // Gombpanel hozzáadása a fő gridhez
            Grid.SetRow(buttonPanel, 1);
            mainGrid.Children.Add(buttonPanel);
            
            // Dialógus tartalmának beállítása
            dialog.Content = mainGrid;
            
            // Visszatérési érték
            ComponentInfo selectedComponent = null;
            
            // Kiválasztás változása eseménykezelő
            componentListView.SelectionChanged += (s, e) =>
            {
                selectButton.IsEnabled = componentListView.SelectedItem != null;
            };
            
            // Dupla kattintás eseménykezelő
            componentListView.MouseDoubleClick += (s, e) =>
            {
                if (componentListView.SelectedItem != null)
                {
                    selectedComponent = componentListView.SelectedItem as ComponentInfo;
                    dialog.DialogResult = true;
                }
            };
            
            // Mégsem gomb eseménykezelő
            cancelButton.Click += (s, e) =>
            {
                dialog.DialogResult = false;
            };
            
            // Kiválasztás gomb eseménykezelő
            selectButton.Click += (s, e) =>
            {
                if (componentListView.SelectedItem != null)
                {
                    selectedComponent = componentListView.SelectedItem as ComponentInfo;
                    dialog.DialogResult = true;
                }
            };
            
            // Dialógus megjelenítése és visszatérés a kiválasztott komponenssel
            bool? result = dialog.ShowDialog();
            return result == true ? selectedComponent : null;
        }

        private int CalculatePerformanceScore()
        {
            // Alappontszám, amit minden konfiguráció minimum kap
            int baseScore = 10;
            int score = baseScore;

            // Pontszám számítása komponensek alapján
            if (selectedComponents.TryGetValue("CPU", out var cpu) && cpu != null)
            {
                // CPU pontszám a magok és szálak számán alapul
                int cpuScore = 15; // Alap CPU pontszám
                score += cpuScore;
            }

            if (selectedComponents.TryGetValue("GPU", out var gpu) && gpu != null)
            {
                // GPU pontszám (egyszerűsített)
                int gpuScore = 25; // Alap GPU pontszám
                score += gpuScore;
            }

            if (selectedComponents.TryGetValue("RAM", out var ram) && ram != null)
            {
                // RAM pontszám (egyszerűsített)
                int ramScore = 15; // Alap RAM pontszám
                score += ramScore;
            }

            if (selectedComponents.TryGetValue("Storage", out var storage) && storage != null)
            {
                // Tárhely pontszám (egyszerűsített)
                int storageScore = 10; // Alap tárhely pontszám
                score += storageScore;
            }

            if (selectedComponents.TryGetValue("Motherboard", out var motherboard) && motherboard != null)
            {
                // Alaplap pontszám (egyszerűsített)
                int motherboardScore = 10; // Alap alaplap pontszám
                score += motherboardScore;
            }

            if (selectedComponents.TryGetValue("PSU", out var psu) && psu != null)
            {
                // Tápegység pontszám (egyszerűsített)
                int psuScore = 5; // Alap tápegység pontszám
                score += psuScore;
            }

            if (selectedComponents.TryGetValue("Case", out var caseComponent) && caseComponent != null)
            {
                // Ház pontszám (egyszerűsített)
                int caseScore = 5; // Alap ház pontszám
                score += caseScore;
            }

            // Ellenőrizzük, hogy a teljesítménypontszám ne legyen 100-nál nagyobb
            return Math.Min(score, 100);
        }

        // Overload metódus a ConfigurationModel-hez
        private int CalculatePerformanceScore(ConfigurationModel config)
        {
            if (config == null || config.Components == null || config.Components.Count == 0)
                return 0;

            // Alappontszám, amit minden konfiguráció minimum kap
            int baseScore = 10;
            int score = baseScore;

            // Pontszám számítása komponensek típusa alapján
            foreach (var component in config.Components)
            {
                switch (component.Type)
                {
                    case "CPU":
                        score += 15; // Alap CPU pontszám
                        break;
                    case "GPU":
                        score += 25; // Alap GPU pontszám
                        break;
                    case "RAM":
                        score += 15; // Alap RAM pontszám
                        break;
                    case "Storage":
                        score += 10; // Alap tárhely pontszám
                        break;
                    case "Motherboard":
                        score += 10; // Alap alaplap pontszám
                        break;
                    case "PSU":
                        score += 5; // Alap tápegység pontszám
                        break;
                    case "Case":
                        score += 5; // Alap ház pontszám
                        break;
                }
            }

            // Ellenőrizzük, hogy a teljesítménypontszám ne legyen 100-nál nagyobb
            return Math.Min(score, 100);
        }

        private void LoadComponentsFromStore()
        {
            try
            {
                // Betöltjük a komponenseket a ConfigBuilderStore-ból
                var store = ConfigBuilderStore.GetInstance();
                if (store.HasStoredComponents)
                {
                    foreach (var componentPair in store.GetComponents())
                    {
                        string componentType = componentPair.Key;
                        var component = componentPair.Value;
                        
                        if (!string.IsNullOrEmpty(componentType) && componentCards.ContainsKey(componentType))
                        {
                            // Kiválasztott komponens adatainak mentése
                            SelectComponent(
                                componentType, 
                                component.Id, 
                                component.Name, 
                                component.Details, 
                                component.Price, 
                                component.PowerConsumption
                            );
                        }
                    }
                    
                    // Frissítjük a kompatibilitást és teljesítmény értékelést
                    UpdateCompatibilityStatus();
                    UpdatePerformanceRating();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Komponensek betöltése");
            }
        }
        
        // A komponens típusa alapján visszaadja a megfelelő táblanevet
        private string GetTableNameByType(string type)
        {
            switch (type)
            {
                case "CPU": return "CPUs";
                case "GPU": return "GPUs";
                case "RAM": return "RAMs";
                case "Storage": return "Storages";
                case "Motherboard": return "Motherboards";
                case "PSU": return "PSUs";
                case "Case": return "Cases";
                default: throw new ArgumentException($"Ismeretlen komponens típus: {type}");
            }
        }
        
        // Létrehozza a részletek string-et a komponens típusa alapján
        private string CreateDetailsString(System.Data.SqlClient.SqlDataReader reader, string componentType)
        {
            try
            {
                switch (componentType)
                {
                    case "CPU":
                        string cores = HasColumn(reader, "Cores") ? reader["Cores"].ToString() : "N/A";
                        string threads = HasColumn(reader, "Threads") ? reader["Threads"].ToString() : "N/A";
                        string baseClockGHz = HasColumn(reader, "BaseClockGHz") ? reader["BaseClockGHz"].ToString() : "N/A";
                        string boostClockGHz = HasColumn(reader, "BoostClockGHz") ? reader["BoostClockGHz"].ToString() : "N/A";
                        return $"{cores} mag / {threads} szál, {baseClockGHz} GHz ({boostClockGHz} GHz)";
                        
                    case "GPU":
                        string memoryGB = HasColumn(reader, "MemoryGB") ? reader["MemoryGB"].ToString() : "N/A";
                        string memoryType = HasColumn(reader, "MemoryType") ? reader["MemoryType"].ToString() : "GDDR";
                        return $"{memoryGB} GB {memoryType}";
                        
                    case "RAM":
                        string capacityGB = HasColumn(reader, "CapacityGB") ? reader["CapacityGB"].ToString() : "N/A";
                        string speedMHz = HasColumn(reader, "SpeedMHz") ? reader["SpeedMHz"].ToString() : "N/A";
                        string type = HasColumn(reader, "Type") ? reader["Type"].ToString() : "N/A";
                        return $"{capacityGB} GB {type}, {speedMHz} MHz";
                        
                    case "Storage":
                        string storageType = HasColumn(reader, "Type") ? reader["Type"].ToString() : "N/A";
                        string storageCapacityGB = HasColumn(reader, "CapacityGB") ? reader["CapacityGB"].ToString() : "N/A";
                        return $"{storageType}, {storageCapacityGB} GB";
                        
                    case "Motherboard":
                        string manufacturer = HasColumn(reader, "Manufacturer") ? reader["Manufacturer"].ToString() : "N/A";
                        
                        // ChipsetTypeId alapján kellene lekérdezni a chipset nevét, de most csak az ID-t használjuk
                        string chipsetType = HasColumn(reader, "ChipsetTypeId") ? reader["ChipsetTypeId"].ToString() : "N/A";
                        
                        // Először a SocketName-et keressük (VIEW vagy JOIN esetén), ha nincs, akkor a Socket mezőt próbáljuk
                        string socket = HasColumn(reader, "SocketName") ? reader["SocketName"].ToString() : 
                                      (HasColumn(reader, "Socket") ? reader["Socket"].ToString() : "N/A");
                        
                        // Formátum meghatározása
                        string socketInfo = !string.IsNullOrEmpty(socket) ? socket : "Ismeretlen foglalat";
                        
                        // Debug információ a konzolra
                        System.Diagnostics.Debug.WriteLine($"Motherboard adatok: {manufacturer}, Chipset: {chipsetType}, Socket: {socketInfo}");
                        
                        return $"{manufacturer}, {socketInfo} foglalat";
                        
                    case "PSU":
                        string wattage = HasColumn(reader, "Wattage") ? reader["Wattage"].ToString() : "N/A";
                        string efficiencyRating = HasColumn(reader, "EfficiencyRating") ? reader["EfficiencyRating"].ToString() : "N/A";
                        return $"{wattage} W, {efficiencyRating}";
                        
                    case "Case":
                        string formFactor = HasColumn(reader, "FormFactor") ? reader["FormFactor"].ToString() : "N/A";
                        string color = HasColumn(reader, "Color") ? reader["Color"].ToString() : "N/A";
                        return $"{formFactor}, {color}";
                        
                    default:
                        return "Részletek nem elérhetőek";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a részletek string létrehozásakor: {ex.Message}");
                return "Részletek nem elérhetőek";
            }
        }
        
        // Becsült fogyasztás meghatározása a típus és adatok alapján
        private int EstimatePowerConsumption(System.Data.SqlClient.SqlDataReader reader, string componentType)
        {
            try
            {
                switch (componentType)
                {
                    case "CPU":
                        // Ha van explicit PowerConsumption érték, azt használjuk
                        if (HasColumn(reader, "PowerConsumption") && reader["PowerConsumption"] != DBNull.Value)
                        {
                            return Convert.ToInt32(reader["PowerConsumption"]);
                        }
                        
                        // Egyébként becsüljük a magok száma alapján
                        if (HasColumn(reader, "Cores") && reader["Cores"] != DBNull.Value)
                        {
                            int cores = Convert.ToInt32(reader["Cores"]);
                            return cores * 15; // Becsült érték magonként
                        }
                        return 95; // Alapértelmezett becslés
                        
                    case "GPU":
                        // Ha van explicit PowerConsumption érték, azt használjuk
                        if (HasColumn(reader, "PowerConsumption") && reader["PowerConsumption"] != DBNull.Value)
                        {
                            return Convert.ToInt32(reader["PowerConsumption"]);
                        }
                        
                        // Memória mennyisége alapján becsüljük
                        if (HasColumn(reader, "MemoryGB") && reader["MemoryGB"] != DBNull.Value)
                        {
                            int memoryGB = Convert.ToInt32(reader["MemoryGB"]);
                            return memoryGB * 20 + 50; // Becsült érték memória és alap alapján
                        }
                        return 200; // Alapértelmezett becslés
                        
                    case "RAM":
                        return 10; // RAM fogyasztás általában alacsony és konstans
                        
                    case "Storage":
                        string storageType = HasColumn(reader, "Type") ? reader["Type"].ToString().ToLower() : "";
                        // SSD és NVMe kevesebbet fogyaszt, mint a HDD
                        if (storageType.Contains("ssd") || storageType.Contains("nvme"))
                            return 5;
                        else if (storageType.Contains("hdd"))
                            return 10;
                        return 5; // Alapértelmezett becslés
                        
                    case "Motherboard":
                        // Ha van explicit érték, azt használjuk
                        if (HasColumn(reader, "PowerConsumption") && reader["PowerConsumption"] != DBNull.Value)
                        {
                            return Convert.ToInt32(reader["PowerConsumption"]);
                        }
                        return 30; // Alapértelmezett becslés
                        
                    case "PSU":
                    case "Case":
                        return 0; // Ezek nem fogyasztanak a rendszer szempontjából
                        
                    default:
                        return 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a fogyasztás becslésekor: {ex.Message}");
                return 0;
            }
        }
        
        // Segédfüggvény annak ellenőrzésére, hogy az olvasó tartalmazza-e az adott oszlopot
        private bool HasColumn(System.Data.SqlClient.SqlDataReader reader, string columnName)
        {
            if (reader == null)
                return false;
                
            try
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (string.Equals(reader.GetName(i), columnName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
                
                return false;
            }
            catch
            {
                return false;
            }
        }
        
        // Segédfüggvény annak ellenőrzésére, hogy létezik-e az adott tábla és oszlop az adatbázisban
        private bool CheckIfTableColumnExists(string tableName, string columnName)
        {
            try
            {
                string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                if (string.IsNullOrEmpty(connStr))
                {
                    System.Diagnostics.Debug.WriteLine("Az adatbázis kapcsolat string nincs konfigurálva.");
                    return false;
                }
                
                using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                {
                    conn.Open();
                    
                    // Ellenőrizzük, hogy létezik-e a tábla
                    string tableCheckQuery = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_NAME = @TableName";
                    
                    using (var cmd = new System.Data.SqlClient.SqlCommand(tableCheckQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@TableName", tableName);
                        int tableCount = (int)cmd.ExecuteScalar();
                        
                        if (tableCount == 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"A(z) {tableName} tábla nem létezik.");
                            return false;
                        }
                    }
                    
                    // Ellenőrizzük, hogy létezik-e az oszlop
                    string columnCheckQuery = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = @TableName 
                        AND COLUMN_NAME = @ColumnName";
                    
                    using (var cmd = new System.Data.SqlClient.SqlCommand(columnCheckQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@TableName", tableName);
                        cmd.Parameters.AddWithValue("@ColumnName", columnName);
                        int columnCount = (int)cmd.ExecuteScalar();
                        
                        if (columnCount == 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"A(z) {columnName} oszlop nem létezik a(z) {tableName} táblában.");
                            return false;
                        }
                    }
                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a tábla/oszlop létezésének ellenőrzésekor: {ex.Message}");
                return false;
            }
        }
    }
}
