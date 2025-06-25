using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Partico_Delivery.Pages
{
    public class PrevPageEnabledConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int page)
                return page > 1;
            return false;
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class NextPageEnabledConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int page && parameter is int pageCount)
                return page < pageCount;
            return false;
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class CurrentPageToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int pageNumber && parameter is int currentPage)
                return pageNumber == currentPage ? Colors.LightBlue : Colors.Transparent;
            return Colors.Transparent;
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
