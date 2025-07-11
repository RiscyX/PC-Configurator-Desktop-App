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
using PC_Configurator.Helpers; // ValidationHelper és ErrorHandler használatához

namespace PC_Configurator.Views.Forms
{
    /// <summary>
    /// Interaction logic for Case.xaml
    /// </summary>
    public partial class Case : UserControl
    {
        // Esemény a sikeres mentés jelzésére
        public event EventHandler SaveCompleted;

        private int? currentId = null;
        
        public Case()
        {
            InitializeComponent();
            SaveButton.Click += SaveButton_Click;
            
            // Validációs események beállítása
            NameTextBox.LostFocus += (s, e) => ValidateName();
            NameTextBox.TextChanged += (s, e) => ValidateName();
            
            PriceTextBox.LostFocus += (s, e) => ValidatePrice();
            PriceTextBox.TextChanged += (s, e) => ValidatePrice();
            
            FormFactorComboBox.SelectionChanged += (s, e) => ValidateFormFactor();
            ColorComboBox.SelectionChanged += (s, e) => ValidateColor();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Adatok validálása ValidationHelper segítségével
            bool isNameValid = ValidateName();
            bool isFormFactorValid = ValidateFormFactor();
            bool isColorValid = ValidateColor();
            bool isPriceValid = ValidatePrice();

            // Ha bármelyik validáció hibás, nem folytatjuk
            if (!ValidationHelper.ValidateForm(isNameValid, isFormFactorValid, isColorValid, isPriceValid))
                return;

            // Adatok kiolvasása
            string name = NameTextBox.Text.Trim();
            string formFactor = (FormFactorComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string color = (ColorComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            decimal price = 0;
            decimal.TryParse(PriceTextBox.Text, out price);

            // Modell létrehozása és mentése
            var model = new PC_Configurator.Models.Case { 
                Name = name, 
                FormFactor = formFactor, 
                Color = color,
                Price = price
            };
            
            try
            {
                // Új gépház mentése
                if (currentId == null)
                {
                    model.SaveToDatabase();
                    MessageBox.Show("Gépház sikeresen mentve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                // Meglévő gépház frissítése
                else
                {
                    model.Id = currentId.Value;
                    model.UpdateInDatabase();
                    MessageBox.Show("Gépház sikeresen frissítve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Sikeres mentés után kiváltjuk az eseményt
                    SaveCompleted?.Invoke(this, EventArgs.Empty);
                }
                
                // Mezők törlése
                NameTextBox.Text = string.Empty;
                FormFactorComboBox.SelectedIndex = 0;
                ColorComboBox.SelectedIndex = 0;
                PriceTextBox.Text = string.Empty;
                
                // Hibaüzenetek elrejtése
                ValidationHelper.ClearErrors(NameError, FormFactorError, ColorError, PriceError);
                
                // Űrlap címének visszaállítása
                FormTitle.Text = "Gépház hozzáadása";
                currentId = null;

                // Esemény kiváltása
                SaveCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleDatabaseError(ex, currentId == null ? "mentés" : "frissítés", "gépház");
            }
        }
        
        /// <summary>
        /// Betölti a megadott ID-val rendelkező gépház adatait szerkesztéshez
        /// </summary>
        public void LoadForEdit(PC_Configurator.Models.Case caseModel)
        {
            if (caseModel == null)
                return;
                
            // Az aktuális ID tárolása a későbbi mentéshez
            currentId = caseModel.Id;
            
            // Form címének módosítása
            FormTitle.Text = "Gépház szerkesztése";
            
            // Adatok betöltése
            NameTextBox.Text = caseModel.Name;
            PriceTextBox.Text = caseModel.Price.ToString();
            
            // Formátum beállítása
            for (int i = 0; i < FormFactorComboBox.Items.Count; i++)
            {
                var item = FormFactorComboBox.Items[i] as ComboBoxItem;
                if (item != null && item.Content.ToString() == caseModel.FormFactor)
                {
                    FormFactorComboBox.SelectedIndex = i;
                    break;
                }
            }
            
            // Szín beállítása, ha létezik
            if (!string.IsNullOrEmpty(caseModel.Color))
            {
                string color = caseModel.Color;
                // Magyar-angol konverzió
                if (color.Equals("Black", StringComparison.OrdinalIgnoreCase)) color = "Fekete";
                else if (color.Equals("White", StringComparison.OrdinalIgnoreCase)) color = "Fehér";
                else if (color.Equals("Silver", StringComparison.OrdinalIgnoreCase)) color = "Ezüst";
                else if (color.Equals("Blue", StringComparison.OrdinalIgnoreCase)) color = "Kék";
                else if (color.Equals("Red", StringComparison.OrdinalIgnoreCase)) color = "Piros";
                
                for (int i = 0; i < ColorComboBox.Items.Count; i++)
                {
                    var item = ColorComboBox.Items[i] as ComboBoxItem;
                    if (item != null && item.Content.ToString() == color)
                    {
                        ColorComboBox.SelectedIndex = i;
                        break;
                    }
                }
            }
        }
        
        private bool ValidateName()
        {
            return ValidationHelper.ValidateTextBox(
                NameTextBox, 
                value => ValidationHelper.Required(value, "Név"),
                NameError
            );
        }
        
        private bool ValidateFormFactor()
        {
            return ValidationHelper.ValidateComboBox(
                FormFactorComboBox,
                selectedItem => ValidationHelper.ComboBoxItemSelected(selectedItem, "Formátum"),
                FormFactorError
            );
        }
        
        private bool ValidateColor()
        {
            return ValidationHelper.ValidateComboBox(
                ColorComboBox,
                selectedItem => ValidationHelper.ComboBoxItemSelected(selectedItem, "Szín"),
                ColorError
            );
        }
        
        private bool ValidatePrice()
        {
            return ValidationHelper.ValidateTextBox(
                PriceTextBox,
                value => ValidationHelper.ValidateDecimal(value, "Ár", 0),
                PriceError
            );
        }
    }
}
