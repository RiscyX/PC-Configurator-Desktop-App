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
    /// Interaction logic for RAM.xaml
    /// </summary>
    public partial class RAM : UserControl
    {
        public RAM()
        {
            InitializeComponent();
            SaveButton.Click += SaveButton_Click;
            
            // Validációs események beállítása
            NameTextBox.LostFocus += (s, e) => ValidateName();
            NameTextBox.TextChanged += (s, e) => ValidateName();
            
            CapacityGBComboBox.SelectionChanged += (s, e) => ValidateCapacity();
            
            SpeedMHzTextBox.LostFocus += (s, e) => ValidateSpeed();
            SpeedMHzTextBox.TextChanged += (s, e) => ValidateSpeed();
            
            TypeComboBox.SelectionChanged += (s, e) => ValidateType();

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
                // Minden mező validálása
                bool isNameValid = ValidateName();
                bool isCapacityValid = ValidateCapacity();
                bool isSpeedValid = ValidateSpeed();
                bool isTypeValid = ValidateType();
                bool isPriceValid = ValidatePrice();
                bool isPowerConsumptionValid = ValidatePowerConsumption();

                // Teljes űrlap validáció
                if (!ValidationHelper.ValidateForm(
                    isNameValid, isCapacityValid, isSpeedValid, isTypeValid, isPriceValid, isPowerConsumptionValid))
                {
                    return;
                }
                
                // Ha minden rendben, kiolvassuk az adatokat
                string name = NameTextBox.Text.Trim();
                int capacityGB = int.Parse((CapacityGBComboBox.SelectedItem as ComboBoxItem).Content.ToString());
                int speedMHz = int.Parse(SpeedMHzTextBox.Text.Trim());
                string type = (TypeComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                
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
                
                var model = new PC_Configurator.Models.RAM { 
                    Name = name, 
                    CapacityGB = capacityGB, 
                    SpeedMHz = speedMHz, 
                    Type = type,
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
                    MessageBox.Show("RAM sikeresen frissítve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Az ablak bezárása, ha szerkesztési módban vagyunk
                    Window.GetWindow(this)?.Close();
                }
                else
                {
                    // Új hozzáadás esetén nincs ID
                    SaveNewToDatabase(model);
                    MessageBox.Show("RAM sikeresen mentve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Űrlap törlése
                    ResetForm();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "RAM mentése");
            }
        }

        private void SaveNewToDatabase(PC_Configurator.Models.RAM ram)
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
            {
                conn.Open();
                var command = new System.Data.SqlClient.SqlCommand(
                    "INSERT INTO RAMs (Name, CapacityGB, SpeedMHz, Type, Price, PowerConsumption) " +
                    "VALUES (@Name, @CapacityGB, @SpeedMHz, @Type, @Price, @PowerConsumption)", conn);
                
                command.Parameters.AddWithValue("@Name", ram.Name);
                command.Parameters.AddWithValue("@CapacityGB", ram.CapacityGB);
                command.Parameters.AddWithValue("@SpeedMHz", ram.SpeedMHz);
                command.Parameters.AddWithValue("@Type", ram.Type);
                command.Parameters.AddWithValue("@Price", ram.Price > 0 ? (object)ram.Price : DBNull.Value);
                command.Parameters.AddWithValue("@PowerConsumption", ram.PowerConsumption > 0 ? (object)ram.PowerConsumption : DBNull.Value);
                
                command.ExecuteNonQuery();
            }
        }
        
        private void SaveChangesToDatabase(PC_Configurator.Models.RAM ram)
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
            {
                conn.Open();
                var command = new System.Data.SqlClient.SqlCommand(
                    "UPDATE RAMs SET Name = @Name, CapacityGB = @CapacityGB, " +
                    "SpeedMHz = @SpeedMHz, Type = @Type, Price = @Price, " +
                    "PowerConsumption = @PowerConsumption " +
                    "WHERE Id = @Id", conn);
                
                command.Parameters.AddWithValue("@Id", ram.Id);
                command.Parameters.AddWithValue("@Name", ram.Name);
                command.Parameters.AddWithValue("@CapacityGB", ram.CapacityGB);
                command.Parameters.AddWithValue("@SpeedMHz", ram.SpeedMHz);
                command.Parameters.AddWithValue("@Type", ram.Type);
                command.Parameters.AddWithValue("@Price", ram.Price > 0 ? (object)ram.Price : DBNull.Value);
                command.Parameters.AddWithValue("@PowerConsumption", ram.PowerConsumption > 0 ? (object)ram.PowerConsumption : DBNull.Value);
                
                command.ExecuteNonQuery();
            }
        }
        
        private void ResetForm()
        {
            // Űrlap törlése
            NameTextBox.Text = string.Empty;
            CapacityGBComboBox.SelectedIndex = 0;
            SpeedMHzTextBox.Text = string.Empty;
            TypeComboBox.SelectedIndex = 0;
            if (PriceTextBox != null)
                PriceTextBox.Text = string.Empty;
            if (PowerConsumptionTextBox != null)
                PowerConsumptionTextBox.Text = string.Empty;
            
            // Hibaüzenetek elrejtése
            NameErrorBlock.Visibility = Visibility.Collapsed;
            CapacityGBErrorBlock.Visibility = Visibility.Collapsed;
            SpeedMHzErrorBlock.Visibility = Visibility.Collapsed;
            TypeErrorBlock.Visibility = Visibility.Collapsed;
            PriceErrorBlock.Visibility = Visibility.Collapsed;
            PowerConsumptionErrorBlock.Visibility = Visibility.Collapsed;
        }

        // Meglévő RAM adatainak betöltése szerkesztéshez
        public void LoadForEdit(int ramId)
        {
            try
            {
                // A cím és a gombszöveg módosítása
                FormTitle.Visibility = Visibility.Visible;
                FormTitle.Text = "RAM szerkesztése";
                SaveButton.Content = "Módosítások mentése";
                
                // Az azonosító tárolása a későbbi mentéshez
                this.Tag = ramId;
                
                try
                {
                    // RAM adatainak lekérése az adatbázisból
                    string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                    using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                    {
                        conn.Open();
                        var command = new System.Data.SqlClient.SqlCommand(
                            "SELECT * FROM RAMs WHERE Id = @Id", conn);
                        command.Parameters.AddWithValue("@Id", ramId);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Az adatok betöltése a form mezőibe
                                NameTextBox.Text = reader["Name"].ToString();
                                
                                // Kapacitás kiválasztása
                                int capacityGB = Convert.ToInt32(reader["CapacityGB"]);
                                SelectComboBoxItemByContent(CapacityGBComboBox, capacityGB.ToString());
                                
                                // Sebesség
                                SpeedMHzTextBox.Text = reader["SpeedMHz"].ToString();
                                
                                // Típus 
                                string type = reader["Type"].ToString();
                                SelectComboBoxItemByContent(TypeComboBox, type);
                                
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
                                MessageBox.Show($"A megadott azonosítójú ({ramId}) RAM nem található.", 
                                    "Betöltési hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                                
                                // Az ablak bezárása, amely tartalmazza ezt a UserControl-t
                                Window.GetWindow(this)?.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a RAM adatok betöltése során: {ex.Message}", 
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

        private bool ValidateCapacity()
        {
            return ValidationHelper.ValidateComboBox(
                CapacityGBComboBox,
                selectedItem => ValidationHelper.ComboBoxItemSelected(selectedItem, "Kapacitás"),
                CapacityGBErrorBlock
            );
        }

        private bool ValidateSpeed()
        {
            return ValidationHelper.ValidateTextBox(
                SpeedMHzTextBox,
                value =>
                {
                    var requiredResult = ValidationHelper.Required(value, "Órajel");
                    if (!requiredResult.IsValid)
                        return requiredResult;

                    return ValidationHelper.Integer(value, "Órajel", 800, 8000);
                },
                SpeedMHzErrorBlock
            );
        }

        private bool ValidateType()
        {
            return ValidationHelper.ValidateComboBox(
                TypeComboBox,
                selectedItem => ValidationHelper.ComboBoxItemSelected(selectedItem, "Típus"),
                TypeErrorBlock
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
