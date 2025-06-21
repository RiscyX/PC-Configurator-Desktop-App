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
    /// Interaction logic for RAM.xaml
    /// </summary>
    public partial class RAM : UserControl
    {
        public RAM()
        {
            InitializeComponent();
            SaveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text?.Trim();
            string capacityStr = (CapacityGBComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string speedStr = SpeedMHzTextBox.Text?.Trim();
            string type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(capacityStr) || string.IsNullOrEmpty(speedStr) || string.IsNullOrEmpty(type))
            {
                MessageBox.Show("Minden mező kitöltése kötelező!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(capacityStr, out int capacityGB) || !int.TryParse(speedStr, out int speedMHz))
            {
                MessageBox.Show("Érvénytelen számformátum!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var model = new PC_Configurator.Models.RAM { Name = name, CapacityGB = capacityGB, SpeedMHz = speedMHz, Type = type };
            try
            {
                model.SaveToDatabase();
                MessageBox.Show("RAM sikeresen mentve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                NameTextBox.Text = string.Empty;
                CapacityGBComboBox.SelectedIndex = 0;
                SpeedMHzTextBox.Text = string.Empty;
                TypeComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba mentés közben: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
