using KivMusic.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace KivMusic.Views.User
{
    /// <summary>
    /// Логика взаимодействия для MainBuyWindow.xaml
    /// </summary>
    public partial class MainBuyWindow : Page
    {
        int myid;
        ObservableCollection<Products> products = new ObservableCollection<Products>();
        ObservableCollection<Products> productsFiltered = new ObservableCollection<Products>();
        ObservableCollection<ConsumerCarts> consumerCarts = new ObservableCollection<ConsumerCarts>();
        ObservableCollection<ProductTypes> productTypes = new ObservableCollection<ProductTypes>();
        ConAPI conAPI = new ConAPI();
        string uri;
        decimal sum = 0;
        public MainBuyWindow(int myid)
        {
            InitializeComponent();
            this.myid = myid;
            uri = conAPI.ReadConJson();
            DataContext = this;
            initDataGrid();
            initCombobox();
            lblSum.Content = sum;
        }

        public class Products
        {
            public int id { get; set; }
            public int productid { get; set; }
            public string productname { get; set; }
            public int characteristiczid { get; set; }
            public string characteristiczname { get; set; }
            public int productypeid { get; set; }
            public string productypename { get; set; }
            public decimal productprice { get; set; }

        }

        public class ConsumerCarts
        {
            public int id { get; set; }
            public string productname { get; set; }
            public decimal productprice { get; set; }
            public int quantityOfProduct { get; set; }
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
                string url = uri + $"/productcharacteristicz";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locProduct = JsonConvert.DeserializeObject<List<ProductCharacteristicz>>(response);

                foreach (ProductCharacteristicz product in locProduct)
                {
                    string urlProduct= uri + $"/product/{product.productid}";
                    HttpWebRequest httpWebRequestProduct = (HttpWebRequest)WebRequest.Create(urlProduct);
                    HttpWebResponse httpWebResponseProduct = (HttpWebResponse)httpWebRequestProduct.GetResponse();
                    string responseProduct;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseProduct.GetResponseStream()))
                    {
                        responseProduct = streamReader.ReadToEnd();
                    }

                    Product producty = JsonConvert.DeserializeObject<Product>(responseProduct);

                    string urlCharacteristicz = uri + $"/characteristicz/{product.characteristiczid}";
                    HttpWebRequest httpWebRequestCharacteristicz = (HttpWebRequest)WebRequest.Create(urlCharacteristicz);
                    HttpWebResponse httpWebResponseCharacteristicz = (HttpWebResponse)httpWebRequestCharacteristicz.GetResponse();
                    string responseCharacteristicz;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseCharacteristicz.GetResponseStream()))
                    {
                        responseCharacteristicz = streamReader.ReadToEnd();
                    }

                    Characteristicz characteristicz = JsonConvert.DeserializeObject<Characteristicz>(responseCharacteristicz);

                    string urlProductType = uri + $"/producttype/{producty.producttypeid}";
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
                        productid = product.productid,
                        productname = producty.productname,
                        characteristiczid = product.characteristiczid,
                        characteristiczname = characteristicz.namecharacteristicz,
                        productypeid = producty.producttypeid,
                        productypename = productType.typename,
                        productprice = producty.productprice,
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

                productTypes.Add(new ProductTypes
                {
                    id = -1,
                    typename = "Без фильтрации",
                });

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

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserSettings(myid));
        }

        private void dtProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtProducts.SelectedItems.Count != 0)
                {
                    var productnameInfo = dtProducts.SelectedCells[2];
                    var characteristicznameInfo = dtProducts.SelectedCells[4];
                    lblProductName.Content= (productnameInfo.Column.GetCellContent(productnameInfo.Item) as TextBlock).Text;
                    txtCharacteristics.Text = (characteristicznameInfo.Column.GetCellContent(characteristicznameInfo.Item) as TextBlock).Text;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Sign_in());
        }

        private void btnPay_Click(object sender, RoutedEventArgs e)
        {
            if(consumerCarts.Count == 0)
            {
                MessageBox.Show("Вы не выбрали товар!");
            }
            else
            {
                NavigationService.Navigate(new CheckOutWindow(myid, sum, consumerCarts));
            }
        }

        private void btnDelProduct_Click(object sender, RoutedEventArgs e)
        {
            if(dtConsumerCart.SelectedItems.Count != 0)
            {
                var idInfo = dtConsumerCart.SelectedCells[0];
                int idcart = Convert.ToInt32((idInfo.Column.GetCellContent(idInfo.Item) as TextBlock).Text);
                foreach(ConsumerCarts carts in consumerCarts)
                {
                    if(carts.id == idcart)
                    {
                        consumerCarts.Remove(carts);
                        break;
                    }
                }
            }
            dtConsumerCart.ItemsSource = consumerCarts;
            dtConsumerCart.Items.Refresh();
            CalcSum();
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dtConsumerCart.Items.Count != 0)
                {
                    dtConsumerCart.ItemsSource = null;
                }
                if (dtProducts.SelectedItems.Count != 0)
                {
                    string url = uri + $"/locationwarehouse";
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    string response;

                    using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        response = streamReader.ReadToEnd();
                    }

                    var locLocationwarehouse = JsonConvert.DeserializeObject<List<LocationWarehouse>>(response);

                    var productidInfo = dtProducts.SelectedCells[1];
                    var productnameInfo = dtProducts.SelectedCells[2];
                    var productpriceInfo = dtProducts.SelectedCells[7];

                    foreach (LocationWarehouse warehouse in locLocationwarehouse)
                    {
                        if(warehouse.productid == Convert.ToInt32((productidInfo.Column.GetCellContent(productidInfo.Item) as TextBlock).Text))
                        {
                            if(warehouse.quantityofgoodsonwarehouse != 0)
                            {
                                if (consumerCarts.Count == 0)
                                {
                                    string priceproduct = (productpriceInfo.Column.GetCellContent(productpriceInfo.Item) as TextBlock).Text;
                                    priceproduct = priceproduct.Replace(".", ",");

                                    consumerCarts.Add(new ConsumerCarts
                                    {
                                        id = Convert.ToInt32((productidInfo.Column.GetCellContent(productidInfo.Item) as TextBlock).Text),
                                        productname = (productnameInfo.Column.GetCellContent(productnameInfo.Item) as TextBlock).Text,
                                        productprice = Convert.ToDecimal(priceproduct),
                                        quantityOfProduct = 1,
                                    });
                                }
                                else
                                {
                                    foreach(ConsumerCarts carts in consumerCarts)
                                    {
                                        if(carts.id == Convert.ToInt32((productidInfo.Column.GetCellContent(productidInfo.Item) as TextBlock).Text))
                                        {
                                            carts.quantityOfProduct++;
                                        }
                                        else
                                        {
                                            string priceproduct = (productpriceInfo.Column.GetCellContent(productpriceInfo.Item) as TextBlock).Text;
                                            priceproduct = priceproduct.Replace(".", ",");

                                            consumerCarts.Add(new ConsumerCarts
                                            {
                                                id = Convert.ToInt32((productidInfo.Column.GetCellContent(productidInfo.Item) as TextBlock).Text),
                                                productname = (productnameInfo.Column.GetCellContent(productnameInfo.Item) as TextBlock).Text,
                                                productprice = Convert.ToDecimal(priceproduct),
                                                quantityOfProduct = 1,
                                            });
                                        }
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Данный товар кончился");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Данного товара нет на складе");
                        }
                    }
                    dtConsumerCart.ItemsSource = consumerCarts;
                    dtConsumerCart.Items.Refresh();
                    CalcSum();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dtConsumerCart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalcSum();
        }

        private void dtConsumerCart_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CalcSum();
        }

        private void cmbProductType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Convert.ToInt32(cmbProductType.SelectedValue) == -1)
            {
                initDataGrid();
            }
            else
            {
                productsFiltered.Clear();
                foreach (Products product in products)
                {
                    if (product.productypeid.Equals(Convert.ToInt32(cmbProductType.SelectedValue)))
                    {
                        productsFiltered.Add(product);
                    }
                }
                dtProducts.ItemsSource = productsFiltered;
            }
        }


        private void cmbSortByPrice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if(dtProducts != null)
                {
                    if (cmbSortByPrice.SelectedIndex == 0)
                    {
                        dtProducts.Items.SortDescriptions.Clear();
                    }
                    else if (cmbSortByPrice.SelectedIndex == 1)
                    {
                        SortDataGrid(dtProducts, 7, ListSortDirection.Ascending);
                    }
                    else if (cmbSortByPrice.SelectedIndex == 2)
                    {
                        SortDataGrid(dtProducts, 7, ListSortDirection.Descending);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public static void SortDataGrid(DataGrid dataGrid, int columnIndex = 0, ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            var column = dataGrid.Columns[columnIndex];

            dataGrid.Items.SortDescriptions.Clear();

            dataGrid.Items.SortDescriptions.Add(new SortDescription(column.SortMemberPath, sortDirection));

            foreach (var col in dataGrid.Columns)
            {
                col.SortDirection = null;
            }
            column.SortDirection = sortDirection;

            dataGrid.Items.Refresh();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSearch.Text.Trim().Length == 0)
            {
                initDataGrid();
            }
            else
            {
                productsFiltered.Clear();
                foreach (Products product in products)
                {
                    if (product.productname.Contains(txtSearch.Text))
                    {
                        productsFiltered.Add(product);
                    }
                }
                dtProducts.ItemsSource = productsFiltered;
            }
        }

        public void CalcSum()
        {
            try
            {
                string url = uri + $"/locationwarehouse";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locLocationwarehouse = JsonConvert.DeserializeObject<List<LocationWarehouse>>(response);

                

                sum = 0;
                for (int i = 0; i < dtConsumerCart.Items.Count; i++)
                {
                    foreach(LocationWarehouse warehouse in locLocationwarehouse)
                    {
                        dynamic datagrid = dtConsumerCart.Items[i];
                        if (warehouse.productid == Convert.ToInt32(datagrid.id))
                        {
                            if (Convert.ToInt32(datagrid.quantityOfProduct) <= warehouse.quantityofgoodsonwarehouse)
                            {
                                sum += (Convert.ToDecimal(datagrid.productprice)) * (Convert.ToInt32(datagrid.quantityOfProduct));
                            }
                            else
                            {
                                MessageBox.Show($"На складе {warehouse.quantityofgoodsonwarehouse}шт. - {datagrid.productname}");
                                datagrid.quantityOfProduct = 1;
                                CalcSum();
                            }
                        }
                    }
                }
                lblSum.Content = sum;
            }
            catch (Exception)
            {

            }
        }

        private void dtConsumerCart_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            CalcSum();
        }
    }
}
