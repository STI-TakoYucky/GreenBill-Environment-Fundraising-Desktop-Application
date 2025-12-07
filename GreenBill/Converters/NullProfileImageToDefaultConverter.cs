using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace GreenBill.Converters {
    public class NullProfileImageToDefaultConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            byte[] imageBytes = value as byte[];
            if (imageBytes == null || imageBytes.Length == 0) {
                // Return default image
                return new BitmapImage(new Uri("pack://application:,,,/GreenBill;component/Assets/Images/blankProfile.jpg"));
            }

            try {
                var image = new BitmapImage();
                using (var mem = new MemoryStream(imageBytes)) {
                    mem.Position = 0;
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = mem;
                    image.EndInit();
                }
                image.Freeze();
                return image;
            } catch {
                // fallback to default image if byte array is invalid
                return new BitmapImage(new Uri("pack://application:,,,/GreenBill;component/Assets/Images/blankProfile.jpg"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
