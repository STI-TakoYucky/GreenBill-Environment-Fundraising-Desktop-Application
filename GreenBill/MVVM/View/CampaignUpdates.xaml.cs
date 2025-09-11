using GreenBill.MVVM.ViewModel;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GreenBill.MVVM.View
{
    /// <summary>
    /// Interaction logic for CampaignUpdates.xaml
    /// </summary>
    public partial class CampaignUpdates : UserControl
    {
        private CampaignUpdatesViewModel ViewModel => DataContext as CampaignUpdatesViewModel;

        public CampaignUpdates()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is CampaignUpdatesViewModel oldViewModel)
            {
                // Unsubscribe from old view model events
                oldViewModel.OnSuccessMessage -= ShowSuccessMessage;
                oldViewModel.OnValidationFailed -= ShowValidationError;
                oldViewModel.OnConfirmDelete -= ShowConfirmDialog;
                oldViewModel.OnStartEditing -= ScrollToForm;
            }

            if (e.NewValue is CampaignUpdatesViewModel newViewModel)
            {
                // Subscribe to new view model events
                newViewModel.OnSuccessMessage += ShowSuccessMessage;
                newViewModel.OnValidationFailed += ShowValidationError;
                newViewModel.OnConfirmDelete += ShowConfirmDialog;
                newViewModel.OnStartEditing += ScrollToForm;
            }
        }

        private void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowValidationError(string message)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async Task<bool> ShowConfirmDialog(string message)
        {
            var result = MessageBox.Show(
                message,
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            return result == MessageBoxResult.Yes;
        }

        private void ScrollToForm()
        {
            // Focus on the title textbox and scroll to form if needed
            TitleTextBox.Focus();

            // Optional: If you have a scroll viewer, you can scroll to the form
            // FormScrollViewer.ScrollToTop();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Guid updateId)
            {
                ViewModel?.EditCommand?.Execute(updateId);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Guid updateId)
            {
                ViewModel?.DeleteCommand?.Execute(updateId);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.SaveCommand?.Execute(null);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.CancelCommand?.Execute(null);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // This is now handled by data binding, but kept for compatibility
        }
    }
}