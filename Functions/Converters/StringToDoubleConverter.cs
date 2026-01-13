using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace Functions.Converters;

public class StringToDoubleConverter : IValueConverter
{
    private static Regex _doubleValueRegex = new(@"^(-?\d+(?:\.\d+)?)$");

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue && _doubleValueRegex.IsMatch(stringValue.Trim()))
        {
            return double.Parse(stringValue);
        }
        else
        {
            return Binding.DoNothing;
        }
    }
}
