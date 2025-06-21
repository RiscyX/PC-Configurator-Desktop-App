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
    /// Interaction logic for Storage.xaml
    /// </summary>
    public partial class Storage : UserControl
    {
        public Storage()
        {
            InitializeComponent();
            // Ensure event is not fired before ComboBox is ready
            Loaded += Storage_Loaded;
            SaveButton.Click += SaveButton_Click;
        }        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text?.Trim();
            string type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string capacityStr = (CapacityGBComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(capacityStr))
            {
                MessageBox.Show("Minden mező kitöltése kötelező!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(capacityStr, out int capacityGB))
            {
                MessageBox.Show("Érvénytelen kapacitás érték!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var model = new PC_Configurator.Models.Storage { Name = name, Type = type, CapacityGB = capacityGB };
            try
            {
                model.SaveToDatabase();
                MessageBox.Show("Meghajtó sikeresen mentve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                NameTextBox.Text = string.Empty;
                TypeComboBox.SelectedIndex = 0;
                UpdateCapacityOptions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba mentés közben: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Storage_Loaded(object sender, RoutedEventArgs e)
        {
            if (TypeComboBox != null)
            {
                TypeComboBox.SelectedIndex = 0;
                UpdateCapacityOptions();
            }
        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeComboBox == null || CapacityGBComboBox == null)
                return;
            UpdateCapacityOptions();
        }        private void UpdateCapacityOptions()
        {
            if (CapacityGBComboBox == null || TypeComboBox == null)
                return;
            CapacityGBComboBox.Items.Clear();
            var selectedType = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            
            // Az értékek hozzáadása ComboBoxItem-ként, hogy be lehessen állítani a színeket
            List<string> capacities = new List<string>();
            if (selectedType == "SSD")
            {
                capacities.AddRange(new[] { "120", "240", "480", "500", "512", "1000", "1024", "2048", "4096" });
            }
            else
            {
                capacities.AddRange(new[] { "1024", "2048", "4096", "8192" });
            }
            
            foreach (var capacity in capacities)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = capacity,
                    Foreground = Brushes.White,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"))
                };
                CapacityGBComboBox.Items.Add(item);
            }
            
            if (CapacityGBComboBox.Items.Count > 0)
                CapacityGBComboBox.SelectedIndex = 0;
        }
    }
}
