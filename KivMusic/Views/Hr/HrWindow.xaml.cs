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

namespace KivMusic.Views.Hr
{
    /// <summary>
    /// Логика взаимодействия для HrWindow.xaml
    /// </summary>
    public partial class HrWindow : Page
    {
        ObservableCollection<VacationTypes> vacationTypes = new ObservableCollection<VacationTypes>();
        ObservableCollection<SickTypes> sickTypes = new ObservableCollection<SickTypes>();
        ConAPI conAPI = new ConAPI();
        string uri;
        int myid;
        public HrWindow(int myid)
        {
            InitializeComponent();
            uri = conAPI.ReadConJson();
            initDataGridVacationType();
            initDataGridSick();
            this.myid = myid;
        }

        public class VacationTypes
        {
            public int id { get; set; }
            public string typename { get; set; }
        }

        public class SickTypes
        {
            public int id { get; set; }
            public string typename { get; set; }
        }

        public void initDataGridVacationType()
        {

            try
            {
                vacationTypes.Clear();
                string url = uri + $"/vacationtype";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locVacation = JsonConvert.DeserializeObject<List<VacationTypes>>(response);

                foreach (VacationTypes vacation in locVacation)
                {
                    vacationTypes.Add(vacation);
                }
                dtVacationType.ItemsSource = vacationTypes;
                dtVacationType.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void initDataGridSick()
        {

            try
            {
                sickTypes.Clear();
                string url = uri + $"/sicktype";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locSicks = JsonConvert.DeserializeObject<List<SickTypes>>(response);

                foreach (SickTypes sicks in locSicks)
                {
                    sickTypes.Add(sicks);
                }
                dtSickType.ItemsSource = sickTypes;
                dtSickType.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btnSickWin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SickWindow(myid));
        }

        private void btnVacationWin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new VacationWindow(myid));
        }

        private void dtSickType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtSickType.SelectedItems.Count != 0)
                {
                    var cellinfo = dtSickType.SelectedCells[1];
                    txtSickType.Text = (cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dtVacationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtVacationType.SelectedItems.Count != 0)
                {
                    var cellinfo = dtVacationType.SelectedCells[1];
                    txtVacationType.Text = (cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAddSick_Click(object sender, RoutedEventArgs e)
        {
            if (txtSickType.Text.Trim().Length == 0)
            {
                MessageBox.Show("Введите значния!");
            }
            else
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var endpoint = new Uri(uri + "/sicktype");
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                        var newSickType = new SickType()
                        {
                            typename = txtSickType.Text.Trim(),
                        };

                        var newSickTypeJson = JsonConvert.SerializeObject(newSickType);
                        var payload = new StringContent(newSickTypeJson, Encoding.UTF8, "application/json");
                        var result = client.PostAsync(endpoint, payload).Result.ToString();

                        if (result.StartsWith("StatusCode: 500"))
                        {
                            MessageBox.Show("Такой тип больничного уже создан");
                        }
                        initDataGridSick();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void btnUpdSick_Click(object sender, RoutedEventArgs e)
        {
            if (dtSickType.SelectedItems.Count != 0)
            {
                var cellinfo = dtSickType.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + "/sicktype");
                    var newSickType = new SickType()
                    {
                        id = Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text),
                        typename = txtSickType.Text.Trim(),
                    };

                    var newSickTypeJson = JsonConvert.SerializeObject(newSickType);
                    var payload = new StringContent(newSickTypeJson, Encoding.UTF8, "application/json");
                    var result = client.PutAsync(endpoint, payload).Result.ToString();

                    if (result.StartsWith("StatusCode: 500"))
                    {
                        MessageBox.Show("Нельзя изменить!");
                    }
                    initDataGridSick();
                }
            }
            else
            {
                MessageBox.Show("Выберите строку в таблице");
            }
        }

        private void btnDelSick_Click(object sender, RoutedEventArgs e)
        {
            if (dtSickType.SelectedItems.Count != 0)
            {
                var cellinfo = dtSickType.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + $"/sicktype/{Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text)}");
                    var result = client.DeleteAsync(endpoint).Result.ToString();
                    initDataGridSick();
                }
            }
            else
            {
                MessageBox.Show("Выберите строку в таблице");
            }
        }

        private void btnAddVacation_Click(object sender, RoutedEventArgs e)
        {
            if (txtVacationType.Text.Trim().Length == 0)
            {
                MessageBox.Show("Введите значния!");
            }
            else
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var endpoint = new Uri(uri + "/vacationtype");
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                        var newVacationType = new VacationType()
                        {
                            typename = txtVacationType.Text.Trim(),
                        };

                        var newVacationTypeJson = JsonConvert.SerializeObject(newVacationType);
                        var payload = new StringContent(newVacationTypeJson, Encoding.UTF8, "application/json");
                        var result = client.PostAsync(endpoint, payload).Result.ToString();

                        if (result.StartsWith("StatusCode: 500"))
                        {
                            MessageBox.Show("Такой тип карты уже создан");
                        }
                        initDataGridVacationType();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void btnUpdVacation_Click(object sender, RoutedEventArgs e)
        {
            if (dtVacationType.SelectedItems.Count != 0)
            {
                var cellinfo = dtVacationType.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + "/vacationtype");
                    var newVacationType = new VacationType()
                    {
                        id = Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text),
                        typename = txtVacationType.Text.Trim(),
                    };

                    var newVacationTypeJson = JsonConvert.SerializeObject(newVacationType);
                    var payload = new StringContent(newVacationTypeJson, Encoding.UTF8, "application/json");
                    var result = client.PutAsync(endpoint, payload).Result.ToString();

                    if (result.StartsWith("StatusCode: 500"))
                    {
                        MessageBox.Show("Нельзя изменить!");
                    }
                    initDataGridVacationType();
                }
            }
            else
            {
                MessageBox.Show("Выберите строку в таблице");
            }
        }

        private void btnDelVacation_Click(object sender, RoutedEventArgs e)
        {
            if (dtVacationType.SelectedItems.Count != 0)
            {
                var cellinfo = dtVacationType.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + $"/vacationtype/{Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text)}");
                    var result = client.DeleteAsync(endpoint).Result.ToString();
                    initDataGridVacationType();
                }
            }
            else
            {
                MessageBox.Show("Выберите строку в таблице");
            }
        }
    }
}
