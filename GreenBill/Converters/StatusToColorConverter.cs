using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace GreenBill.Converters
{
    public class StatusToColorValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is string status)
            {
                switch (status.ToLower())
                {
                    case "verified":
                        return new SolidColorBrush(Colors.Green);

                    case "in review":
                    case "inreview":
                        return new SolidColorBrush(Colors.Orange);

                    case "rejected":
                        return new SolidColorBrush(Colors.Red);

                    default:
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#666666"));
                }
            }

            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#666666"));
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
