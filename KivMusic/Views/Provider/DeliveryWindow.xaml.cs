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
using static KivMusic.Views.Admin.AddUserWindow;
using static KivMusic.Views.Keeper.KeeperWindow;
using static KivMusic.Views.Keeper.SalaryWindow;

namespace KivMusic.Views.Provider
{
    /// <summary>
    /// Логика взаимодействия для DeliveryWindow.xaml
    /// </summary>
    public partial class DeliveryWindow : Page
    {
        ObservableCollection<Warehouses> warehouses = new ObservableCollection<Warehouses>();
        ObservableCollection<Products> products = new ObservableCollection<Products>();
        ObservableCollection<Providers> providers = new ObservableCollection<Providers>();
        ObservableCollection<Deliveries> deliveries = new ObservableCollection<Deliveries>();
        int myid;
        ConAPI conAPI = new ConAPI();
        string uri;
        public DeliveryWindow(int myid)
        {
            InitializeComponent();
            this.myid = myid;
            uri = conAPI.ReadConJson();
            DataContext = this;
            initDataGrid();
            initComboboxWarehouse();
            initComboboxProduct();
            initComboboxProvider();
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
        public class Warehouses
        {
            public int id { get; set; }
            public string cell { get; set; }
        }

        public class Products
        {
            public int id { get; set; }
            public string productname { get; set; }
        }

        public class Providers
        {
            public int id { get; set; }
            public string providersname { get; set; }
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

                    string urlWarehouse= uri + $"/warehouse/{delivery.warehouseid}";
                    HttpWebRequest httpWebRequestWarehouse = (HttpWebRequest)WebRequest.Create(urlWarehouse);
                    HttpWebResponse httpWebResponseWarehouse = (HttpWebResponse)httpWebRequestWarehouse.GetResponse();
                    string responseWarehouse;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseWarehouse.GetResponseStream()))
                    {
                        responseWarehouse = streamReader.ReadToEnd();
                    }

                    Warehouse warehouse = JsonConvert.DeserializeObject<Warehouse>(responseWarehouse);

                    if(warehouse.productavailability == true)
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
                dtDelevery.ItemsSource = deliveries;
                dtDelevery.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void initComboboxWarehouse()
        {
            try
            {
                warehouses.Clear();
                string url = uri + $"/warehouse";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locWarehouse = JsonConvert.DeserializeObject<List<Warehouse>>(response);

                foreach (Warehouse warehouse in locWarehouse)
                {
                    if(warehouse.productavailability == true)
                    {

                    }
                    else
                    {
                        warehouses.Add(new Warehouses
                        {
                            id = warehouse.id,
                            cell = warehouse.warehousecell,
                        });
                    }
                }
                cmbWarehouse.ItemsSource = warehouses;
                cmbWarehouse.Items.Refresh();
                cmbWarehouse.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void initComboboxProduct()
        {
            try
            {
                products.Clear();
                string url = uri + $"/product";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locProduct = JsonConvert.DeserializeObject<List<Product>>(response);

                foreach (Product product in locProduct)
                {
                    products.Add(new Products
                    {
                        id = product.id,
                        productname = product.productname,
                    });
                }
                cmbProduct.ItemsSource = products;
                cmbProduct.Items.Refresh();
                cmbProduct.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void initComboboxProvider()
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

                var locProvider = JsonConvert.DeserializeObject<List<Providery>>(response);

                foreach (Providery provider in locProvider)
                {
                    providers.Add(new Providers
                    {
                        id = provider.id,
                        providersname = provider.nameprovider,
                    });
                }
                cmbProvider.ItemsSource = providers;
                cmbProvider.Items.Refresh();
                cmbProvider.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void dtDelevery_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtDelevery.SelectedItems.Count != 0)
                {
                    var warehouseInfo = dtDelevery.SelectedCells[2];
                    var productInfo = dtDelevery.SelectedCells[6];
                    var providerInfo = dtDelevery.SelectedCells[8];
                    cmbWarehouse.SelectedValue = Convert.ToInt32((warehouseInfo.Column.GetCellContent(warehouseInfo.Item) as TextBlock).Text);
                    cmbProduct.SelectedValue = Convert.ToInt32((productInfo.Column.GetCellContent(productInfo.Item) as TextBlock).Text);
                    cmbProvider.SelectedValue = Convert.ToInt32((providerInfo.Column.GetCellContent(providerInfo.Item) as TextBlock).Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + "/delivery");
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    var newDelivery = new Delivery()
                    {
                        warehouseid = Convert.ToInt32(cmbWarehouse.SelectedValue),
                        profileid = myid,
                        productid = Convert.ToInt32(cmbProduct.SelectedValue),
                        providerid = Convert.ToInt32(cmbProvider.SelectedValue),
                    };

                    var newDeliveryJson = JsonConvert.SerializeObject(newDelivery);
                    var payload = new StringContent(newDeliveryJson, Encoding.UTF8, "application/json");
                    var result = client.PostAsync(endpoint, payload).Result.ToString();

                    if (result.StartsWith("StatusCode: 500"))
                    {
                        MessageBox.Show("Такая зарплата уже создана");
                    }
                    initDataGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnUpd_Click(object sender, RoutedEventArgs e)
        {
            if (dtDelevery.SelectedItems.Count != 0)
            {
                var cellinfo = dtDelevery.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + "/delivery");
                    var newDelivery = new Delivery()
                    {
                        id = Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text),
                        warehouseid = Convert.ToInt32(cmbWarehouse.SelectedValue),
                        profileid = myid,
                        productid = Convert.ToInt32(cmbProduct.SelectedValue),
                        providerid = Convert.ToInt32(cmbProvider.SelectedValue),
                    };

                    var newDeliveryJson = JsonConvert.SerializeObject(newDelivery);
                    var payload = new StringContent(newDeliveryJson, Encoding.UTF8, "application/json");
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
            if (dtDelevery.SelectedItems.Count != 0)
            {
                var cellinfo = dtDelevery.SelectedCells[0];
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
