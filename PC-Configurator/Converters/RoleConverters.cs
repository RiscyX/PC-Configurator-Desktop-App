using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PC_Configurator.Views.App
{
    /// <summary>
    /// Converter to change background color based on boolean value
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isAdmin && isAdmin)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007E33")); // Green for admin
            }
            else
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0078D4")); // Blue for regular user
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter to change button text based on admin role status
    /// </summary>
    public class AdminRoleTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isAdmin && isAdmin)
            {
                return "Visszavonás"; // Demote from admin
            }
            else
            {
                return "Admin jogok"; // Promote to admin
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
