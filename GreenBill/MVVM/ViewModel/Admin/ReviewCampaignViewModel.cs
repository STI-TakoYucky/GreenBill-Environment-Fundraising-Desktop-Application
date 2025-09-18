using GreenBill.IServices;
using GreenBill.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.MVVM.ViewModel.Admin {
    public class ReviewCampaignViewModel : Core.ViewModel {

        private string _campaignId;
        public string CampaignId {
            get => _campaignId;
            set {
                _campaignId = value;
                OnPropertyChanged();
            }
        }
        private ICampaignService _campaignService;

        private GreenBill.MVVM.Model.Campaign _selectedCampaign;
        public GreenBill.MVVM.Model.Campaign SelectedCampaign {
            get => _selectedCampaign;
            set {
                _selectedCampaign = value;
                OnPropertyChanged();
            }
        }

        public ReviewCampaignViewModel() {

        }

        public ReviewCampaignViewModel(ICampaignService campaignService) { 
        
            _campaignService = campaignService;
        }


        public async void ApplyNavigationParameter(object parameter) {
            if (parameter == null) return;
            var id = parameter.ToString();
            SelectedCampaign = await _campaignService.GetCampaignByIdAsync(
                id,
                new CampaignIncludeOptions { IncludeUser = true }
             );
        }
    }

}
