using KivMusic.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
using static KivMusic.Views.Admin.TypeCardWindow;

namespace KivMusic.Views.Seller
{
    /// <summary>
    /// Логика взаимодействия для CharacteristicsWindow.xaml
    /// </summary>
    public partial class CharacteristicsWindow : Page
    {
        ObservableCollection<Characteristics> characteristics = new ObservableCollection<Characteristics>();
        ConAPI conAPI = new ConAPI();
        string uri;
        public CharacteristicsWindow()
        {
            InitializeComponent();
            uri = conAPI.ReadConJson();
            initDataGrid();
        }

        public class Characteristics
        {
            public int id { get; set; }
            public string namecharacteristicz { get; set; }
        }
        public void initDataGrid()
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

                var locCharacteristics = JsonConvert.DeserializeObject<List<Characteristics>>(response);

                foreach (Characteristics characteristic in locCharacteristics)
                {
                    characteristics.Add(characteristic);
                }
                dtCharacteristics.ItemsSource = characteristics;
                dtCharacteristics.Items.Refresh();
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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtCharacteristics.Text.Trim().Length == 0)
            {
                MessageBox.Show("Введите значния!");
            }
            else
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var endpoint = new Uri(uri + "/characteristicz");
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                        var newCharacteristics = new Characteristicz()
                        {
                            namecharacteristicz = txtCharacteristics.Text.Trim(),
                        };

                        var newCharacteristicsJson = JsonConvert.SerializeObject(newCharacteristics);
                        var payload = new StringContent(newCharacteristicsJson, Encoding.UTF8, "application/json");
                        var result = client.PostAsync(endpoint, payload).Result.ToString();

                        if (result.StartsWith("StatusCode: 500"))
                        {
                            MessageBox.Show("Такая характеристика уже создана");
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
            if (dtCharacteristics.SelectedItems.Count != 0)
            {
                var cellinfo = dtCharacteristics.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + "/characteristicz");
                    var newCharacteristics = new Characteristicz()
                    {
                        id = Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text),
                        namecharacteristicz = txtCharacteristics.Text.Trim(),
                    };

                    var newCharacteristicsJson = JsonConvert.SerializeObject(newCharacteristics);
                    var payload = new StringContent(newCharacteristicsJson, Encoding.UTF8, "application/json");
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
            if (dtCharacteristics.SelectedItems.Count != 0)
            {
                var cellinfo = dtCharacteristics.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + $"/characteristicz/{Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text)}");
                    var result = client.DeleteAsync(endpoint).Result.ToString();
                    initDataGrid();
                }
            }
            else
            {
                MessageBox.Show("Выберите строку в таблице");
            }
        }

        private void dtCharacteristics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtCharacteristics.SelectedItems.Count != 0)
                {
                    var cellinfo = dtCharacteristics.SelectedCells[1];
                    txtCharacteristics.Text = (cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
