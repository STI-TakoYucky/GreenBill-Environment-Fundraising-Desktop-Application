using System;
using System.Globalization;
using System.Windows.Data;

namespace GreenBill.Converters {
    public class MultiFileParameterConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            return values; // pass the values array to command
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
