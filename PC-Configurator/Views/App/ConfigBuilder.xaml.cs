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
        
        public ConfigBuilder(int userId = 0)
        {
            InitializeComponent();
            
            // Beállítjuk az aktuális felhasználó azonosítóját
            _currentUserId = userId;
            
            // Inicializáljuk a szótárakat
            InitializeDictionaries();
            
            // Update the total price and power for the initial state
            UpdateTotals();
            componentCards.Add("Case", CaseCard);
            
            // Initialize the component name blocks dictionary
            componentNameBlocks.Add("CPU", CPUNameBlock);
            componentNameBlocks.Add("GPU", GPUNameBlock);
            componentNameBlocks.Add("RAM", RAMNameBlock);
            componentNameBlocks.Add("Storage", StorageNameBlock);
            componentNameBlocks.Add("Motherboard", MotherboardNameBlock);
            componentNameBlocks.Add("PSU", PSUNameBlock);
            componentNameBlocks.Add("Case", CaseNameBlock);
            
            // Initialize the component detail blocks dictionary
            componentDetailBlocks.Add("CPU", CPUDetailBlock);
            componentDetailBlocks.Add("GPU", GPUDetailBlock);
            componentDetailBlocks.Add("RAM", RAMDetailBlock);
            componentDetailBlocks.Add("Storage", StorageDetailBlock);
            componentDetailBlocks.Add("Motherboard", MotherboardDetailBlock);
            componentDetailBlocks.Add("PSU", PSUDetailBlock);
            componentDetailBlocks.Add("Case", CaseDetailBlock);
            
            // Initialize compatibility dictionary
            componentCompatibility.Add("CPU", false);
            componentCompatibility.Add("Motherboard", false);
            componentCompatibility.Add("RAM", false);
            
            // Betöltjük a Components osztálytól érkező alkatrészeket, ha vannak
            LoadComponentsFromStore();
            
            // Update the total price and power for the initial state
            UpdateTotals();
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
            // In a real implementation, this would open a dialog with components from the database
            // For now, we'll simulate it with a message box
            MessageBox.Show($"A {componentType} komponens választó jelenleg fejlesztés alatt áll.\n\nEbben a demó verzióban már előre be vannak állítva mintaadatok a gépösszeállítás bemutatására.", 
                           "Komponens választó", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // For demo purposes, select a default component if not already selected
            if (!selectedComponents.ContainsKey(componentType))
            {
                switch(componentType)
                {
                    case "RAM":
                        SelectComponent(componentType, 1, "Kingston HyperX 32GB", "DDR4, 3600MHz, CL16", 49900M, 25);
                        break;
                    case "Motherboard":
                        SelectComponent(componentType, 2, "MSI MAG B550 TOMAHAWK", "AMD B550 chipset, AM4 foglalat", 54900M, 30);
                        break;
                    case "Storage":
                        SelectComponent(componentType, 1, "Samsung 970 EVO 1TB", "NVMe SSD, 3500/3300 MB/s", 44900M, 8);
                        break;
                    case "PSU":
                        SelectComponent(componentType, 1, "Corsair RM850x", "850W, Gold, moduláris", 38900M, 0);
                        break;
                    case "Case":
                        SelectComponent(componentType, 4, "Lian Li PC-O11 Dynamic", "Mid Tower, fekete", 42900M, 0);
                        break;
                }
            }
            
            // Update compatibility after selection
            UpdateCompatibilityStatus();
            UpdatePerformanceRating();
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
        
        private void InitializeDictionaries()
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

            componentNameBlocks = new Dictionary<string, TextBlock>
            {
                { "CPU", CPUNameBlock },
                { "Motherboard", MotherboardNameBlock },
                { "RAM", RAMNameBlock },
                { "GPU", GPUNameBlock },
                { "Storage", StorageNameBlock },
                { "PSU", PSUNameBlock },
                { "Case", CaseNameBlock }
            };

            componentDetailBlocks = new Dictionary<string, TextBlock>
            {
                { "CPU", CPUDetailBlock },
                { "Motherboard", MotherboardDetailBlock },
                { "RAM", RAMDetailBlock },
                { "GPU", GPUDetailBlock },
                { "Storage", StorageDetailBlock },
                { "PSU", PSUDetailBlock },
                { "Case", CaseDetailBlock }
            };

            // Initialize selected components too
            selectedComponents = new Dictionary<string, ComponentItem>();

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

                // Komponensek hozzáadása a konfigurációhoz
                foreach (var componentPair in selectedComponents)
                {
                    if (componentPair.Value != null)
                    {
                        var componentInfo = new ComponentInfo
                        {
                            Id = componentPair.Value.Id,
                            Name = componentPair.Value.Name,
                            Details = componentPair.Value.Details,
                            Price = componentPair.Value.Price,
                            Power = componentPair.Value.PowerConsumption
                        };
                        
                        config.Components.Add(componentInfo);
                    }
                }

                // Konfiguráció mentése az adatbázisba
                config.SaveToDatabase();

                MessageBox.Show("A konfiguráció sikeresen mentésre került!",
                              "Sikeres Mentés", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleDatabaseError(ex, "mentés", "konfiguráció");
            }
        }
        
        public void LoadConfiguration(int configId)
        {
            try
            {
                // A betöltés módosítva a ConfigurationModel.LoadUserConfigurations-ből egyedi lekérdezésre
                var configs = ConfigurationModel.LoadUserConfigurations(_currentUserId);
                var config = configs.FirstOrDefault(c => c.Id == configId);
                
                if (config != null)
                {
                    LoadConfiguration(config);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Konfiguráció betöltése");
            }
        }

        public void LoadConfiguration(ConfigurationModel config)
        {
            try
            {
                if (config == null || config.Components == null)
                {
                    MessageBox.Show("A konfiguráció betöltése sikertelen: Hiányzó adatok",
                                  "Betöltési Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Tisztítjuk a jelenlegi komponenseket
                ClearConfig();

                // Betöltjük az új komponenseket a ComponentInfo-ból a selectedComponents szótárba
                foreach (var component in config.Components)
                {
                    if (component != null)
                    {
                        // A komponens típusát a ComponentType tulajdonságból olvassuk ki
                        string componentType = DetermineComponentType(component);
                        if (!string.IsNullOrEmpty(componentType))
                        {
                            // Létrehozunk egy új ComponentItem-et a ComponentInfo alapján
                            var componentItem = new ComponentItem
                            {
                                Id = component.Id,
                                Name = component.Name,
                                Details = component.Details,
                                Price = component.Price,
                                PowerConsumption = component.Power
                            };
                            
                            selectedComponents[componentType] = componentItem;
                            
                            // Frissítjük a felhasználói felületet
                            if (componentNameBlocks.ContainsKey(componentType))
                                componentNameBlocks[componentType].Text = component.Name;
                                
                            if (componentDetailBlocks.ContainsKey(componentType))
                                componentDetailBlocks[componentType].Text = component.Details;
                                
                            if (componentCards.ContainsKey(componentType))
                                componentCards[componentType].Style = (Style)FindResource("ComponentCardSelected");
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
                // Itt kellene betölteni a komponenseket egy átmeneti tárolóból, más osztályból vagy szolgáltatásból
                // Ebben a példában csak egy üres implementációt adunk, ami nem csinál semmit
                
                // Példa implementáció a ConfigBuilderStore osztályra támaszkodva (ha létezne)
                /*
                var store = ConfigBuilderStore.GetInstance();
                if (store.HasStoredComponents)
                {
                    foreach (var component in store.GetComponents())
                    {
                        string componentType = component.Type;
                        if (!string.IsNullOrEmpty(componentType) && _selectedComponents.ContainsKey(componentType))
                        {
                            _selectedComponents[componentType] = component;
                        }
                    }
                    UpdateComponentDisplay();
                }
                */
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Komponensek betöltése");
            }
        }
    }
}
