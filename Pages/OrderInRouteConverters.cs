using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Partico_Delivery.Models;
using Partico_Delivery.ViewModels;

namespace Partico_Delivery.Pages
{
    // Geeft een andere style terug als de order in de route zit
    public class OrderInRouteToStyleConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Order order)
            {
                var resources = Application.Current?.Resources;
                return order.InRoute
                    ? resources?["SelectedOrderFrameStyle"] ?? resources?["DefaultOrderFrameStyle"]
                    : resources?["DefaultOrderFrameStyle"];
            }
            return null;
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    // Toont de + knop als de order niet in de route zit
    public class OrderInRouteToAddVisibleConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Order order)
                return !order.InRoute;
            return false;
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    // Toont de - knop als de order wél in de route zit
    public class OrderInRouteToRemoveVisibleConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Order order)
                return order.InRoute;
            return false;
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
