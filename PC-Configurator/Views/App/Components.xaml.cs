using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel; // DependencyPropertyDescriptor-hoz
using PC_Configurator.Helpers;
using PC_Configurator.Models; // PermissionManager és ErrorHandler használatához

namespace PC_Configurator.Views.App
{
    /// <summary>
    /// Interaction logic for Components.xaml
    /// </summary>
    public partial class Components : UserControl
    {
        private string _currentType;
        private List<object> _allComponents; // Az összes komponens tárolása az aktuális típusból
        private string _lastSearchTerm = ""; // Az utolsó keresési kifejezés
        
        public Components()
        {
            try
            {
                InitializeComponent();
                
                // Alapértelmezett típus beállítása
                _currentType = "CPU";
                SetTypeSelector(_currentType);
                UpdateComponentTypeTitle(_currentType);
                
                // Admin jogosultság ellenőrzése a gombok megjelenítéséhez
                SetupPermissions();
                
                try
                {
                    LoadComponents(_currentType);
                }
                catch (Exception ex)
                {
                    ErrorHandler.HandleError(ex, "Komponensek betöltése");
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Komponensek inicializálása");
            }
        }private void SetTypeSelector(string type)
        {
            try
            {
                // Definiáljuk a színeket
                SolidColorBrush defaultBg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D333B"));
                SolidColorBrush activeBg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0D66D0"));
                SolidColorBrush defaultBorder = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444C56"));
                SolidColorBrush activeBorder = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#539BF5"));
                
                // Az összes gomb alapbeállítása
                BtnCPU.Background = defaultBg;
                BtnGPU.Background = defaultBg;
                BtnRAM.Background = defaultBg;
                BtnStorage.Background = defaultBg;
                BtnMotherboard.Background = defaultBg;
                BtnPSU.Background = defaultBg;
                BtnCase.Background = defaultBg;
                
                BtnCPU.BorderBrush = defaultBorder;
                BtnGPU.BorderBrush = defaultBorder;
                BtnRAM.BorderBrush = defaultBorder;
                BtnStorage.BorderBrush = defaultBorder;
                BtnMotherboard.BorderBrush = defaultBorder;
                BtnPSU.BorderBrush = defaultBorder;
                BtnCase.BorderBrush = defaultBorder;
                
                // Az aktív gomb kiemelése
                switch (type)
                {
                    case "CPU":
                        BtnCPU.Background = activeBg;
                        BtnCPU.BorderBrush = activeBorder;
                        break;
                    case "GPU":
                        BtnGPU.Background = activeBg;
                        BtnGPU.BorderBrush = activeBorder;
                        break;
                    case "RAM":
                        BtnRAM.Background = activeBg;
                        BtnRAM.BorderBrush = activeBorder;
                        break;
                    case "Storage":
                        BtnStorage.Background = activeBg;
                        BtnStorage.BorderBrush = activeBorder;
                        break;
                    case "Motherboard":
                        BtnMotherboard.Background = activeBg;
                        BtnMotherboard.BorderBrush = activeBorder;
                        break;
                    case "PSU":
                        BtnPSU.Background = activeBg;
                        BtnPSU.BorderBrush = activeBorder;
                        break;
                    case "Case":
                        BtnCase.Background = activeBg;
                        BtnCase.BorderBrush = activeBorder;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a típus kijelölésénél: {ex.Message}", "Típus kijelölési hiba");
            }
        }        private void TypeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.Tag is string type)
                {
                    // Tároljuk az aktuálisan kiválasztott típust
                    _currentType = type;
                    
                    // A keresőmező aktuális értékét is figyelembe vesszük
                    string searchTerm = SearchTextBox.Text?.Trim() ?? "";
                    
                    // Frissítsük a kijelölést és töltsük be a komponenseket
                    SetTypeSelector(type);
                    LoadComponents(type, searchTerm);
                    
                    // Frissítsük a címsort is
                    UpdateComponentTypeTitle(type);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a típusválasztásnál: {ex.Message}", "Típusválasztási hiba");
            }
        }
        
