using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace GreenBill.Converters {
    public class FileTypeToPreviewConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            try {
                if (values == null || values.Length < 2)
                    return GetDefaultIcon();

                byte[] fileData = values[0] as byte[];
                string extension = values[1] as string;

                if (fileData == null || string.IsNullOrEmpty(extension))
                    return GetDefaultIcon();

                extension = extension.ToLower();

                // IMAGE TYPES
                string[] imageMimeTypes = { "image/png", "image/jpg", "image/jpeg", "image/bmp", "image/gif" };
                if (imageMimeTypes.Contains(extension)) // extension here is actually MIME type
                    return LoadImage(fileData) ?? GetDefaultIcon();

                // OTHER → fallback
                return GetDefaultIcon();
            } catch {
                return GetDefaultIcon();
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        private BitmapImage LoadImage(byte[] data) {
            if (data == null || data.Length == 0) return null;

            try {
                using (var ms = new MemoryStream(data)) {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.StreamSource = ms;
                    img.EndInit();
                    img.Freeze();
                    return img;
                }
            } catch {
                return null;
            }
        }

        private BitmapImage GetDefaultIcon() {
            try {
                return new BitmapImage(new Uri("pack://application:,,,/GreenBill;component/Assets/Images/file.png"));
            } catch {
                return null;
            }
        }
    }
}
