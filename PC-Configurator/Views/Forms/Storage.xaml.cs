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
using System.Data.SqlClient;
using PC_Configurator.Helpers; // ValidationHelper és ErrorHandler használatához
using PC_Configurator.Models;

namespace PC_Configurator.Views.Forms
{
    /// <summary>
    /// Interaction logic for Storage.xaml
    /// </summary>
    public partial class Storage : UserControl
    {
        private int _editId = 0; // 0 = új elem, >0 = meglévő szerkesztése
        private bool _isEditMode = false;
        
        public Storage()
        {
            InitializeComponent();
            // Ensure event is not fired before ComboBox is ready
            Loaded += Storage_Loaded;
            SaveButton.Click += SaveButton_Click;

            // Ár validáció eseménykezelő
            if (PriceTextBox != null)
            {
                PriceTextBox.LostFocus += (s, e) => ValidatePrice();
                PriceTextBox.TextChanged += (s, e) => ValidatePrice();
            }

            // Fogyasztás validáció eseménykezelő
            if (PowerConsumptionTextBox != null)
            {
                PowerConsumptionTextBox.LostFocus += (s, e) => ValidatePowerConsumption();
                PowerConsumptionTextBox.TextChanged += (s, e) => ValidatePowerConsumption();
            }
        }
        
        // Meglévő meghajtó betöltése szerkesztésre
        public void LoadForEdit(int id)
        {
            try
            {
                _editId = id;
                _isEditMode = true;
                FormTitle.Text = "Meghajtó szerkesztése";
                
                // Adatok betöltése az adatbázisból
                string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string query = "SELECT * FROM Storages WHERE Id = @Id";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Az adatok betöltése a form mezőibe
                                NameTextBox.Text = reader["Name"].ToString();
                                
                                // Típus beállítása
                                string type = reader["Type"].ToString();
                                SetComboBoxItemByContent(TypeComboBox, type);
                                
                                // Kapacitás beállítása
                                UpdateCapacityOptions();
                                int capacityGB = Convert.ToInt32(reader["CapacityGB"]);
                                SetComboBoxItemByContent(CapacityGBComboBox, capacityGB.ToString());
                                
                                // Ár beállítása
                                if (reader["Price"] != DBNull.Value)
                                {
                                    PriceTextBox.Text = reader["Price"].ToString();
                                }

                                // Fogyasztás beállítása
                                if (reader["PowerConsumption"] != DBNull.Value)
                                {
                                    PowerConsumptionTextBox.Text = reader["PowerConsumption"].ToString();
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Nem található meghajtó a következő azonosítóval: {id}", 
                                                "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                                // Visszaállítás új mód állapotba
                                _editId = 0;
                                _isEditMode = false;
                                FormTitle.Text = "Meghajtó hozzáadása";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleDatabaseError(ex, "betöltés", "meghajtó");
            }
        }
        
        // ComboBox beállítása a megadott tartalom alapján
        private void SetComboBoxItemByContent(ComboBox comboBox, string content)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Content.ToString() == content)
                {
                    comboBox.SelectedItem = item;
                    return;
                }
            }
            // Ha nem találta, akkor az első elemet választjuk
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Adatok validálása ValidationHelper segítségével
            bool isValid = true;

            // Név validálása
            isValid &= ValidationHelper.ValidateRequired(NameTextBox, NameError, "A meghajtó neve kötelező");

            // Típus validálása
            isValid &= ValidationHelper.ValidateComboBoxSelection(TypeComboBox, TypeError, "Válasszon típust");

            // Kapacitás validálása
            isValid &= ValidationHelper.ValidateComboBoxSelection(CapacityGBComboBox, CapacityError, "Válasszon kapacitást");
            
            // Ár validálása
            isValid &= ValidatePrice();

            // Fogyasztás validálása
            isValid &= ValidatePowerConsumption();

            // Ha bármelyik validáció hibás, nem folytatjuk
            if (!isValid)
                return;

