using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GreenBill.MVVM.View.Admin {
    /// <summary>
    /// Interaction logic for AdminAccPreview.xaml
    /// </summary>
    public partial class AdminAccPreview : UserControl {
        public AdminAccPreview() {
            InitializeComponent();
        }

        private void MyDataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
            if (sender is DataGrid dataGrid) {
                // Find the internal ScrollViewer of the DataGrid
                var scrollViewer = FindVisualChild<ScrollViewer>(dataGrid);
                if (scrollViewer == null)
                    return;

                // --- Horizontal scrolling with mouse wheel ---
                // Scroll horizontally using vertical wheel motion
                // (negative delta moves right, positive delta moves left)
                double newOffset = scrollViewer.HorizontalOffset - (e.Delta / 3.0);
                scrollViewer.ScrollToHorizontalOffset(newOffset);

                e.Handled = true; // prevent vertical scrolling
            }
        }

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++) {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T correctlyTyped)
                    return correctlyTyped;

                var descendent = FindVisualChild<T>(child);
                if (descendent != null)
                    return descendent;
            }
            return null;
        }
    }
}
