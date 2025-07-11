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
using PC_Configurator.Helpers;

namespace PC_Configurator.Views.Forms
{
    /// <summary>
    /// Interaction logic for Motherboard.xaml
    /// </summary>
    public partial class Motherboard : UserControl
    {
        // Esemény a sikeres mentés jelzésére
        public event EventHandler SaveCompleted;

        public class ChipsetInfo
        {
            public string Chipset { get; set; }
            public string Socket { get; set; }
            public string Category { get; set; }
            public string Notes { get; set; }
        }

        // Dinamikus chipset listák az adatbázisból
        private List<ChipsetType> amdChipsets;
        private List<ChipsetType> intelChipsets;
        private List<ChipsetType> chipsets; // Összes chipset
        private List<SocketType> socketTypes;
        
        // Chipset és socket ID-k tárolása
        private int selectedChipsetId = 0;
        private int selectedSocketId = 0;

        private int? currentId = null;
        

        public Motherboard()
        {
            InitializeComponent();
            
            // Betöltjük a chipseteket és socketeket az adatbázisból
            LoadChipsetsAndSockets();
            
            ManufacturerComboBox.SelectedIndex = 0;
            PopulateChipsets();
            SaveButton.Click += SaveButton_Click;
            
            // Validációs események beállítása
            NameTextBox.LostFocus += (s, e) => ValidateName();
            NameTextBox.TextChanged += (s, e) => ValidateName();
            
            PriceTextBox.LostFocus += (s, e) => ValidatePrice();
            PriceTextBox.TextChanged += (s, e) => ValidatePrice();
            
            if (PowerConsumptionTextBox != null)
            {
                PowerConsumptionTextBox.LostFocus += (s, e) => ValidatePowerConsumption();
                PowerConsumptionTextBox.TextChanged += (s, e) => ValidatePowerConsumption();
            }
            
            ManufacturerComboBox.SelectionChanged += ManufacturerComboBox_SelectionChanged;
            ChipsetComboBox.SelectionChanged += ChipsetComboBox_SelectionChanged;
            FormFactorComboBox.SelectionChanged += (s, e) => ValidateFormFactor();
            SocketComboBox.SelectionChanged += SocketComboBox_SelectionChanged;
        }
        
        // Chipset és Socket adatok betöltése az adatbázisból
        private void LoadChipsetsAndSockets()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            
            try
            {
                using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    connection.Open();
                    System.Diagnostics.Debug.WriteLine("Adatbázis kapcsolat megnyitva.");
                    
                    // Chipset típusok betöltése
                    using (var command = new System.Data.SqlClient.SqlCommand(
                        "SELECT * FROM ChipsetTypes ORDER BY Manufacturer, ChipsetName", connection))
                    {
                        System.Diagnostics.Debug.WriteLine("Chipset típusok betöltése...");
                        
                        var chipsetList = new List<ChipsetType>();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var chipset = new ChipsetType
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    ChipsetName = reader["ChipsetName"].ToString(),
                                    Manufacturer = reader["Manufacturer"].ToString()
                                };

                                try { chipset.Generation = reader["Generation"].ToString(); } catch { }
                                try { chipset.Description = reader["Description"].ToString(); } catch { }
                                try { if (reader["Features"] != DBNull.Value) chipset.Description = reader["Features"].ToString(); } catch { }

                                chipsetList.Add(chipset);
                            }
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"Betöltve {chipsetList.Count} chipset típus.");
                        
