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
using static KivMusic.Views.Stockman.DeleteLocWarehouseWindow;
using static KivMusic.Views.User.UserSettings;

namespace KivMusic.Views.Admin
{
    /// <summary>
    /// Логика взаимодействия для ProductHistoryWin.xaml
    /// </summary>
    public partial class ProductHistoryWin : Page
    {
        ConAPI conAPI = new ConAPI();
        string uri;
        ObservableCollection<ProductHistories> productHistories = new ObservableCollection<ProductHistories>();
        public ProductHistoryWin()
        {
            InitializeComponent();
            uri = conAPI.ReadConJson();
            initDataGrid();
        }

        public class ProductHistories
        {
            public int id { get; set; }
            public string statusrecord { get; set; }
            public string productinfo { get; set; }
            public string characteristiczinfo { get; set; }
            public DateTime dateCreate { get; set; }
        }

        public void initDataGrid()
        {

            try
            {
                productHistories.Clear();
                string url = uri + $"/producthistory";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locProductHistory = JsonConvert.DeserializeObject<List<ProductHistory>>(response);

                foreach (ProductHistory histories in locProductHistory)
                {
                    productHistories.Add(new ProductHistories
                    {
                        id = histories.id,
                        statusrecord = histories.statusrecord,
                        characteristiczinfo = histories.characteristiczinfo,
                        dateCreate = histories.dateCreate,
                        productinfo = histories.productinfo,
                    });
                }
                dtProductHistory.ItemsSource = productHistories;
                dtProductHistory.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (dtProductHistory.SelectedItems.Count != 0)
            {
                var cellinfo = dtProductHistory.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + $"/producthistory/{Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text)}");
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
