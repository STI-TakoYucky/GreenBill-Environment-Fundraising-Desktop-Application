using GreenBill.MVVM.ViewModel;
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

namespace GreenBill
{
    /// <summary>
    /// Interaction logic for AdminDashboard.xaml
    /// </summary>
<<<<<<< HEAD
    public partial class AdminWindow : Window
    {
        public AdminWindow()
=======
    public partial class AdminDashboard : Window
    {
        public AdminDashboard()
>>>>>>> 3c78753372bd1e2fd5f9825f5267bfa36b3d0956
        {
            InitializeComponent();
            DataContext = new AdminDashboardViewModel();
        }
<<<<<<< HEAD

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }
=======
>>>>>>> 3c78753372bd1e2fd5f9825f5267bfa36b3d0956
    }
}
