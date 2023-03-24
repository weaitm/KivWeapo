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
using static KivMusic.Views.Admin.BankWindow;

namespace KivMusic.Views.Admin
{
    /// <summary>
    /// Логика взаимодействия для PaymentSystemWindow.xaml
    /// </summary>
    public partial class PaymentSystemWindow : Page
    {
        ObservableCollection<PaymentSystems> paymentSystems = new ObservableCollection<PaymentSystems>();
        ConAPI conAPI = new ConAPI();
        string uri;
        public PaymentSystemWindow()
        {
            InitializeComponent();
            uri = conAPI.ReadConJson();
            initDataGrid();
        }

        public class PaymentSystems
        {
            public int id { get; set; }
            public string namesystem { get; set; }
        }

        public void initDataGrid()
        {

            try
            {
                paymentSystems.Clear();
                string url = uri + $"/paymentsystem";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locPayments = JsonConvert.DeserializeObject<List<PaymentSystem>>(response);

                foreach (PaymentSystem payments in locPayments)
                {
                    paymentSystems.Add(new PaymentSystems
                    {
                        id = payments.id,
                        namesystem = payments.namesystem,
                    });
                }
                dtPaymentSystem.ItemsSource = paymentSystems;
                dtPaymentSystem.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtPaymentName.Text.Trim().Length == 0)
            {
                MessageBox.Show("Введите значния!");
            }
            else
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var endpoint = new Uri(uri + "/paymentsystem");
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                        var newPayment = new PaymentSystem()
                        {
                            namesystem = txtPaymentName.Text.Trim(),
                        };

                        var newPaymentJson = JsonConvert.SerializeObject(newPayment);
                        var payload = new StringContent(newPaymentJson, Encoding.UTF8, "application/json");
                        var result = client.PostAsync(endpoint, payload).Result.ToString();

                        if (result.StartsWith("StatusCode: 500"))
                        {
                            MessageBox.Show("Такая система уже создана");
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
            if (dtPaymentSystem.SelectedItems.Count != 0)
            {
                var cellinfo = dtPaymentSystem.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + "/paymentsystem");
                    var newPayment = new PaymentSystem()
                    {
                        id = Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text),
                        namesystem = txtPaymentName.Text.Trim(),
                    };

                    var newPaymentJson = JsonConvert.SerializeObject(newPayment);
                    var payload = new StringContent(newPaymentJson, Encoding.UTF8, "application/json");
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
            if (dtPaymentSystem.SelectedItems.Count != 0)
            {
                var cellinfo = dtPaymentSystem.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + $"/paymentsystem/{Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text)}");
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

        private void dtPaymentSystem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtPaymentSystem.SelectedItems.Count != 0)
                {
                    var cellinfo = dtPaymentSystem.SelectedCells[1];
                    txtPaymentName.Text = (cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
