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
                
                try
                {
                    LoadComponents(_currentType);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a komponensek betöltése közben: {ex.Message}\n\n{ex.StackTrace}", "Adatbetöltési hiba");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba az inicializálás során: {ex.Message}\n\n{ex.StackTrace}", "Inicializálási hiba");
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
                using (var cmd = new System.Data.SqlClient.SqlCommand("SELECT * FROM CPUs", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var cpu = new CPUView
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString(),
                            Tooltip = $"Gyártó: {reader["Manufacturer"]}\nMagok: {reader["Cores"]}\nSzálak: {reader["Threads"]}\nAlap órajel: {reader["BaseClockGHz"]} GHz\nMax órajel: {reader["BoostClockGHz"]} GHz"
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
            using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand("SELECT * FROM Motherboards", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var mb = new MotherboardView
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString(),
                            Tooltip = $"Gyártó: {reader["Manufacturer"]}\nChipset: {reader["Chipset"]}\nFoglalat: {reader["Socket"]}"
                        };
                        list.Add(mb);
                    }
                }
            }
            return list;
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
                // Egyelőre csak jelezzük a felhasználónak, hogy melyik komponenst választotta ki
                MessageBox.Show($"A(z) {type} típusú, {id} azonosítójú komponens részletei.\n\nEz a funkció jelenleg fejlesztés alatt áll.", 
                    "Komponens részletei", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // TODO: A jövőben itt megnyithatunk egy dialógust a komponens részletes adataival,
                // vagy átirányíthatjuk a felhasználót a szerkesztő felületre
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a komponens részleteinek megjelenítése közben: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
