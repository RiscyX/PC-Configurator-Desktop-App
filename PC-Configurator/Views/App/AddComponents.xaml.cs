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
using System.Windows.Shapes;

namespace PC_Configurator.Views.App
{
    /// <summary>
    /// Interaction logic for AddComponents.xaml
    /// </summary>
    public partial class AddComponents : UserControl
    {
        public AddComponents()
        {
            InitializeComponent();
        }

        private void AddCpuButton_Click(object sender, RoutedEventArgs e)
        {
            FormHost.Content = new PC_Configurator.Views.Forms.CPU();
        }

        private void AddGpuButton_Click(object sender, RoutedEventArgs e)
        {
            FormHost.Content = new PC_Configurator.Views.Forms.GPU();
        }

        private void AddMotherboardButton_Click(object sender, RoutedEventArgs e)
        {
            FormHost.Content = new PC_Configurator.Views.Forms.Motherboard();
        }

        private void AddRamButton_Click(object sender, RoutedEventArgs e)
        {
            FormHost.Content = new PC_Configurator.Views.Forms.RAM();
        }

        private void AddStorageButton_Click(object sender, RoutedEventArgs e)
        {
            FormHost.Content = new PC_Configurator.Views.Forms.Storage();
        }

        private void AddPsuButton_Click(object sender, RoutedEventArgs e)
        {
            FormHost.Content = new PC_Configurator.Views.Forms.PSU();
        }

        private void AddCaseButton_Click(object sender, RoutedEventArgs e)
        {
            FormHost.Content = new PC_Configurator.Views.Forms.Case();
        }
    }
}
