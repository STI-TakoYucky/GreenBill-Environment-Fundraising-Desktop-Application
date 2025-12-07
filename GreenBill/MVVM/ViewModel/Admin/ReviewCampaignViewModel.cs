using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.MVVM.View.CampaignDetailsTabs;
using GreenBill.Services;
using LiveCharts;
using LiveCharts.Wpf;
using LiveChartsCore;
using MongoDB.Bson;
using MongoDB.Libmongocrypt;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace GreenBill.MVVM.ViewModel.Admin {
    public class ReviewCampaignViewModel : Core.ViewModel, INavigatableService {

        private ICampaignService _campaignService;
        private ISupportingDocumentService _supportingDocumentService;
        private IDonationRecordService _donationRecordService;

        public SeriesCollection CartesianSeries { get; set; }
        public ICommand ApproveCampaignAsyncCommand { get; set; }
        public ICommand ReviewCampaignAsyncCommand { get; set; }
        public ICommand RejectCampaign { get; set; }
        public ICommand ApproveSupportingDocumentCommand { get; set; }
        public ICommand ReviewSupportingDocumentCommand { get; set; }
        public ICommand RejectDocument { get; set; }

        private ICommand _previewImageCommand;


        public ICommand PreviewImageCommand {
            get => _previewImageCommand;
            set {
                _previewImageCommand = value;
                OnPropertyChanged();
            }
        }



        private ObjectId _campaignId;
        public ObservableCollection<GreenBill.MVVM.Model.Campaign> Campaigns { get; set; } = new ObservableCollection<GreenBill.MVVM.Model.Campaign>();
        public ObjectId CampaignId {
            get => _campaignId;
            set {
                _campaignId = value;
                OnPropertyChanged();
            }
        }

        //selected campaign
        private GreenBill.MVVM.Model.Campaign _selectedCampaign;
        public GreenBill.MVVM.Model.Campaign SelectedCampaign {
            get => _selectedCampaign;
            set {
                _selectedCampaign = value;
                OnPropertyChanged();
            }
        }

        //supporting docs for that campaign
        private ObservableCollection<SupportingDocument> _supportingDocument = new ObservableCollection<SupportingDocument>();
        private ITabNavigationService _navigationService;

        public ObservableCollection<SupportingDocument> SupportingDocument {
            get => _supportingDocument;
            set {
                _supportingDocument = value;
                OnPropertyChanged();
            }
        }
        public ITabNavigationService Navigation {
            get => _navigationService;
            set {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        private string docCount;
        public string DocCount {
            get => docCount;
            set {
                docCount = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateBack { get; set; }

        public Func<double, string> DateFormatter { get; set; }
        //Constructor
        public ReviewCampaignViewModel(ICampaignService campaignService, ISupportingDocumentService supportingDocumentService, ITabNavigationService navService, IDonationRecordService donationRecordService) {
            _campaignService = campaignService;
            _supportingDocumentService = supportingDocumentService;
            Navigation = navService;
            CartesianSeries = new SeriesCollection();
            _donationRecordService = donationRecordService;
            ApproveCampaignAsyncCommand = new RelayCommand(o => ApproveCampaignAsync());
            ReviewCampaignAsyncCommand = new RelayCommand(o => ReviewCampaignAsync());
            ApproveSupportingDocumentCommand = new RelayCommand(o => ApproveSupportingDocumentAsync(o));
            RejectCampaign = new RelayCommand(o => RejectCampaignAsync(o));
            RejectDocument = new RelayCommand(o => RejectDocumentAsync(o));
            _previewImageCommand = new RelayCommand(param => PreviewFile(param));
            NavigateBack = new RelayCommand(o => Navigation.NavigateToTab<AdminCampaignAnalyticsViewModel>());
            ReviewSupportingDocumentCommand = new RelayCommand(o => ReviewSupportingDocumentAsync(o));

        }

        public async void GetDonationRecords(ObjectId campaignID) {
            var donations = await _donationRecordService.GetByCampaignIdAsync(campaignID);
            BuildLineChart(donations);
        }

        private void BuildLineChart(List<DonationRecord> donationRecords) {
            List<DateTime> ChartDates = new List<DateTime>();
            var donations = donationRecords;

            var donationValues = new ChartValues<long>();
            ChartDates.Clear();

            if (donations == null || donations.Count == 0)
                return;

            DateTime donationCreatedDate =
                donations.OrderBy(c => c.CreatedAt).FirstOrDefault()?.CreatedAt.Date
                ?? DateTime.Now.Date;

            DateTime startDate = donationCreatedDate;
            DateTime endDate = DateTime.Now.Date;

            var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                                     .Select(offset => startDate.AddDays(offset))
                                     .ToList();

            foreach (var day in allDates) {

                long donationAmountPerDay = donations
                    .Where(c => c.CreatedAt.Date == day)
                    .Sum(c => c.Amount);

                if (donationAmountPerDay == 0)
                    continue;

                ChartDates.Add(day);
                donationValues.Add(donationAmountPerDay);
            }

            DateFormatter = value => {
                int index = (int)value;
                if (index < 0 || index >= ChartDates.Count) return "";
                return ChartDates[index].ToString("MMM dd yyyy");
            };
            OnPropertyChanged(nameof(DateFormatter));

            CartesianSeries = new SeriesCollection {
        new ColumnSeries {   // <-- BAR CHART IS ColumnSeries in LiveCharts
            Title = "Campaign Donations",
            Values = donationValues,
            DataLabels = false,
            Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00A86B")),
            Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00A86B")),
            LabelPoint = chartPoint =>
                $"{ChartDates[(int)chartPoint.Key]:MMM dd, yyyy} → ₱{chartPoint.Y:N0}"
        }
    };

            OnPropertyChanged(nameof(CartesianSeries));
        }

        private void PreviewFile(object parameter) {
            if (parameter is SupportingDocument doc) {
                byte[] fileData = doc.FileData;
                string extension = doc.ContentType; // or extract from FileName if you prefer

                if (fileData == null || string.IsNullOrEmpty(extension))
                    return;

                extension = extension.ToLower();

                if (extension.Contains("image")) // jpeg/png
                    PreviewImage(fileData);
                else if (extension.Contains("pdf"))
                    PreviewPdf(fileData);
                else
                    PreviewDocument(fileData, Path.GetExtension(doc.FileName));
            } else if (parameter is byte[] image) {
                PreviewImage(image);
            }
        }



        private void PreviewPdf(byte[] fileData) {
            string temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".pdf");
            File.WriteAllBytes(temp, fileData);

            var win = new Window {
                Title = "PDF Preview",
                Width = 800,
                Height = 600,
                Content = new WebBrowser { Source = new Uri(temp) }
            };

            win.ShowDialog();
        }

        private void PreviewDocument(byte[] fileData, string ext) {
            string temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ext);
            File.WriteAllBytes(temp, fileData);

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo {
                FileName = temp,
                UseShellExecute = true
            });
        }


        public void PreviewImage(object parameter) {
            if (parameter is byte[] imageBytes) {
                // Convert byte array to ImageSource
                var image = new BitmapImage();
                using (var stream = new MemoryStream(imageBytes)) {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    image.Freeze(); // optional, makes it cross-thread safe
                }

                var window = new Window {
                    Title = SelectedCampaign?.Title ?? "Preview",
                    Width = 800,
                    Height = 600,
                    Content = new Image {
                        Source = image,
                        Stretch = Stretch.Uniform
                    }
                };
                window.ShowDialog();
            }
        }

        //public void Approve_ReviewCampaignAsync() {

        //    if (SelectedCampaign.Status == "Verified") {
        //        MessageBoxResult docReviewMsg = MessageBox.Show(
        //           "This campaign will be back to the reviewing stage. Do you want to continue?",
        //           "Review Campaign",
        //           MessageBoxButton.YesNo,
        //           MessageBoxImage.Question
        //       );

        //        if (docReviewMsg == MessageBoxResult.No) {
        //            return;
        //        }

        //        _campaignService.StageReviewCampaign(CampaignId);
        //        SelectedCampaign.Status = "in review";
        //        return;
        //    }

        //    //check if there are documents that needs verification
        //    int docCount = 0;
        //    foreach (var item in SupportingDocument) {
        //        if (item.Status == "in review") {
        //            docCount++;
        //        }
        //    }

        //    if (docCount > 0) {
        //        MessageBoxResult docReviewMsg = MessageBox.Show(
        //           docCount + " document/s needs to be reviewed. Are you sure you want to verify now?",
        //           "Review Pending",
        //           MessageBoxButton.YesNo,
        //           MessageBoxImage.Question
        //       );

        //        if (docReviewMsg == MessageBoxResult.No) {
        //            return;
        //        }
        //    } else {
        //        MessageBoxResult msg = MessageBox.Show(
        //            "Verify this campaign now?",
        //            "Verify Campaign",
        //            MessageBoxButton.YesNo,
        //            MessageBoxImage.Question
        //        );

        //        if (msg == MessageBoxResult.No) {
        //            return;
        //        }
        //    }

        //    _campaignService.ApproveCampaign(CampaignId);
        //    SelectedCampaign.Status = "Verified";
        //}

        public void ApproveCampaignAsync() {
            // Check if there are documents requiring review
            int docCount = SupportingDocument.Count(d => d.Status == "in review" || d.Status == "Pending");

            if (docCount > 0) {
                MessageBoxResult docReviewMsg = MessageBox.Show(
                    $"{docCount} document/s need to be reviewed. Are you sure you want to verify now?",
                    "Review Pending",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (docReviewMsg == MessageBoxResult.No)
                    return;
            } else {
                MessageBoxResult msg = MessageBox.Show(
                    "Verify this campaign now?",
                    "Verify Campaign",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (msg == MessageBoxResult.No)
                    return;
            }

            _campaignService.ApproveCampaign(CampaignId);
            SelectedCampaign.Status = "Verified";
        }


        public void RejectCampaignAsync(object parameter) {
            MessageBoxResult msg = MessageBox.Show(
                "Reject this campaign?",
                "Reject Campaign",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
                );

            if (msg == MessageBoxResult.No) { return; }

            ObjectId _id = ObjectId.Parse(parameter.ToString());
            _campaignService.RejectCampaign(_id);
            SelectedCampaign.Status = "Rejected";
        }

        public void ReviewCampaignAsync() {
            MessageBoxResult docReviewMsg = MessageBox.Show(
                "This campaign will go back to the reviewing stage. Do you want to continue?",
                "Review Campaign",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (docReviewMsg == MessageBoxResult.No)
                return;

            _campaignService.StageReviewCampaign(CampaignId);
            SelectedCampaign.Status = "in review";
        }

        //public void Approve_ReviewSupportingDocumentAsync(object parameter) {

        //    SupportingDocument selectedDoc = SupportingDocument.FirstOrDefault(c => c.Id == (string)parameter);
        //    ObjectId _id = ObjectId.Parse(parameter.ToString());

        //    if (selectedDoc.Status == "Verified") {
        //        MessageBoxResult docReviewMsg = MessageBox.Show(
        //           "This document will be back to the reviewing stage. Do you want to continue?",
        //           "Review Document",
        //           MessageBoxButton.YesNo,
        //           MessageBoxImage.Question
        //       );

        //        if (docReviewMsg == MessageBoxResult.No) {
        //            return;
        //        }

        //        _supportingDocumentService.StageReviewSupportingDocument(_id);
        //        selectedDoc.Status = "in review";
        //        return;
        //    }

        //    MessageBoxResult msg = MessageBox.Show(
        //            "Verify this document now?",
        //            "Verify Document",
        //            MessageBoxButton.YesNo,
        //            MessageBoxImage.Question
        //        );

        //    if (msg == MessageBoxResult.No) {
        //        return;
        //    }

        //    _supportingDocumentService.ApproveSupportingDocument(_id);
        //    selectedDoc.Status = "Verified";
        //    _supportingDocumentService.UpdateComments(_id, selectedDoc.ReviewComments);
        //}

        public void ApproveSupportingDocumentAsync(object parameter) {
            var selectedDoc = SupportingDocument.FirstOrDefault(c => c.Id == (string)parameter);
            if (selectedDoc == null)
                return;

            ObjectId id = ObjectId.Parse(parameter.ToString());

            MessageBoxResult msg = MessageBox.Show(
                "Verify this document now?",
                "Verify Document",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (msg == MessageBoxResult.No)
                return;

            _supportingDocumentService.ApproveSupportingDocument(id);

            selectedDoc.Status = "Verified";

            // Save comments after approval
            _supportingDocumentService.UpdateComments(id, selectedDoc.ReviewComments);
        }

        public void ReviewSupportingDocumentAsync(object parameter) {
            var selectedDoc = SupportingDocument.FirstOrDefault(c => c.Id == (string)parameter);
            if (selectedDoc == null)
                return;

            ObjectId id = ObjectId.Parse(parameter.ToString());

            MessageBoxResult result = MessageBox.Show(
                "This document will be back to the reviewing stage. Do you want to continue?",
                "Review Document",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.No)
                return;

            _supportingDocumentService.StageReviewSupportingDocument(id);
            selectedDoc.Status = "Pending";
        }


        public void RejectDocumentAsync(object parameter) {
            SupportingDocument selectedDoc = SupportingDocument.FirstOrDefault(c => c.Id == (string)parameter);
            ObjectId _id = ObjectId.Parse(parameter.ToString());

            MessageBoxResult msg = MessageBox.Show(
                "Reject this document?",
                "Reject Document",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
                );

            if (msg == MessageBoxResult.No) { return; }

            _supportingDocumentService.RejectDocument(_id);
            selectedDoc.Status = "Rejected";
            _supportingDocumentService.UpdateComments(_id, selectedDoc.ReviewComments);
        }

        //fetch the campaign from the database based on the parameter from the table in the campaigns tab 
        public async void ApplyNavigationParameter(object parameter) {
            try {
                if (parameter == null) return;
                var id = parameter.ToString();
                CampaignId = ObjectId.Parse(id);
                SelectedCampaign = await _campaignService.GetCampaignByIdAsync(
                    id,
                    new CampaignIncludeOptions { IncludeUser = true }
                 );
                if (SelectedCampaign == null) return;

                var docs = await _supportingDocumentService.GetByCampaignIdAsync(id);
                DocCount = docs.Count.ToString();

                if (SupportingDocument != null) {
                    SupportingDocument.Clear();
                }
                foreach (var doc in docs) {
                    SupportingDocument.Add(doc);
                }
               
                Campaigns.Add(new MVVM.Model.Campaign {
                    Id = SelectedCampaign.Id,
                });

                GetDonationRecords(SelectedCampaign.Id);
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }
    }

}
