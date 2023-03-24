using KivMusic.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace KivMusic.Views.Provider
{
    /// <summary>
    /// Логика взаимодействия для ProviderWindow.xaml
    /// </summary>
    public partial class ProviderWindow : Page
    {
        ConAPI conAPI = new ConAPI();
        string uri;
        ObservableCollection<Providers> providers = new ObservableCollection<Providers>();
        int myid;
        public ProviderWindow(int myid)
        {
            InitializeComponent();
            uri = conAPI.ReadConJson();
            initDataGrid();
            this.myid = myid;
        }

        public class Providers
        {
            public int id { get; set; }
            public string nameprovider { get; set; }
        }

        public void initDataGrid()
        {

            try
            {
                providers.Clear();
                string url = uri + $"/provider";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locProvider = JsonConvert.DeserializeObject<List<Providers>>(response);

                foreach (Providers provider in locProvider)
                {
                    providers.Add(provider);
                }
                dtProvider.ItemsSource = providers;
                dtProvider.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void dtProvider_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtProvider.SelectedItems.Count != 0)
                {
                    var cellinfo = dtProvider.SelectedCells[1];
                    txtProvider.Text = (cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDeliveryWin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DeliveryWindow(myid));
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtProvider.Text.Trim().Length == 0)
            {
                MessageBox.Show("Введите значния!");
            }
            else
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var endpoint = new Uri(uri + "/provider");
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                        var newProvider = new Providery()
                        {
                            nameprovider = txtProvider.Text.Trim(),
                        };

                        var newProviderJson = JsonConvert.SerializeObject(newProvider);
                        var payload = new StringContent(newProviderJson, Encoding.UTF8, "application/json");
                        var result = client.PostAsync(endpoint, payload).Result.ToString();

                        if (result.StartsWith("StatusCode: 500"))
                        {
                            MessageBox.Show("Такой поставщик уже создан");
                        }
                        initDataGrid();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void btnUpd_Click(object sender, RoutedEventArgs e)
        {
            if (dtProvider.SelectedItems.Count != 0)
            {
                var cellinfo = dtProvider.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + "/provider");
                    var newProvider = new Providery()
                    {
                        id = Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text),
                        nameprovider = txtProvider.Text.Trim(),
                    };

                    var newProviderJson = JsonConvert.SerializeObject(newProvider);
                    var payload = new StringContent(newProviderJson, Encoding.UTF8, "application/json");
                    var result = client.PutAsync(endpoint, payload).Result.ToString();

                    if (result.StartsWith("StatusCode: 500"))
                    {
                        MessageBox.Show("Нельзя изменить!");
                    }
                    initDataGrid();
                }
            }
            else
            {
                MessageBox.Show("Выберите строку в таблице");
            }
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (dtProvider.SelectedItems.Count != 0)
            {
                var cellinfo = dtProvider.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + $"/provider/{Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text)}");
                    var result = client.DeleteAsync(endpoint).Result.ToString();
                    initDataGrid();
                }
            }
            else
            {
                MessageBox.Show("Выберите строку в таблице");
            }
        }
    }
}
