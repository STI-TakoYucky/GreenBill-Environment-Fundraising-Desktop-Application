using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace GreenBill.Converters {
    public class EmptyCollectionToVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            try {
                int count = int.TryParse(value as string, out var c) ? c : 0;
                
                if(count > 0) {
                    return Visibility.Hidden;
                }
                return Visibility.Visible;
            } catch (Exception ex) {    
                Debug.WriteLine($"[EmptyCollectionToVisibilityConverter] Error: {ex.Message}");
                return Visibility.Visible;
            }
        }

        // values[0] expected: int count (e.g. Campaigns.Count)
        // values[1] expected: the collection itself (e.g. Campaigns)
        //public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {

        //    try {
        //        if (values == null || values.Length < 2)
        //            return Visibility.Visible;

        //        // If the collection object is null -> show (Visible)
        //        if (values[1] == null)
        //            return Visibility.Visible;

        //        // If the first binding provided a count int, prefer that (fast)
        //        if (values[0] is int count) {
        //            return count == 0 ? Visibility.Visible : Visibility.Collapsed;
        //        }

        //        // Otherwise attempt to inspect the collection for any items
        //        if (values[1] is IEnumerable enumerable) {
        //            var enumerator = enumerable.GetEnumerator();
        //            bool hasAny = enumerator.MoveNext();
        //            return hasAny ? Visibility.Collapsed : Visibility.Visible;
        //        }

        //        // fallback: show the placeholder
        //        return Visibility.Visible;
        //    } catch {
        //        // on error return visible to avoid hiding UI incorrectly
        //        return Visibility.Visible;
        //    }
        //}

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}