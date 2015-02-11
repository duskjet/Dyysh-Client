using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Dyysh
{
    public enum AuthorizeState
    {
        Initial,
        Denied,
        Granted,
        Pending
    }

    class EnumToStringConverter : IValueConverter
    {
        // Summary:
        //     Converts a value.
        //
        // Parameters:
        //   value:
        //     The value produced by the binding source.
        //
        //   targetType:
        //     The type of the binding target property.
        //
        //   parameter:
        //     The converter parameter to use.
        //
        //   culture:
        //     The culture to use in the converter.
        //
        // Returns:
        //     A converted value. If the method returns null, the valid null value is used.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((AuthorizeState)value)
            {
                case AuthorizeState.Initial:
                    return "Need Authorization";
                case AuthorizeState.Denied:
                    return "Denied";
                case AuthorizeState.Granted:
                    return "Granted";
                default:
                    throw new Exception("Invalid value object");
            }
        }
        //
        // Summary:
        //     Converts a value.
        //
        // Parameters:
        //   value:
        //     The value that is produced by the binding target.
        //
        //   targetType:
        //     The type to convert to.
        //
        //   parameter:
        //     The converter parameter to use.
        //
        //   culture:
        //     The culture to use in the converter.
        //
        // Returns:
        //     A converted value. If the method returns null, the valid null value is used.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("It is forbidden to convert back from string to enum. Conversion is one-way from source only.");
        }
    }
}
