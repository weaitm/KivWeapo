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

namespace KivMusic.Views.Stockman
{
    /// <summary>
    /// Логика взаимодействия для DeleteLocWarehouseWindow.xaml
    /// </summary>
    public partial class DeleteLocWarehouseWindow : Page
    {
        ObservableCollection<LocWarehouse> locWarehouses = new ObservableCollection<LocWarehouse>();
        ConAPI conAPI = new ConAPI();
        string uri;
        public DeleteLocWarehouseWindow()
        {
            InitializeComponent();
            uri = conAPI.ReadConJson();
            initDataGrid();
        }

        public class LocWarehouse
        {
            public int id { get; set; }
            public string product { get; set; }
            public int productid { get; set; }
            public string warehouse { get; set; }
            public int warehouseid { get; set; }
            public int quantityofgoodsonwarehouse { get; set; }
            public string profileFIO { get; set; }
            public int profileid { get; set; }
        }

        public void initDataGrid()
        {

            try
            {
                locWarehouses.Clear();
                string url = uri + $"/locationwarehouse";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locLocationWarehouse = JsonConvert.DeserializeObject<List<LocationWarehouse>>(response);

                foreach (LocationWarehouse warehouse in locLocationWarehouse)
                {
                    string urlProduct = uri + $"/product/{warehouse.productid}";
                    HttpWebRequest httpWebRequestProduct = (HttpWebRequest)WebRequest.Create(urlProduct);
                    HttpWebResponse httpWebResponseProduct = (HttpWebResponse)httpWebRequestProduct.GetResponse();
                    string responseProduct;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseProduct.GetResponseStream()))
                    {
                        responseProduct = streamReader.ReadToEnd();
                    }

                    Product locProduct = JsonConvert.DeserializeObject<Product>(responseProduct);

                    string urlWarehouse = uri + $"/warehouse/{warehouse.warehouseid}";
                    HttpWebRequest httpWebRequestWarehouse = (HttpWebRequest)WebRequest.Create(urlWarehouse);
                    HttpWebResponse httpWebResponseWarehouse = (HttpWebResponse)httpWebRequestWarehouse.GetResponse();
                    string responseWarehouse;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseWarehouse.GetResponseStream()))
                    {
                        responseWarehouse = streamReader.ReadToEnd();
                    }

                    Warehouse locWarehouse = JsonConvert.DeserializeObject<Warehouse>(responseWarehouse);

                    string urlProfile= uri + $"/profile/{warehouse.profileid}";
                    HttpWebRequest httpWebRequestProfile = (HttpWebRequest)WebRequest.Create(urlProfile);
                    HttpWebResponse httpWebResponseProfile = (HttpWebResponse)httpWebRequestProfile.GetResponse();
                    string responseProfile;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseProfile.GetResponseStream()))
                    {
                        responseProfile = streamReader.ReadToEnd();
                    }

                    Profile locProfile = JsonConvert.DeserializeObject<Profile>(responseProfile);

                    locWarehouses.Add(new LocWarehouse
                    {
                        id = warehouse.id,
                        product = locProduct.productname,
                        productid = warehouse.productid,
                        warehouse = locWarehouse.warehousecell,
                        warehouseid = warehouse.warehouseid,
                        quantityofgoodsonwarehouse = warehouse.quantityofgoodsonwarehouse,
                        profileFIO = $"{locProfile.lastname} {locProfile.firstname.Substring(0, 1)}. {locProfile.middlename.Substring(0, 1)}.",
                        profileid = warehouse.id,
                    });
                }
                dtLocWarehouse.ItemsSource = locWarehouses;
                dtLocWarehouse.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (dtLocWarehouse.SelectedItems.Count != 0)
            {
                var cellinfo = dtLocWarehouse.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + $"/locationwarehouse/{Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text)}");
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
