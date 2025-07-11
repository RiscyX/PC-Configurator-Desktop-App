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
using PC_Configurator.Models;
using PC_Configurator.Views.App;

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

        private int _currentUserId = 0; // Az aktuális felhasználó azonosítója
        
        public Configs()
        {
            InitializeComponent();
            
            // Gyűjtemény inicializálása már a kezdetben
            _allConfigurations = new ObservableCollection<ConfigurationItem>();

            // Jelenlegi felhasználó azonosítója
            // Ezt az értéket a bejelentkezett felhasználó azonosítójából kéne megszerezni,
            // ami általában a Session-ben vagy más globális helyen tárolva van
            try {
                // Például, ha az App.xaml.cs-ben van egy CurrentUser property:
                // _currentUserId = ((App)Application.Current).CurrentUser.Id;
                
                // Mivel nem látjuk ezt a kódrészt, itt most teszteléshez az 1-es ID-t használjuk
                _currentUserId = 1;
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Nem sikerült lekérni a jelenlegi felhasználó azonosítóját: {ex.Message}");
                _currentUserId = 1; // Alapértelmezett érték, ha hiba történik
            }

            try
            {
                // Adatbázisból töltjük be a konfigurációkat
                LoadConfigurationsFromDatabase();
                
                // Konfiguráció darabszám megjelenítése
                UpdateConfigCounter();
                
                // Annak ellenőrzése, hogy van-e konfiguráció
                CheckIfEmpty();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a Configs inicializálásakor: {ex.Message}");
                MessageBox.Show($"Hiba történt az alkalmazás indításakor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadConfigurationsFromDatabase()
        {
            try
            {
                // Inicializáljuk a gyűjteményeket üres listákkal, hogy biztosan ne legyenek null-ok
                if (_allConfigurations == null)
                {
                    _allConfigurations = new ObservableCollection<ConfigurationItem>();
                }
                else
                {
                    _allConfigurations.Clear();
                }
                
                // A szűrési funkció el lett távolítva
                
                List<ConfigurationModel> configModels = null;
                
                try
                {
                    // Konfigurációk lekérése a vw_FullConfigurations nézetből a felhasználó azonosítója alapján
                    // A vw_FullConfigurations már tartalmazza az összes komponenst és azok adatait, így nincs szükség további lekérésekre
                    configModels = ConfigurationModel.LoadUserConfigurations(_currentUserId);
                    
                    // Debug információ
                    System.Diagnostics.Debug.WriteLine($"Sikeresen betöltve {configModels?.Count ?? 0} konfiguráció modell és komponenseik a vw_FullConfigurations nézetből.");
                }
                catch (Exception loadEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Hiba a konfigurációk lekérésekor a vw_FullConfigurations nézetből: {loadEx.Message}");
                    configModels = new List<ConfigurationModel>(); // Ha hiba van, üres listát használunk
                }
                
                // Csak akkor próbálunk feldolgozni, ha valóban vannak konfiguációk
                if (configModels != null && configModels.Count > 0)
                {
                    // Végigmegyünk minden egyes konfiguráción
                    foreach (var configModel in configModels)
                    {
                        try
                        {
                            if (configModel != null)
                            {
                                try
                                {
                                    // Mivel a configModel már tartalmazza az összes adatot a vw_FullConfigurations nézetből,
                                    // nincs szükség újabb adatbázis lekérdezésre
                                    
                                    // Már itt van minden komponens adata a vw_FullConfigurations nézetből, 
                                    // ezért nincs szükség a konfigurációk újratöltésére
                                    
                                    // Részletes log a komponensekről
                                    System.Diagnostics.Debug.WriteLine($"Konfiguráció ID {configModel.Id} komponensei: " +
                                        $"\nCPU: {(configModel.CPU != null ? configModel.CPU.Manufacturer + " " + configModel.CPU.Name : "hiányzik")}, " +
                                        $"\nGPU: {(configModel.GPU != null ? configModel.GPU.Manufacturer + " " + configModel.GPU.Name : "hiányzik")}, " +
                                        $"\nRAM: {(configModel.RAM != null ? configModel.RAM.CapacityGB + "GB " + configModel.RAM.Type : "hiányzik")}, " +
                                        $"\nMotherboard: {(configModel.Motherboard != null ? configModel.Motherboard.Manufacturer + " " + configModel.Motherboard.Name : "hiányzik")}, " +
                                        $"\nStorage: {(configModel.Storage != null ? configModel.Storage.CapacityGB + "GB " + configModel.Storage.Type : "hiányzik")}, " +
                                        $"\nCase: {(configModel.Case != null ? configModel.Case.Name : "hiányzik")}");
                                    
                                    // A konfigurációt közvetlenül konvertáljuk és adjuk hozzá a listához
                                    var configItem = ConvertToConfigurationItem(configModel);
                                    _allConfigurations.Add(configItem);
                                }
                                catch (Exception componentEx)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Hiba a konfiguráció ({configModel.Id}) feldolgozása közben: {componentEx.Message}");
                                    if (componentEx.InnerException != null)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Belső kivétel: {componentEx.InnerException.Message}");
                                    }
                                    
                                    // Megpróbáljuk menteni ami menthető, és konvertáljuk az eredeti configmodelt
                                    var configItem = ConvertToConfigurationItem(configModel);
                                    _allConfigurations.Add(configItem);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Ha egy konfiguráció betöltése hibát okoz, azt loggoljuk, de folytatjuk a többivel
                            System.Diagnostics.Debug.WriteLine($"Hiba egy konfiguráció feldolgozásakor: {ex.Message}");
                        }
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"Sikeresen betöltve {_allConfigurations.Count} konfiguráció az adatbázisból a {_currentUserId} azonosítójú felhasználó számára");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a konfigurációk betöltése közben: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Belső hiba: {ex.InnerException.Message}");
                }
                
                // Hibaüzenet megjelenítése a felhasználónak
                MessageBox.Show($"Nem sikerült betölteni a konfigurációkat: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            // Frissítjük a megjelenítést, attól függetlenül, hogy sikerült-e betölteni adatokat
            try {
                RefreshConfigList();
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Hiba a lista frissítésekor: {ex.Message}");
            }
        }

        private void UpdateConfigCounter()
        {
            try
            {
                // Null ellenőrzés a gyűjteményre - csak az _allConfigurations-t használjuk
                if (_allConfigurations == null)
                {
                    _allConfigurations = new ObservableCollection<ConfigurationItem>();
                }
                
                // Null ellenőrzés a kontrollra
                if (ConfigCountBlock == null)
                {
                    System.Diagnostics.Debug.WriteLine("ConfigCountBlock kontroll null az UpdateConfigCounter metódusban!");
                    return;
                }
                
                int count = _allConfigurations.Count;
                ConfigCountBlock.Text = $"{count} konfiguráció";
                ConfigCountBlock.FontSize = 20; // Nagyobb betűméret a számlálónak
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a konfiguráció számláló frissítésekor: {ex.Message}");
            }
        }

        private void CheckIfEmpty()
        {
            try
            {
                // Kontrollok null ellenőrzése
                if (EmptyState == null || ConfigList == null)
                {
                    System.Diagnostics.Debug.WriteLine("EmptyState vagy ConfigList kontroll null a CheckIfEmpty metódusban!");
                    return;
                }
                
                // Gyűjtemény null ellenőrzése - csak az _allConfigurations-t használjuk
                if (_allConfigurations == null || _allConfigurations.Count == 0)
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba az üres állapot ellenőrzésekor: {ex.Message}");
            }
        }
        
        // Helper metódus a ConfigurationModel átalakításához ConfigurationItem formátumba
        private ConfigurationItem ConvertToConfigurationItem(ConfigurationModel config)
        {
            if (config == null)
            {
                return new ConfigurationItem
                {
                    Id = 0,
                    Name = "Ismeretlen konfiguráció",
                    SaveDate = DateTime.Now.ToString("yyyy.MM.dd. HH:mm"),
                    CPU = "Nincs kiválasztva",
                    GPU = "Nincs kiválasztva",
                    RAM = "Nincs kiválasztva",
                    Motherboard = "Nincs kiválasztva",
                    Storage = "Nincs kiválasztva",
                    Case = "Nincs kiválasztva",
                    Price = 0,
                    PerformanceScore = 0
                };
            }
            
            try
            {
                // Komponensek adatainak előkészítése a megjelenítéshez
                string cpuText = config.CPU != null ? $"{config.CPU.Manufacturer} {config.CPU.Name}" : "Nincs kiválasztva";
                string gpuText = config.GPU != null ? $"{config.GPU.Manufacturer} {config.GPU.Name}" : "Nincs kiválasztva";
                string ramText = config.RAM != null ? $"{config.RAM.CapacityGB}GB {config.RAM.Type} {config.RAM.SpeedMHz}MHz" : "Nincs kiválasztva";
                string mbText = config.Motherboard != null ? $"{config.Motherboard.Manufacturer} {config.Motherboard.Name}" : "Nincs kiválasztva";
                string storageText = config.Storage != null ? $"{config.Storage.CapacityGB}GB {config.Storage.Type}" : "Nincs kiválasztva";
                string caseText = config.Case != null ? $"{config.Case.Name}" : "Nincs kiválasztva";
                
                return new ConfigurationItem
                {
                    Id = config.Id,
                    Name = config.Name ?? "Névtelen konfiguráció",
                    SaveDate = config.CreatedAt.ToString("yyyy.MM.dd. HH:mm"),
                    CPU = cpuText,
                    GPU = gpuText,
                    RAM = ramText,
                    Motherboard = mbText,
                    Storage = storageText,
                    Case = caseText,
                    Price = (int)config.Price,
                    PerformanceScore = config.PerformanceScore
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a konfiguráció konvertálása közben: {ex.Message}");
                
                // Hibás konvertálás esetén is visszaadunk egy alapértelmezett konfigurációt
                return new ConfigurationItem
                {
                    Id = config.Id,
                    Name = config.Name ?? "Hibás konfiguráció",
                    SaveDate = DateTime.Now.ToString("yyyy.MM.dd. HH:mm"),
                    CPU = "Hiba történt",
                    GPU = "Hiba történt",
                    RAM = "Hiba történt",
                    Motherboard = "Hiba történt",
                    Storage = "Hiba történt",
                    Case = "Hiba történt",
                    Price = 0,
                    PerformanceScore = 0
                };
            }
        }

        // A szűrés funkcionalitás teljesen eltávolítva a felhasználó kérése alapján
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Nem csinálunk semmit, a szűrés ki lett kapcsolva
        }

        // A rendezés funkcionalitás eltávolítva a felhasználó kérése alapján
        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Nem csinálunk semmit, a rendezés ki lett kapcsolva
        }

        private void RefreshConfigList()
        {
            try
            {
                // Ellenőrizzük, hogy a ConfigList nem null
                if (ConfigList == null)
                {
                    System.Diagnostics.Debug.WriteLine("ConfigList kontroll null a RefreshConfigList metódusban!");
                    return;
                }
                
                // Tisztítsuk a listát
                ConfigList.Items.Clear();
                
                // Biztosítsuk, hogy a gyűjtemény ne legyen null
                if (_allConfigurations == null)
                {
                    _allConfigurations = new ObservableCollection<ConfigurationItem>();
                }
                
                // Mivel a szűrés teljesen eltávolításra került, csak az _allConfigurations-t használjuk
                
                // Frissítsük a számlálót és az üres állapotot
                UpdateConfigCounter();
                CheckIfEmpty();
                
                // Ha nincs elem, kilépünk
                if (_allConfigurations.Count == 0)
                {
                    return;
                }
                
                // Végigmegyünk az elemeken és létrehozzuk a UI elemeket - csak az _allConfigurations-t használjuk
                foreach (var config in _allConfigurations)
                {
                // Létrehozzuk a konfiguráció kártyát - még nagyobb mérettel
                Border configBorder = new Border
                {
                    Style = (Style)Resources["ConfigItem"],
                    Tag = config.Id,
                    Margin = new Thickness(20, 0, 20, 40), // Jelentősen növeljük a margókat minden oldalon
                    HorizontalAlignment = HorizontalAlignment.Stretch,  // Teljes szélességet használjuk
                    VerticalAlignment = VerticalAlignment.Stretch,      // Teljes magasságot használjuk
                    MinHeight = 350 // Minimális magasság
                };
                configBorder.MouseUp += ConfigItem_Click;
                // Jóval nagyobb padding a tartalom körül
                configBorder.Padding = new Thickness(50, 40, 50, 40);
                
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
                    FontSize = 24,  // Nagyobb betűméret
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 0, 0, 8)  // Több hely alul
                };
                TextBlock dateBlock = new TextBlock
                {
                    Text = $"Mentve: {config.SaveDate}",
                    FontSize = 16,  // Nagyobb betűméret
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#999999"),
                    Margin = new Thickness(0, 0, 0, 10)  // Több hely alul
                };
                headerLeft.Children.Add(nameBlock);
                headerLeft.Children.Add(dateBlock);
                Grid.SetColumn(headerLeft, 0);

                StackPanel buttonsPanel = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
                
                Button deleteButton = new Button
                {
                    Style = (Style)Resources["DeleteButton"],
                    Tag = config.Id,
                    Padding = new Thickness(12) // Nagyobb padding a gombnak
                };
                deleteButton.Click += DeleteConfig_Click;
                TextBlock deleteIcon = new TextBlock
                {
                    Text = "\uE74D", // Törlés ikon
                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                    FontSize = 20 // Nagyobb ikon méret
                };
                deleteButton.Content = deleteIcon;
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
                    FontSize = 18, // Nagyobb betűméret
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#CCCCCC"),
                    Margin = new Thickness(0, 0, 0, 12) // Nagyobb térköz
                };
                TextBlock gpuBlock = new TextBlock
                {
                    Text = $"GPU: {config.GPU}",
                    FontSize = 18, // Nagyobb betűméret
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#CCCCCC"),
                    Margin = new Thickness(0, 0, 0, 12) // Nagyobb térköz
                };
                TextBlock ramBlock = new TextBlock
                {
                    Text = $"RAM: {config.RAM}",
                    FontSize = 18, // Nagyobb betűméret
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#CCCCCC"),
                    Margin = new Thickness(0, 0, 0, 12) // Nagyobb térköz
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
                    FontSize = 18, // Nagyobb betűméret
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#CCCCCC"),
                    Margin = new Thickness(0, 0, 0, 12) // Nagyobb térköz
                };
                TextBlock storageBlock = new TextBlock
                {
                    Text = $"Tárhely: {config.Storage}",
                    FontSize = 18, // Nagyobb betűméret
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#CCCCCC"),
                    Margin = new Thickness(0, 0, 0, 12) // Nagyobb térköz
                };
                TextBlock caseBlock = new TextBlock
                {
                    Text = $"Ház: {config.Case}",
                    FontSize = 18, // Nagyobb betűméret
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#CCCCCC"),
                    Margin = new Thickness(0, 0, 0, 12) // Nagyobb térköz
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
                    CornerRadius = new CornerRadius(8), // Kicsit nagyobb lekerekítés
                    Padding = new Thickness(24, 12, 24, 12) // Nagyobb padding
                };
                TextBlock priceBlock = new TextBlock
                {
                    Text = $"{config.Price:N0} Ft",
                    FontSize = 24, // Nagyobb betűméret
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
                    FontSize = 18, // Nagyobb betűméret
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#999999"),
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 10, 12, 0) // Nagyobb térköz
                };
                Grid.SetColumn(perfLabelBlock, 0);

                Grid progressGrid = new Grid { VerticalAlignment = VerticalAlignment.Center };
                ProgressBar progressBar = new ProgressBar
                {
                    Value = config.PerformanceScore,
                    Maximum = 100,
                    Height = 12, // Magasabb progress bar
                    Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#333333"),
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#0D66D0")
                };
                TextBlock scoreBlock = new TextBlock
                {
                    Text = $"{config.PerformanceScore}/100",
                    FontSize = 16, // Nagyobb betűméret
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#CCCCCC"),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 16, 0, 0) // Nagyobb térköz
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a konfiguráció lista frissítésekor: {ex.Message}");
                MessageBox.Show($"Hiba történt a konfiguráció lista frissítése közben: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ConfigItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                int configId = (int)border.Tag;
                
                try
                {
                    // A vw_FullConfigurations nézetből már betöltöttük a konfigurációt és komponenseit
                    // így egyből megnyithatjuk, vagy újratölthetjük ha biztosak akarunk lenni az aktuális adatokban
                    
                    // Opcionálisan: Konfiguráció frissítése az adatbázisból a legfrissebb adatokkal
                    var config = ConfigurationModel.LoadFromDatabase(configId);
                    
                    if (config != null)
                    {
                        // ConfigBuilder oldal megnyitása a konfiguráció részleteivel
                        var configBuilder = new ConfigBuilder();
                        configBuilder.LoadConfiguration(config);
                        
                        // A főablak megkeresése
                        var dashboard = Window.GetWindow(this) as Dashboard;
                        if (dashboard != null)
                        {
                            // A tartalmi terület frissítése a ConfigBuilder-rel
                            dashboard.MainContentArea.Content = configBuilder;
                        }
                        else
                        {
                            MessageBox.Show("Nem sikerült megtalálni a főablakot.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"A {configId} azonosítójú konfiguráció nem található.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Hiba a konfiguráció megnyitása közben: {ex.Message}");
                    MessageBox.Show($"Hiba történt a konfiguráció megnyitása közben: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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
                try
                {
                    // Konfiguráció törlése az adatbázisból
                    bool deleteSuccessful = ConfigurationModel.DeleteFromDatabase(configId);
                    
                    if (deleteSuccessful)
                    {
                        // Konfiguráció törlése a listából
                        if (_allConfigurations != null)
                        {
                            var configToRemove = _allConfigurations.FirstOrDefault(c => c.Id == configId);
                            if (configToRemove != null)
                            {
                                _allConfigurations.Remove(configToRemove);
                                
                                // A szűrési funkció el lett távolítva
                                
                                // Lista frissítése
                                UpdateConfigCounter();
                                CheckIfEmpty();
                                RefreshConfigList();
                                
                                MessageBox.Show("A konfiguráció sikeresen törölve!", "Sikeres törlés", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        else
                        {
                            // Ha valami miatt _allConfigurations null lenne
                            MessageBox.Show("A konfigurációkat nem sikerült betölteni. Kérjük frissítsd az oldalt.", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                            _allConfigurations = new ObservableCollection<ConfigurationItem>();
                            
                            // Újratöltjük az adatokat
                            LoadConfigurationsFromDatabase();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Nem sikerült törölni a konfigurációt az adatbázisból.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Hiba a konfiguráció törlése közben: {ex.Message}");
                    MessageBox.Show($"Hiba történt a konfiguráció törlése közben: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void NewConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ConfigBuilder oldal megnyitása
                var configBuilder = new ConfigBuilder();
                    
                // A főablak megkeresése
                var dashboard = Window.GetWindow(this) as Dashboard;
                if (dashboard != null)
                {
                    // A tartalmi terület frissítése a ConfigBuilder-rel
                    dashboard.MainContentArea.Content = configBuilder;
                }
                else
                {
                    MessageBox.Show("Nem sikerült megtalálni a főablakot.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba az új konfiguráció létrehozása közben: {ex.Message}");
                MessageBox.Show($"Hiba történt az új konfiguráció létrehozása közben: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
