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

namespace KivMusic.Views.Seller
{
    /// <summary>
    /// Логика взаимодействия для ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Page
    {
        ObservableCollection<Products> products = new ObservableCollection<Products>();
        ObservableCollection<ProductTypes> productTypes = new ObservableCollection<ProductTypes>();
        ConAPI conAPI = new ConAPI();
        string uri;
        public ProductWindow()
        {
            InitializeComponent();
            uri = conAPI.ReadConJson();
            DataContext = this;
            initDataGrid();
            initCombobox();
            Loaded += Page_Loaded;
        }

        public class Products
        {
            public int id { get; set; }
            public string productname { get; set; }
            public decimal productprice { get; set; }
            public int producttypeid { get; set; }
            public string producttype { get; set; }
        }

        public class ProductTypes
        {
            public int id { get; set; }
            public string typename { get; set; }
        }

        public void initDataGrid()
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

                var locProduct = JsonConvert.DeserializeObject<List<Products>>(response);

                foreach (Products product in locProduct)
                {
                    string urlProductType = uri + $"/producttype/{product.producttypeid}";
                    HttpWebRequest httpWebRequestProductType = (HttpWebRequest)WebRequest.Create(urlProductType);
                    HttpWebResponse httpWebResponseProductType = (HttpWebResponse)httpWebRequestProductType.GetResponse();
                    string responseProductType;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseProductType.GetResponseStream()))
                    {
                        responseProductType = streamReader.ReadToEnd();
                    }

                    ProductType productType = JsonConvert.DeserializeObject<ProductType>(responseProductType);

                    products.Add(new Products
                    {
                        id = product.id,
                        productname = product.productname,
                        productprice = product.productprice,
                        producttypeid = product.id,
                        producttype = productType.typename,
                    });
                }
                dtProducts.ItemsSource = products;
                dtProducts.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void initCombobox()
        {
            try
            {
                productTypes.Clear();
                string url = uri + $"/producttype";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locProductType = JsonConvert.DeserializeObject<List<ProductType>>(response);

                foreach (ProductType type in locProductType)
                {
                    productTypes.Add(new ProductTypes
                    {
                        id = type.id,
                        typename = type.typename,
                    });
                }
                cmbProductType.ItemsSource = productTypes;
                cmbProductType.Items.Refresh();
                cmbProductType.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btnProductTypeWin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TypeProductWindow());
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + "/product");
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    var newProduct = new Product()
                    {
                        productname = txtProductName.Text,
                        productprice = Convert.ToDecimal((txtProductPrice.Text).Replace(".", ",")),
                        producttypeid = Convert.ToInt32(cmbProductType.SelectedValue),
                    };

                    var newProductJson = JsonConvert.SerializeObject(newProduct);
                    var payload = new StringContent(newProductJson, Encoding.UTF8, "application/json");
                    var result = client.PostAsync(endpoint, payload).Result.ToString();

                    if (result.StartsWith("StatusCode: 500"))
                    {
                        MessageBox.Show("Такая продукт уже создана");
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
            if (dtProducts.SelectedItems.Count != 0)
            {
                var cellinfo = dtProducts.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + "/product");
                    var newProduct = new Product()
                    {
                        id = Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text),
                        productname = txtProductName.Text,
                        productprice = Convert.ToDecimal((txtProductPrice.Text).Replace(".", ",")),
                        producttypeid = Convert.ToInt32(cmbProductType.SelectedValue),
                    };

                    var newProductJson = JsonConvert.SerializeObject(newProduct);
                    var payload = new StringContent(newProductJson, Encoding.UTF8, "application/json");
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
            if (dtProducts.SelectedItems.Count != 0)
            {
                var cellinfo = dtProducts.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + $"/product/{Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text)}");
                    var result = client.DeleteAsync(endpoint).Result.ToString();
                    initDataGrid();
                }
            }
            else
            {
                MessageBox.Show("Выберите строку в таблице");
            }
        }

        private void dtProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtProducts.SelectedItems.Count != 0)
                {
                    var productnameInfo = dtProducts.SelectedCells[1];
                    var productpriceInfo = dtProducts.SelectedCells[2];
                    var producttypeidInfo = dtProducts.SelectedCells[4];

                    string productprice = (productpriceInfo.Column.GetCellContent(productpriceInfo.Item) as TextBlock).Text;

                    txtProductName.Text = (productnameInfo.Column.GetCellContent(productnameInfo.Item) as TextBlock).Text;
                    txtProductPrice.Text = productprice.Replace(".", ",");
                    cmbProductType.SelectedValue = Convert.ToInt32((producttypeidInfo.Column.GetCellContent(producttypeidInfo.Item) as TextBlock).Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            initDataGrid();
            initCombobox();
        }
    }
}
