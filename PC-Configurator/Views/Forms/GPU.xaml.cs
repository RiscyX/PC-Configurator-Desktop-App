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
    /// Interaction logic for GPU.xaml
    /// </summary>
    public partial class GPU : UserControl
    {
        public GPU()
        {
            InitializeComponent();
            SaveButton.Click += SaveButton_Click;
            
            // Validációs események beállítása
            NameTextBox.LostFocus += (s, e) => ValidateName();
            NameTextBox.TextChanged += (s, e) => ValidateName();
            
            ManufacturerComboBox.SelectionChanged += (s, e) => ValidateManufacturer();
            MemoryGBComboBox.SelectionChanged += (s, e) => ValidateMemory();
            
            if (PriceTextBox != null)
            {
                PriceTextBox.LostFocus += (s, e) => ValidatePrice();
                PriceTextBox.TextChanged += (s, e) => ValidatePrice();
            }

            if (PowerConsumptionTextBox != null)
            {
                PowerConsumptionTextBox.LostFocus += (s, e) => ValidatePowerConsumption();
                PowerConsumptionTextBox.TextChanged += (s, e) => ValidatePowerConsumption();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                // Minden mező validálása és az eredmények összegyűjtése
                bool isNameValid = ValidateName();
                bool isManufacturerValid = ValidateManufacturer();
                bool isMemoryValid = ValidateMemory();
                bool isPriceValid = ValidatePrice();
                bool isPowerConsumptionValid = ValidatePowerConsumption();

                // Teljes űrlap validáció
                if (!ValidationHelper.ValidateForm(isNameValid, isManufacturerValid, isMemoryValid, isPriceValid, isPowerConsumptionValid))
                {
                    return;
                }

                // Ha minden mező valid, akkor kiolvassuk az értékeket
                string name = NameTextBox.Text.Trim();
                string manufacturer = (ManufacturerComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                int memoryGB = int.Parse((MemoryGBComboBox.SelectedItem as ComboBoxItem).Content.ToString());
                
                // Az ár kiolvasása, ha van
                decimal price = 0;
                if (!string.IsNullOrEmpty(PriceTextBox?.Text))
                {
                    decimal.TryParse(
                        PriceTextBox.Text.Replace(',', '.'),
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out price
                    );
                }

                // A fogyasztás kiolvasása, ha van
                int powerConsumption = 0;
                if (!string.IsNullOrEmpty(PowerConsumptionTextBox?.Text))
                {
                    int.TryParse(PowerConsumptionTextBox.Text, out powerConsumption);
                }

                // Az objektum létrehozása
                var model = new PC_Configurator.Models.GPU { 
                    Name = name, 
                    Manufacturer = manufacturer, 
                    MemoryGB = memoryGB,
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
                    MessageBox.Show("Videokártya sikeresen frissítve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Az ablak bezárása, ha szerkesztési módban vagyunk
                    Window.GetWindow(this)?.Close();
                }
                else
                {
                    // Új hozzáadás esetén nincs ID
                    SaveNewToDatabase(model);
                    MessageBox.Show("Videokártya sikeresen mentve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Űrlap törlése
                    ResetForm();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Videokártya mentése");
            }
        }

        private void SaveNewToDatabase(PC_Configurator.Models.GPU gpu)
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
            {
                conn.Open();
                var command = new System.Data.SqlClient.SqlCommand(
                    "INSERT INTO GPUs (Name, Manufacturer, MemoryGB, Price, PowerConsumption) " +
                    "VALUES (@Name, @Manufacturer, @MemoryGB, @Price, @PowerConsumption)", conn);
                
                command.Parameters.AddWithValue("@Name", gpu.Name);
                command.Parameters.AddWithValue("@Manufacturer", gpu.Manufacturer);
                command.Parameters.AddWithValue("@MemoryGB", gpu.MemoryGB);
                command.Parameters.AddWithValue("@Price", gpu.Price > 0 ? (object)gpu.Price : DBNull.Value);
                command.Parameters.AddWithValue("@PowerConsumption", gpu.PowerConsumption > 0 ? (object)gpu.PowerConsumption : DBNull.Value);
                
                command.ExecuteNonQuery();
            }
        }
        
        private void SaveChangesToDatabase(PC_Configurator.Models.GPU gpu)
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
            {
                conn.Open();
                var command = new System.Data.SqlClient.SqlCommand(
                    "UPDATE GPUs SET Name = @Name, Manufacturer = @Manufacturer, " +
                    "MemoryGB = @MemoryGB, Price = @Price, PowerConsumption = @PowerConsumption " +
                    "WHERE Id = @Id", conn);
                
                command.Parameters.AddWithValue("@Id", gpu.Id);
                command.Parameters.AddWithValue("@Name", gpu.Name);
                command.Parameters.AddWithValue("@Manufacturer", gpu.Manufacturer);
                command.Parameters.AddWithValue("@MemoryGB", gpu.MemoryGB);
                command.Parameters.AddWithValue("@Price", gpu.Price > 0 ? (object)gpu.Price : DBNull.Value);
                command.Parameters.AddWithValue("@PowerConsumption", gpu.PowerConsumption > 0 ? (object)gpu.PowerConsumption : DBNull.Value);
                
                command.ExecuteNonQuery();
            }
        }
        
        private void ResetForm()
        {
            // Űrlap törlése
            NameTextBox.Text = string.Empty;
            ManufacturerComboBox.SelectedIndex = 0;
            MemoryGBComboBox.SelectedIndex = 0;
            if (PriceTextBox != null)
                PriceTextBox.Text = string.Empty;
            if (PowerConsumptionTextBox != null)
                PowerConsumptionTextBox.Text = string.Empty;
            
            // Error feliratok elrejtése
            NameErrorBlock.Visibility = Visibility.Collapsed;
            ManufacturerErrorBlock.Visibility = Visibility.Collapsed;
            MemoryGBErrorBlock.Visibility = Visibility.Collapsed;
            PriceErrorBlock.Visibility = Visibility.Collapsed;
            PowerConsumptionErrorBlock.Visibility = Visibility.Collapsed;
        }

        // Meglévő GPU adatainak betöltése szerkesztéshez
        public void LoadForEdit(int gpuId)
        {
            try
            {
                // A cím és a gombszöveg módosítása
                FormTitle.Visibility = Visibility.Visible;
                FormTitle.Text = "Videokártya szerkesztése";
                SaveButton.Content = "Módosítások mentése";
                
                // Az azonosító tárolása a későbbi mentéshez
                this.Tag = gpuId;
                
                try
                {
                    // GPU adatainak lekérése az adatbázisból
                    string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                    using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                    {
                        conn.Open();
                        var command = new System.Data.SqlClient.SqlCommand(
                            "SELECT * FROM GPUs WHERE Id = @Id", conn);
                        command.Parameters.AddWithValue("@Id", gpuId);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Az adatok betöltése a form mezőibe
                                NameTextBox.Text = reader["Name"].ToString();
                                
                                // Gyártó kiválasztása
                                string manufacturer = reader["Manufacturer"].ToString();
                                SelectComboBoxItemByContent(ManufacturerComboBox, manufacturer);
                                
                                // Memória méret
                                int memoryGB = Convert.ToInt32(reader["MemoryGB"]);
                                SelectComboBoxItemByContent(MemoryGBComboBox, memoryGB.ToString());
                                
                                // Ár (ha van)
                                if (reader["Price"] != DBNull.Value && PriceTextBox != null)
                                {
                                    PriceTextBox.Text = reader["Price"].ToString();
                                }

                                // Fogyasztás (ha van)
                                if (reader["PowerConsumption"] != DBNull.Value && PowerConsumptionTextBox != null)
                                {
                                    PowerConsumptionTextBox.Text = reader["PowerConsumption"].ToString();
                                }
                            }
                            else
                            {
                                MessageBox.Show($"A megadott azonosítójú ({gpuId}) videokártya nem található.", 
                                    "Betöltési hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                                
                                // Az ablak bezárása, amely tartalmazza ezt a UserControl-t
                                Window.GetWindow(this)?.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a videokártya adatok betöltése során: {ex.Message}", 
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

        private bool ValidateMemory()
        {
            return ValidationHelper.ValidateComboBox(
                MemoryGBComboBox,
                selectedItem => ValidationHelper.ComboBoxItemSelected(selectedItem, "Memória méret"),
                MemoryGBErrorBlock
            );
        }

        private bool ValidatePrice()
        {
            if (PriceTextBox == null || string.IsNullOrWhiteSpace(PriceTextBox.Text))
                return true;

            return ValidationHelper.ValidateTextBox(
                PriceTextBox,
                value => ValidationHelper.ValidateDecimalField(value, "Ár", true),
                PriceErrorBlock
            );
        }

        private bool ValidatePowerConsumption()
        {
            if (PowerConsumptionTextBox == null || string.IsNullOrWhiteSpace(PowerConsumptionTextBox.Text))
                return true;

            return ValidationHelper.ValidateTextBox(
                PowerConsumptionTextBox,
                value => ValidationHelper.ValidateIntegerField(value, "Fogyasztás", true),
                PowerConsumptionErrorBlock
            );
        }
    }
}
