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
    /// Interaction logic for GPU.xaml
    /// </summary>
    public partial class GPU : UserControl
    {
        public GPU()
        {
            InitializeComponent();
            SaveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text?.Trim();
            string manufacturer = (ManufacturerComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string memoryStr = (MemoryGBComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(manufacturer) || string.IsNullOrEmpty(memoryStr))
            {
                MessageBox.Show("Minden mező kitöltése kötelező!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(memoryStr, out int memoryGB))
            {
                MessageBox.Show("Érvénytelen memória érték!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var model = new PC_Configurator.Models.GPU { Name = name, Manufacturer = manufacturer, MemoryGB = memoryGB };
            try
            {
                model.SaveToDatabase();
                MessageBox.Show("Videokártya sikeresen mentve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                NameTextBox.Text = string.Empty;
                ManufacturerComboBox.SelectedIndex = 0;
                MemoryGBComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba mentés közben: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
