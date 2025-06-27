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
using PC_Configurator.Helpers;

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
            
            // Validációs események beállítása
            NameTextBox.LostFocus += (s, e) => ValidateName();
            NameTextBox.TextChanged += (s, e) => ValidateName();
            
            ManufacturerComboBox.SelectionChanged += (s, e) => ValidateManufacturer();
            CoresComboBox.SelectionChanged += (s, e) => ValidateCores();
            ThreadsComboBox.SelectionChanged += (s, e) => ValidateThreads();
            
            BaseClockTextBox.LostFocus += (s, e) => ValidateBaseClock();
            BaseClockTextBox.TextChanged += (s, e) => ValidateBaseClock();
            
            BoostClockTextBox.LostFocus += (s, e) => ValidateBoostClock();
            BoostClockTextBox.TextChanged += (s, e) => ValidateBoostClock();
            
            PriceTextBox.LostFocus += (s, e) => ValidatePrice();
            PriceTextBox.TextChanged += (s, e) => ValidatePrice();
            
            PowerConsumptionTextBox.LostFocus += (s, e) => ValidatePowerConsumption();
            PowerConsumptionTextBox.TextChanged += (s, e) => ValidatePowerConsumption();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Minden mező validálása és az eredmények összegyűjtése
                bool isNameValid = ValidateName();
                bool isManufacturerValid = ValidateManufacturer();
                bool isCoresValid = ValidateCores();
                bool isThreadsValid = ValidateThreads();
                bool isBaseClockValid = ValidateBaseClock();
                bool isBoostClockValid = ValidateBoostClock();
                bool isPriceValid = ValidatePrice();
                bool isPowerConsumptionValid = ValidatePowerConsumption();

                // Teljes űrlap validáció
                if (!ValidationHelper.ValidateForm(
                    isNameValid, isManufacturerValid, isCoresValid,
                    isThreadsValid, isBaseClockValid, isBoostClockValid,
                    isPriceValid, isPowerConsumptionValid))
                {
                    return;
                }

                // Ha minden mező valid, akkor kiolvassuk az értékeket
                string name = NameTextBox.Text.Trim();
                string manufacturer = (ManufacturerComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                int cores = int.Parse((CoresComboBox.SelectedItem as ComboBoxItem)?.Content.ToString());
                int threads = int.Parse((ThreadsComboBox.SelectedItem as ComboBoxItem)?.Content.ToString());
                
                // Számok konvertálása kulturától függetlenül
                float baseClock = (float)double.Parse(
                    BaseClockTextBox.Text.Replace(',', '.'), 
                    System.Globalization.NumberStyles.Any, 
                    System.Globalization.CultureInfo.InvariantCulture
                );
                
                float boostClock = (float)double.Parse(
                    BoostClockTextBox.Text.Replace(',', '.'), 
                    System.Globalization.NumberStyles.Any, 
                    System.Globalization.CultureInfo.InvariantCulture
                );

                decimal price = 0;
                if (!string.IsNullOrEmpty(PriceTextBox.Text))
                {
                    decimal.TryParse(
                        PriceTextBox.Text.Replace(',', '.'),
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out price
                    );
                }
                
                int powerConsumption = 0;
                if (!string.IsNullOrEmpty(PowerConsumptionTextBox.Text))
                {
                    int.TryParse(
                        PowerConsumptionTextBox.Text.Trim(),
                        out powerConsumption
                    );
                }

                // Az objektum létrehozása
                var model = new PC_Configurator.Models.CPU 
                { 
                    Name = name, 
                    Manufacturer = manufacturer, 
                    Socket = SocketTextBox.Text.Trim(),  // Socket hozzáadása
                    Cores = cores, 
                    Threads = threads, 
                    BaseClockGHz = baseClock, 
                    BoostClockGHz = boostClock,
                    Price = price,
                    PowerConsumption = powerConsumption
                };
                
                // Ellenőrizzük, hogy szerkesztés vagy új hozzáadás történik
                bool isEditing = Tag != null && Tag is int;
                
                if (isEditing)
                {
                    // Szerkesztés esetén az ID beállítása
                    model.Id = (int)Tag;
                    SaveChangesToDatabase(model);
                    MessageBox.Show("CPU sikeresen frissítve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Az ablak bezárása, ha szerkesztési módban vagyunk
                    Window.GetWindow(this)?.Close();
                }
                else
                {
                    // Új hozzáadás esetén nincs ID
                    SaveNewToDatabase(model);
                    MessageBox.Show("CPU sikeresen hozzáadva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Űrlap törlése új hozzáadás esetén
                    ResetForm();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "CPU mentése");
            }
        }
        
        private void SaveNewToDatabase(PC_Configurator.Models.CPU cpu)
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
            {
                conn.Open();
                var command = new System.Data.SqlClient.SqlCommand(
                    "INSERT INTO CPUs (Name, Manufacturer, Socket, Cores, Threads, BaseClockGHz, BoostClockGHz, Price, PowerConsumption) " +
                    "VALUES (@Name, @Manufacturer, @Socket, @Cores, @Threads, @BaseClockGHz, @BoostClockGHz, @Price, @PowerConsumption)", conn);
                
                command.Parameters.AddWithValue("@Name", cpu.Name);
                command.Parameters.AddWithValue("@Manufacturer", cpu.Manufacturer);
                command.Parameters.AddWithValue("@Socket", string.IsNullOrEmpty(cpu.Socket) ? DBNull.Value : (object)cpu.Socket);
                command.Parameters.AddWithValue("@Cores", cpu.Cores);
                command.Parameters.AddWithValue("@Threads", cpu.Threads);
                command.Parameters.AddWithValue("@BaseClockGHz", cpu.BaseClockGHz);
                command.Parameters.AddWithValue("@BoostClockGHz", cpu.BoostClockGHz);
                command.Parameters.AddWithValue("@Price", cpu.Price > 0 ? (object)cpu.Price : DBNull.Value);
                command.Parameters.AddWithValue("@PowerConsumption", cpu.PowerConsumption > 0 ? (object)cpu.PowerConsumption : DBNull.Value);
                
                command.ExecuteNonQuery();
            }
        }
        
        private void SaveChangesToDatabase(PC_Configurator.Models.CPU cpu)
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
            {
                conn.Open();
                var command = new System.Data.SqlClient.SqlCommand(
                    "UPDATE CPUs SET Name = @Name, Manufacturer = @Manufacturer, Socket = @Socket, " +
                    "Cores = @Cores, Threads = @Threads, BaseClockGHz = @BaseClockGHz, " +
                    "BoostClockGHz = @BoostClockGHz, Price = @Price, PowerConsumption = @PowerConsumption " +
                    "WHERE Id = @Id", conn);
                
                command.Parameters.AddWithValue("@Id", cpu.Id);
                command.Parameters.AddWithValue("@Name", cpu.Name);
                command.Parameters.AddWithValue("@Manufacturer", cpu.Manufacturer);
                command.Parameters.AddWithValue("@Socket", string.IsNullOrEmpty(cpu.Socket) ? DBNull.Value : (object)cpu.Socket);
                command.Parameters.AddWithValue("@Cores", cpu.Cores);
                command.Parameters.AddWithValue("@Threads", cpu.Threads);
                command.Parameters.AddWithValue("@BaseClockGHz", cpu.BaseClockGHz);
                command.Parameters.AddWithValue("@BoostClockGHz", cpu.BoostClockGHz);
                command.Parameters.AddWithValue("@Price", cpu.Price > 0 ? (object)cpu.Price : DBNull.Value);
                command.Parameters.AddWithValue("@PowerConsumption", cpu.PowerConsumption > 0 ? (object)cpu.PowerConsumption : DBNull.Value);
                
                command.ExecuteNonQuery();
            }
        }
        
        private void ResetForm()
        {
            // Űrlap törlése
            NameTextBox.Text = string.Empty;
            ManufacturerComboBox.SelectedIndex = 0;
            SocketTextBox.Text = string.Empty;
            CoresComboBox.SelectedIndex = 0;
            ThreadsComboBox.SelectedIndex = 0;
            BaseClockTextBox.Text = string.Empty;
            BoostClockTextBox.Text = string.Empty;
            PriceTextBox.Text = string.Empty;
            PowerConsumptionTextBox.Text = string.Empty;
            
            // Error feliratok elrejtése
            NameErrorBlock.Visibility = Visibility.Collapsed;
            ManufacturerErrorBlock.Visibility = Visibility.Collapsed;
            CoresErrorBlock.Visibility = Visibility.Collapsed;
            ThreadsErrorBlock.Visibility = Visibility.Collapsed;
            BaseClockErrorBlock.Visibility = Visibility.Collapsed;
            BoostClockErrorBlock.Visibility = Visibility.Collapsed;
        }

        // Meglévő CPU adatainak betöltése szerkesztéshez
        public void LoadForEdit(int cpuId)
        {
            try
            {
                // A cím és a gombszöveg módosítása
                FormTitle.Visibility = Visibility.Visible;
                FormTitle.Text = "CPU szerkesztése";
                SaveButton.Content = "Módosítások mentése";
                
                // Az azonosító tárolása a későbbi mentéshez
                this.Tag = cpuId;
                
                try
                {
                    // CPU adatainak lekérése az adatbázisból
                    string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                    using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                    {
                        conn.Open();
                        var command = new System.Data.SqlClient.SqlCommand(
                            "SELECT * FROM CPUs WHERE Id = @Id", conn);
                        command.Parameters.AddWithValue("@Id", cpuId);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Az adatok betöltése a form mezőibe
                                NameTextBox.Text = reader["Name"].ToString();
                                
                                // Gyártó kiválasztása
                                string manufacturer = reader["Manufacturer"].ToString();
                                SelectComboBoxItemByContent(ManufacturerComboBox, manufacturer);
                                
                                // Magok száma
                                int cores = Convert.ToInt32(reader["Cores"]);
                                SelectComboBoxItemByContent(CoresComboBox, cores.ToString());
                                
                                // Szálak száma
                                int threads = Convert.ToInt32(reader["Threads"]);
                                SelectComboBoxItemByContent(ThreadsComboBox, threads.ToString());
                                
                                // Órajelek
                                BaseClockTextBox.Text = reader["BaseClockGHz"].ToString().Replace(',', '.');
                                BoostClockTextBox.Text = reader["BoostClockGHz"].ToString().Replace(',', '.');
                                
                                // Socket (ha van)
                                if (reader["Socket"] != DBNull.Value)
                                {
                                    SocketTextBox.Text = reader["Socket"].ToString();
                                }
                                
                                // Ár (ha van)
                                if (reader["Price"] != DBNull.Value)
                                {
                                    PriceTextBox.Text = reader["Price"].ToString();
                                }
                                
                                // PowerConsumption (ha van)
                                if (reader["PowerConsumption"] != DBNull.Value)
                                {
                                    PowerConsumptionTextBox.Text = reader["PowerConsumption"].ToString();
                                }
                            }
                            else
                            {
                                MessageBox.Show($"A megadott azonosítójú ({cpuId}) CPU nem található.", 
                                    "Betöltési hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                                
                                // Az ablak bezárása, amely tartalmazza ezt a UserControl-t
                                Window.GetWindow(this)?.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a CPU adatok betöltése során: {ex.Message}", 
                        "Adatbetöltési hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Váratlan hiba: {ex.Message}", 
                    "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        // Segédfüggvény a ComboBox elem kiválasztásához a szöveg alapján
        private void SelectComboBoxItemByContent(ComboBox comboBox, string content)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Content.ToString() == content)
                {
                    comboBox.SelectedItem = item;
                    return;
                }
            }
            
            // Ha nem találtuk meg, akkor az első elemet választjuk ki
            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }

        private bool ValidateName()
        {
            return ValidationHelper.ValidateTextBox(
                NameTextBox,
                value => ValidationHelper.Required(value, "Név"),
                NameErrorBlock
            );
        }

        private bool ValidateManufacturer()
        {
            return ValidationHelper.ValidateComboBox(
                ManufacturerComboBox,
                selectedItem => ValidationHelper.ComboBoxItemSelected(selectedItem, "Gyártó"),
                ManufacturerErrorBlock
            );
        }

        private bool ValidateCores()
        {
            return ValidationHelper.ValidateComboBox(
                CoresComboBox,
                selectedItem => ValidationHelper.ComboBoxItemSelected(selectedItem, "Magok száma"),
                CoresErrorBlock
            );
        }

        private bool ValidateThreads()
        {
            return ValidationHelper.ValidateComboBox(
                ThreadsComboBox,
                selectedItem => ValidationHelper.ComboBoxItemSelected(selectedItem, "Szálak száma"),
                ThreadsErrorBlock
            );
        }

        private bool ValidateBaseClock()
        {
            return ValidationHelper.ValidateTextBox(
                BaseClockTextBox,
                value =>
                {
                    var requiredResult = ValidationHelper.Required(value, "Alap órajel");
                    if (!requiredResult.IsValid)
                        return requiredResult;

                    return ValidationHelper.Number(value, "Alap órajel", 0.1, 8.0);
                },
                BaseClockErrorBlock
            );
        }

        private bool ValidateBoostClock()
        {
            return ValidationHelper.ValidateTextBox(
                BoostClockTextBox,
                value =>
                {
                    var requiredResult = ValidationHelper.Required(value, "Max órajel");
                    if (!requiredResult.IsValid)
                        return requiredResult;

                    return ValidationHelper.Number(value, "Max órajel", 0.1, 8.0);
                },
                BoostClockErrorBlock
            );
        }
        
        private bool ValidatePrice()
        {
            if (string.IsNullOrEmpty(PriceTextBox.Text))
            {
                ValidationHelper.ShowError(PriceErrorBlock, "Az ár megadása kötelező.");
                return false;
            }
            
            if (!decimal.TryParse(PriceTextBox.Text.Replace(',', '.'), 
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, 
                out decimal price) || price < 0)
            {
                ValidationHelper.ShowError(PriceErrorBlock, "Az ár csak pozitív szám lehet.");
                return false;
            }
            
            ValidationHelper.ClearErrors(PriceErrorBlock);
            return true;
        }
        
        private bool ValidatePowerConsumption()
        {
            if (string.IsNullOrEmpty(PowerConsumptionTextBox.Text))
            {
                // A fogyasztás opcionális, így üres értékként is elfogadjuk
                ValidationHelper.ClearErrors(PowerConsumptionErrorBlock);
                return true;
            }
            
            if (!int.TryParse(PowerConsumptionTextBox.Text, out int value) || value < 0)
            {
                ValidationHelper.ShowError(PowerConsumptionErrorBlock, "A fogyasztás csak pozitív egész szám lehet.");
                return false;
            }
            
            ValidationHelper.ClearErrors(PowerConsumptionErrorBlock);
            return true;
        }
    }
}
