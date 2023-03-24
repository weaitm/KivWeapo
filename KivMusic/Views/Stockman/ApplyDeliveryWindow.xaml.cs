using KivMusic.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Policy;
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

namespace KivMusic.Views.Stockman
{
    /// <summary>
    /// Логика взаимодействия для ApplyDeliveryWindow.xaml
    /// </summary>
    public partial class ApplyDeliveryWindow : Page
    {
        ObservableCollection<Deliveries> deliveries = new ObservableCollection<Deliveries>();
        int myid;
        ConAPI conAPI = new ConAPI();
        string uri;
        public ApplyDeliveryWindow(int myid)
        {
            InitializeComponent();
            this.myid = myid;
            uri = conAPI.ReadConJson();
            DataContext = this;
            initDataGrid();
        }

        public class Deliveries
        {
            public int id { get; set; }
            public int warehouseid { get; set; }
            public string warehouse { get; set; }
            public int profileid { get; set; }
            public string profileFIO { get; set; }
            public int productid { get; set; }
            public string productname { get; set; }
            public int providerid { get; set; }
            public string providername { get; set; }
        }

        public void initDataGrid()
        {

            try
            {
                deliveries.Clear();
                string url = uri + $"/delivery";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locDelivery = JsonConvert.DeserializeObject<List<Delivery>>(response);

                foreach (Delivery delivery in locDelivery)
                {
                    string urlemployee = uri + $"/profile/{delivery.profileid}";
                    HttpWebRequest httpWebRequestEmployee = (HttpWebRequest)WebRequest.Create(urlemployee);
                    HttpWebResponse httpWebResponseEmployee = (HttpWebResponse)httpWebRequestEmployee.GetResponse();
                    string responseEmployee;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseEmployee.GetResponseStream()))
                    {
                        responseEmployee = streamReader.ReadToEnd();
                    }

                    Profile employee = JsonConvert.DeserializeObject<Profile>(responseEmployee);

                    string urlProduct = uri + $"/product/{delivery.productid}";
                    HttpWebRequest httpWebRequestProduct = (HttpWebRequest)WebRequest.Create(urlProduct);
                    HttpWebResponse httpWebResponseProduct = (HttpWebResponse)httpWebRequestProduct.GetResponse();
                    string responseProduct;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseProduct.GetResponseStream()))
                    {
                        responseProduct = streamReader.ReadToEnd();
                    }

                    Product product = JsonConvert.DeserializeObject<Product>(responseProduct);

                    string urlProvider = uri + $"/provider/{delivery.providerid}";
                    HttpWebRequest httpWebRequestProvider = (HttpWebRequest)WebRequest.Create(urlProvider);
                    HttpWebResponse httpWebResponseProvider = (HttpWebResponse)httpWebRequestProvider.GetResponse();
                    string responseProvider;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseProvider.GetResponseStream()))
                    {
                        responseProvider = streamReader.ReadToEnd();
                    }

                    Providery provider = JsonConvert.DeserializeObject<Providery>(responseProvider);

                    string urlWarehouse = uri + $"/warehouse/{delivery.warehouseid}";
                    HttpWebRequest httpWebRequestWarehouse = (HttpWebRequest)WebRequest.Create(urlWarehouse);
                    HttpWebResponse httpWebResponseWarehouse = (HttpWebResponse)httpWebRequestWarehouse.GetResponse();
                    string responseWarehouse;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseWarehouse.GetResponseStream()))
                    {
                        responseWarehouse = streamReader.ReadToEnd();
                    }

                    Warehouse warehouse = JsonConvert.DeserializeObject<Warehouse>(responseWarehouse);

                    if (warehouse.productavailability == true)
                    {

                    }
                    else
                    {
                        deliveries.Add(new Deliveries
                        {
                            id = delivery.id,
                            warehouseid = delivery.warehouseid,
                            warehouse = warehouse.warehousecell,
                            profileid = delivery.profileid,
                            profileFIO = $"{employee.lastname} {employee.firstname.Substring(0, 1)}. {employee.middlename.Substring(0, 1)}.",
                            productid = delivery.productid,
                            productname = product.productname,
                            providerid = delivery.providerid,
                            providername = provider.nameprovider,
                        });
                    }
                }
                dtDeleveries.ItemsSource = deliveries;
                dtDeleveries.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if(dtDeleveries.SelectedItems.Count != 0 && txtAmount.Text.Trim().Length != 0)
            {
                try
                {
                    var warehouseidinfo = dtDeleveries.SelectedCells[2];
                    var productidinfo = dtDeleveries.SelectedCells[6];

                    using (var client = new HttpClient())
                    {
                        var endpoint = new Uri(uri + "/locationwarehouse");
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                        var newLocationWarehouse = new LocationWarehouse()
                        {
                            warehouseid = Convert.ToInt32((warehouseidinfo.Column.GetCellContent(warehouseidinfo.Item) as TextBlock).Text),
                            profileid = myid,
                            productid = Convert.ToInt32((productidinfo.Column.GetCellContent(productidinfo.Item) as TextBlock).Text),
                            quantityofgoodsonwarehouse = Convert.ToInt32(txtAmount.Text.Trim()),
                        };

                        string urlWarehouse = uri + $"/warehouse/{Convert.ToInt32((warehouseidinfo.Column.GetCellContent(warehouseidinfo.Item) as TextBlock).Text)}";
                        HttpWebRequest httpWebRequestWarehouse = (HttpWebRequest)WebRequest.Create(urlWarehouse);
                        HttpWebResponse httpWebResponseWarehouse = (HttpWebResponse)httpWebRequestWarehouse.GetResponse();
                        string responseWarehouse;

                        using (StreamReader streamReader = new StreamReader(httpWebResponseWarehouse.GetResponseStream()))
                        {
                            responseWarehouse = streamReader.ReadToEnd();
                        }

                        Warehouse warehouse = JsonConvert.DeserializeObject<Warehouse>(responseWarehouse);

                        var endpointWarehouse = new Uri(uri + "/warehouse");
                        var newWarehouse = new Warehouse()
                        {
                            id = Convert.ToInt32((warehouseidinfo.Column.GetCellContent(warehouseidinfo.Item) as TextBlock).Text),
                            warehousecell = warehouse.warehousecell,
                            productavailability = true,
                        };

                        var newLocationWarehouseJson = JsonConvert.SerializeObject(newLocationWarehouse);
                        var payload = new StringContent(newLocationWarehouseJson, Encoding.UTF8, "application/json");
                        var result = client.PostAsync(endpoint, payload).Result.ToString();

                        var newWarehouseJson = JsonConvert.SerializeObject(newWarehouse);
                        var payloadWarehouse = new StringContent(newWarehouseJson, Encoding.UTF8, "application/json");
                        var resultWarehouse = client.PutAsync(endpointWarehouse, payloadWarehouse).Result.ToString();
                        if (resultWarehouse.StartsWith("StatusCode: 500"))
                        {
                            MessageBox.Show("Действие невозможно!");
                        }
                        if (result.StartsWith("StatusCode: 500"))
                        {
                            MessageBox.Show("Действие невозможно!");
                        }
                        initDataGrid();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            else
            {
                MessageBox.Show("Выберите строку в таблице или введите значение!");
            }
        }

        private void btnCancle_Click(object sender, RoutedEventArgs e)
        {
            if (dtDeleveries.SelectedItems.Count != 0)
            {
                var cellinfo = dtDeleveries.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + $"/delivery/{Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text)}");
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
