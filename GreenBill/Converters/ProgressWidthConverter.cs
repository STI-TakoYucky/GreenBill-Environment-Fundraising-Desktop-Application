using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GreenBill.Converters
{
    public class ProgressWidthValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is decimal currentAmount &&
                values[1] is decimal goalAmount &&
                values[2] is double containerWidth &&
                goalAmount > 0)
            {
                var percentage = (double)(currentAmount / goalAmount);
                percentage = Math.Min(percentage, 1.0); 
                return containerWidth * percentage;
            }
            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
