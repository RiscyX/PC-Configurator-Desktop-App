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
    /// Interaction logic for Case.xaml
    /// </summary>
    public partial class Case : UserControl
    {
        public Case()
        {
            InitializeComponent();
            SaveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text?.Trim();
            string formFactor = (FormFactorComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(formFactor))
            {
                MessageBox.Show("Minden mező kitöltése kötelező!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var model = new PC_Configurator.Models.Case { Name = name, FormFactor = formFactor };
            try
            {
                model.SaveToDatabase();
                MessageBox.Show("Gépház sikeresen mentve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                NameTextBox.Text = string.Empty;
                FormFactorComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba mentés közben: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
