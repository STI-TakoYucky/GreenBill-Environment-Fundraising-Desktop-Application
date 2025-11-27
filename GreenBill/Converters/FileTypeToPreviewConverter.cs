using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace GreenBill.Converters {
    public class FileTypeToPreviewConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            byte[] data = values[0] as byte[];
            string ext = values[1] as string;

            if (data == null)
                return null;

            // try load image
            try {
                using (MemoryStream ms = new MemoryStream(data)) {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
                }
            } catch { }

            // fallback icons
            string iconPath = "/Assets/Images/fileIcon.png";

            ext = ext != null ? ext.ToLower() : "";

            if (ext.EndsWith(".pdf"))
                iconPath = "/Assets/Images/pdfIcon.png";
            else if (ext.EndsWith(".doc") || ext.EndsWith(".docx"))
                iconPath = "/Assets/Images/wordIcon.png";
            else if (ext.EndsWith(".xls") || ext.EndsWith(".xlsx"))
                iconPath = "/Assets/Images/excelIcon.png";

            return new BitmapImage(new Uri(iconPath, UriKind.Relative));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
