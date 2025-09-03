using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.MVVM.ViewModel
{
    public class UserCampaignsViewModel : Core.ViewModel, INavigationAware
    {
        public bool ShowNavigation => true;
    }
}
