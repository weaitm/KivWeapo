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

namespace KivMusic.Views.Keeper
{
    /// <summary>
    /// Логика взаимодействия для KeeperWindow.xaml
    /// </summary>
    public partial class KeeperWindow : Page
    {
        ConAPI conAPI = new ConAPI();
        string uri;
        ObservableCollection<PayTypes> payTypes = new ObservableCollection<PayTypes>();
        int myid;
        public KeeperWindow(int myid)
        {
            InitializeComponent();
            uri = conAPI.ReadConJson();
            initDataGrid();
            this.myid = myid;
        }
        public class PayTypes
        {
            public int id { get; set; }
            public string typename { get; set; }
        }

        public void initDataGrid()
        {

            try
            {
                payTypes.Clear();
                string url = uri + $"/paytype";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locPayTypes = JsonConvert.DeserializeObject<List<PayTypes>>(response);

                foreach (PayTypes payType in locPayTypes)
                {
                    payTypes.Add(payType);
                }
                dtPayType.ItemsSource = payTypes;
                dtPayType.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dtSalaryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtPayType.SelectedItems.Count != 0)
                {
                    var cellinfo = dtPayType.SelectedCells[1];
                    txtPayType.Text = (cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text;
                }
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

        private void btnSalaryWin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SalaryWindow(myid));
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtPayType.Text.Trim().Length == 0)
            {
                MessageBox.Show("Введите значния!");
            }
            else
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var endpoint = new Uri(uri + "/paytype");
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                        var newPayType = new PayType()
                        {
                            typename = txtPayType.Text.Trim(),
                        };

                        var newPayTypeJson = JsonConvert.SerializeObject(newPayType);
                        var payload = new StringContent(newPayTypeJson, Encoding.UTF8, "application/json");
                        var result = client.PostAsync(endpoint, payload).Result.ToString();

                        if (result.StartsWith("StatusCode: 500"))
                        {
                            MessageBox.Show("Такой тип зарплаты уже создан");
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
            if (dtPayType.SelectedItems.Count != 0)
            {
                var cellinfo = dtPayType.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + "/paytype");
                    var newPayType = new PayType()
                    {
                        id = Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text),
                        typename = txtPayType.Text.Trim(),
                    };

                    var newPayTypeJson = JsonConvert.SerializeObject(newPayType);
                    var payload = new StringContent(newPayTypeJson, Encoding.UTF8, "application/json");
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
            if (dtPayType.SelectedItems.Count != 0)
            {
                var cellinfo = dtPayType.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + $"/paytype/{Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text)}");
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
