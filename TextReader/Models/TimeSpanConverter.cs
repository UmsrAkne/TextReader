namespace TextReader.Models
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((TimeSpan)value).TotalMilliseconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TimeSpan.FromMilliseconds(double.Parse((string)value));
        }
    }
}
