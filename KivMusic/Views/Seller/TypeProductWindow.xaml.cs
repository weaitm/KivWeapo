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
    /// Логика взаимодействия для TypeProductWindow.xaml
    /// </summary>
    public partial class TypeProductWindow : Page
    {
        ObservableCollection<ProductsType> productsTypes = new ObservableCollection<ProductsType>();
        ConAPI conAPI = new ConAPI();
        string uri;
        public TypeProductWindow()
        {
            InitializeComponent();
            uri = conAPI.ReadConJson();
            initDataGrid();
        }

        public class ProductsType
        {
            public int id { get; set; }
            public string typename { get; set; }
        }

        public void initDataGrid()
        {

            try
            {
                productsTypes.Clear();
                string url = uri + $"/producttype";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locProductTypes = JsonConvert.DeserializeObject<List<ProductsType>>(response);

                foreach (ProductsType type in locProductTypes)
                {
                    productsTypes.Add(type);
                }
                dtProductType.ItemsSource = productsTypes;
                dtProductType.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dtProductType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtProductType.SelectedItems.Count != 0)
                {
                    var cellinfo = dtProductType.SelectedCells[1];
                    txtTypeProduct.Text = (cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text;
                }
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

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (dtProductType.SelectedItems.Count != 0)
            {
                var cellinfo = dtProductType.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + $"/producttype/{Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text)}");
                    var result = client.DeleteAsync(endpoint).Result.ToString();
                    initDataGrid();
                }
            }
            else
            {
                MessageBox.Show("Выберите строку в таблице");
            }
        }

        private void btnUpd_Click(object sender, RoutedEventArgs e)
        {
            if (dtProductType.SelectedItems.Count != 0)
            {
                var cellinfo = dtProductType.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + "/producttype");
                    var newProductType = new ProductType()
                    {
                        id = Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text),
                        typename = txtTypeProduct.Text.Trim(),
                    };

                    var newProductTypeJson = JsonConvert.SerializeObject(newProductType);
                    var payload = new StringContent(newProductTypeJson, Encoding.UTF8, "application/json");
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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtTypeProduct.Text.Trim().Length == 0)
            {
                MessageBox.Show("Введите значния!");
            }
            else
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var endpoint = new Uri(uri + "/producttype");
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                        var newProductType = new ProductType()
                        {
                            typename = txtTypeProduct.Text.Trim(),
                        };

                        var newProductTypeJson = JsonConvert.SerializeObject(newProductType);
                        var payload = new StringContent(newProductTypeJson, Encoding.UTF8, "application/json");
                        var result = client.PostAsync(endpoint, payload).Result.ToString();

                        if (result.StartsWith("StatusCode: 500"))
                        {
                            MessageBox.Show("Такой тип продукта уже создан");
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
    }
}
