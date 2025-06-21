using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace PC_Configurator.Views.App
{
    /// <summary>
    /// Interaction logic for Configs.xaml
    /// </summary>
    public partial class Configs : UserControl
    {
        // Az adatformázáshoz használt osztály - Demo adatokhoz
        public class ConfigurationItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string SaveDate { get; set; }
            public string CPU { get; set; }
            public string GPU { get; set; }
            public string RAM { get; set; }
            public string Motherboard { get; set; }
            public string Storage { get; set; }
            public string Case { get; set; }
            public int Price { get; set; }
            public int PerformanceScore { get; set; }
        }

        private ObservableCollection<ConfigurationItem> _allConfigurations;
        private ObservableCollection<ConfigurationItem> _filteredConfigurations;

        public Configs()
        {
            InitializeComponent();
            
            // Demo adatok betöltése
            LoadDemoData();
            
            // Konfiguráció darabszám megjelenítése
            UpdateConfigCounter();
            
            // Annak ellenőrzése, hogy van-e konfiguráció
            CheckIfEmpty();
        }

        private void LoadDemoData()
        {
            // Demo adatok létrehozása
            _allConfigurations = new ObservableCollection<ConfigurationItem>
            {
                new ConfigurationItem
                {
                    Id = 1,
                    Name = "Játékra optimalizált konfiguráció",
                    SaveDate = "2025.06.20. 15:30",
                    CPU = "AMD Ryzen 7 5800X",
                    GPU = "NVIDIA RTX 3070",
                    RAM = "32GB DDR4 3600MHz",
                    Motherboard = "MSI MAG B550",
                    Storage = "1TB NVMe SSD",
                    Case = "Lian Li PC-O11",
                    Price = 435000,
                    PerformanceScore = 85
                },
                new ConfigurationItem
                {
                    Id = 2,
                    Name = "Irodai munkaállomás",
                    SaveDate = "2025.06.18. 10:15",
                    CPU = "Intel Core i5-12600K",
                    GPU = "Integrated",
                    RAM = "16GB DDR4 3200MHz",
                    Motherboard = "Gigabyte Z690",
                    Storage = "500GB SSD",
                    Case = "Fractal Design Meshify",
                    Price = 225000,
                    PerformanceScore = 45
                },
                new ConfigurationItem
                {
                    Id = 3,
                    Name = "Kreatív munkaállomás",
                    SaveDate = "2025.06.15. 09:45",
                    CPU = "AMD Ryzen 9 5900X",
                    GPU = "NVIDIA RTX 3090",
                    RAM = "64GB DDR4 3600MHz",
                    Motherboard = "ASUS ROG X570",
                    Storage = "2TB NVMe + 4TB HDD",
                    Case = "be quiet! Dark Base 700",
                    Price = 750000,
                    PerformanceScore = 95
                }
            };

            _filteredConfigurations = new ObservableCollection<ConfigurationItem>(_allConfigurations);
        }

        private void UpdateConfigCounter()
        {
            int count = _filteredConfigurations.Count;
            ConfigCountBlock.Text = $"{count} konfiguráció";
        }

        private void CheckIfEmpty()
        {
            if (_filteredConfigurations.Count == 0)
            {
                EmptyState.Visibility = Visibility.Visible;
                ConfigList.Visibility = Visibility.Collapsed;
            }
            else
            {
                EmptyState.Visibility = Visibility.Collapsed;
                ConfigList.Visibility = Visibility.Visible;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();
            
            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Ha üres a keresőmező, minden konfigurációt megjelenít
                _filteredConfigurations.Clear();
                foreach (var config in _allConfigurations)
                {
                    _filteredConfigurations.Add(config);
                }
            }
            else
            {
                // Keresés a név, CPU, GPU stb. mezőkben
                _filteredConfigurations.Clear();
                var filtered = _allConfigurations.Where(c => 
                    c.Name.ToLower().Contains(searchText) || 
                    c.CPU.ToLower().Contains(searchText) || 
                    c.GPU.ToLower().Contains(searchText) || 
                    c.RAM.ToLower().Contains(searchText) || 
                    c.Motherboard.ToLower().Contains(searchText) || 
                    c.Storage.ToLower().Contains(searchText) || 
                    c.Case.ToLower().Contains(searchText));
                
                foreach (var config in filtered)
                {
                    _filteredConfigurations.Add(config);
                }
            }
            
            UpdateConfigCounter();
            CheckIfEmpty();
            RefreshConfigList();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortComboBox.SelectedItem == null) return;
            
            ComboBoxItem selectedItem = SortComboBox.SelectedItem as ComboBoxItem;
            string sortOption = selectedItem?.Content.ToString();
            
            switch (sortOption)
            {
                case "Legújabb elöl":
                    _filteredConfigurations = new ObservableCollection<ConfigurationItem>(
                        _filteredConfigurations.OrderByDescending(c => c.SaveDate));
                    break;
                case "Legrégebbi elöl":
                    _filteredConfigurations = new ObservableCollection<ConfigurationItem>(
                        _filteredConfigurations.OrderBy(c => c.SaveDate));
                    break;
                case "Név szerint (A-Z)":
                    _filteredConfigurations = new ObservableCollection<ConfigurationItem>(
                        _filteredConfigurations.OrderBy(c => c.Name));
                    break;
                case "Név szerint (Z-A)":
                    _filteredConfigurations = new ObservableCollection<ConfigurationItem>(
                        _filteredConfigurations.OrderByDescending(c => c.Name));
                    break;
                case "Ár szerint (növekvő)":
                    _filteredConfigurations = new ObservableCollection<ConfigurationItem>(
                        _filteredConfigurations.OrderBy(c => c.Price));
                    break;
                case "Ár szerint (csökkenő)":
                    _filteredConfigurations = new ObservableCollection<ConfigurationItem>(
                        _filteredConfigurations.OrderByDescending(c => c.Price));
                    break;
            }
            
            RefreshConfigList();
        }

        private void RefreshConfigList()
        {
            // Dinamikusan renderjük újra a konfigurációkat az ItemsControl-ban
            ConfigList.Items.Clear();

            foreach (var config in _filteredConfigurations)
            {
                // Létrehozzuk a konfiguráció kártyát
                Border configBorder = new Border
                {
                    Style = (Style)Resources["ConfigItem"],
                    Tag = config.Id
                };
                configBorder.MouseUp += ConfigItem_Click;                // A Border-nek adjuk a padding-ot
                configBorder.Padding = new Thickness(20);
                
                // A belső Grid létrehozása
                Grid mainGrid = new Grid();
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                // Header rész
                Grid headerGrid = new Grid();
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                StackPanel headerLeft = new StackPanel();
                TextBlock nameBlock = new TextBlock
                {
                    Text = config.Name,
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White
                };
                TextBlock dateBlock = new TextBlock
                {
                    Text = $"Mentve: {config.SaveDate}",
                    FontSize = 12,
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#999999"),
                    Margin = new Thickness(0, 4, 0, 0)
                };
                headerLeft.Children.Add(nameBlock);
                headerLeft.Children.Add(dateBlock);
                Grid.SetColumn(headerLeft, 0);

                StackPanel buttonsPanel = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
                
                Button editButton = new Button
                {
                    Style = (Style)Resources["IconButton"],
                    Tag = config.Id,
                };
                editButton.Click += EditConfig_Click;
                TextBlock editIcon = new TextBlock
                {
                    Text = "\uE70F", // Szerkesztés ikon
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    FontSize = 14
                };
                editButton.Content = editIcon;
                
                Button deleteButton = new Button
                {
                    Style = (Style)Resources["DeleteButton"],
                    Tag = config.Id
                };
                deleteButton.Click += DeleteConfig_Click;
                TextBlock deleteIcon = new TextBlock
                {
                    Text = "\uE74D", // Törlés ikon
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    FontSize = 14
                };
                deleteButton.Content = deleteIcon;
                
                buttonsPanel.Children.Add(editButton);
                buttonsPanel.Children.Add(deleteButton);
                Grid.SetColumn(buttonsPanel, 1);

                headerGrid.Children.Add(headerLeft);
                headerGrid.Children.Add(buttonsPanel);
                Grid.SetRow(headerGrid, 0);

                // Komponensek rész
                Grid detailsGrid = new Grid { Margin = new Thickness(0, 16, 0, 16) };
                detailsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                detailsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                detailsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                // Bal oldali komponensek
                StackPanel leftComponents = new StackPanel();
                TextBlock cpuBlock = new TextBlock
                {
                    Text = $"CPU: {config.CPU}",
                    FontSize = 14,
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#CCCCCC"),
                    Margin = new Thickness(0, 0, 0, 6)
                };
                TextBlock gpuBlock = new TextBlock
                {
                    Text = $"GPU: {config.GPU}",
                    FontSize = 14,
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#CCCCCC"),
                    Margin = new Thickness(0, 0, 0, 6)
                };
                TextBlock ramBlock = new TextBlock
                {
                    Text = $"RAM: {config.RAM}",
                    FontSize = 14,
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#CCCCCC"),
                    Margin = new Thickness(0, 0, 0, 6)
                };
                leftComponents.Children.Add(cpuBlock);
                leftComponents.Children.Add(gpuBlock);
                leftComponents.Children.Add(ramBlock);
                Grid.SetColumn(leftComponents, 0);

                // Középső komponensek
                StackPanel middleComponents = new StackPanel();
                TextBlock motherboardBlock = new TextBlock
                {
                    Text = $"Alaplap: {config.Motherboard}",
                    FontSize = 14,
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#CCCCCC"),
                    Margin = new Thickness(0, 0, 0, 6)
                };
                TextBlock storageBlock = new TextBlock
                {
                    Text = $"Tárhely: {config.Storage}",
                    FontSize = 14,
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#CCCCCC"),
                    Margin = new Thickness(0, 0, 0, 6)
                };
                TextBlock caseBlock = new TextBlock
                {
                    Text = $"Ház: {config.Case}",
                    FontSize = 14,
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#CCCCCC"),
                    Margin = new Thickness(0, 0, 0, 6)
                };
                middleComponents.Children.Add(motherboardBlock);
                middleComponents.Children.Add(storageBlock);
                middleComponents.Children.Add(caseBlock);
                Grid.SetColumn(middleComponents, 1);

                // Ár
                StackPanel pricePanel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Right };
                Border priceBorder = new Border
                {
                    Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#0D66D0"),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(16, 8, 16, 8)
                };
                TextBlock priceBlock = new TextBlock
                {
                    Text = $"{config.Price:N0} Ft",
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White
                };
                priceBorder.Child = priceBlock;
                pricePanel.Children.Add(priceBorder);
                Grid.SetColumn(pricePanel, 2);

                detailsGrid.Children.Add(leftComponents);
                detailsGrid.Children.Add(middleComponents);
                detailsGrid.Children.Add(pricePanel);
                Grid.SetRow(detailsGrid, 1);

                // Teljesítmény rész
                Grid performanceGrid = new Grid();
                performanceGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                performanceGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                TextBlock perfLabelBlock = new TextBlock
                {
                    Text = "Teljesítmény:",
                    FontSize = 14,
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#999999"),
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 8, 0)
                };
                Grid.SetColumn(perfLabelBlock, 0);

                Grid progressGrid = new Grid { VerticalAlignment = VerticalAlignment.Center };
                ProgressBar progressBar = new ProgressBar
                {
                    Value = config.PerformanceScore,
                    Maximum = 100,
                    Height = 8,
                    Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#333333"),
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#0D66D0")
                };
                TextBlock scoreBlock = new TextBlock
                {
                    Text = $"{config.PerformanceScore}/100",
                    FontSize = 12,
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#CCCCCC"),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 12, 0, 0)
                };
                progressGrid.Children.Add(progressBar);
                progressGrid.Children.Add(scoreBlock);
                Grid.SetColumn(progressGrid, 1);

                performanceGrid.Children.Add(perfLabelBlock);
                performanceGrid.Children.Add(progressGrid);
                Grid.SetRow(performanceGrid, 2);

                // Minden elem hozzáadása a fő Grid-hez
                mainGrid.Children.Add(headerGrid);
                mainGrid.Children.Add(detailsGrid);
                mainGrid.Children.Add(performanceGrid);

                configBorder.Child = mainGrid;
                ConfigList.Items.Add(configBorder);
            }
        }

        private void ConfigItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                int configId = (int)border.Tag;
                MessageBox.Show($"A {configId} azonosítójú konfigurációt részletei megnyitva", "Konfiguráció megnyitása", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Itt nyithatnánk meg a konfiguráció részleteit vagy szerkesztését
                // Átirányítás a ConfigBuilder oldalra, a konfiguráció ID-jával
            }
        }

        private void EditConfig_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int configId = (int)button.Tag;
            
            // Az esemény terjedésének megakadályozása, hogy ne nyíljon meg a konfiguráció részletei is
            e.Handled = true;
            
            MessageBox.Show($"A {configId} azonosítójú konfiguráció szerkesztése", "Szerkesztés", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Itt nyithatnánk meg a konfiguráció szerkesztését
            // Átirányítás a ConfigBuilder oldalra, szerkesztési módban, a konfiguráció ID-jával
        }

        private void DeleteConfig_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int configId = (int)button.Tag;
            
            // Az esemény terjedésének megakadályozása
            e.Handled = true;
            
            MessageBoxResult result = MessageBox.Show(
                "Biztosan törölni szeretnéd ezt a konfigurációt?", 
                "Konfiguráció törlése", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                // Konfiguráció törlése
                var configToRemove = _allConfigurations.FirstOrDefault(c => c.Id == configId);
                if (configToRemove != null)
                {
                    _allConfigurations.Remove(configToRemove);
                    _filteredConfigurations = new ObservableCollection<ConfigurationItem>(_allConfigurations);
                    
                    // Lista frissítése
                    UpdateConfigCounter();
                    CheckIfEmpty();
                    RefreshConfigList();
                    
                    MessageBox.Show("A konfiguráció sikeresen törölve!", "Sikeres törlés", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void NewConfig_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Új konfiguráció készítése", "Új konfiguráció", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // Itt nyithatnánk meg a ConfigBuilder oldalt új konfiguráció készítésére
            // Például:
            // var configBuilder = new ConfigBuilder();
            // MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            // if (mainWindow != null)
            // {
            //     mainWindow.MainContent.Content = configBuilder;
            // }
        }
    }
}