            // Adatok kiolvasása
            string name = NameTextBox.Text.Trim();
            string type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string capacityStr = (CapacityGBComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            
            // Ár kiolvasása
            decimal price = 0;
            if (!string.IsNullOrWhiteSpace(PriceTextBox.Text))
            {
                decimal.TryParse(
                    PriceTextBox.Text.Replace(',', '.'),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out price
                );
            }

            // Fogyasztás kiolvasása
            int powerConsumption = 0;
            if (!string.IsNullOrWhiteSpace(PowerConsumptionTextBox.Text))
            {
                int.TryParse(PowerConsumptionTextBox.Text, out powerConsumption);
            }
            
            if (!int.TryParse(capacityStr, out int capacityGB))
            {
                ValidationHelper.ShowError(CapacityError, "Érvénytelen kapacitás érték");
                return;
            }

            try
            {
                // Modell létrehozása
                var model = new PC_Configurator.Models.Storage 
                { 
                    Id = _editId,
                    Name = name, 
                    Type = type, 
                    CapacityGB = capacityGB,
                    Price = price,
                    PowerConsumption = powerConsumption
                };
                
                // Mentés vagy frissítés az _isEditMode alapján
                if (_isEditMode)
                {
                    SaveChangesToDatabase(model);
                    MessageBox.Show("Meghajtó sikeresen frissítve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    SaveNewToDatabase(model);
                    MessageBox.Show("Meghajtó sikeresen hozzáadva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
                // Mezők törlése
                NameTextBox.Text = string.Empty;
                TypeComboBox.SelectedIndex = 0;
                UpdateCapacityOptions();
                PriceTextBox.Text = string.Empty;
                PowerConsumptionTextBox.Text = string.Empty;
                
                // Visszaállítás új állapotba
                _editId = 0;
                _isEditMode = false;
                FormTitle.Text = "Meghajtó hozzáadása";
                
                // Hibaüzenetek elrejtése
                ValidationHelper.ClearErrors(NameError, TypeError, CapacityError, PriceError, PowerConsumptionError);
            }
            catch (Exception ex)
            {
                string action = _isEditMode ? "frissítés" : "mentés";
                ErrorHandler.HandleDatabaseError(ex, action, "meghajtó");
            }
        }
        
        private void SaveNewToDatabase(PC_Configurator.Models.Storage model)
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "INSERT INTO Storages (Name, Type, CapacityGB, Price, PowerConsumption) " +
                             "VALUES (@Name, @Type, @CapacityGB, @Price, @PowerConsumption)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", model.Name);
                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@CapacityGB", model.CapacityGB);
                    cmd.Parameters.AddWithValue("@Price", model.Price > 0 ? (object)model.Price : DBNull.Value);
                    cmd.Parameters.AddWithValue("@PowerConsumption", model.PowerConsumption > 0 ? (object)model.PowerConsumption : DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        
        private void SaveChangesToDatabase(PC_Configurator.Models.Storage model)
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "UPDATE Storages SET Name = @Name, Type = @Type, CapacityGB = @CapacityGB, " + 
                             "Price = @Price, PowerConsumption = @PowerConsumption WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", model.Id);
                    cmd.Parameters.AddWithValue("@Name", model.Name);
                    cmd.Parameters.AddWithValue("@Type", model.Type);
                    cmd.Parameters.AddWithValue("@CapacityGB", model.CapacityGB);
                    cmd.Parameters.AddWithValue("@Price", model.Price > 0 ? (object)model.Price : DBNull.Value);
                    cmd.Parameters.AddWithValue("@PowerConsumption", model.PowerConsumption > 0 ? (object)model.PowerConsumption : DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void Storage_Loaded(object sender, RoutedEventArgs e)
        {
            if (TypeComboBox != null)
            {
                TypeComboBox.SelectedIndex = 0;
                UpdateCapacityOptions();
            }
        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeComboBox == null || CapacityGBComboBox == null)
                return;
            UpdateCapacityOptions();
        }        private void UpdateCapacityOptions()
        {
            if (CapacityGBComboBox == null || TypeComboBox == null)
                return;
            CapacityGBComboBox.Items.Clear();
            var selectedType = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            
            // Az értékek hozzáadása ComboBoxItem-ként, hogy be lehessen állítani a színeket
            List<string> capacities = new List<string>();
            if (selectedType == "SSD")
            {
                capacities.AddRange(new[] { "120", "240", "480", "500", "512", "1000", "1024", "2048", "4096" });
            }
            else
            {
                capacities.AddRange(new[] { "1024", "2048", "4096", "8192" });
            }
            
            foreach (var capacity in capacities)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = capacity,
                    Foreground = Brushes.White,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"))
                };
                CapacityGBComboBox.Items.Add(item);
            }
            
            if (CapacityGBComboBox.Items.Count > 0)
                CapacityGBComboBox.SelectedIndex = 0;
        }

        // Validációs segédmetódusok
        private bool ValidatePrice()
        {
            if (PriceTextBox == null || string.IsNullOrWhiteSpace(PriceTextBox.Text))
                return true;

            return ValidationHelper.ValidateTextBox(
                PriceTextBox,
                value => ValidationHelper.ValidateDecimalField(value, "Ár", true),
                PriceError
            );
        }

        private bool ValidatePowerConsumption()
        {
            if (PowerConsumptionTextBox == null || string.IsNullOrWhiteSpace(PowerConsumptionTextBox.Text))
                return true;

            return ValidationHelper.ValidateTextBox(
                PowerConsumptionTextBox,
                value => ValidationHelper.ValidateIntegerField(value, "Fogyasztás", true),
                PowerConsumptionError
            );
        }
    }
}
