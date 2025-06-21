using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ConfigBuilder.xaml
    /// </summary>
    public partial class ConfigBuilder : UserControl, INotifyPropertyChanged
    {
        // Componenet model structs for storing selected items
        private class ComponentItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Details { get; set; }
            public decimal Price { get; set; }
            public int PowerConsumption { get; set; }
        }
        
        // Property changed implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        // Property for storing total price
        private decimal _totalPrice = 0;
        public decimal TotalPrice 
        { 
            get { return _totalPrice; } 
            set 
            { 
                if (_totalPrice != value)
                {
                    _totalPrice = value;
                    TotalPriceBlock.Text = $"{_totalPrice:N0} Ft";
                    NotifyPropertyChanged(nameof(TotalPrice));
                }
            }
        }
        
        // Property for storing total power consumption
        private int _totalPower = 0;
        public int TotalPower 
        { 
            get { return _totalPower; } 
            set 
            { 
                if (_totalPower != value)
                {
                    _totalPower = value;
                    PowerConsumptionBlock.Text = $"{_totalPower} W";
                    NotifyPropertyChanged(nameof(TotalPower));
                }
            }
        }
        
        // Store the selected components
        private Dictionary<string, ComponentItem> selectedComponents = new Dictionary<string, ComponentItem>();
        
        // Dictionary of component cards by type
        private Dictionary<string, Border> componentCards = new Dictionary<string, Border>();
        
        // Dictionary of component name blocks by type
        private Dictionary<string, TextBlock> componentNameBlocks = new Dictionary<string, TextBlock>();
        
        // Dictionary of component detail blocks by type
        private Dictionary<string, TextBlock> componentDetailBlocks = new Dictionary<string, TextBlock>();
        
        // Dictionary for tracking compatibility
        private Dictionary<string, bool> componentCompatibility = new Dictionary<string, bool>();
        
        // Currently selected component type (for showing selection dialog)
        private string currentComponentType = "";
        
        public ConfigBuilder()
        {
            InitializeComponent();
            
            // Initialize the component cards dictionary
            componentCards.Add("CPU", CpuCard);
            componentCards.Add("GPU", GpuCard);
            componentCards.Add("RAM", RamCard);
            componentCards.Add("Storage", StorageCard);
            componentCards.Add("Motherboard", MotherboardCard);
            componentCards.Add("PSU", PsuCard);
            componentCards.Add("Case", CaseCard);
            
            // Initialize the component name blocks dictionary
            componentNameBlocks.Add("CPU", CpuNameBlock);
            componentNameBlocks.Add("GPU", GpuNameBlock);
            componentNameBlocks.Add("RAM", RamNameBlock);
            componentNameBlocks.Add("Storage", StorageNameBlock);
            componentNameBlocks.Add("Motherboard", MotherboardNameBlock);
            componentNameBlocks.Add("PSU", PsuNameBlock);
            componentNameBlocks.Add("Case", CaseNameBlock);
            
            // Initialize the component detail blocks dictionary
            componentDetailBlocks.Add("CPU", CpuDetailsBlock);
            componentDetailBlocks.Add("GPU", GpuDetailsBlock);
            componentDetailBlocks.Add("RAM", RamDetailsBlock);
            componentDetailBlocks.Add("Storage", StorageDetailsBlock);
            componentDetailBlocks.Add("Motherboard", MotherboardDetailsBlock);
            componentDetailBlocks.Add("PSU", PsuDetailsBlock);
            componentDetailBlocks.Add("Case", CaseDetailsBlock);
            
            // Initialize compatibility dictionary
            componentCompatibility.Add("CPU", false);
            componentCompatibility.Add("Motherboard", false);
            componentCompatibility.Add("RAM", false);
            
            // Add some mock data for demo
            SetupMockData();
            
            // Update the total price and power for the initial state
            UpdateTotals();
        }
        
        // Set up mock data for demo purposes
        private void SetupMockData()
        {
            // This is just a demo implementation - would be replaced by real data
            // Set up some demo components
            SelectComponent("CPU", 2, "AMD Ryzen 7 5800X", "8 mag / 16 szál, 3.8 GHz (4.7 GHz)", 89900M, 105);
            SelectComponent("GPU", 3, "NVIDIA RTX 3070", "8 GB GDDR6", 159900M, 220);
            
            // Update compatibility and performance metrics
            UpdateCompatibilityStatus();
            UpdatePerformanceRating();
        }
        
        private void ComponentCard_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag != null)
            {
                string componentType = border.Tag.ToString();
                currentComponentType = componentType;
                
                ShowComponentSelectionDialog(componentType);
            }
        }
        
        private void ShowComponentSelectionDialog(string componentType)
        {
            // In a real implementation, this would open a dialog with components from the database
            // For now, we'll simulate it with a message box
            MessageBox.Show($"A {componentType} komponens választó jelenleg fejlesztés alatt áll.\n\nEbben a demó verzióban már előre be vannak állítva mintaadatok a gépösszeállítás bemutatására.", 
                           "Komponens választó", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // For demo purposes, select a default component if not already selected
            if (!selectedComponents.ContainsKey(componentType))
            {
                switch(componentType)
                {
                    case "RAM":
                        SelectComponent(componentType, 1, "Kingston HyperX 32GB", "DDR4, 3600MHz, CL16", 49900M, 25);
                        break;
                    case "Motherboard":
                        SelectComponent(componentType, 2, "MSI MAG B550 TOMAHAWK", "AMD B550 chipset, AM4 foglalat", 54900M, 30);
                        break;
                    case "Storage":
                        SelectComponent(componentType, 1, "Samsung 970 EVO 1TB", "NVMe SSD, 3500/3300 MB/s", 44900M, 8);
                        break;
                    case "PSU":
                        SelectComponent(componentType, 1, "Corsair RM850x", "850W, Gold, moduláris", 38900M, 0);
                        break;
                    case "Case":
                        SelectComponent(componentType, 4, "Lian Li PC-O11 Dynamic", "Mid Tower, fekete", 42900M, 0);
                        break;
                }
            }
            
            // Update compatibility after selection
            UpdateCompatibilityStatus();
            UpdatePerformanceRating();
        }
        
        private void SelectComponent(string type, int id, string name, string details, decimal price, int power)
        {
            // Store the selected component
            selectedComponents[type] = new ComponentItem
            {
                Id = id,
                Name = name,
                Details = details,
                Price = price,
                PowerConsumption = power
            };
            
            // Update the UI
            if (componentNameBlocks.ContainsKey(type))
            {
                componentNameBlocks[type].Text = name;
            }
            
            if (componentDetailBlocks.ContainsKey(type))
            {
                componentDetailBlocks[type].Text = details;
            }
            
            // Mark the card as selected
            if (componentCards.ContainsKey(type))
            {
                componentCards[type].Style = (Style)FindResource("ComponentCardSelected");
            }
            
            // Update the total price and power
            UpdateTotals();
        }
        
        private void UpdateTotals()
        {
            // Calculate the total price
            decimal totalPrice = 0;
            foreach (var component in selectedComponents.Values)
            {
                totalPrice += component.Price;
            }
            TotalPrice = totalPrice;
            
            // Calculate the total power consumption
            int totalPower = 0;
            foreach (var component in selectedComponents.Values)
            {
                totalPower += component.PowerConsumption;
            }
            TotalPower = totalPower;
        }
        
        private void UpdateCompatibilityStatus()
        {
            // Check if CPU and Motherboard are compatible (mock implementation)
            if (selectedComponents.ContainsKey("CPU") && selectedComponents.ContainsKey("Motherboard"))
            {
                // In a real implementation, this would check actual compatibility rules
                // For demo, we'll assume compatibility based on the selected components
                bool compatible = true;
                
                // Update the compatibility message
                if (compatible)
                {
                    CompatibilityMessage.Text = "Az összes kiválasztott komponens kompatibilis egymással.";
                    CompatibilityMessage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"));
                }
                else
                {
                    CompatibilityMessage.Text = "Kompatibilitási probléma! Ellenőrizd a CPU és alaplap illeszkedését.";
                    CompatibilityMessage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F44336"));
                }
            }
            else
            {
                CompatibilityMessage.Text = "Válassz CPU-t és Alaplapot a kompatibilitás ellenőrzéséhez.";
                CompatibilityMessage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#AAAAAA"));
            }
        }
        
        private void UpdatePerformanceRating()
        {
            // Simple performance rating calculation based on selected components
            int performanceScore = 0;
            string recommendedUse = "Általános felhasználás";
            
            // Calculate performance score based on components (mock implementation)
            if (selectedComponents.ContainsKey("CPU"))
            {
                performanceScore += 25;  // CPU adds 25 points
            }
            
            if (selectedComponents.ContainsKey("GPU"))
            {
                performanceScore += 35;  // GPU adds 35 points
            }
            
            if (selectedComponents.ContainsKey("RAM"))
            {
                performanceScore += 15;  // RAM adds 15 points
            }
            
            if (selectedComponents.ContainsKey("Storage"))
            {
                performanceScore += 10;  // Storage adds 10 points
            }
            
            // Set recommended use based on performance score
            if (performanceScore >= 60)
            {
                recommendedUse = "Játék és professzionális felhasználás";
            }
            else if (performanceScore >= 40)
            {
                recommendedUse = "Modern játékok és multimédia";
            }
            else if (performanceScore >= 20)
            {
                recommendedUse = "Irodai munka és alapvető multimédia";
            }
            
            // Update the UI
            PerformanceRating.Text = $"{performanceScore}/100";
            RecommendedUseBlock.Text = recommendedUse;
            
            // Update the progress bar
            var progressBar = PerformanceRating.Parent as Grid;
            if (progressBar != null && progressBar.Children.Count > 0 && progressBar.Children[0] is ProgressBar)
            {
                (progressBar.Children[0] as ProgressBar).Value = performanceScore;
            }
        }
        
        private void SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            if (selectedComponents.Count > 0)
            {
                // In a real app, this would save the configuration to a database
                MessageBox.Show("A konfiguráció mentése jelenleg fejlesztés alatt áll.\n\nA konfiguráció adatai:\n" +
                               $"Összesen {selectedComponents.Count} komponens\n" +
                               $"Teljes ár: {TotalPrice:N0} Ft\n" +
                               $"Becsült teljesítményigény: {TotalPower} W",
                               "Konfiguráció mentése", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Nem menthető üres konfiguráció. Kérlek, válassz komponenseket!", 
                               "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        
        private void ClearConfig_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.Yes;
            
            if (selectedComponents.Count > 0)
            {
                result = MessageBox.Show("Biztosan törölni szeretnéd az összes kiválasztott komponenst?", 
                                        "Konfiguráció törlése", MessageBoxButton.YesNo, MessageBoxImage.Question);
            }
            
            if (result == MessageBoxResult.Yes)
            {
                // Clear the selected components
                selectedComponents.Clear();
                
                // Reset the UI
                foreach (var type in componentNameBlocks.Keys)
                {
                    componentNameBlocks[type].Text = $"Válassz {TranslateComponentType(type).ToLower()}";
                    componentDetailBlocks[type].Text = "Nincs kiválasztva";
                    componentCards[type].Style = (Style)FindResource("ComponentCard");
                }
                
                // Reset total price and power
                TotalPrice = 0;
                TotalPower = 0;
                
                // Reset compatibility message
                CompatibilityMessage.Text = "Válassz komponenseket a kompatibilitás ellenőrzéséhez";
                CompatibilityMessage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#AAAAAA"));
                
                // Reset performance rating
                UpdatePerformanceRating();
            }
        }
        
        private string TranslateComponentType(string type)
        {
            switch (type)
            {
                case "CPU": return "processzort";
                case "GPU": return "videókártyát";
                case "RAM": return "memóriát";
                case "Storage": return "tárhelyet";
                case "Motherboard": return "alaplapot";
                case "PSU": return "tápegységet";
                case "Case": return "számítógépházat";
                default: return type;
            }
        }
    }
}
