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
    /// Interaction logic for PSU.xaml
    /// </summary>
    public partial class PSU : UserControl
    {
        private int _editId = 0; // 0 = új elem, >0 = meglévő szerkesztése
        private bool _isEditMode = false;
        
        public PSU()
        {
            InitializeComponent();
            SaveButton.Click += SaveButton_Click;

            // Ár validáció eseménykezelő
            if (PriceTextBox != null)
            {
                PriceTextBox.LostFocus += (s, e) => ValidatePrice();
                PriceTextBox.TextChanged += (s, e) => ValidatePrice();
            }
        }
        
        // Meglévő tápegység betöltése szerkesztésre
        public void LoadForEdit(int id)
        {
            try
            {
                // Először töröljük az űrlap tartalmát
                NameTextBox.Text = string.Empty;
                WattageComboBox.SelectedIndex = 0;
                EfficiencyRatingComboBox.SelectedIndex = 0;
                PriceTextBox.Text = string.Empty;
                ValidationHelper.ClearErrors(NameError, WattageError, EfficiencyError, PriceError);

                // Adatok betöltése az adatbázisból
                string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string query = "SELECT * FROM PSUs WHERE Id = @Id";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Adatok kitöltése a form mezőibe
                                // HasColumn segédfüggvény használata az oszlopok ellenőrzéséhez
                                if (HasColumn(reader, "Name"))
                                {
                                    NameTextBox.Text = reader["Name"].ToString();
                                }
                                
                                // Teljesítmény beállítása
                                if (HasColumn(reader, "Wattage") && reader["Wattage"] != DBNull.Value)
                                {
                                    int wattage = Convert.ToInt32(reader["Wattage"]);
                                    SetComboBoxItemByContent(WattageComboBox, wattage.ToString());
                                }
                                
                                // Hatásfok beállítása
                                if (HasColumn(reader, "EfficiencyRating") && reader["EfficiencyRating"] != DBNull.Value)
                                {
                                    string efficiency = reader["EfficiencyRating"].ToString();
                                    SetComboBoxItemByContent(EfficiencyRatingComboBox, efficiency);
                                }
                                
                                // Ár beállítása
                                if (HasColumn(reader, "Price") && reader["Price"] != DBNull.Value)
                                {
                                    decimal price = Convert.ToDecimal(reader["Price"]);
                                    PriceTextBox.Text = price.ToString(System.Globalization.CultureInfo.InvariantCulture);
                                }

                                // Beállítjuk a szerkesztési módot
                                _editId = id;
                                _isEditMode = true;
                                FormTitle.Text = "Tápegység szerkesztése";
                            }
                            else
                            {
                                MessageBox.Show($"Nem található tápegység a következő azonosítóval: {id}", 
                                                "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                                // Visszaállítás új mód állapotba
                                _editId = 0;
                                _isEditMode = false;
                                FormTitle.Text = "Tápegység hozzáadása";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleDatabaseError(ex, "betöltés", "tápegység");
                // Hiba esetén visszaállítjuk új mód állapotba
                _editId = 0;
                _isEditMode = false;
                FormTitle.Text = "Tápegység hozzáadása";
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
        }

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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Adatok validálása ValidationHelper segítségével
                bool isValid = true;

                // Név validálása
                isValid &= ValidationHelper.ValidateRequired(NameTextBox, NameError, "A tápegység neve kötelező");

                // Teljesítmény validálása
                isValid &= ValidationHelper.ValidateComboBoxSelection(WattageComboBox, WattageError, "Válasszon teljesítményt");

                // Hatásfok validálása
                isValid &= ValidationHelper.ValidateComboBoxSelection(EfficiencyRatingComboBox, EfficiencyError, "Válasszon hatásfokot");
                
                // Ár validálása
                isValid &= ValidatePrice();

                // Ha bármelyik validáció hibás, nem folytatjuk
                if (!isValid)
                    return;

                // Adatok kiolvasása
                string name = NameTextBox.Text.Trim();
                string wattageStr = (WattageComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                string efficiency = (EfficiencyRatingComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                
                // Ár kiolvasása
                decimal price = 0;
                if (!string.IsNullOrWhiteSpace(PriceTextBox.Text))
                {
                    if (!decimal.TryParse(
                        PriceTextBox.Text.Replace(',', '.'),
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out price))
                    {
                        ValidationHelper.ShowError(PriceError, "Érvénytelen ár formátum");
                        return;
                    }
                }

                if (!int.TryParse(wattageStr, out int wattage))
                {
                    ValidationHelper.ShowError(WattageError, "Érvénytelen teljesítmény érték");
                    return;
                }

                // Modell létrehozása
                var model = new PC_Configurator.Models.PSU 
                { 
                    Name = name, 
                    Wattage = wattage, 
                    EfficiencyRating = efficiency,
                    Price = price
                };
                
                // Mentés vagy frissítés az _isEditMode alapján
                if (_isEditMode)
                {
                    model.Id = _editId;  // ID beállítása szerkesztés esetén
                    model.UpdateInDatabase();
                }
                else
                {
                    model.SaveToDatabase();
                }
                
                // Értesítjük a szülő ablakot a sikeres mentésről
                SaveCompleted?.Invoke(this, EventArgs.Empty);
                
                // Mezők törlése
                NameTextBox.Text = string.Empty;
                WattageComboBox.SelectedIndex = 0;
                EfficiencyRatingComboBox.SelectedIndex = 0;
                PriceTextBox.Text = string.Empty;
                
                // Visszaállítás új állapotba
                _editId = 0;
                _isEditMode = false;
                FormTitle.Text = "Tápegység hozzáadása";
                
                // Hibaüzenetek elrejtése
                ValidationHelper.ClearErrors(NameError, WattageError, EfficiencyError, PriceError);

                // Esemény kiváltása a sikeres mentés jelzésére
                SaveCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                string action = _isEditMode ? "frissítés" : "mentés";
                ErrorHandler.HandleDatabaseError(ex, action, "tápegység");
            }
        }
        
        private bool HasColumn(SqlDataReader reader, string columnName)
        {
            try
            {
                return reader.GetOrdinal(columnName) >= 0;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        // Esemény a sikeres mentés jelzéséhez
        public event EventHandler SaveCompleted;
    }
}
