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

namespace PC_Configurator.Views.Forms
{
    /// <summary>
    /// Interaction logic for Motherboard.xaml
    /// </summary>
    public partial class Motherboard : UserControl
    {
        public class ChipsetInfo
        {
            public string Chipset { get; set; }
            public string Socket { get; set; }
            public string Category { get; set; }
            public string Notes { get; set; }
        }

        private readonly List<ChipsetInfo> amdChipsets = new List<ChipsetInfo>
        {
            new ChipsetInfo { Chipset = "A320", Socket = "AM4", Category = "Entry", Notes = "No OC, keves USB/PCIe" },
            new ChipsetInfo { Chipset = "B350", Socket = "AM4", Category = "Mid", Notes = "OC támogatott" },
            new ChipsetInfo { Chipset = "X370", Socket = "AM4", Category = "High", Notes = "Teljes OC, több PCIe sáv" },
            new ChipsetInfo { Chipset = "B450", Socket = "AM4", Category = "Mid", Notes = "Frissített BIOS, jó ár/érték" },
            new ChipsetInfo { Chipset = "X470", Socket = "AM4", Category = "High", Notes = "Ryzen 2000/3000 támogatás" },
            new ChipsetInfo { Chipset = "B550", Socket = "AM4", Category = "Mid", Notes = "PCIe 4.0 CPU-ról" },
            new ChipsetInfo { Chipset = "X570", Socket = "AM4", Category = "High", Notes = "PCIe 4.0 CPU és chipset szinten" },
            new ChipsetInfo { Chipset = "A620", Socket = "AM5", Category = "Entry", Notes = "OC nem támogatott" },
            new ChipsetInfo { Chipset = "B650", Socket = "AM5", Category = "Mid", Notes = "PCIe 5.0 CPU-tól, jó ár/érték" },
            new ChipsetInfo { Chipset = "B650E", Socket = "AM5", Category = "Mid", Notes = "PCIe 5.0 GPU + SSD támogatás" },
            new ChipsetInfo { Chipset = "X670", Socket = "AM5", Category = "High", Notes = "PCIe 5.0 SSD/USB" },
            new ChipsetInfo { Chipset = "X670E", Socket = "AM5", Category = "Enthusiast", Notes = "Teljes PCIe 5.0 támogatás" }
        };

        private readonly List<ChipsetInfo> intelChipsets = new List<ChipsetInfo>
        {
            new ChipsetInfo { Chipset = "H410", Socket = "LGA1200", Category = "Entry", Notes = "Kevés funkció" },
            new ChipsetInfo { Chipset = "B460", Socket = "LGA1200", Category = "Mid", Notes = "Nincs OC" },
            new ChipsetInfo { Chipset = "H470", Socket = "LGA1200", Category = "Mid", Notes = "Extra USB, SATA" },
            new ChipsetInfo { Chipset = "Z490", Socket = "LGA1200", Category = "High", Notes = "OC támogatott" },
            new ChipsetInfo { Chipset = "B560", Socket = "LGA1200", Category = "Mid", Notes = "11th gen RAM OC" },
            new ChipsetInfo { Chipset = "Z590", Socket = "LGA1200", Category = "High", Notes = "PCIe 4.0 (CPU-tól függően)" },
            new ChipsetInfo { Chipset = "H610", Socket = "LGA1700", Category = "Entry", Notes = "DDR4 vagy DDR5" },
            new ChipsetInfo { Chipset = "B660", Socket = "LGA1700", Category = "Mid", Notes = "RAM OC, PCIe 4.0" },
            new ChipsetInfo { Chipset = "H670", Socket = "LGA1700", Category = "Mid", Notes = "Több I/O" },
            new ChipsetInfo { Chipset = "Z690", Socket = "LGA1700", Category = "High", Notes = "Teljes OC támogatás" },
            new ChipsetInfo { Chipset = "B760", Socket = "LGA1700", Category = "Mid", Notes = "13/14th gen-hez ajánlott" },
            new ChipsetInfo { Chipset = "Z790", Socket = "LGA1700", Category = "High", Notes = "PCIe 5.0, DDR5 támogatás" }
        };

        public Motherboard()
        {
            InitializeComponent();
            ManufacturerComboBox.SelectedIndex = 0;
            PopulateChipsets();
            SaveButton.Click += SaveButton_Click;
        }        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text?.Trim();
            string manufacturer = (ManufacturerComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string chipset = (ChipsetComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string socket = (SocketComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(manufacturer) || string.IsNullOrEmpty(chipset) || string.IsNullOrEmpty(socket))
            {
                MessageBox.Show("Minden mező kitöltése kötelező!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var model = new PC_Configurator.Models.Motherboard { Name = name, Manufacturer = manufacturer, Chipset = chipset, Socket = socket };
            try
            {
                model.SaveToDatabase();
                MessageBox.Show("Alaplap sikeresen mentve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                NameTextBox.Text = string.Empty;
                ManufacturerComboBox.SelectedIndex = 0;
                PopulateChipsets();
            }
            catch (Exception ex)
            {
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
        }        private void PopulateChipsets()
        {
            ChipsetComboBox.Items.Clear();
            SocketComboBox.Items.Clear();
            var selectedManufacturer = (ManufacturerComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            List<ChipsetInfo> chipsets = selectedManufacturer == "Intel" ? intelChipsets : amdChipsets;
            
            foreach (var c in chipsets)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = c.Chipset,
                    Foreground = Brushes.White,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"))
                };
                ChipsetComboBox.Items.Add(item);
            }
            
            if (ChipsetComboBox.Items.Count > 0)
                ChipsetComboBox.SelectedIndex = 0;
        }

        private void PopulateSockets()
        {
            SocketComboBox.Items.Clear();
            var selectedManufacturer = (ManufacturerComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            var selectedChipset = (ChipsetComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            List<ChipsetInfo> chipsets = selectedManufacturer == "Intel" ? intelChipsets : amdChipsets;
            var sockets = chipsets.Where(c => c.Chipset == selectedChipset).Select(c => c.Socket).Distinct().ToList();
            
            foreach (var s in sockets)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = s,
                    Foreground = Brushes.White,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"))
                };
                SocketComboBox.Items.Add(item);
            }
            
            if (SocketComboBox.Items.Count > 0)
                SocketComboBox.SelectedIndex = 0;
        }
    }
}