        // Címsor frissítése a kiválasztott típusnak megfelelően
        private void UpdateComponentTypeTitle(string type)
        {
            if (ComponentTypeTitle != null)
            {
                ComponentTypeTitle.Text = $"{type} komponensek";
            }
        }private void LoadComponents(string type, string searchTerm = "")
        {
            try
            {
                List<object> items = new List<object>();
                bool databaseLoadSuccessful = false;
                _lastSearchTerm = searchTerm; // Utolsó keresési kifejezés mentése
                
                try
                {
                    // Valódi adatok betöltése az adatbázisból
                    switch (type)
                    {
                        case "CPU":
                            items = LoadCPUs().Cast<object>().ToList();
                            break;
                        case "GPU":
                            items = LoadGPUs().Cast<object>().ToList();
                            break;
                        case "RAM":
                            items = LoadRAMs().Cast<object>().ToList();
                            break;
                        case "Storage":
                            items = LoadStorages().Cast<object>().ToList();
                            break;
                        case "Motherboard":
                            items = LoadMotherboards().Cast<object>().ToList();
                            break;
                        case "PSU":
                            items = LoadPSUs().Cast<object>().ToList();
                            break;
                        case "Case":
                            items = LoadCases().Cast<object>().ToList();
                            break;
                    }
                    
                    // Ha vannak adatok, akkor sikeres volt a betöltés
                    if (items.Count > 0)
                    {
                        databaseLoadSuccessful = true;
                        _allComponents = items; // Mentjük az összes komponenst
                    
                        // Ha van keresési kifejezés, szűrjük a komponenseket
                        if (!string.IsNullOrWhiteSpace(searchTerm))
                        {
                            var filteredItems = FilterComponentsByName(items, searchTerm);
                            ComponentList.ItemsSource = filteredItems;
                            
                            // Nincs találat üzenet kezelése
                            NoResultsPanel.Visibility = filteredItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                            ComponentScroller.Visibility = filteredItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                        }
                        else
                        {
                            ComponentList.ItemsSource = items;
                            NoResultsPanel.Visibility = Visibility.Collapsed;
                            ComponentScroller.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        // Ha üres a lista, akkor jelezzük, hogy tesztadatokat fogunk betölteni
                        System.Diagnostics.Debug.WriteLine($"Nem találhatók adatok az adatbázisban a(z) {type} típushoz. Tesztadatok betöltése.");
                    }
                }
                catch (Exception dbEx)
                {
                    // Adatbázis hiba esetén logoljuk a hibát
                    System.Diagnostics.Debug.WriteLine($"Adatbázis hiba: {dbEx.Message}. Tesztadatok betöltése.");
                }
                
                // Ha nem sikerült az adatbázisból betölteni, akkor tesztadatok betöltése
                if (!databaseLoadSuccessful)
                {
                    var testItems = CreateTestData(type);
                    _allComponents = testItems; // Mentjük az összes komponenst
                    
                    // Ha van keresési kifejezés, szűrjük a komponenseket
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        var filteredItems = FilterComponentsByName(testItems, searchTerm);
                        ComponentList.ItemsSource = filteredItems;
                        
                        // Nincs találat üzenet kezelése
                        NoResultsPanel.Visibility = filteredItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                        ComponentScroller.Visibility = filteredItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                    }
                    else
                    {
                        ComponentList.ItemsSource = testItems;
                        NoResultsPanel.Visibility = Visibility.Collapsed;
                        ComponentScroller.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a komponensek betöltése közben: {ex.Message}", "Betöltési hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                
                // Végső megoldásként próbáljunk meg tesztadatokat betölteni
                try
                {
                    var testItems = CreateTestData(type);
                    _allComponents = testItems;
                    
                    // Ha van keresési kifejezés, szűrjük a komponenseket
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        var filteredItems = FilterComponentsByName(testItems, searchTerm);
                        ComponentList.ItemsSource = filteredItems;
                        
                        // Nincs találat üzenet kezelése
                        NoResultsPanel.Visibility = filteredItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                        ComponentScroller.Visibility = filteredItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                    }
                    else
                    {
                        ComponentList.ItemsSource = testItems;
                        NoResultsPanel.Visibility = Visibility.Collapsed;
                        ComponentScroller.Visibility = Visibility.Visible;
                    }
                }
                catch
                {
                    // Ha még ez sem sikerül, akkor üres listát jelenítünk meg
                    _allComponents = new List<object>();
                    ComponentList.ItemsSource = new List<object>();
                    
                    // Üres lista esetén is megjelenítjük a "nincs találat" üzenetet
                    NoResultsPanel.Visibility = Visibility.Visible;
                    ComponentScroller.Visibility = Visibility.Collapsed;
                }
            }
        }
          // Teszt adatok létrehozása a listához
        private List<object> CreateTestData(string type)
        {
            var result = new List<object>();
            
            switch (type)
            {
                case "CPU":
                    result.Add(new { Id = 1, Name = "Intel Core i5-12600K", Tooltip = "Gyártó: Intel\nMagok: 10\nSzálak: 16\nAlap órajel: 3.7 GHz\nMax órajel: 4.9 GHz" });
                    result.Add(new { Id = 2, Name = "AMD Ryzen 7 5800X", Tooltip = "Gyártó: AMD\nMagok: 8\nSzálak: 16\nAlap órajel: 3.8 GHz\nMax órajel: 4.7 GHz" });
                    result.Add(new { Id = 3, Name = "Intel Core i9-12900K", Tooltip = "Gyártó: Intel\nMagok: 16\nSzálak: 24\nAlap órajel: 3.2 GHz\nMax órajel: 5.2 GHz" });
                    result.Add(new { Id = 4, Name = "AMD Ryzen 9 5900X", Tooltip = "Gyártó: AMD\nMagok: 12\nSzálak: 24\nAlap órajel: 3.7 GHz\nMax órajel: 4.8 GHz" });
                    result.Add(new { Id = 5, Name = "Intel Core i3-10100", Tooltip = "Gyártó: Intel\nMagok: 4\nSzálak: 8\nAlap órajel: 3.6 GHz\nMax órajel: 4.3 GHz" });
                    result.Add(new { Id = 6, Name = "AMD Ryzen 5 3600", Tooltip = "Gyártó: AMD\nMagok: 6\nSzálak: 12\nAlap órajel: 3.6 GHz\nMax órajel: 4.2 GHz" });
                    result.Add(new { Id = 7, Name = "Intel Core i7-11700K", Tooltip = "Gyártó: Intel\nMagok: 8\nSzálak: 16\nAlap órajel: 3.6 GHz\nMax órajel: 5.0 GHz" });
                    result.Add(new { Id = 8, Name = "AMD Ryzen 3 3100", Tooltip = "Gyártó: AMD\nMagok: 4\nSzálak: 8\nAlap órajel: 3.6 GHz\nMax órajel: 3.9 GHz" });
                    result.Add(new { Id = 9, Name = "Intel Core i5-10400", Tooltip = "Gyártó: Intel\nMagok: 6\nSzálak: 12\nAlap órajel: 2.9 GHz\nMax órajel: 4.3 GHz" });
                    result.Add(new { Id = 10, Name = "Intel Core i5-11600K", Tooltip = "Gyártó: Intel\nMagok: 6\nSzálak: 12\nAlap órajel: 3.9 GHz\nMax órajel: 4.9 GHz" });
                    break;
                case "GPU":
                    result.Add(new { Id = 1, Name = "NVIDIA RTX 3080", Tooltip = "Gyártó: NVIDIA\nMemória: 10 GB" });
                    result.Add(new { Id = 2, Name = "AMD RX 6800 XT", Tooltip = "Gyártó: AMD\nMemória: 16 GB" });
                    result.Add(new { Id = 3, Name = "NVIDIA RTX 3070", Tooltip = "Gyártó: NVIDIA\nMemória: 8 GB" });
                    result.Add(new { Id = 4, Name = "AMD RX 6700 XT", Tooltip = "Gyártó: AMD\nMemória: 12 GB" });
                    result.Add(new { Id = 5, Name = "NVIDIA RTX 3060", Tooltip = "Gyártó: NVIDIA\nMemória: 12 GB" });
                    result.Add(new { Id = 6, Name = "AMD RX 6600 XT", Tooltip = "Gyártó: AMD\nMemória: 8 GB" });
                    result.Add(new { Id = 7, Name = "NVIDIA RTX 3090", Tooltip = "Gyártó: NVIDIA\nMemória: 24 GB" });
                    result.Add(new { Id = 8, Name = "AMD RX 6900 XT", Tooltip = "Gyártó: AMD\nMemória: 16 GB" });
                    break;
                case "RAM":
                    result.Add(new { Id = 1, Name = "Kingston HyperX 32GB", Tooltip = "Kapacitás: 32 GB\nÓrajel: 3600 MHz\nTípus: DDR4" });
                    result.Add(new { Id = 2, Name = "Corsair Vengeance 16GB", Tooltip = "Kapacitás: 16 GB\nÓrajel: 3200 MHz\nTípus: DDR4" });
                    result.Add(new { Id = 3, Name = "G.Skill Trident Z 64GB", Tooltip = "Kapacitás: 64 GB\nÓrajel: 3600 MHz\nTípus: DDR4" });
                    result.Add(new { Id = 4, Name = "Crucial Ballistix 32GB", Tooltip = "Kapacitás: 32 GB\nÓrajel: 3000 MHz\nTípus: DDR4" });
                    result.Add(new { Id = 5, Name = "Kingston Fury 16GB", Tooltip = "Kapacitás: 16 GB\nÓrajel: 3200 MHz\nTípus: DDR4" });
                    result.Add(new { Id = 6, Name = "Corsair Dominator 32GB", Tooltip = "Kapacitás: 32 GB\nÓrajel: 3600 MHz\nTípus: DDR4" });
                    result.Add(new { Id = 7, Name = "G.Skill Ripjaws 16GB", Tooltip = "Kapacitás: 16 GB\nÓrajel: 3200 MHz\nTípus: DDR4" });
                    result.Add(new { Id = 8, Name = "G.Skill Trident Z 32GB", Tooltip = "Kapacitás: 32 GB\nÓrajel: 3600 MHz\nTípus: DDR5" });
                    result.Add(new { Id = 9, Name = "Kingston Fury Beast 32GB", Tooltip = "Kapacitás: 32 GB\nÓrajel: 4800 MHz\nTípus: DDR5" });
                    break;
                case "Storage":
                    result.Add(new { Id = 1, Name = "Samsung 970 EVO 1TB", Tooltip = "Típus: SSD\nKapacitás: 1000 GB" });
                    result.Add(new { Id = 2, Name = "Seagate Barracuda 2TB", Tooltip = "Típus: HDD\nKapacitás: 2000 GB" });
                    result.Add(new { Id = 3, Name = "Western Digital Black 1TB", Tooltip = "Típus: SSD\nKapacitás: 1000 GB" });
                    result.Add(new { Id = 4, Name = "Crucial MX500 500GB", Tooltip = "Típus: SSD\nKapacitás: 500 GB" });
                    result.Add(new { Id = 5, Name = "Samsung 980 PRO 2TB", Tooltip = "Típus: SSD\nKapacitás: 2000 GB" });
                    result.Add(new { Id = 6, Name = "Western Digital Blue 4TB", Tooltip = "Típus: HDD\nKapacitás: 4000 GB" });
                    result.Add(new { Id = 7, Name = "Kingston A2000 1TB", Tooltip = "Típus: SSD\nKapacitás: 1000 GB" });
                    result.Add(new { Id = 8, Name = "Seagate Ironwolf 8TB", Tooltip = "Típus: HDD\nKapacitás: 8000 GB" });
                    break;
                case "Motherboard":
                    result.Add(new { Id = 1, Name = "ASUS ROG STRIX Z690-E", Tooltip = "Gyártó: ASUS\nChipset: Z690\nFoglalat: LGA 1700" });
                    result.Add(new { Id = 2, Name = "MSI MAG B550 TOMAHAWK", Tooltip = "Gyártó: MSI\nChipset: B550\nFoglalat: AM4" });
                    result.Add(new { Id = 3, Name = "Gigabyte Z590 AORUS ELITE", Tooltip = "Gyártó: Gigabyte\nChipset: Z590\nFoglalat: LGA 1200" });
                    result.Add(new { Id = 4, Name = "ASRock B450M PRO4", Tooltip = "Gyártó: ASRock\nChipset: B450\nFoglalat: AM4" });
                    result.Add(new { Id = 5, Name = "ASUS TUF GAMING X570-PLUS", Tooltip = "Gyártó: ASUS\nChipset: X570\nFoglalat: AM4" });
                    result.Add(new { Id = 6, Name = "MSI MPG B550 GAMING EDGE", Tooltip = "Gyártó: MSI\nChipset: B550\nFoglalat: AM4" });
                    result.Add(new { Id = 7, Name = "Gigabyte B660 AORUS MASTER", Tooltip = "Gyártó: Gigabyte\nChipset: B660\nFoglalat: LGA 1700" });
                    break;
                case "PSU":
                    result.Add(new { Id = 1, Name = "Corsair RM850x", Tooltip = "Teljesítmény: 850 W\nHatásfok: Gold" });
                    result.Add(new { Id = 2, Name = "EVGA SuperNOVA 750 G5", Tooltip = "Teljesítmény: 750 W\nHatásfok: Gold" });
                    result.Add(new { Id = 3, Name = "Seasonic Focus GX-650", Tooltip = "Teljesítmény: 650 W\nHatásfok: Gold" });
                    result.Add(new { Id = 4, Name = "be quiet! Straight Power 11 750W", Tooltip = "Teljesítmény: 750 W\nHatásfok: Gold" });
                    result.Add(new { Id = 5, Name = "Corsair HX1000", Tooltip = "Teljesítmény: 1000 W\nHatásfok: Platinum" });
                    result.Add(new { Id = 6, Name = "Seasonic Prime TX-1000", Tooltip = "Teljesítmény: 1000 W\nHatásfok: Titanium" });
                    result.Add(new { Id = 7, Name = "EVGA SuperNOVA 1600 T2", Tooltip = "Teljesítmény: 1600 W\nHatásfok: Titanium" });
                    break;
                case "Case":
                    result.Add(new { Id = 1, Name = "NZXT H510", Tooltip = "Formátum: Mid Tower" });
                    result.Add(new { Id = 2, Name = "Corsair 4000D", Tooltip = "Formátum: Mid Tower" });
                    result.Add(new { Id = 3, Name = "Fractal Design Meshify C", Tooltip = "Formátum: Mid Tower" });
                    result.Add(new { Id = 4, Name = "Lian Li PC-O11 Dynamic", Tooltip = "Formátum: Mid Tower" });
                    result.Add(new { Id = 5, Name = "Phanteks Eclipse P500A", Tooltip = "Formátum: Mid Tower" });
                    result.Add(new { Id = 6, Name = "be quiet! Pure Base 500DX", Tooltip = "Formátum: Mid Tower" });
                    result.Add(new { Id = 7, Name = "Corsair iCUE 5000X", Tooltip = "Formátum: Mid Tower" });
                    result.Add(new { Id = 8, Name = "Fractal Design Define 7", Tooltip = "Formátum: Full Tower" });
                    result.Add(new { Id = 9, Name = "Thermaltake View 71", Tooltip = "Formátum: Full Tower" });
                    break;
            }
            
            return result;
        }

        private List<CPUView> LoadCPUs()
        {
            var list = new List<CPUView>();
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand(@"
                    SELECT c.*, s.SocketName, s.Generation 
                    FROM CPUs c 
                    LEFT JOIN SocketTypes s ON c.SocketTypeId = s.Id", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var cpu = new CPUView
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString(),
                            Tooltip = $"Gyártó: {reader["Manufacturer"]}\nFoglalat: {reader["SocketName"]} {(reader["Generation"] != DBNull.Value ? $"({reader["Generation"]})" : "")}\nMagok: {reader["Cores"]}\nSzálak: {reader["Threads"]}\nAlap órajel: {reader["BaseClockGHz"]} GHz\nMax órajel: {reader["BoostClockGHz"]} GHz"
                        };
                        list.Add(cpu);
                    }
                }
            }
            return list;
        }

        private List<GPUView> LoadGPUs()
        {
            var list = new List<GPUView>();
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand("SELECT * FROM GPUs", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var gpu = new GPUView
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString(),
                            Tooltip = $"Gyártó: {reader["Manufacturer"]}\nMemória: {reader["MemoryGB"]} GB"
                        };
                        list.Add(gpu);
                    }
                }
            }
            return list;
        }

        private List<RAMView> LoadRAMs()
        {
            var list = new List<RAMView>();
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand("SELECT * FROM RAMs", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var ram = new RAMView
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString(),
                            Tooltip = $"Kapacitás: {reader["CapacityGB"]} GB\nÓrajel: {reader["SpeedMHz"]} MHz\nTípus: {reader["Type"]}"
                        };
                        list.Add(ram);
                    }
                }
            }
            return list;
        }

        private List<StorageView> LoadStorages()
        {
            var list = new List<StorageView>();
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand("SELECT * FROM Storages", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var storage = new StorageView
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString(),
                            Tooltip = $"Típus: {reader["Type"]}\nKapacitás: {reader["CapacityGB"]} GB"
                        };
                        list.Add(storage);
                    }
                }
            }
            return list;
        }

        private List<MotherboardView> LoadMotherboards()
        {
            var list = new List<MotherboardView>();
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            
            try
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                {
                    conn.Open();
                    
                    // Ellenőrizzük, hogy létezik-e a MotherboardView nézet
                    bool viewExists = CheckIfViewExists("MotherboardView", conn);
                    string query = viewExists 
                        ? "SELECT * FROM MotherboardView" 
                        : "SELECT m.*, s.SocketName FROM Motherboards m LEFT JOIN SocketTypes s ON m.SocketTypeId = s.Id";
                    
                    System.Diagnostics.Debug.WriteLine($"Alaplapok lekérdezése: {query}");
                    
                    using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        // Debug: kiírjuk az összes oszlop nevét
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            System.Diagnostics.Debug.WriteLine($"Oszlop {i}: {reader.GetName(i)}");
                        }
                        
                        while (reader.Read())
                        {
                            string socketInfo = "";
                            
                            // Ha létezik a SocketName oszlop, azt használjuk
                            if (HasColumn(reader, "SocketName"))
                            {
                                socketInfo = reader["SocketName"].ToString();
                            }
                            // Ha nem, akkor megnézzük, hogy van-e Socket oszlop
                            else if (HasColumn(reader, "Socket"))
                            {
                                socketInfo = reader["Socket"].ToString();
                            }
                            
                            // Chipset információ
                            string chipsetInfo = HasColumn(reader, "ChipsetTypeId") 
                                ? $"Chipset ID: {reader["ChipsetTypeId"]}" 
                                : (HasColumn(reader, "Chipset") ? reader["Chipset"].ToString() : "N/A");
                            
                            var mb = new MotherboardView
                            {
                                Id = (int)reader["Id"],
                                Name = reader["Name"].ToString(),
                                Tooltip = $"Gyártó: {reader["Manufacturer"]}\n{chipsetInfo}\nFoglalat: {socketInfo}"
                            };
                            list.Add(mb);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba az alaplapok betöltésekor: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Belső hiba: {ex.InnerException.Message}");
                }
            }
            
            return list;
        }
        
        // Ellenőrizzük, hogy létezik-e a megadott nézet
        private bool CheckIfViewExists(string viewName, System.Data.SqlClient.SqlConnection connection)
        {
            try
            {
                string query = @"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.VIEWS 
                    WHERE TABLE_NAME = @ViewName";
                
                using (var cmd = new System.Data.SqlClient.SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ViewName", viewName);
                    int count = (int)cmd.ExecuteScalar();
                    
                    System.Diagnostics.Debug.WriteLine($"A {viewName} nézet létezik: {count > 0}");
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a nézet létezésének ellenőrzésekor ({viewName}): {ex.Message}");
                return false;
            }
        }
        
        // Az eredeti HasColumn metódus az 573. sorban már definiálva van
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba az oszlop ellenőrzése közben: {columnName} - {ex.Message}");
                return false;
            }
        }

        private List<PSUView> LoadPSUs()
        {
            var list = new List<PSUView>();
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand("SELECT * FROM PSUs", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var psu = new PSUView
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString(),
                            Tooltip = $"Teljesítmény: {reader["Wattage"]} W\nHatásfok: {reader["EfficiencyRating"]}"
                        };
                        list.Add(psu);
                    }
                }
            }
            return list;
        }

        private List<CaseView> LoadCases()
        {
            var list = new List<CaseView>();
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand("SELECT * FROM Cases", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var c = new CaseView
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString(),
                            Tooltip = $"Formátum: {reader["FormFactor"]}"
                        };
                        list.Add(c);
                    }
                }
            }
            return list;
        }

        // Komponensek keresése név alapján
        private List<object> FilterComponentsByName(List<object> items, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return items;
                
            // Kisbetűsítjük a keresési kifejezést az ékezet-érzékeny összehasonlításhoz
            searchTerm = searchTerm.ToLower();
            
            // LINQ lekérdezéssel szűrjük a neveket a "Name" tulajdonság alapján
            // A dynamic típusú obj segítségével elérjük a "Name" tulajdonságot
            var filteredItems = items.Where(item => 
            {
                dynamic obj = item;
                string name = obj.Name.ToString().ToLower();
                return name.Contains(searchTerm);
            }).ToList();
            
            return filteredItems;
        }
        
        // Keresés gomb eseménykezelője
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            PerformSearch();
        }
        
        // Keresés a TextBox-ba történő gépeléskor (KeyUp esemény)
        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            // Ha Enter-t ütünk, azonnal keresünk
            if (e.Key == Key.Enter)
            {
                PerformSearch();
            }
        }
        
        // Keresés végrehajtása
        private void PerformSearch()
        {
            try
            {
                string searchTerm = SearchTextBox.Text?.Trim() ?? "";
                
                // Ha a keresési kifejezés megváltozott, akkor végezzünk keresést
                if (searchTerm != _lastSearchTerm || _allComponents == null)
                {
                    LoadComponents(_currentType, searchTerm);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a keresés során: {ex.Message}", "Keresési hiba");
            }
        }
        
        // ViewModel-ek a listához (property-k, hogy működjön a binding)
        private class CPUView { public int Id { get; set; } public string Name { get; set; } public string Tooltip { get; set; } }
        private class GPUView { public int Id { get; set; } public string Name { get; set; } public string Tooltip { get; set; } }
        private class RAMView { public int Id { get; set; } public string Name { get; set; } public string Tooltip { get; set; } }        private class StorageView { public int Id { get; set; } public string Name { get; set; } public string Tooltip { get; set; } }        private class MotherboardView { public int Id { get; set; } public string Name { get; set; } public string Tooltip { get; set; } }
        private class PSUView { public int Id { get; set; } public string Name { get; set; } public string Tooltip { get; set; } }
        private class CaseView { public int Id { get; set; } public string Name { get; set; } public string Tooltip { get; set; } }
        
        // Komponensre kattintás eseménykezelő
        private void ComponentItem_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender is Border border && border.Tag != null)
                {
                    int componentId = Convert.ToInt32(border.Tag);
                    ShowComponentDetails(_currentType, componentId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a komponens megnyitása közben: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        // Komponens részletek megjelenítése
        private void ShowComponentDetails(string type, int id)
        {
            try
            {
                // Komponens adatok betöltése az adatbázisból
                object component = LoadComponentById(type, id);
                
                if (component == null)
                {
                    MessageBox.Show($"A(z) {id} azonosítójú {type} komponens nem található.", 
                        "Komponens részletei", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // Konfiguráció építőhöz adás lehetőségének felajánlása
                MessageBoxResult result = MessageBox.Show(
                    $"Szeretné hozzáadni a kiválasztott {type} komponenst a konfigurációs építőhöz?", 
                    "Komponens hozzáadása", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    // Komponens hozzáadása a konfigurációhoz
                    AddComponentToBuilder(type, component);
                    
                    // Átváltás a konfiguráció építőre
                    SwitchToConfigBuilder();
                }
                else
                {
                    // Ha nem akar a felhasználó hozzáadni, akkor csak megjeleníti a komponens részleteit
                    MessageBox.Show($"A(z) {type} típusú, {id} azonosítójú komponens részletei.\n\nEz a funkció jelenleg fejlesztés alatt áll.", 
                        "Komponens részletei", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a komponens részleteinek megjelenítése közben: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        // Komponens hozzáadása a konfigurációs építőhöz
        private void AddComponentToBuilder(string type, object component)
        {
            try
            {
                // ComponentInfo létrehozása a komponens típusa alapján
                ComponentInfo componentInfo = null;
                
                switch (type)
                {
                    case "CPU":
                        var cpu = component as Models.CPU;
                        if (cpu != null)
                        {
                            componentInfo = new ComponentInfo
                            {
                                Id = cpu.Id,
                                Name = cpu.Name,
                                Details = $"{cpu.Cores} mag / {cpu.Threads} szál, {cpu.BaseClockGHz} GHz ({cpu.BoostClockGHz} GHz)",
                                Price = cpu.Price,
                                Power = 95, // Becsült fogyasztás
                                Type = type
                            };
                        }
                        break;
                    case "GPU":
                        var gpu = component as Models.GPU;
                        if (gpu != null)
                        {
                            componentInfo = new ComponentInfo
                            {
                                Id = gpu.Id,
                                Name = gpu.Name,
                                Details = $"{gpu.MemoryGB} GB {gpu.MemoryType}",
                                Price = gpu.Price,
                                Power = 220, // Becsült fogyasztás
                                Type = type
                            };
                        }
                        break;
                    case "RAM":
                        var ram = component as Models.RAM;
                        if (ram != null)
                        {
                            componentInfo = new ComponentInfo
                            {
                                Id = ram.Id,
                                Name = ram.Name,
                                Details = $"{ram.CapacityGB} GB {ram.Type}, {ram.SpeedMHz} MHz",
                                Price = ram.Price,
                                Power = 10, // Becsült fogyasztás
                                Type = type
                            };
                        }
                        break;
                    case "Storage":
                        var storage = component as Models.Storage;
                        if (storage != null)
                        {
                            componentInfo = new ComponentInfo
                            {
                                Id = storage.Id,
                                Name = storage.Name,
                                Details = $"{storage.Type}, {storage.CapacityGB} GB",
                                Price = storage.Price,
                                Power = 5, // Becsült fogyasztás
                                Type = type
                            };
                        }
                        break;
                    case "Motherboard":
                        var mb = component as Models.Motherboard;
                        if (mb != null)
                        {
                            componentInfo = new ComponentInfo
                            {
                                Id = mb.Id,
                                Name = mb.Name,
                                Details = $"{mb.Manufacturer} {mb.Chipset}, {mb.Socket}",
                                Price = mb.Price,
                                Power = 20, // Becsült fogyasztás
                                Type = type
                            };
                        }
                        break;
                    case "PSU":
                        var psu = component as Models.PSU;
                        if (psu != null)
                        {
                            componentInfo = new ComponentInfo
                            {
                                Id = psu.Id,
                                Name = psu.Name,
                                Details = $"{psu.Wattage} W, {psu.EfficiencyRating}",
                                Price = psu.Price,
                                Power = 0, // Tápegység nem fogyaszt a rendszer szempontjából
                                Type = type
                            };
                        }
                        break;
                    case "Case":
                        var pcCase = component as Models.Case;
                        if (pcCase != null)
                        {
                            componentInfo = new ComponentInfo
                            {
                                Id = pcCase.Id,
                                Name = pcCase.Name,
                                Details = $"{pcCase.FormFactor}, {pcCase.Color}",
                                Price = pcCase.Price,
                                Power = 0, // Ház nem fogyaszt a rendszer szempontjából
                                Type = type
                            };
                        }
                        break;
                }
                
                if (componentInfo != null)
                {
                    // Komponens hozzáadása a ConfigBuilderStore-hoz
                    var store = ConfigBuilderStore.GetInstance();
                    store.AddComponent(type, componentInfo);
                    
                    MessageBox.Show($"A(z) {type} komponens hozzáadva a konfigurációs építőhöz!", 
                        "Komponens hozzáadva", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Komponens hozzáadása a konfigurációhoz");
            }
        }
        
        // Átváltás a konfiguráció építőre
        private void SwitchToConfigBuilder()
        {
            try
            {
                var dashboard = Window.GetWindow(this) as Views.App.Dashboard;
                if (dashboard != null)
                {
                    dashboard.SetActivePage(typeof(ConfigBuilder));
                }
                else
                {
                    MessageBox.Show("Nem sikerült átváltani a konfiguráció építőre.", 
                        "Navigációs hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Átváltás a konfiguráció építőre");
            }
        }

        // Új komponens hozzáadási gomb eseménykezelő
        private void AddComponentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // A szülő ablakot (Dashboard) megkeressük és a típusnak megfelelő AddComponents oldalra navigálunk
                var dashboard = Window.GetWindow(this) as Dashboard;
                if (dashboard != null)
                {                    // A SetActivePage metódust használjuk a NavigateToPage helyett
                    dashboard.SetActivePage(typeof(PC_Configurator.Views.App.AddComponents));
                    
                    // Átadhatjuk a kiválasztott komponens típust is
                    var addComponentsControl = dashboard.FindName("MainContentArea") as ContentControl;
                    if (addComponentsControl != null && addComponentsControl.Content is AddComponents addComponentsPage)
                    {
                        // Ha van ilyen metódus az AddComponents oldalon, akkor beállíthatjuk a típust
                        // addComponentsPage.SelectComponentType(_currentType);
                    }
                }
                else
                {
                    MessageBox.Show("Nem sikerült hozzáférni a fő ablakhoz.", "Navigációs hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba az új komponens hozzáadása során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetupPermissions()
        {
            try
            {
                // Az adatok betöltése után a DataTemplate minden eleméhez beállítjuk az admin gombok láthatóságát
                ComponentList.Loaded += ComponentList_Loaded;
                
                // ItemsControl elemei ütemezetten készülnek el, ezért feliratkozunk a konténerek létrehozására
                ComponentList.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
                
                // A ComponentList.ItemsSource változásakor is frissítjük a láthatóságot
                DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(
                    ItemsControl.ItemsSourceProperty,
                    typeof(ListView)
                );
                
                descriptor.AddValueChanged(ComponentList, ItemsSourceChanged);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Admin jogosultságok beállítása");
            }
        }
        
        private void ComponentList_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kezdeti betöltéskor az összes admin gomb láthatóságának beállítása
                UpdateAdminButtonsVisibility();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a komponens lista betöltésekor: {ex.Message}");
            }
        }
        
        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            try
            {
                // Ha a konténerek elkészültek, frissítjük a gombok láthatóságát
                if (ComponentList.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                {
                    UpdateAdminButtonsVisibility();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a konténerek létrehozása közben: {ex.Message}");
            }
        }
        
        private void ItemsSourceChanged(object sender, EventArgs e)
        {
            try
            {
                // Késleltetett frissítés, hogy a konténerek biztosan elkészüljenek
                ComponentList.Dispatcher.BeginInvoke(
                    new Action(() => UpdateAdminButtonsVisibility()),
                    System.Windows.Threading.DispatcherPriority.Loaded
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba az adatforrás változásakor: {ex.Message}");
            }
        }
        
        private void UpdateAdminButtonsVisibility()
        {
            try
            {
                foreach (var item in ComponentList.Items)
                {
                    if (ComponentList.ItemContainerGenerator.ContainerFromItem(item) is FrameworkElement container)
                    {
                        ApplyAdminButtonsVisibility(container);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a gombok láthatóságának frissítésekor: {ex.Message}");
            }
        }
        
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Jogosultság ellenőrzés
                if (!PermissionManager.IsAdmin)
                {
                    ErrorHandler.ShowError("Csak admin felhasználók szerkeszthetik az alkatrészeket!", "Jogosultság megtagadva");
                    return;
                }
                
                Button btn = sender as Button;
                if (btn != null)
                {
                    int componentId = Convert.ToInt32(btn.Tag);
                    OpenEditFormByType(_currentType, componentId);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Komponens szerkesztése");
            }
        }
        
        private void OpenEditFormByType(string type, int id)
        {
            try
            {
                // Komponens adatok betöltése az adatbázisból
                object component = LoadComponentById(type, id);
                
                if (component == null)
                {
                    MessageBox.Show($"A(z) {id} azonosítójú {type} komponens nem található.", 
                        "Szerkesztési hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // A szerkesztési ablak létrehozása
                Window editWindow = new Window
                {
                    Title = $"{type} szerkesztése",
                    Width = 900,
                    Height = 750,
                    MinWidth = 850,
                    MinHeight = 650,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    ResizeMode = ResizeMode.CanResize,
                    SizeToContent = SizeToContent.Manual,
                    Background = new SolidColorBrush(Color.FromRgb(18, 18, 18)) // #121212
                };

                // A megfelelő form betöltése a UserControl típus alapján
                UserControl formControl = null;
                
                switch (type)
                {
                    case "CPU":
                        var cpuForm = new Views.Forms.CPU();
                        cpuForm.LoadForEdit(id);
                        cpuForm.SaveCompleted += (s, args) => editWindow.Close();
                        formControl = cpuForm;
                        break;
                        
                    case "GPU":
                        var gpuForm = new Views.Forms.GPU();
                        gpuForm.LoadForEdit(id);
                        gpuForm.SaveCompleted += (s, args) => editWindow.Close();
                        formControl = gpuForm;
                        break;
                        
                    case "RAM":
                        var ramForm = new Views.Forms.RAM();
                        ramForm.LoadForEdit(id);
                        ramForm.SaveCompleted += (s, args) => editWindow.Close();
                        formControl = ramForm;
                        break;
                        
                    case "Storage":
                        var storageForm = new Views.Forms.Storage();
                        storageForm.LoadForEdit(id);
                        storageForm.SaveCompleted += (s, args) => editWindow.Close();
                        formControl = storageForm;
                        break;
                        
                    case "Motherboard":
                        var motherboardForm = new Views.Forms.Motherboard();
                        var motherboard = component as Models.Motherboard;
                        if (motherboard != null)
                        {
                            motherboardForm.LoadForEdit(motherboard);
                            motherboardForm.SaveCompleted += (s, args) => editWindow.Close();
                        }
                        formControl = motherboardForm;
                        break;
                        
                    case "PSU":
                        var psuForm = new Views.Forms.PSU();
                        psuForm.LoadForEdit(id);
                        psuForm.SaveCompleted += (s, args) => editWindow.Close();
                        formControl = psuForm;
                        break;
                        
                    case "Case":
                        var caseForm = new Views.Forms.Case();
                        var caseModel = component as Models.Case;
                        if (caseModel != null)
                        {
                            caseForm.LoadForEdit(caseModel);
                            caseForm.SaveCompleted += (s, args) => editWindow.Close();
                        }
                        formControl = caseForm;
                        break;
                        
                    default:
                        MessageBox.Show($"Ismeretlen komponens típus: {type}", 
                            "Szerkesztési hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }
                
                // A form hozzáadása az ablakhoz, ha sikeresen létrehoztuk
                if (formControl != null)
                {
                    // ScrollViewer létrehozása, hogy gördíthető legyen a tartalom
                    ScrollViewer scrollViewer = new ScrollViewer
                    {
                        VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                        Content = formControl
                    };
                    
                    editWindow.Content = scrollViewer;
                    editWindow.Owner = Window.GetWindow(this);
                    editWindow.ShowDialog();
                    
                    // Form bezárása után frissítjük a listát
                    LoadComponents(_currentType);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Szerkesztő űrlap megnyitása");
                
                // Fallback: demó üzenet a felhasználónak
                MessageBox.Show($"A(z) {id} ID-jú {type} szerkesztése indulna! (Demó mód)", 
                            "Demó szerkesztés", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        private object LoadComponentById(string type, int id)
        {
            try
            {
                // Az adott típusú komponens betöltése az adatbázisból az ID alapján
                string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                {
                    conn.Open();
                    string tableName = GetTableNameByType(type);
                    
                    // Ellenőrizzük, hogy a tábla létezik-e az adatbázisban
                    bool tableExists = false;
                    string checkTable = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName";
                    using (var checkCmd = new System.Data.SqlClient.SqlCommand(checkTable, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@TableName", tableName);
                        int tableCount = (int)checkCmd.ExecuteScalar();
                        tableExists = (tableCount > 0);
                    }
                    
                    if (!tableExists)
                    {
                        ErrorHandler.ShowError($"A(z) {tableName} tábla nem található az adatbázisban.", "Adatbázis hiba");
                        return null;
                    }
                    
                    string query = $"SELECT * FROM {tableName} WHERE Id = @Id";
                    
                    using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Itt a megfelelő típusú objektumot hozzuk létre
                                switch (type)
                                {
                                    case "CPU":
                                        return new Models.CPU
                                        {
                                            Id = id,
                                            Name = reader["Name"].ToString(),
                                            Manufacturer = HasColumn(reader, "Manufacturer") ? reader["Manufacturer"].ToString() : "Ismeretlen",
                                            Socket = HasColumn(reader, "Socket") ? reader["Socket"].ToString() : "LGA1200",
                                            Cores = HasColumn(reader, "Cores") ? Convert.ToInt32(reader["Cores"]) : 4,
                                            Threads = HasColumn(reader, "Threads") ? Convert.ToInt32(reader["Threads"]) : 8,
                                            BaseClockGHz = HasColumn(reader, "BaseClockGHz") ? (float)Convert.ToDouble(reader["BaseClockGHz"]) : 3.5f,
                                            BoostClockGHz = HasColumn(reader, "BoostClockGHz") ? (float)Convert.ToDouble(reader["BoostClockGHz"]) : 4.0f,
                                            Price = HasColumn(reader, "Price") && reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : 0
                                        };
                                    
                                    case "GPU":
                                        return new Models.GPU
                                        {
                                            Id = id,
                                            Name = reader["Name"].ToString(),
                                            Manufacturer = reader["Manufacturer"].ToString(),
                                            MemoryGB = Convert.ToInt32(reader["MemoryGB"]),
                                            // A MemoryType és Price nem biztos hogy szerepel a táblában
                                            MemoryType = HasColumn(reader, "MemoryType") && reader["MemoryType"] != DBNull.Value 
                                                ? reader["MemoryType"].ToString() 
                                                : "GDDR6",
                                            Price = HasColumn(reader, "Price") && reader["Price"] != DBNull.Value 
                                                ? Convert.ToDecimal(reader["Price"]) 
                                                : 0
                                        };
                                    
                                    case "RAM":
                                        return new Models.RAM
                                        {
                                            Id = id,
                                            Name = reader["Name"].ToString(),
                                            CapacityGB = Convert.ToInt32(reader["CapacityGB"]),
                                            SpeedMHz = Convert.ToInt32(reader["SpeedMHz"]),
                                            Type = HasColumn(reader, "Type") && reader["Type"] != DBNull.Value 
                                                ? reader["Type"].ToString() 
                                                : "DDR4",
                                            Price = HasColumn(reader, "Price") && reader["Price"] != DBNull.Value 
                                                ? Convert.ToDecimal(reader["Price"]) 
                                                : 0
                                        };
                                    
                                    case "Storage":
                                        return new Models.Storage
                                        {
                                            Id = id,
                                            Name = reader["Name"].ToString(),
                                            Type = HasColumn(reader, "Type") && reader["Type"] != DBNull.Value
                                                ? reader["Type"].ToString()
                                                : "SSD",
                                            CapacityGB = Convert.ToInt32(reader["CapacityGB"]),
                                            Price = HasColumn(reader, "Price") && reader["Price"] != DBNull.Value
                                                ? Convert.ToDecimal(reader["Price"])
                                                : 0
                                        };
                                        
                                    case "PSU":
                                        return new Models.PSU
                                        {
                                            Id = id,
                                            Name = reader["Name"].ToString(),
                                            Wattage = Convert.ToInt32(reader["Wattage"]),
                                            EfficiencyRating = HasColumn(reader, "EfficiencyRating") && reader["EfficiencyRating"] != DBNull.Value
                                                ? reader["EfficiencyRating"].ToString()
                                                : "80+",
                                            Price = HasColumn(reader, "Price") && reader["Price"] != DBNull.Value
                                                ? Convert.ToDecimal(reader["Price"])
                                                : 0
                                        };
                                        
                                    case "Motherboard":
                                        return new Models.Motherboard
                                        {
                                            Id = id,
                                            Name = reader["Name"].ToString(),
                                            Manufacturer = HasColumn(reader, "Manufacturer") ? reader["Manufacturer"].ToString() : "Ismeretlen",
                                            Chipset = HasColumn(reader, "Chipset") ? reader["Chipset"].ToString() : "B660",
                                            Socket = HasColumn(reader, "Socket") ? reader["Socket"].ToString() : "LGA1700",
                                            FormFactor = HasColumn(reader, "FormFactor") ? reader["FormFactor"].ToString() : "ATX",
                                            MaxMemoryGB = HasColumn(reader, "MaxMemoryGB") ? Convert.ToInt32(reader["MaxMemoryGB"]) : 128,
                                            PowerConsumption = HasColumn(reader, "PowerConsumption") ? Convert.ToInt32(reader["PowerConsumption"]) : 65,
                                            Price = HasColumn(reader, "Price") && reader["Price"] != DBNull.Value
                                                ? Convert.ToDecimal(reader["Price"])
                                                : 0
                                        };
                                        
                                    case "Case":
                                        return new Models.Case
                                        {
                                            Id = id,
                                            Name = reader["Name"].ToString(),
                                            FormFactor = HasColumn(reader, "FormFactor") ? reader["FormFactor"].ToString() : "ATX",
                                            Color = HasColumn(reader, "Color") ? reader["Color"].ToString() : "Black",
                                            Price = HasColumn(reader, "Price") && reader["Price"] != DBNull.Value
                                                ? Convert.ToDecimal(reader["Price"])
                                                : 0
                                        };
                                    
                                    // További komponens típusok betöltése hasonló módon...
                                    
                                    default:
                                        throw new ArgumentException($"Ismeretlen komponens típus: {type}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Részletesebb hibaüzenet megjelenítése
                ErrorHandler.HandleDatabaseError(ex, "betöltés", type);
                System.Diagnostics.Debug.WriteLine($"Hiba a {type} komponens betöltése közben (ID: {id}): {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                // Ha ellenőrizni szeretnénk az adatbázis kapcsolatot
                try
                {
                    string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                    using (var testConn = new System.Data.SqlClient.SqlConnection(connStr))
                    {
                        testConn.Open();
                        System.Diagnostics.Debug.WriteLine("Adatbázis kapcsolat sikeres");
                    }
                }
                catch (Exception dbEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Hiba az adatbázis kapcsolat tesztelése közben: {dbEx.Message}");
                }
            }
            
            // Fallback: null visszaadása hiba esetén
            return null;
        }
        
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!PermissionManager.IsAdmin)
            {
                MessageBox.Show("Csak admin felhasználók törölhetik az alkatrészeket!", 
                                "Jogosultság megtagadva", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            Button btn = sender as Button;
            if (btn != null)
            {
                int componentId = Convert.ToInt32(btn.Tag);
                MessageBoxResult result = MessageBox.Show($"Biztosan törölni szeretné a(z) {componentId} ID-jú alkatrészt?", 
                                             "Törlés megerősítése", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Itt jöne a törlés logikája
                        DeleteComponentById(componentId);
                        // Frissítsük a listát a törlés után
                        LoadComponents(_currentType);
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.HandleDatabaseError(ex, "törlés", _currentType);
                    }
                }
            }
        }
        
        private void DeleteComponentById(int id)
        {
            try
            {
                // A komponens típusától függően törlési logika az adatbázisból
                // Valós adatbázis esetén:
                string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                {
                    try
                    {
                        conn.Open();
                        string tableName = GetTableNameByType(_currentType);
                        string query = $"DELETE FROM {tableName} WHERE Id = @Id";
                        
                        using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            int affected = cmd.ExecuteNonQuery();
                            
                            if (affected > 0)
                            {
                                // Sikeres törlés
                                MessageBox.Show($"A(z) {id} azonosítójú {_currentType} komponens sikeresen törölve.", 
                                            "Sikeres törlés", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                // Nem létezik ilyen ID
                                ErrorHandler.ShowError($"A megadott azonosítójú ({id}) komponens nem található vagy már törölve lett.", "Törlési hiba");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Adatbázis hiba esetén
                        ErrorHandler.HandleError(ex, "Komponens törlése");
                        
                        // Fallback: demó üzenet a felhasználónak
                        MessageBox.Show($"A(z) {id} ID-jú {_currentType} sikeresen törölve lenne! (Demó mód)", 
                                    "Demó törlés", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Komponens törlés");
            }
        }
        
        private string GetTableNameByType(string type)
        {
            // A komponens típusa alapján visszaadja a megfelelő táblanevet
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

        private void ApplyAdminButtonsVisibility(FrameworkElement container)
        {
            try
            {
                // A container elemben megkeressük az AdminButtons StackPanelt
                if (FindChild<StackPanel>(container, "AdminButtons") is StackPanel adminButtons)
                {
                    // Admin felhasználó esetén látszik, más esetben nem
                    adminButtons.Visibility = PermissionManager.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                // Hiba esetén nem bukunk el, csak logolja a hibát
                System.Diagnostics.Debug.WriteLine($"Hiba az admin gombok láthatóságának beállításakor: {ex.Message}");
            }
        }
        
        // Segédfüggvény UI elem kereséséhez a vizuális fában
        private static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;
            
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                
                if (child is FrameworkElement fe && fe.Name == childName && child is T)
                {
                    return (T)child;
                }
                
                T foundChild = FindChild<T>(child, childName);
                if (foundChild != null) return foundChild;
            }
            
            return null;
        }
        
        // A HasColumn metódus már definiálva van fentebb, így ezt a másodlagos implementációt eltávolítottuk
    }
}
