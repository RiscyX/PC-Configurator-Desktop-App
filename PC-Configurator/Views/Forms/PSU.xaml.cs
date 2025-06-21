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
    /// Interaction logic for PSU.xaml
    /// </summary>
    public partial class PSU : UserControl
    {
        public PSU()
        {
            InitializeComponent();
            SaveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text?.Trim();
            string wattageStr = (WattageComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string efficiency = (EfficiencyRatingComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(wattageStr) || string.IsNullOrEmpty(efficiency))
            {
                MessageBox.Show("Minden mező kitöltése kötelező!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(wattageStr, out int wattage))
            {
                MessageBox.Show("Érvénytelen teljesítmény érték!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var model = new PC_Configurator.Models.PSU { Name = name, Wattage = wattage, EfficiencyRating = efficiency };
            try
            {
                model.SaveToDatabase();
                MessageBox.Show("Tápegység sikeresen mentve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                NameTextBox.Text = string.Empty;
                WattageComboBox.SelectedIndex = 0;
                EfficiencyRatingComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba mentés közben: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
