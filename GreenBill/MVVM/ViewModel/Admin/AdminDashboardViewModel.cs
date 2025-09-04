using GreenBill.MVVM.Model;
using System;
using System.Collections.ObjectModel;

namespace GreenBill.MVVM.ViewModel
{

    public class AdminDashboardViewModel
    {

        public ObservableCollection<Campaign> Campaigns { get; set; }

        public AdminDashboardViewModel()
        {
            Campaigns = new ObservableCollection<Campaign>();
        }
    }
}
