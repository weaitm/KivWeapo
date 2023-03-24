using KivMusic.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
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

namespace KivMusic.Views.Seller
{
    /// <summary>
    /// Логика взаимодействия для ShopWindow.xaml
    /// </summary>
    public partial class ShopWindow : Page
    {
        ObservableCollection<Shops> shops = new ObservableCollection<Shops>();
        ConAPI conAPI = new ConAPI();
        string uri;
        public ShopWindow()
        {
            InitializeComponent();
            uri = conAPI.ReadConJson();
            initDataGrid();
        }

        public class Shops
        {
            public int id { get; set; }
            public string shopname { get; set; }
            public string shopaddress { get; set; }
        }

        public void initDataGrid()
        {

            try
            {
                shops.Clear();
                string url = uri + $"/shop";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locShop = JsonConvert.DeserializeObject<List<Shops>>(response);

                foreach (Shops shop in locShop)
                {
                    shops.Add(shop);
                }
                dtShops.ItemsSource = shops;
                dtShops.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dtShops_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtShops.SelectedItems.Count != 0)
                {
                    var shopNameInfo = dtShops.SelectedCells[1];
                    txtShopName.Text = (shopNameInfo.Column.GetCellContent(shopNameInfo.Item) as TextBlock).Text;
                    var shopAddressInfo = dtShops.SelectedCells[2];
                    txtShopAddress.Text = (shopAddressInfo.Column.GetCellContent(shopAddressInfo.Item) as TextBlock).Text;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtShopName.Text.Trim().Length == 0 && txtShopAddress.Text.Trim().Length == 0)
            {
                MessageBox.Show("Введите значния!");
            }
            else
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var endpoint = new Uri(uri + "/shop");
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                        var newShop = new Shop()
                        {
                            shopname = txtShopName.Text.Trim(),
                            shopaddress = txtShopAddress.Text.Trim(),
                        };

                        var newShopJson = JsonConvert.SerializeObject(newShop);
                        var payload = new StringContent(newShopJson, Encoding.UTF8, "application/json");
                        var result = client.PostAsync(endpoint, payload).Result.ToString();

                        if (result.StartsWith("StatusCode: 500"))
                        {
                            MessageBox.Show("Такой магазин уже создан");
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
            if (dtShops.SelectedItems.Count != 0)
            {
                var cellinfo = dtShops.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + "/shop");
                    var newShop = new Shop()
                    {
                        id = Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text),
                        shopname = txtShopName.Text.Trim(),
                        shopaddress = txtShopAddress.Text.Trim(),
                    };

                    var newShopJson = JsonConvert.SerializeObject(newShop);
                    var payload = new StringContent(newShopJson, Encoding.UTF8, "application/json");
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
            if (dtShops.SelectedItems.Count != 0)
            {
                var cellinfo = dtShops.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + $"/shop/{Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text)}");
                    var result = client.DeleteAsync(endpoint).Result.ToString();
                    initDataGrid();
                }
            }
            else
            {
                MessageBox.Show("Выберите строку в таблице");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
