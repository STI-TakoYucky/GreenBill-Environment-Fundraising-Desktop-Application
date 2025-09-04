using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace GreenBill.Converters
{
    public class TabStyleConverter : IValueConverter
    {
        public Style TabStyle { get; set; }
        public Style ActiveTabStyle { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string selectedTab = value as string;
            string currentTab = parameter as string;

            return selectedTab == currentTab ? ActiveTabStyle : TabStyle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
