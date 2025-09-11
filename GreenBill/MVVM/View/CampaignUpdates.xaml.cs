using GreenBill.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace GreenBill.MVVM.View
{
    /// <summary>
    /// Interaction logic for CampaignUpdates.xaml
    /// </summary>
    public partial class CampaignUpdates : UserControl
    {
        private ObservableCollection<CampaignUpdate> _updates;
        private ObservableCollection<CampaignUpdate> _filteredUpdates;
        private CampaignUpdate _editingUpdate;
        private bool _isEditing;

        public CampaignUpdates()
        {
            InitializeComponent();
            InitializeData();
            LoadSampleData();
        }

        private void InitializeData()
        {
            _updates = new ObservableCollection<CampaignUpdate>();
            _filteredUpdates = new ObservableCollection<CampaignUpdate>();
            UpdatesItemsControl.ItemsSource = _filteredUpdates;

            // Set default category selection
            CategoryComboBox.SelectedIndex = 0;
        }

        private void LoadSampleData()
        {
            // Add some sample data for demonstration
            var sampleUpdates = new List<CampaignUpdate>
            {
                new CampaignUpdate
                {
                    Id = Guid.NewGuid(),
                    Title = "Campaign Launch Success!",
                    Description = "We're thrilled to announce the official launch of our green energy campaign! Thanks to early supporters, we've already reached 15% of our goal in the first week. Your contributions are making a real difference in promoting sustainable energy solutions.",
                    Category = "Milestone Reached",
                    Tags = "launch, milestone, thank you",
                    DateCreated = DateTime.Now.AddDays(-7)
                },
                new CampaignUpdate
                {
                    Id = Guid.NewGuid(),
                    Title = "Media Coverage Update",
                    Description = "Our campaign was featured in the local environmental magazine! This exposure has brought in new supporters and increased awareness about our cause. We're gaining momentum every day.",
                    Category = "Media Coverage",
                    Tags = "media, awareness, progress",
                    DateCreated = DateTime.Now.AddDays(-3)
                },
                new CampaignUpdate
                {
                    Id = Guid.NewGuid(),
                    Title = "50% Goal Reached!",
                    Description = "Incredible news! We've officially reached 50% of our funding goal! This milestone wouldn't have been possible without each and every one of our amazing supporters. We're halfway there!",
                    Category = "Milestone Reached",
                    Tags = "milestone, halfway, celebration",
                    DateCreated = DateTime.Now.AddDays(-1)
                }
            };

            foreach (var update in sampleUpdates)
            {
                _updates.Add(update);
                _filteredUpdates.Add(update);
            }

            UpdateEmptyState();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            if (_isEditing && _editingUpdate != null)
            {
                // Update existing
                _editingUpdate.Title = TitleTextBox.Text.Trim();
                _editingUpdate.Description = DescriptionTextBox.Text.Trim();
                _editingUpdate.Category = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "General Update";
                _editingUpdate.DateModified = DateTime.Now;

                ShowSuccessMessage("Update modified successfully!");
            }
            else
            {
                // Create new
                var newUpdate = new CampaignUpdate
                {
                    Id = Guid.NewGuid(),
                    Title = TitleTextBox.Text.Trim(),
                    Description = DescriptionTextBox.Text.Trim(),
                    Category = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "General Update",
                    DateCreated = DateTime.Now
                };

                _updates.Insert(0, newUpdate); // Add to beginning for chronological order
                ApplySearch(); // Refresh filtered list
                ShowSuccessMessage("Update added successfully!");
            }

            ResetForm();
            UpdateEmptyState();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Guid updateId)
            {
                var update = _updates.FirstOrDefault(u => u.Id == updateId);
                if (update != null)
                {
                    StartEditing(update);
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Guid updateId)
            {
                var result = MessageBox.Show(
                    "Are you sure you want to delete this update? This action cannot be undone.",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var update = _updates.FirstOrDefault(u => u.Id == updateId);
                    if (update != null)
                    {
                        _updates.Remove(update);
                        ApplySearch();
                        UpdateEmptyState();
                        ShowSuccessMessage("Update deleted successfully!");

                        // If we were editing this update, reset the form
                        if (_isEditing && _editingUpdate?.Id == updateId)
                        {
                            ResetForm();
                        }
                    }
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplySearch();
        }

        private void StartEditing(CampaignUpdate update)
        {
            _isEditing = true;
            _editingUpdate = update;

            // Populate form with existing data
            TitleTextBox.Text = update.Title;
            DescriptionTextBox.Text = update.Description;

            // Set category
            for (int i = 0; i < CategoryComboBox.Items.Count; i++)
            {
                if ((CategoryComboBox.Items[i] as ComboBoxItem)?.Content?.ToString() == update.Category)
                {
                    CategoryComboBox.SelectedIndex = i;
                    break;
                }
            }

            // Update UI
            FormTitle.Text = "Edit Update";
            SaveButton.Content = "Save Changes";
            CancelButton.Visibility = Visibility.Visible;

            // Scroll to form
            TitleTextBox.Focus();
        }

        private void ResetForm()
        {
            _isEditing = false;
            _editingUpdate = null;

            // Clear form
            TitleTextBox.Clear();
            DescriptionTextBox.Clear();
            CategoryComboBox.SelectedIndex = 0;

            // Reset UI
            FormTitle.Text = "Add New Update";
            SaveButton.Content = "Add Update";
            CancelButton.Visibility = Visibility.Collapsed;
        }

        private bool ValidateForm()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
                errors.Add("Title is required.");

            if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
                errors.Add("Description is required.");

            if (TitleTextBox.Text?.Trim().Length > 200)
                errors.Add("Title must be 200 characters or less.");

            if (DescriptionTextBox.Text?.Trim().Length > 2000)
                errors.Add("Description must be 2000 characters or less.");

            if (errors.Any())
            {
                MessageBox.Show(string.Join("\n", errors), "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void ApplySearch()
        {
            _filteredUpdates.Clear();

            var searchTerm = SearchTextBox.Text?.Trim().ToLower() ?? "";
            var filteredItems = string.IsNullOrEmpty(searchTerm)
                ? _updates
                : _updates.Where(u =>
                    u.Title.ToLower().Contains(searchTerm) ||
                    u.Description.ToLower().Contains(searchTerm) ||
                    u.Category.ToLower().Contains(searchTerm) ||
                    u.Tags.ToLower().Contains(searchTerm));

            foreach (var item in filteredItems.OrderByDescending(u => u.DateCreated))
            {
                _filteredUpdates.Add(item);
            }

            UpdateEmptyState();
        }

        private void UpdateEmptyState()
        {
            EmptyStatePanel.Visibility = _filteredUpdates.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}