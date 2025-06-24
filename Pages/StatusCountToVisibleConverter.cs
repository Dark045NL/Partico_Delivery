using System.Globalization;

namespace Partico_Delivery.Pages
{
    public class StatusCountToVisibleConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int count)
                return count > 0 ? true : false;
            if (value is IList<object> list)
                return list.Count > 0 ? true : false;
            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}