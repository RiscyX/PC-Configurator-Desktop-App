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
using PC_Configurator.Views;
using PC_Configurator.Views.User;

namespace PC_Configurator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Lehetővé teszi az ablak mozgatását
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow regWin = new RegistrationWindow();
            regWin.Owner = this;
            this.Hide();
            regWin.ShowDialog();  // blokkolja a MainWindow futását, amíg regWin le nem zárul
            this.Close();         // csak ezután zárjuk le végleg
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWin = new LoginWindow();
            loginWin.Owner = this;
            this.Hide();
            loginWin.ShowDialog();
            this.Close();
        }
    }
}

