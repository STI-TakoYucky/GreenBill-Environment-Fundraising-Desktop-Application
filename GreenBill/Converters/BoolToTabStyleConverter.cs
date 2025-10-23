using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GreenBill.Converters {
    public class BoolToTabStyleConverter : IValueConverter {
        public Style ActiveStyle { get; set; }
        public Style InactiveStyle { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            bool isActive = value is bool b && b;
            return isActive ? ActiveStyle : InactiveStyle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}

