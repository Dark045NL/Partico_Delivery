using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Partico_Delivery.Pages
{
    public class StatusToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int state)
            {
                return state switch
                {
                    1 => "In behandeling",
                    2 => "Onderweg",
                    3 => "Afgeleverd",
                    4 => "Niet thuis",
                    _ => "Onbekend"
                };
            }
            return "Onbekend";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

