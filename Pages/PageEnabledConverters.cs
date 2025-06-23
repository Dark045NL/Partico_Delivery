using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Partico_Delivery.Pages
{
    public class PreviousPageEnabledConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int page)
                return page > 1;
            return false;
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class NextPageEnabledConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int page && parameter is int totalPages)
                return page < totalPages;
            return false;
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class PageNumberToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Get current page from DeliveriesViewModel (assumes BindingContext is DeliveriesViewModel)
            var currentPage = 1;
            if (Application.Current?.MainPage is ContentPage page && page.BindingContext is Partico_Delivery.ViewModels.DeliveriesViewModel vm)
                currentPage = vm.CurrentPage;
            if (value is int pageNumber)
                return pageNumber == currentPage ? Color.FromArgb("#8E24AA") : Colors.LightGray; // Use purple
            return Colors.LightGray;
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}