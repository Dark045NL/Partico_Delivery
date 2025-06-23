using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using System.Linq;

namespace Partico_Delivery.Pages
{
    public class PageCountConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int totalOrders && parameter is int pageSize && pageSize > 0)
            {
                int pageCount = (int)Math.Ceiling((double)totalOrders / pageSize);
                return Enumerable.Range(1, pageCount).ToList();
            }
            return Enumerable.Empty<int>().ToList();
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
