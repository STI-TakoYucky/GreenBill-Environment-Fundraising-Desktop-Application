using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace GreenBill.Converters {
    public class FileTypeToPreviewConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if (value == null)
                return null;

            // Try to load as an image
            try {
                byte[] data = value as byte[];
                if (data != null) {
                    using (MemoryStream ms = new MemoryStream(data)) {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        return bitmap;
                    }
                }
            } catch {
                // If not an image → continue to icon fallback
            }

            // Extract extension (string passed from ConverterParameter)
            string fileExt = "";
            if (parameter != null)
                fileExt = parameter.ToString().ToLower();

            // Default icon
            string iconPath = "/Assets/Images/fileIcon.png";

            if (fileExt.EndsWith(".pdf"))
                iconPath = "/Assets/Images/pdfIcon.png";
            else if (fileExt.EndsWith(".doc") || fileExt.EndsWith(".docx"))
                iconPath = "/Assets/Images/wordIcon.png";
            else if (fileExt.EndsWith(".xls") || fileExt.EndsWith(".xlsx"))
                iconPath = "/Assets/Images/excelIcon.png";

            return new BitmapImage(new Uri(iconPath, UriKind.Relative));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
