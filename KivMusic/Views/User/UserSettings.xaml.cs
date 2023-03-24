using KivMusic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
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
using static KivMusic.Views.Admin.TypeCardWindow;

namespace KivMusic.Views.User
{
    /// <summary>
    /// Логика взаимодействия для UserSettings.xaml
    /// </summary>
    public partial class UserSettings : Page
    {
        ConAPI conAPI = new ConAPI();
        string uri;
        ObservableCollection<ProductChecks> productChecks = new ObservableCollection<ProductChecks>();
        int myid;
        public UserSettings(int myid)
        {
            InitializeComponent();
            uri = conAPI.ReadConJson();
            this.myid = myid;
            //initDataGrid();
        }

        public class ProductChecks
        {
            public int id { get; set; }
            public string checknumber { get; set; }
            public string shiftnumber { get; set; }
            public double purchasedate { get; set; }
            public decimal totalsum { get; set; }
            public decimal inputsum { get; set; }
            public int shopid { get; set; }
            public string shopname { get; set; }
            public int profilecardid { get; set; }
            public int profileid { get; set; }

        }

        private void btnCardWin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CardWindow(myid));
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
