using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using KivMusic.Models;
using LiveCharts;
using LiveCharts.Wpf;
using Newtonsoft.Json;

namespace KivMusic.Views.Seller
{
    /// <summary>
    /// Логика взаимодействия для ChartWindow.xaml
    /// </summary>
    public partial class ChartWindow : Page
    {
        ObservableCollection<BillByProfile> billByProfiles = new ObservableCollection<BillByProfile>();
        ConAPI conAPI = new ConAPI();
        string uri;
        public ChartWindow()
        {
            InitializeComponent();
            uri = conAPI.ReadConJson();
            try
            {
                string urlProfile = uri + $"/profile";
                HttpWebRequest httpWebRequestProfile = (HttpWebRequest)WebRequest.Create(urlProfile);
                HttpWebResponse httpWebResponseProfile = (HttpWebResponse)httpWebRequestProfile.GetResponse();
                string responseProfile;

                using (StreamReader streamReader = new StreamReader(httpWebResponseProfile.GetResponseStream()))
                {
                    responseProfile = streamReader.ReadToEnd();
                }

                var locProfile = JsonConvert.DeserializeObject<List<Profile>>(responseProfile);

                SeriesCollection = new SeriesCollection{};

                foreach (Profile profile in locProfile)
                {
                    string url = uri + $"/productcheck";
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    string response;

                    using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        response = streamReader.ReadToEnd();
                    }

                    var locProductCheck = JsonConvert.DeserializeObject<List<ProductCheck>>(response);
                    decimal sum = 0;
                    foreach (ProductCheck check in locProductCheck)
                    {
                        if (profile.id == check.profileid)
                        {
                            billByProfiles.Add(new BillByProfile
                            {
                                fullname = $"{profile.lastname} {profile.firstname.Substring(0, 1)}. {profile.middlename.Substring(0, 1)}.",
                                totalsum = +sum + check.totalsum,
                                date = check.purchasedate.ToShortDateString(),
                            });
                        }
                    }

                }
                foreach (BillByProfile bill in billByProfiles)
                {
                    SeriesCollection.Add(new ColumnSeries
                    {
                        Title = bill.date,
                        Values = new ChartValues<decimal> { bill.totalsum }
                    });

                    Labels = new[] { bill.fullname };
                    Formatter = value => value.ToString("N");

                    DataContext = this;
                }
            }
            catch (Exception)
            {

            }           
        }

        public class BillByProfile
        {
            public string fullname{ get; set; }
            public decimal totalsum { get; set; }
            public string date { get; set; }
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<decimal, string> Formatter { get; set; }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
