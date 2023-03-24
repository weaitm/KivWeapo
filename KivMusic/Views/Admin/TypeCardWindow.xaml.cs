using KivMusic.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
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
using System.Collections.ObjectModel;
using System.Dynamic;

namespace KivMusic.Views.Admin
{
    /// <summary>
    /// Логика взаимодействия для TypeCardWindow.xaml
    /// </summary>
    public partial class TypeCardWindow : Page
    {
        ConAPI conAPI = new ConAPI();
        string uri;
        ObservableCollection<TypeCards> typeCards = new ObservableCollection<TypeCards>();
        public TypeCardWindow()
        {
            InitializeComponent();
            uri = conAPI.ReadConJson();
            initDataGrid();
        }

        public class TypeCards
        {
            public int id { get; set; }
            public string typename { get; set; }
        }

        public void initDataGrid()
        {

            try
            {
                typeCards.Clear();
                string url = uri + $"/typecard";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var loctypeCards = JsonConvert.DeserializeObject<List<TypeCards>>(response);

                foreach(TypeCards card in loctypeCards)
                {
                    typeCards.Add(card);
                }
                dtTypeCard.ItemsSource = typeCards;
                dtTypeCard.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtTypeCard.Text.Trim().Length == 0)
            {
                MessageBox.Show("Введите значния!");
            }
            else
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var endpoint = new Uri(uri + "/typecard");
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                        var newTypeCard = new TypeCard()
                        {
                            typename = txtTypeCard.Text.Trim(),
                        };

                        var newTypeCardJson = JsonConvert.SerializeObject(newTypeCard);
                        var payload = new StringContent(newTypeCardJson, Encoding.UTF8, "application/json");
                        var result = client.PostAsync(endpoint, payload).Result.ToString();

                        if (result.StartsWith("StatusCode: 500"))
                        {
                            MessageBox.Show("Такой тип карты уже создан");
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
            if(dtTypeCard.SelectedItems.Count != 0)
            {
                var cellinfo = dtTypeCard.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + "/typecard");
                    var newTypeCard = new TypeCard()
                    {
                        id = Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text),
                        typename = txtTypeCard.Text.Trim(),
                    };

                    var newTypeCardJson = JsonConvert.SerializeObject(newTypeCard);
                    var payload = new StringContent(newTypeCardJson, Encoding.UTF8, "application/json");
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
            if (dtTypeCard.SelectedItems.Count != 0)
            {
                var cellinfo = dtTypeCard.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + $"/typecard/{Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text)}");
                    var result = client.DeleteAsync(endpoint).Result.ToString();
                    initDataGrid();
                }
            }
            else
            {
                MessageBox.Show("Выберите строку в таблице");
            }
        }

        private void dtTypeCard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtTypeCard.SelectedItems.Count != 0)
                {
                    var cellinfo = dtTypeCard.SelectedCells[1];
                    txtTypeCard.Text = (cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text;
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
    }
}
