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
    /// Interaction logic for CPU.xaml
    /// </summary>
    public partial class CPU : UserControl
    {
        public CPU()
        {
            InitializeComponent();
            SaveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text?.Trim();
            string manufacturer = (ManufacturerComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string coresStr = (CoresComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string threadsStr = (ThreadsComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string baseClockStr = BaseClockTextBox.Text?.Trim();
            string boostClockStr = BoostClockTextBox.Text?.Trim();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(manufacturer) || string.IsNullOrEmpty(coresStr) || string.IsNullOrEmpty(threadsStr) || string.IsNullOrEmpty(baseClockStr) || string.IsNullOrEmpty(boostClockStr))
            {
                MessageBox.Show("Minden mező kitöltése kötelező!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(coresStr, out int cores) || !int.TryParse(threadsStr, out int threads) || !float.TryParse(baseClockStr.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float baseClock) || !float.TryParse(boostClockStr.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float boostClock))
            {
                MessageBox.Show("Érvénytelen számformátum!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var model = new PC_Configurator.Models.CPU { Name = name, Manufacturer = manufacturer, Cores = cores, Threads = threads, BaseClockGHz = baseClock, BoostClockGHz = boostClock };
            try
            {
                model.SaveToDatabase();
                MessageBox.Show("CPU sikeresen mentve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                NameTextBox.Text = string.Empty;
                ManufacturerComboBox.SelectedIndex = 0;
                CoresComboBox.SelectedIndex = 0;
                ThreadsComboBox.SelectedIndex = 0;
                BaseClockTextBox.Text = string.Empty;
                BoostClockTextBox.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba mentés közben: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
