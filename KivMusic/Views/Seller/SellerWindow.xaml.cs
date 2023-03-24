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
using static KivMusic.Views.Provider.DeliveryWindow;
using static KivMusic.Views.Seller.ProductWindow;

namespace KivMusic.Views.Seller
{
    /// <summary>
    /// Логика взаимодействия для SellerWindow.xaml
    /// </summary>
    public partial class SellerWindow : Page
    {
        ObservableCollection<ProductChars> productChars = new ObservableCollection<ProductChars>();
        ObservableCollection<Products> products = new ObservableCollection<Products>();
        ObservableCollection<Characteristics> characteristics = new ObservableCollection<Characteristics>();
        int myid;
        ConAPI conAPI = new ConAPI();
        string uri;
        public SellerWindow(int myid)
        {
            InitializeComponent();
            this.myid = myid;
            uri = conAPI.ReadConJson();
            DataContext = this;
            initDataGrid();
            initComboboxProduct();
            initComboboxCharacteristic();
            Loaded += Page_Loaded;
        }

        public class ProductChars
        {
            public int id { get; set; }
            public string product { get; set; }
            public int productid { get; set; }
            public string characteristicz { get; set; }
            public int characteristiczid { get; set; }
        }

        public class Products
        {
            public int id { get; set; }
            public string productname { get; set; }
        }

        public class Characteristics
        {
            public int id { get; set; }
            public string characteristicsname { get; set; }
        }

        public void initDataGrid()
        {

            try
            {
                productChars.Clear();
                string url = uri + $"/productcharacteristicz";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locProductCharacteristicz = JsonConvert.DeserializeObject<List<ProductCharacteristicz>>(response);

                foreach (ProductCharacteristicz productCharsacteristics in locProductCharacteristicz)
                {
                    string urlProduct = uri + $"/product/{productCharsacteristics.productid}";
                    HttpWebRequest httpWebRequestProduct = (HttpWebRequest)WebRequest.Create(urlProduct);
                    HttpWebResponse httpWebResponseProduct = (HttpWebResponse)httpWebRequestProduct.GetResponse();
                    string responseProduct;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseProduct.GetResponseStream()))
                    {
                        responseProduct = streamReader.ReadToEnd();
                    }

                    Product product = JsonConvert.DeserializeObject<Product>(responseProduct);


                    string urlCharacteristicz = uri + $"/characteristicz/{productCharsacteristics.characteristiczid}";
                    HttpWebRequest httpWebRequestCharacteristicz = (HttpWebRequest)WebRequest.Create(urlCharacteristicz);
                    HttpWebResponse httpWebResponseCharacteristicz = (HttpWebResponse)httpWebRequestCharacteristicz.GetResponse();
                    string responseCharacteristicz;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseCharacteristicz.GetResponseStream()))
                    {
                        responseCharacteristicz = streamReader.ReadToEnd();
                    }

                    Characteristicz characteristicz = JsonConvert.DeserializeObject<Characteristicz>(responseCharacteristicz);


                    productChars.Add(new ProductChars
                    {
                        id = productCharsacteristics.id,
                        product = product.productname,
                        productid = productCharsacteristics.productid,
                        characteristicz = characteristicz.namecharacteristicz,
                        characteristiczid = productCharsacteristics.characteristiczid,

                    });
                }
                dtProductChar.ItemsSource = productChars;
                dtProductChar.Items.Refresh();
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

        public void initComboboxCharacteristic()
        {
            try
            {
                characteristics.Clear();
                string url = uri + $"/characteristicz";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locCharacteristicz = JsonConvert.DeserializeObject<List<Characteristicz>>(response);

                foreach (Characteristicz chars in locCharacteristicz)
                {
                    characteristics.Add(new Characteristics
                    {
                        id = chars.id,
                        characteristicsname = chars.namecharacteristicz,
                    });
                }
                cmbCharacteristicz.ItemsSource = characteristics;
                cmbCharacteristicz.Items.Refresh();
                cmbCharacteristicz.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dtProductChar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtProductChar.SelectedItems.Count != 0)
                {
                    var productidInfo = dtProductChar.SelectedCells[2];
                    var characteristiczidInfo = dtProductChar.SelectedCells[4];
                    cmbProduct.SelectedValue = Convert.ToInt32((productidInfo.Column.GetCellContent(productidInfo.Item) as TextBlock).Text);
                    cmbCharacteristicz.SelectedValue = Convert.ToInt32((characteristiczidInfo.Column.GetCellContent(characteristiczidInfo.Item) as TextBlock).Text);
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
                    var endpoint = new Uri(uri + "/productcharacteristicz");
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    var newProductCharacteristicz = new ProductCharacteristicz()
                    {
                        productid = Convert.ToInt32(cmbProduct.SelectedValue),
                        characteristiczid = Convert.ToInt32(cmbCharacteristicz.SelectedValue),
                    };

                    var newProductCharacteristiczJson = JsonConvert.SerializeObject(newProductCharacteristicz);
                    var payload = new StringContent(newProductCharacteristiczJson, Encoding.UTF8, "application/json");
                    var result = client.PostAsync(endpoint, payload).Result.ToString();

                    if (result.StartsWith("StatusCode: 500"))
                    {
                        MessageBox.Show("Такой товар уже создана!");
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
            if (dtProductChar.SelectedItems.Count != 0)
            {
                var cellinfo = dtProductChar.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + "/productcharacteristicz");
                    var newProductCharacteristicz = new ProductCharacteristicz()
                    {
                        id = Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text),
                        productid = Convert.ToInt32(cmbProduct.SelectedValue),
                        characteristiczid = Convert.ToInt32(cmbCharacteristicz.SelectedValue),
                    };

                    var newProductCharacteristiczJson = JsonConvert.SerializeObject(newProductCharacteristicz);
                    var payload = new StringContent(newProductCharacteristiczJson, Encoding.UTF8, "application/json");
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
            if (dtProductChar.SelectedItems.Count != 0)
            {
                var cellinfo = dtProductChar.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + $"/productcharacteristicz/{Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text)}");
                    var result = client.DeleteAsync(endpoint).Result.ToString();
                    initDataGrid();
                }
            }
            else
            {
                MessageBox.Show("Выберите строку в таблице");
            }
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btnProductWin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProductWindow());
        }

        private void btnCharacteristiczWin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CharacteristicsWindow());
        }

        private void btnShopWin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ShopWindow());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            initDataGrid();
            initComboboxCharacteristic();
            initComboboxProduct();
        }

        private void btnChartWin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ChartWindow());
        }
    }
}