                        // Összes chipset és a gyártók szerinti szűrt listák
                        this.chipsets = chipsetList;
                        amdChipsets = chipsetList.Where(c => c.Manufacturer == "AMD").ToList();
                        intelChipsets = chipsetList.Where(c => c.Manufacturer == "Intel").ToList();
                    }
                    
                    // Socket típusok betöltése
                    using (var command = new System.Data.SqlClient.SqlCommand(
                        "SELECT * FROM SocketTypes ORDER BY Manufacturer, SocketName", connection))
                    {
                        System.Diagnostics.Debug.WriteLine("Socket típusok betöltése...");
                        
                        var socketList = new List<SocketType>();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var socket = new SocketType
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    SocketName = reader["SocketName"].ToString(),
                                    Manufacturer = reader["Manufacturer"].ToString()
                                };
                                
                                try { socket.Generation = reader["Generation"].ToString(); } catch { }
                                try { socket.Description = reader["Description"].ToString(); } catch { }
                                
                                socketList.Add(socket);
                            }
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"Betöltve {socketList.Count} socket típus.");
                        this.socketTypes = socketList;
                    }
                    
                    // Debug információ
                    if (amdChipsets != null && amdChipsets.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"AMD chipset példák: {amdChipsets.Count} db");
                        foreach (var chip in amdChipsets.Take(3))
                        {
                            System.Diagnostics.Debug.WriteLine($" - {chip.ChipsetName} (ID: {chip.Id})");
                        }
                    }
                    
                    if (intelChipsets != null && intelChipsets.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Intel chipset példák: {intelChipsets.Count} db");
                        foreach (var chip in intelChipsets.Take(3))
                        {
                            System.Diagnostics.Debug.WriteLine($" - {chip.ChipsetName} (ID: {chip.Id})");
                        }
                    }
                    
                    if (socketTypes != null && socketTypes.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Socket példák: {socketTypes.Count} db");
                        foreach (var socket in socketTypes.Take(3))
                        {
                            System.Diagnostics.Debug.WriteLine($" - {socket.SocketName} (Gyártó: {socket.Manufacturer}, ID: {socket.Id})");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a chipset és socket típusok betöltésekor: {ex.Message}");
                
                // Fallback listák használata hiba esetén
                amdChipsets = GetFallbackAmdChipsets();
                intelChipsets = GetFallbackIntelChipsets();
                chipsets = new List<ChipsetType>();
                chipsets.AddRange(amdChipsets);
                chipsets.AddRange(intelChipsets);
                socketTypes = GetFallbackSocketTypes();
            }
        }
        
        // Fallback chipset és socket listák
        private List<ChipsetType> GetFallbackAmdChipsets()
        {
            var list = new List<ChipsetType>();
            int id = 1;
            
            // AM4 Chipsets
            list.Add(new ChipsetType { Id = id++, ChipsetName = "A320", Manufacturer = "AMD", Generation = "Ryzen 1000, 2000, 3000", Description = "Entry-level chipset for AM4" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "B350", Manufacturer = "AMD", Generation = "Ryzen 1000, 2000, 3000", Description = "Mid-range chipset for AM4" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "B450", Manufacturer = "AMD", Generation = "Ryzen 1000, 2000, 3000, 5000", Description = "Mid-range chipset for AM4" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "X370", Manufacturer = "AMD", Generation = "Ryzen 1000, 2000, 3000", Description = "High-end chipset for AM4" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "X470", Manufacturer = "AMD", Generation = "Ryzen 1000, 2000, 3000, 5000", Description = "High-end chipset for AM4" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "B550", Manufacturer = "AMD", Generation = "Ryzen 3000, 5000", Description = "Mid-range chipset for AM4 with PCIe 4.0" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "X570", Manufacturer = "AMD", Generation = "Ryzen 3000, 5000", Description = "High-end chipset for AM4 with PCIe 4.0" });

            // AM5 Chipsets
            list.Add(new ChipsetType { Id = id++, ChipsetName = "B650", Manufacturer = "AMD", Generation = "Ryzen 7000", Description = "Mid-range chipset for AM5" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "X670", Manufacturer = "AMD", Generation = "Ryzen 7000", Description = "High-end chipset for AM5" });
            
            return list;
        }
        
        private List<ChipsetType> GetFallbackIntelChipsets()
        {
            var list = new List<ChipsetType>();
            int id = 100;
            
            // LGA1151 Chipsets
            list.Add(new ChipsetType { Id = id++, ChipsetName = "H310", Manufacturer = "Intel", Generation = "8th and 9th Gen", Description = "Entry-level chipset for Coffee Lake" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "B360", Manufacturer = "Intel", Generation = "8th and 9th Gen", Description = "Mid-range chipset for Coffee Lake" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "B365", Manufacturer = "Intel", Generation = "8th and 9th Gen", Description = "Mid-range chipset for Coffee Lake" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "H370", Manufacturer = "Intel", Generation = "8th and 9th Gen", Description = "Mid-range chipset for Coffee Lake" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "Z370", Manufacturer = "Intel", Generation = "8th Gen", Description = "High-end chipset for Coffee Lake" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "Z390", Manufacturer = "Intel", Generation = "8th and 9th Gen", Description = "High-end chipset for Coffee Lake refresh" });

            // LGA1200 Chipsets
            list.Add(new ChipsetType { Id = id++, ChipsetName = "H410", Manufacturer = "Intel", Generation = "10th Gen", Description = "Entry-level chipset for Comet Lake" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "B460", Manufacturer = "Intel", Generation = "10th Gen", Description = "Mid-range chipset for Comet Lake" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "H470", Manufacturer = "Intel", Generation = "10th Gen", Description = "Mid-range chipset for Comet Lake" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "Z490", Manufacturer = "Intel", Generation = "10th Gen", Description = "High-end chipset for Comet Lake" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "H510", Manufacturer = "Intel", Generation = "11th Gen", Description = "Entry-level chipset for Rocket Lake" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "B560", Manufacturer = "Intel", Generation = "11th Gen", Description = "Mid-range chipset for Rocket Lake" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "H570", Manufacturer = "Intel", Generation = "11th Gen", Description = "Mid-range chipset for Rocket Lake" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "Z590", Manufacturer = "Intel", Generation = "11th Gen", Description = "High-end chipset for Rocket Lake" });

            // LGA1700 Chipsets
            list.Add(new ChipsetType { Id = id++, ChipsetName = "H610", Manufacturer = "Intel", Generation = "12th and 13th Gen", Description = "Entry-level chipset for Alder Lake and Raptor Lake" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "B660", Manufacturer = "Intel", Generation = "12th and 13th Gen", Description = "Mid-range chipset for Alder Lake and Raptor Lake" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "H670", Manufacturer = "Intel", Generation = "12th and 13th Gen", Description = "Mid-range chipset for Alder Lake and Raptor Lake" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "Z690", Manufacturer = "Intel", Generation = "12th and 13th Gen", Description = "High-end chipset for Alder Lake and Raptor Lake" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "B760", Manufacturer = "Intel", Generation = "13th Gen", Description = "Mid-range chipset for Raptor Lake" });
            list.Add(new ChipsetType { Id = id++, ChipsetName = "Z790", Manufacturer = "Intel", Generation = "13th Gen", Description = "High-end chipset for Raptor Lake" });
            
            return list;
        }
        
        private List<SocketType> GetFallbackSocketTypes()
        {
            var list = new List<SocketType>();
            int id = 1;
            
            // AMD Socket Types
            list.Add(new SocketType { Id = id++, SocketName = "AM4", Manufacturer = "AMD", Generation = "Ryzen 1000, 2000, 3000, 5000", Description = "Socket for Ryzen CPUs" });
            list.Add(new SocketType { Id = id++, SocketName = "AM5", Manufacturer = "AMD", Generation = "Ryzen 7000", Description = "Socket for Ryzen 7000 series and newer" });
            list.Add(new SocketType { Id = id++, SocketName = "TR4", Manufacturer = "AMD", Generation = "Threadripper 1000, 2000", Description = "Socket for Threadripper CPUs" });
            list.Add(new SocketType { Id = id++, SocketName = "sTRX4", Manufacturer = "AMD", Generation = "Threadripper 3000", Description = "Socket for Threadripper 3000 series" });

            // Intel Socket Types
            list.Add(new SocketType { Id = id++, SocketName = "LGA1151", Manufacturer = "Intel", Generation = "8th and 9th Gen", Description = "Socket for Coffee Lake CPUs" });
            list.Add(new SocketType { Id = id++, SocketName = "LGA1200", Manufacturer = "Intel", Generation = "10th and 11th Gen", Description = "Socket for Comet Lake and Rocket Lake CPUs" });
            list.Add(new SocketType { Id = id++, SocketName = "LGA1700", Manufacturer = "Intel", Generation = "12th and 13th Gen", Description = "Socket for Alder Lake and Raptor Lake CPUs" });
            list.Add(new SocketType { Id = id++, SocketName = "LGA2066", Manufacturer = "Intel", Generation = "Core X-series", Description = "Socket for High-End Desktop (HEDT) CPUs" });
            
            return list;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Minden mező validálása
            bool isNameValid = ValidateName();
            bool isManufacturerValid = ValidateManufacturer();
            bool isChipsetValid = ValidateChipset();
            bool isFormFactorValid = ValidateFormFactor();
            bool isSocketValid = ValidateSocket();
            bool isPriceValid = ValidatePrice();
            bool isPowerConsumptionValid = ValidatePowerConsumption();

            // Teljes űrlap validáció
            if (!ValidationHelper.ValidateForm(
                isNameValid, isManufacturerValid, isChipsetValid, isFormFactorValid, 
                isSocketValid, isPriceValid, isPowerConsumptionValid))
            {
                return;
            }
            
            // Ha minden rendben, kiolvassuk az adatokat
            string name = NameTextBox.Text.Trim();
            string manufacturer = (ManufacturerComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            
            var chipsetItem = ChipsetComboBox.SelectedItem as ComboBoxItem;
            var socketItem = SocketComboBox.SelectedItem as ComboBoxItem;
            
            // Chipset és Socket név és ID kiolvasása
            string chipsetName = chipsetItem.Content.ToString();
            string socketName = socketItem.Content.ToString();
            
            // Az ID-k kiolvasása a Tag-ből
            int chipsetId = chipsetItem.Tag != null ? Convert.ToInt32(chipsetItem.Tag) : 0;
            int socketId = socketItem.Tag != null ? Convert.ToInt32(socketItem.Tag) : 0;
            
            string formFactor = (FormFactorComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            if (formFactor == "Válasszon...") formFactor = "ATX"; // Alapértelmezett formátum
            
            decimal price = 0;
            if (!string.IsNullOrWhiteSpace(PriceTextBox.Text))
            {
                decimal.TryParse(
                    PriceTextBox.Text.Replace(',', '.'),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out price
                );
            }

            // Fogyasztás kiolvasása
            int powerConsumption = 0;
            if (!string.IsNullOrWhiteSpace(PowerConsumptionTextBox?.Text))
            {
                int.TryParse(PowerConsumptionTextBox.Text, out powerConsumption);
            }
            else
            {
                powerConsumption = 65; // Alapértelmezett fogyasztás
            }
            
            // Memória slotok alapértelmezett értéke
            int memorySlots = 4; // Alapértelmezett érték
            
            // Maximum memória alapértelmezett értéke
            int maxMemoryGB = 128; // Alapértelmezett érték
            
            // Új alaplap modell létrehozása a mentéshez
            var model = new PC_Configurator.Models.Motherboard { 
                Name = name, 
                Manufacturer = manufacturer,
                Chipset = chipsetName, // Kompatibilitási okokból
                ChipsetTypeId = chipsetId,
                Socket = socketName,  // Kompatibilitási okokból
                SocketTypeId = socketId,
                FormFactor = formFactor,
                Price = price,
                PowerConsumption = powerConsumption,
                MemorySlots = memorySlots,
                MaxMemoryGB = maxMemoryGB
            };
            
            try
            {
                System.Diagnostics.Debug.WriteLine($"Alaplap mentése: {name}, ChipsetTypeId: {chipsetId}, SocketTypeId: {socketId}");
                
                // Új alaplap mentése
                if (currentId == null)
                {
                    model.SaveToDatabase();
                    MessageBox.Show("Alaplap sikeresen mentve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                    System.Diagnostics.Debug.WriteLine($"Alaplap mentve, ID: {model.Id}");
                }
                // Meglévő alaplap frissítése
                else
                {
                    model.Id = currentId.Value;
                    model.UpdateInDatabase();
                    MessageBox.Show("Alaplap sikeresen frissítve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                    System.Diagnostics.Debug.WriteLine($"Alaplap frissítve, ID: {model.Id}");
                }
                
                // Űrlap törlése
                NameTextBox.Text = string.Empty;
                ManufacturerComboBox.SelectedIndex = 0;
                PopulateChipsets();
                PriceTextBox.Text = string.Empty;
                PowerConsumptionTextBox.Text = string.Empty;
                FormFactorComboBox.SelectedIndex = 0;
                
                // Hiba feliratok elrejtése
                NameErrorBlock.Visibility = Visibility.Collapsed;
                ManufacturerErrorBlock.Visibility = Visibility.Collapsed;
                ChipsetErrorBlock.Visibility = Visibility.Collapsed;
                FormFactorErrorBlock.Visibility = Visibility.Collapsed;
                SocketErrorBlock.Visibility = Visibility.Collapsed;
                PriceErrorBlock.Visibility = Visibility.Collapsed;
                PowerConsumptionErrorBlock.Visibility = Visibility.Collapsed;
                
                // Sikeres mentés esemény kiváltása
                currentId = null; // Reset current ID
                SaveCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba alaplap mentése közben: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Belső hiba: {ex.InnerException.Message}");
                }
                MessageBox.Show($"Hiba mentés közben: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ManufacturerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateChipsets();
        }

        private void ChipsetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateSockets();
        }
        
        private void SocketComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedSocketItem = SocketComboBox.SelectedItem as ComboBoxItem;
            if (selectedSocketItem != null && selectedSocketItem.Tag != null)
            {
                // Elmentsük a kiválasztott socket ID-ját
                selectedSocketId = Convert.ToInt32(selectedSocketItem.Tag);
                System.Diagnostics.Debug.WriteLine($"Socket kiválasztva: {selectedSocketItem.Content}, ID: {selectedSocketId}");
            }
            
            ValidateSocket();
        }        private void PopulateChipsets()
        {
            ChipsetComboBox.Items.Clear();
            SocketComboBox.Items.Clear();
            
            var selectedManufacturer = (ManufacturerComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            
            // Ha nincs gyártó kiválasztva vagy "Válasszon..." van kiválasztva, akkor nem töltünk be semmit
            if (string.IsNullOrEmpty(selectedManufacturer) || selectedManufacturer == "Válasszon...")
            {
                System.Diagnostics.Debug.WriteLine("Nincs gyártó kiválasztva vagy 'Válasszon...' van kiválasztva.");
                return;
            }
            
            System.Diagnostics.Debug.WriteLine($"Kiválasztott gyártó: {selectedManufacturer}");
            
            // A megfelelő gyártó chipsetjeit töltjük be
            List<ChipsetType> selectedChipsets;
            if (selectedManufacturer == "Intel")
            {
                selectedChipsets = intelChipsets;
            }
            else
            {
                selectedChipsets = amdChipsets;
            }
            
            if (selectedChipsets != null && selectedChipsets.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine($"{selectedManufacturer} chipset-ek betöltése: {selectedChipsets.Count} db");
                
                // Tooltipek előkészítése a chipsetekhez
                foreach (var c in selectedChipsets)
                {
                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = c.ChipsetName,
                        Tag = c.Id, // Az ID tárolása a Tag tulajdonságban
                        Foreground = Brushes.White,
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"))
                    };
                    
                    // Tooltip hozzáadása, ha van leírás vagy generáció
                    string tooltipText = "";
                    if (!string.IsNullOrEmpty(c.Generation))
                        tooltipText += c.Generation + "\n";
                    if (!string.IsNullOrEmpty(c.Description))
                        tooltipText += c.Description;
                    
                    if (!string.IsNullOrEmpty(tooltipText))
                        item.ToolTip = tooltipText;
                    
                    ChipsetComboBox.Items.Add(item);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Nem találhatók chipset-ek a {selectedManufacturer} gyártóhoz!");
                
                // Hozzáadunk egy alapértelmezett elemet
                ComboBoxItem defaultItem = new ComboBoxItem
                {
                    Content = "Nincs elérhető chipset",
                    Foreground = Brushes.Gray,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E")),
                    IsEnabled = false
                };
                ChipsetComboBox.Items.Add(defaultItem);
            }
            
            if (ChipsetComboBox.Items.Count > 0)
                ChipsetComboBox.SelectedIndex = 0;
            
            // Socket lista frissítése
            PopulateSockets();
        }

        private void PopulateSockets()
        {
            SocketComboBox.Items.Clear();
            var selectedManufacturer = (ManufacturerComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            var selectedChipsetItem = ChipsetComboBox.SelectedItem as ComboBoxItem;
            
            // Ha nincs gyártó kiválasztva vagy "Válasszon..." van kiválasztva, akkor nem töltünk be semmit
            if (string.IsNullOrEmpty(selectedManufacturer) || selectedManufacturer == "Válasszon...")
            {
                System.Diagnostics.Debug.WriteLine("PopulateSockets: Nincs gyártó kiválasztva vagy 'Válasszon...' van kiválasztva.");
                return;
            }
            
            if (selectedChipsetItem == null || !selectedChipsetItem.IsEnabled)
            {
                System.Diagnostics.Debug.WriteLine("PopulateSockets: Nincs chipset kiválasztva vagy a kiválasztott chipset nem engedélyezett");
                return;
            }
            
            string selectedChipsetName = selectedChipsetItem.Content.ToString();
            System.Diagnostics.Debug.WriteLine($"PopulateSockets: Kiválasztott chipset: {selectedChipsetName}");
            
            // Chipset ID kiolvasása és mentése
            if (selectedChipsetItem.Tag != null)
            {
                selectedChipsetId = Convert.ToInt32(selectedChipsetItem.Tag);
                System.Diagnostics.Debug.WriteLine($"PopulateSockets: Chipset ID: {selectedChipsetId}");
            }
            
            // Szűrjük a kompatibilis socket típusokat a kiválasztott gyártó alapján
            var compatibleSockets = socketTypes?.Where(s => s.Manufacturer == selectedManufacturer).ToList();
                
            if (compatibleSockets != null && compatibleSockets.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine($"PopulateSockets: Kompatibilis socketek száma: {compatibleSockets.Count}");
                
                // Alapértelmezett socket meghatározása chipset alapján
                string defaultSocketName = null;
                
                // Chipset neve alapján meghatározzuk az alapértelmezett socketet
                if (selectedManufacturer == "AMD")
                {
                    if (selectedChipsetName.Contains("550") || selectedChipsetName.Contains("450") || 
                        selectedChipsetName.Contains("570") || selectedChipsetName.Contains("470") || 
                        selectedChipsetName.Contains("350") || selectedChipsetName.Contains("370"))
                    {
                        defaultSocketName = "AM4";
                    }
                    else if (selectedChipsetName.Contains("650") || selectedChipsetName.Contains("670"))
                    {
                        defaultSocketName = "AM5";
                    }
                }
                else if (selectedManufacturer == "Intel")
                {
                    if (selectedChipsetName.Contains("390") || selectedChipsetName.Contains("365") || 
                        selectedChipsetName.Contains("310"))
                    {
                        defaultSocketName = "LGA1151";
                    }
                    else if (selectedChipsetName.Contains("460") || selectedChipsetName.Contains("490") || 
                             selectedChipsetName.Contains("560") || selectedChipsetName.Contains("590"))
                    {
                        defaultSocketName = "LGA1200";
                    }
                    else if (selectedChipsetName.Contains("610") || selectedChipsetName.Contains("660") || 
                             selectedChipsetName.Contains("670") || selectedChipsetName.Contains("690") || 
                             selectedChipsetName.Contains("760") || selectedChipsetName.Contains("790"))
                    {
                        defaultSocketName = "LGA1700";
                    }
                }
                
                // Végigmegyünk a kompatibilis socketeken és hozzáadjuk őket a ComboBox-hoz
                int defaultIndex = -1;
                for (int i = 0; i < compatibleSockets.Count; i++)
                {
                    var socket = compatibleSockets[i];
                    System.Diagnostics.Debug.WriteLine($"PopulateSockets: Socket hozzáadása: {socket.SocketName} (ID: {socket.Id})");
                    
                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = socket.SocketName,
                        Tag = socket.Id, // Az ID tárolása a Tag tulajdonságban
                        Foreground = Brushes.White,
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"))
                    };
                    
                    // Tooltip hozzáadása, ha van leírás vagy generáció
                    string tooltipText = "";
                    if (!string.IsNullOrEmpty(socket.Manufacturer))
                        tooltipText += socket.Manufacturer + " ";
                    if (!string.IsNullOrEmpty(socket.Generation))
                        tooltipText += socket.Generation + "\n";
                    if (!string.IsNullOrEmpty(socket.Description))
                        tooltipText += socket.Description;
                    
                    if (!string.IsNullOrEmpty(tooltipText))
                        item.ToolTip = tooltipText;
                    
                    SocketComboBox.Items.Add(item);
                    
                    // Ha ez az alapértelmezett socket, jelöljük meg az indexét
                    if (socket.SocketName == defaultSocketName)
                    {
                        defaultIndex = i;
                    }
                }
                
                // Az alapértelmezett socket kiválasztása
                if (defaultIndex >= 0)
                {
                    SocketComboBox.SelectedIndex = defaultIndex;
                }
                else if (SocketComboBox.Items.Count > 0)
                {
                    SocketComboBox.SelectedIndex = 0;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("PopulateSockets: Nem található kompatibilis socket a kiválasztott gyártóhoz.");
                
                // Hozzáadunk egy alapértelmezett elemet
                ComboBoxItem defaultItem = new ComboBoxItem
                {
                    Content = "Nincs elérhető socket",
                    Foreground = Brushes.Gray,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E")),
                    IsEnabled = false
                };
                SocketComboBox.Items.Add(defaultItem);
                SocketComboBox.SelectedIndex = 0;
            }
            
            // Ha nincs kiválasztott elem, de van elem a listában
            if (SocketComboBox.SelectedIndex < 0 && SocketComboBox.Items.Count > 0)
            {
                SocketComboBox.SelectedIndex = 0;
            }
        }
        
        // Segédfüggvény a socket elem hozzáadásához
        private void AddSocketItem(string socketName)
        {
            ComboBoxItem item = new ComboBoxItem
            {
                Content = socketName,
                Tag = 0, // Nincs ID a fallback esetén
                Foreground = Brushes.White,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"))
            };
            SocketComboBox.Items.Add(item);
        }

        // Segédfüggvény a ComboBox elem kiválasztásához a szöveg alapján
        private void SelectComboBoxItemByContent(ComboBox comboBox, string content)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Content.ToString() == content)
                {
                    comboBox.SelectedItem = item;
                    return;
                }
            }
            
            // Ha nem találtuk meg, akkor az első elemet választjuk ki
            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }

        private bool ValidateName()
        {
            return ValidationHelper.ValidateTextBox(
                NameTextBox,
                value => ValidationHelper.Required(value, "Név"),
                NameErrorBlock
            );
        }

        private bool ValidateManufacturer()
        {
            return ValidationHelper.ValidateComboBox(
                ManufacturerComboBox,
                selectedItem => ValidationHelper.ComboBoxItemSelected(selectedItem, "Gyártó"),
                ManufacturerErrorBlock
            );
        }

        private bool ValidateChipset()
        {
            return ValidationHelper.ValidateComboBox(
                ChipsetComboBox,
                selectedItem => ValidationHelper.ComboBoxItemSelected(selectedItem, "Chipkészlet"),
                ChipsetErrorBlock
            );
        }

        private bool ValidateFormFactor()
        {
            return ValidationHelper.ValidateComboBox(
                FormFactorComboBox,
                selectedItem => ValidationHelper.ComboBoxItemSelected(selectedItem, "Form Factor"),
                FormFactorErrorBlock
            );
        }

        private bool ValidateSocket()
        {
            return ValidationHelper.ValidateComboBox(
                SocketComboBox,
                selectedItem => ValidationHelper.ComboBoxItemSelected(selectedItem, "Foglalat"),
                SocketErrorBlock
            );
        }

        private bool ValidatePrice()
        {
            if (PriceTextBox == null || string.IsNullOrWhiteSpace(PriceTextBox.Text))
                return true;

            return ValidationHelper.ValidateTextBox(
                PriceTextBox,
                value => ValidationHelper.ValidateDecimalField(value, "Ár", true),
                PriceErrorBlock
            );
        }

        private bool ValidatePowerConsumption()
        {
            if (PowerConsumptionTextBox == null || string.IsNullOrWhiteSpace(PowerConsumptionTextBox.Text))
                return true;

            return ValidationHelper.ValidateTextBox(
                PowerConsumptionTextBox,
                value => ValidationHelper.ValidateIntegerField(value, "Fogyasztás", true),
                PowerConsumptionErrorBlock
            );
        }
        
        /// <summary>
        /// Betölti a megadott ID-val rendelkező alaplap adatait szerkesztéshez
        /// </summary>
        public void LoadForEdit(PC_Configurator.Models.Motherboard motherboard)
        {
            if (motherboard == null)
                return;
                
            // Az aktuális ID tárolása a későbbi mentéshez
            currentId = motherboard.Id;
            
            // ChipsetTypeId és SocketTypeId elmentése
            selectedChipsetId = motherboard.ChipsetTypeId;
            selectedSocketId = motherboard.SocketTypeId;
            
            System.Diagnostics.Debug.WriteLine($"Alaplap szerkesztése: ID={motherboard.Id}, " +
                                              $"ChipsetTypeId={motherboard.ChipsetTypeId} ({motherboard.Chipset}), " +
                                              $"SocketTypeId={motherboard.SocketTypeId} ({motherboard.Socket})");
            
            // Form címének módosítása
            FormTitle.Text = "Alaplap szerkesztése";
            
            // Adatok betöltése
            NameTextBox.Text = motherboard.Name;
            PriceTextBox.Text = motherboard.Price.ToString();
            PowerConsumptionTextBox.Text = motherboard.PowerConsumption.ToString();
            
            // Gyártó kiválasztása
            for (int i = 0; i < ManufacturerComboBox.Items.Count; i++)
            {
                var item = ManufacturerComboBox.Items[i] as ComboBoxItem;
                if (item != null && item.Content.ToString() == motherboard.Manufacturer)
                {
                    ManufacturerComboBox.SelectedIndex = i;
                    break;
                }
            }
            
            // Ha a gyártó nem található, próbáljunk meg meghatározni a socket vagy chipset alapján
            if (ManufacturerComboBox.SelectedIndex <= 0)
            {
                string socketName = motherboard.Socket ?? "";
                string chipsetName = motherboard.Chipset ?? "";
                
                if (socketName.StartsWith("AM") || chipsetName.StartsWith("B") || chipsetName.StartsWith("X"))
                {
                    // Valószínűleg AMD
                    for (int i = 0; i < ManufacturerComboBox.Items.Count; i++)
                    {
                        var item = ManufacturerComboBox.Items[i] as ComboBoxItem;
                        if (item != null && item.Content.ToString() == "AMD")
                        {
                            ManufacturerComboBox.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else if (socketName.StartsWith("LGA") || chipsetName.Contains("Z") || chipsetName.Contains("H"))
                {
                    // Valószínűleg Intel
                    for (int i = 0; i < ManufacturerComboBox.Items.Count; i++)
                    {
                        var item = ManufacturerComboBox.Items[i] as ComboBoxItem;
                        if (item != null && item.Content.ToString() == "Intel")
                        {
                            ManufacturerComboBox.SelectedIndex = i;
                            break;
                        }
                    }
                }
                
                // Ha még mindig nincs kiválasztva gyártó
                if (ManufacturerComboBox.SelectedIndex <= 0)
                {
                    ManufacturerComboBox.SelectedIndex = 1; // Feltételezzük, hogy az AMD az 1-es indexen van
                }
            }
            
            // Chipkészlet és foglalat betöltése - A PopulateChipsets automatikusan meghívja a PopulateSockets-et is
            PopulateChipsets();
            
            // Chipset kiválasztása ID alapján vagy név alapján
            bool chipsetFound = false;
            
            // Először ID alapján keresünk, ha érvényes ID-t kaptunk
            if (motherboard.ChipsetTypeId > 0)
            {
                for (int i = 0; i < ChipsetComboBox.Items.Count; i++)
                {
                    var item = ChipsetComboBox.Items[i] as ComboBoxItem;
                    if (item != null && item.Tag != null && Convert.ToInt32(item.Tag) == motherboard.ChipsetTypeId)
                    {
                        ChipsetComboBox.SelectedIndex = i;
                        chipsetFound = true;
                        System.Diagnostics.Debug.WriteLine($"Chipset kiválasztva ID alapján: {item.Content} (ID: {item.Tag})");
                        break;
                    }
                }
            }
            
            // Ha ID alapján nem találtuk meg, akkor név szerint keresünk
            if (!chipsetFound && !string.IsNullOrEmpty(motherboard.Chipset))
            {
                for (int i = 0; i < ChipsetComboBox.Items.Count; i++)
                {
                    var item = ChipsetComboBox.Items[i] as ComboBoxItem;
                    if (item != null && item.Content.ToString() == motherboard.Chipset)
                    {
                        ChipsetComboBox.SelectedIndex = i;
                        chipsetFound = true;
                        System.Diagnostics.Debug.WriteLine($"Chipset kiválasztva név alapján: {item.Content}");
                        break;
                    }
                }
                
                // Ha pontos egyezést nem találtunk, keresünk hasonlót
                if (!chipsetFound)
                {
                    for (int i = 0; i < ChipsetComboBox.Items.Count; i++)
                    {
                        var item = ChipsetComboBox.Items[i] as ComboBoxItem;
                        if (item != null && item.Content.ToString().Contains(motherboard.Chipset))
                        {
                            ChipsetComboBox.SelectedIndex = i;
                            chipsetFound = true;
                            System.Diagnostics.Debug.WriteLine($"Chipset kiválasztva részleges egyezés alapján: {item.Content}");
                            break;
                        }
                    }
                }
            }
            
            // Ha még mindig nem találtuk meg a chipsetet, de van elem a ComboBox-ban
            if (!chipsetFound && ChipsetComboBox.Items.Count > 0)
            {
                ChipsetComboBox.SelectedIndex = 0;
                System.Diagnostics.Debug.WriteLine("Nem sikerült megfelelő chipsetet találni, alapértelmezett kiválasztva.");
            }
            
            // A socket lista frissítése
            PopulateSockets();
            
            // Socket kiválasztása ID alapján vagy név alapján
            bool socketFound = false;
            
            // Először ID alapján keresünk, ha érvényes ID-t kaptunk
            if (motherboard.SocketTypeId > 0)
            {
                for (int i = 0; i < SocketComboBox.Items.Count; i++)
                {
                    var item = SocketComboBox.Items[i] as ComboBoxItem;
                    if (item != null && item.Tag != null && Convert.ToInt32(item.Tag) == motherboard.SocketTypeId)
                    {
                        SocketComboBox.SelectedIndex = i;
                        socketFound = true;
                        System.Diagnostics.Debug.WriteLine($"Socket kiválasztva ID alapján: {item.Content} (ID: {item.Tag})");
                        break;
                    }
                }
            }
            
            // Ha ID alapján nem találtuk meg, akkor név szerint keresünk
            if (!socketFound && !string.IsNullOrEmpty(motherboard.Socket))
            {
                for (int i = 0; i < SocketComboBox.Items.Count; i++)
                {
                    var item = SocketComboBox.Items[i] as ComboBoxItem;
                    if (item != null && item.Content.ToString() == motherboard.Socket)
                    {
                        SocketComboBox.SelectedIndex = i;
                        socketFound = true;
                        System.Diagnostics.Debug.WriteLine($"Socket kiválasztva név alapján: {item.Content}");
                        break;
                    }
                }
                
                // Ha pontos egyezést nem találtunk, keresünk hasonlót
                if (!socketFound)
                {
                    for (int i = 0; i < SocketComboBox.Items.Count; i++)
                    {
                        var item = SocketComboBox.Items[i] as ComboBoxItem;
                        if (item != null && motherboard.Socket.Contains(item.Content.ToString()))
                        {
                            SocketComboBox.SelectedIndex = i;
                            socketFound = true;
                            System.Diagnostics.Debug.WriteLine($"Socket kiválasztva részleges egyezés alapján: {item.Content}");
                            break;
                        }
                    }
                }
            }
            
            // Ha még mindig nem találtuk meg a socketet, de van elem a ComboBox-ban
            if (!socketFound && SocketComboBox.Items.Count > 0)
            {
                SocketComboBox.SelectedIndex = 0;
                System.Diagnostics.Debug.WriteLine("Nem sikerült megfelelő socketet találni, alapértelmezett kiválasztva.");
            }
            
            // Formátum beállítása
            if (!string.IsNullOrEmpty(motherboard.FormFactor))
            {
                for (int i = 0; i < FormFactorComboBox.Items.Count; i++)
                {
                    var item = FormFactorComboBox.Items[i] as ComboBoxItem;
                    if (item != null && item.Content.ToString() == motherboard.FormFactor)
                    {
                        FormFactorComboBox.SelectedIndex = i;
                        break;
                    }
                }
            }
            else
            {
                // Alapértelmezett formátum
                for (int i = 0; i < FormFactorComboBox.Items.Count; i++)
                {
                    var item = FormFactorComboBox.Items[i] as ComboBoxItem;
                    if (item != null && item.Content.ToString() == "ATX")
                    {
                        FormFactorComboBox.SelectedIndex = i;
                        break;
                    }
                }
            }
        }
    }
}
