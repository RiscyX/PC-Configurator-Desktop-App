using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PC_Configurator.Helpers
{
    /// <summary>
    /// Segédosztály az űrlap validációkhoz
    /// </summary>
    public static class ValidationHelper
    {
        private static readonly SolidColorBrush ErrorBrush = new SolidColorBrush(Colors.Red);
        private static readonly SolidColorBrush DefaultBrush = new SolidColorBrush(Color.FromRgb(51, 51, 51)); // #333333
        private static readonly SolidColorBrush SuccessBrush = new SolidColorBrush(Color.FromRgb(0, 126, 51)); // #007E33

        /// <summary>
        /// Validációs eredmény típus
        /// </summary>
        public class ValidationResult
        {
            public bool IsValid { get; set; }
            public string ErrorMessage { get; set; }

            public ValidationResult(bool isValid, string errorMessage = null)
            {
                IsValid = isValid;
                ErrorMessage = errorMessage;
            }

            public static ValidationResult Valid => new ValidationResult(true);
            public static ValidationResult Invalid(string errorMessage) => new ValidationResult(false, errorMessage);
        }

        /// <summary>
        /// TextBox validáció
        /// </summary>
        /// <param name="textBox">A validálandó TextBox</param>
        /// <param name="validator">Validációs függvény, ami ValidationResult-ot ad vissza</param>
        /// <param name="errorTextBlock">Opcionális hibamegjelenítő TextBlock</param>
        /// <returns>True ha valid, false ha invalid</returns>
        public static bool ValidateTextBox(TextBox textBox, Func<string, ValidationResult> validator, TextBlock errorTextBlock = null)
        {
            string value = textBox.Text?.Trim();
            var result = validator(value);

            if (result.IsValid)
            {
                textBox.BorderBrush = SuccessBrush;
                if (errorTextBlock != null)
                {
                    errorTextBlock.Visibility = Visibility.Collapsed;
                }
                return true;
            }
            else
            {
                textBox.BorderBrush = ErrorBrush;
                if (errorTextBlock != null)
                {
                    errorTextBlock.Text = result.ErrorMessage;
                    errorTextBlock.Visibility = Visibility.Visible;
                }
                return false;
            }
        }

        /// <summary>
        /// ComboBox validáció
        /// </summary>
        /// <param name="comboBox">A validálandó ComboBox</param>
        /// <param name="validator">Validációs függvény, ami ValidationResult-ot ad vissza</param>
        /// <param name="errorTextBlock">Opcionális hibamegjelenítő TextBlock</param>
        /// <returns>True ha valid, false ha invalid</returns>
        public static bool ValidateComboBox(ComboBox comboBox, Func<object, ValidationResult> validator, TextBlock errorTextBlock = null)
        {
            var selectedItem = comboBox.SelectedItem;
            var result = validator(selectedItem);

            if (result.IsValid)
            {
                comboBox.BorderBrush = SuccessBrush;
                if (errorTextBlock != null)
                {
                    errorTextBlock.Visibility = Visibility.Collapsed;
                }
                return true;
            }
            else
            {
                comboBox.BorderBrush = ErrorBrush;
                if (errorTextBlock != null)
                {
                    errorTextBlock.Text = result.ErrorMessage;
                    errorTextBlock.Visibility = Visibility.Visible;
                }
                return false;
            }
        }

        /// <summary>
        /// Űrlap validáció
        /// </summary>
        /// <param name="validationResults">Validációs eredmények listája</param>
        /// <returns>True ha minden eredmény valid, false ha bármelyik invalid</returns>
        public static bool ValidateForm(params bool[] validationResults)
        {
            foreach (bool result in validationResults)
            {
                if (!result)
                {
                    return false;
                }
            }
            return true;
        }

        #region Validátor függvények

        /// <summary>
        /// Kötelező mező validátor
        /// </summary>
        public static ValidationResult Required(string value, string fieldName)
        {
            if (string.IsNullOrEmpty(value))
            {
                return ValidationResult.Invalid($"A(z) {fieldName} mező kitöltése kötelező.");
            }
            return ValidationResult.Valid;
        }

        /// <summary>
        /// Szám validátor
        /// </summary>
        public static ValidationResult Number(string value, string fieldName, double? min = null, double? max = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                return ValidationResult.Valid; // Üres értéket más validátor ellenőriz
            }

            if (!double.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
            {
                return ValidationResult.Invalid($"A(z) {fieldName} mezőbe csak számot írhat.");
            }

            if (min.HasValue && number < min.Value)
            {
                return ValidationResult.Invalid($"A(z) {fieldName} értéke nem lehet kisebb, mint {min.Value}.");
            }

            if (max.HasValue && number > max.Value)
            {
                return ValidationResult.Invalid($"A(z) {fieldName} értéke nem lehet nagyobb, mint {max.Value}.");
            }

            return ValidationResult.Valid;
        }

        /// <summary>
        /// Egész szám validátor
        /// </summary>
        public static ValidationResult Integer(string value, string fieldName, int? min = null, int? max = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                return ValidationResult.Valid; // Üres értéket más validátor ellenőriz
            }

            if (!int.TryParse(value, out int number))
            {
                return ValidationResult.Invalid($"A(z) {fieldName} mezőbe csak egész számot írhat.");
            }

            if (min.HasValue && number < min.Value)
            {
                return ValidationResult.Invalid($"A(z) {fieldName} értéke nem lehet kisebb, mint {min.Value}.");
            }

            if (max.HasValue && number > max.Value)
            {
                return ValidationResult.Invalid($"A(z) {fieldName} értéke nem lehet nagyobb, mint {max.Value}.");
            }

            return ValidationResult.Valid;
        }

        /// <summary>
        /// ComboBox validátor
        /// </summary>
        public static ValidationResult ComboBoxItemSelected(object selectedItem, string fieldName)
        {
            if (selectedItem == null)
            {
                return ValidationResult.Invalid($"Kérem válasszon {fieldName} értéket!");
            }
            
            if (selectedItem is ComboBoxItem comboBoxItem && comboBoxItem.Content?.ToString() == "Válasszon...")
            {
                return ValidationResult.Invalid($"Kérem válasszon {fieldName} értéket!");
            }
            
            return ValidationResult.Valid;
        }

        /// <summary>
        /// Email formátum validátor
        /// </summary>
        public static ValidationResult Email(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return ValidationResult.Valid; // Üres értéket más validátor ellenőriz
            }

            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(value, pattern))
            {
                return ValidationResult.Invalid("Érvénytelen email formátum.");
            }

            return ValidationResult.Valid;
        }

        /// <summary>
        /// Decimális szám validátor
        /// </summary>
        public static ValidationResult ValidateDecimal(string value, string fieldName, decimal? min = null, decimal? max = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                return ValidationResult.Valid; // Üres értéket más validátor ellenőriz
            }

            if (!decimal.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal number))
            {
                return ValidationResult.Invalid($"A(z) {fieldName} mezőbe csak számot írhat.");
            }

            if (min.HasValue && number < min.Value)
            {
                return ValidationResult.Invalid($"A(z) {fieldName} értéke nem lehet kisebb, mint {min.Value}.");
            }

            if (max.HasValue && number > max.Value)
            {
                return ValidationResult.Invalid($"A(z) {fieldName} értéke nem lehet nagyobb, mint {max.Value}.");
            }

            return ValidationResult.Valid;
        }

        /// <summary>
        /// Kötelező mező validátor - rövidített változat
        /// </summary>
        public static bool ValidateRequired(TextBox textBox, TextBlock errorTextBlock, string fieldName)
        {
            return ValidateTextBox(textBox, value => Required(value, fieldName), errorTextBlock);
        }
        
        /// <summary>
        /// ComboBox kiválasztás validátor - rövidített változat
        /// </summary>
        public static bool ValidateComboBoxSelection(ComboBox comboBox, TextBlock errorTextBlock, string fieldName)
        {
            return ValidateComboBox(comboBox, selectedItem => ComboBoxItemSelected(selectedItem, fieldName), errorTextBlock);
        }

        public static ValidationResult ValidateDecimalField(string value, string fieldName, bool isOptional = false)
        {
            if (string.IsNullOrEmpty(value))
            {
                return isOptional ? ValidationResult.Valid : ValidationResult.Invalid($"A(z) {fieldName} mező kitöltése kötelező.");
            }

            if (!decimal.TryParse(value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal number))
            {
                return ValidationResult.Invalid($"A(z) {fieldName} mezőbe csak számot írhat.");
            }

            if (number < 0)
            {
                return ValidationResult.Invalid($"A(z) {fieldName} értéke nem lehet negatív.");
            }

            return ValidationResult.Valid;
        }

        public static ValidationResult ValidateIntegerField(string value, string fieldName, bool isOptional = false)
        {
            if (string.IsNullOrEmpty(value))
            {
                return isOptional ? ValidationResult.Valid : ValidationResult.Invalid($"A(z) {fieldName} mező kitöltése kötelező.");
            }

            if (!int.TryParse(value, out int number))
            {
                return ValidationResult.Invalid($"A(z) {fieldName} mezőbe csak egész számot írhat.");
            }

            if (number < 0)
            {
                return ValidationResult.Invalid($"A(z) {fieldName} értéke nem lehet negatív.");
            }

            return ValidationResult.Valid;
        }

        #endregion

        /// <summary>
        /// Hibaüzenet megjelenítése
        /// </summary>
        public static void ShowError(TextBlock errorTextBlock, string message)
        {
            if (errorTextBlock != null)
            {
                errorTextBlock.Text = message;
                errorTextBlock.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Hibaüzenetek törlése
        /// </summary>
        public static void ClearErrors(params TextBlock[] errorTextBlocks)
        {
            foreach (var errorTextBlock in errorTextBlocks)
            {
                if (errorTextBlock != null)
                {
                    errorTextBlock.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
