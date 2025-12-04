using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using GreenBill.MVVM.ViewModel;

namespace GreenBill.MVVM.View
{
    /// <summary>
    /// Interaction logic for Campaigns.xaml
    /// </summary>
    public partial class Campaigns : UserControl
    {
        private const string SearchPlaceholder = "Search";

        public Campaigns()
        {
            InitializeComponent();
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && textBox.Text == SearchPlaceholder)
            {
                textBox.Text = "";
                textBox.Foreground = new SolidColorBrush(Color.FromRgb(51, 51, 51)); // #333333
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = SearchPlaceholder;
                textBox.Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153)); // #999999
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && DataContext is CampaignsViewModel viewModel)
            {
                string searchText = textBox.Text == SearchPlaceholder ? string.Empty : textBox.Text;
                viewModel.SearchCommand?.Execute(searchText);
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            var clickedButton = sender as Button;
            if (clickedButton == null) return;

            ResetFilterButtons();
            SetActiveFilterButton(clickedButton);

            string filterType = clickedButton.Name.Replace("Button", "");
            ApplyFilter(filterType);
        }

        private void ResetFilterButtons()
        {
            var inactiveStyle = (Style)FindResource("FilterButtonStyle");
     
        }

        private void SetActiveFilterButton(Button button)
        {
            var activeStyle = (Style)FindResource("ActiveFilterButtonStyle");
            button.Style = activeStyle;
        }

        private void ApplyFilter(string filterType)
        {
            if (DataContext is CampaignsViewModel viewModel)
            {
                viewModel.ApplyFilterCommand?.Execute(filterType);
            }

            switch (filterType.ToLower())
            {
                case "trending":
                    break;
                case "nearyou":
                    break;
                case "nonprofits":
                    break;
                default:
                    break;
            }
        }
    }

    public class PercentageToWidthConverter : IValueConverter
    {
        public static readonly PercentageToWidthConverter Instance = new PercentageToWidthConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double percentage)
            {
                return $"{percentage}%";
            }
            return "0%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}