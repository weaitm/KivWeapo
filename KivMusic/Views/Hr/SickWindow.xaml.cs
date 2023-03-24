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

namespace KivMusic.Views.Hr
{
    /// <summary>
    /// Логика взаимодействия для SickWindow.xaml
    /// </summary>
    public partial class SickWindow : Page
    {
        ConAPI conAPI = new ConAPI();
        string uri;
        ObservableCollection<Sicks> sicks = new ObservableCollection<Sicks>();
        ObservableCollection<Employees> employees = new ObservableCollection<Employees>();
        ObservableCollection<SickTypes> sickTypes = new ObservableCollection<SickTypes>();
        int myid;
        public SickWindow(int myid)
        {
            InitializeComponent();
            this.myid = myid;
            uri = conAPI.ReadConJson();
            DataContext = this;
            initDataGrid();
            initComboboxEmployees();
            initComboboxSickTypes();
        }

        public class Sicks
        {
            public int id { get; set; }
            public DateTime startdate { get; set; }
            public DateTime enddate { get; set; }
            public int employeeid { get; set; }
            public string employeeFIO { get; set; }
            public int hrmanagerid { get; set; }
            public string hrmanagerFIO { get; set; }
            public int sicktypeid { get; set; }
            public string sicktypename { get; set; }
        }

        public class Employees
        {
            public int id { get; set; }
            public string fullname { get; set; }
        }

        public class SickTypes
        {
            public int id { get; set; }
            public string typename { get; set; }
        }

        public void initDataGrid()
        {

            try
            {
                sicks.Clear();
                string url = uri + $"/sickleav";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locSickleav = JsonConvert.DeserializeObject<List<Sicks>>(response);

                foreach (Sicks sick in locSickleav)
                {
                    string urlemployee = uri + $"/profile/{sick.employeeid}";
                    HttpWebRequest httpWebRequestEmployee = (HttpWebRequest)WebRequest.Create(urlemployee);
                    HttpWebResponse httpWebResponseEmployee = (HttpWebResponse)httpWebRequestEmployee.GetResponse();
                    string responseEmployee;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseEmployee.GetResponseStream()))
                    {
                        responseEmployee = streamReader.ReadToEnd();
                    }

                    Profile employee = JsonConvert.DeserializeObject<Profile>(responseEmployee);

                    string urlHr = uri + $"/profile/{sick.hrmanagerid}";
                    HttpWebRequest httpWebRequestHr = (HttpWebRequest)WebRequest.Create(urlHr);
                    HttpWebResponse httpWebResponseHr = (HttpWebResponse)httpWebRequestHr.GetResponse();
                    string responseHr;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseHr.GetResponseStream()))
                    {
                        responseHr = streamReader.ReadToEnd();
                    }

                    Profile hr = JsonConvert.DeserializeObject<Profile>(responseHr);

                    string urlSickType = uri + $"/sicktype/{sick.sicktypeid}";
                    HttpWebRequest httpWebRequestSickType = (HttpWebRequest)WebRequest.Create(urlSickType);
                    HttpWebResponse httpWebResponseSickType = (HttpWebResponse)httpWebRequestSickType.GetResponse();
                    string responseSickType;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseSickType.GetResponseStream()))
                    {
                        responseSickType = streamReader.ReadToEnd();
                    }

                    SickType sickType = JsonConvert.DeserializeObject<SickType>(responseSickType);

                    sicks.Add(new Sicks
                    {
                        id = sick.id,
                        startdate = sick.startdate,
                        enddate = sick.enddate,
                        employeeid = sick.employeeid,
                        employeeFIO = $"{employee.lastname} {employee.firstname.Substring(0,1)}. {employee.middlename.Substring(0, 1)}.",
                        hrmanagerid = sick.hrmanagerid,
                        hrmanagerFIO = $"{hr.lastname} {hr.firstname.Substring(0, 1)}. {hr.middlename.Substring(0, 1)}.",
                        sicktypeid = sick.sicktypeid,
                        sicktypename = sickType.typename,
                    });
                }
                dtSicks.ItemsSource = sicks;
                dtSicks.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void initComboboxEmployees()
        {
            try
            {
                employees.Clear();
                string url = uri + $"/profile";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locEmployee = JsonConvert.DeserializeObject<List<Profile>>(response);

                foreach (Profile employee in locEmployee)
                {
                    string urlrole = uri + $"/role/{employee.rolesid}";
                    HttpWebRequest httpWebRequestRole = (HttpWebRequest)WebRequest.Create(urlrole);
                    HttpWebResponse httpWebResponseRole = (HttpWebResponse)httpWebRequestRole.GetResponse();
                    string responseRole;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseRole.GetResponseStream()))
                    {
                        responseRole = streamReader.ReadToEnd();
                    }

                    Role role = JsonConvert.DeserializeObject<Role>(responseRole);

                    if(role.rolename == "user")
                    {

                    }
                    else
                    {
                        employees.Add(new Employees
                        {
                            id = employee.id,
                            fullname = $"{employee.lastname} {employee.firstname.Substring(0, 1)}. {employee.middlename.Substring(0, 1)}."
                        });
                    }
                }
                cmbEmployee.ItemsSource = employees;
                cmbEmployee.Items.Refresh();
                cmbEmployee.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void initComboboxSickTypes()
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

                var locSickType = JsonConvert.DeserializeObject<List<SickType>>(response);

                foreach (SickType type in locSickType)
                {
                    sickTypes.Add(new SickTypes
                    {
                        id = type.id,
                        typename = type.typename,
                    });
                }
                cmbSickType.ItemsSource = sickTypes;
                cmbSickType.Items.Refresh();
                cmbSickType.SelectedIndex = 0;
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

        private void dtSicks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtSicks.SelectedItems.Count != 0)
                {
                    var employeeidInfo = dtSicks.SelectedCells[4];
                    var sicktypeidInfo = dtSicks.SelectedCells[8];
                    cmbEmployee.SelectedValue = Convert.ToInt32((employeeidInfo.Column.GetCellContent(employeeidInfo.Item) as TextBlock).Text);
                    cmbSickType.SelectedValue = Convert.ToInt32((sicktypeidInfo.Column.GetCellContent(sicktypeidInfo.Item) as TextBlock).Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (dpStart.SelectedDate == null && dpEnd.SelectedDate == null)
            {
                MessageBox.Show("Введите значния!");
            }
            else if(dpStart.SelectedDate < DateTime.Today && dpEnd.SelectedDate < dpStart.SelectedDate && dpEnd.SelectedDate < DateTime.Today)
            {
                MessageBox.Show("Неверное значение у даты");
            }
            else
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var endpoint = new Uri(uri + "/sickleav");
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                        var newSickLeav = new SickLeav()
                        {
                            startdate = DateTime.Parse(dpStart.SelectedDate.Value.ToShortDateString()),
                            enddate = DateTime.Parse(dpStart.SelectedDate.Value.ToShortDateString()),
                            employeeid = Convert.ToInt32(cmbEmployee.SelectedValue),
                            hrmanagerid = myid,
                            sicktypeid = Convert.ToInt32(cmbSickType.SelectedValue)
                        };

                        var newSickLeavJson = JsonConvert.SerializeObject(newSickLeav);
                        var payload = new StringContent(newSickLeavJson, Encoding.UTF8, "application/json");
                        var result = client.PostAsync(endpoint, payload).Result.ToString();

                        if (result.StartsWith("StatusCode: 500"))
                        {
                            MessageBox.Show("Такой больничный уже создан");
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
            if (dtSicks.SelectedItems.Count != 0)
            {
                if(dpStart.SelectedDate != null && dpEnd.SelectedDate != null)
                {
                    var cellinfo = dtSicks.SelectedCells[0];
                    using (var client = new HttpClient())
                    {
                        var endpoint = new Uri(uri + "/sickleav");
                        var newSickLeav = new SickLeav()
                        {
                            id = Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text),
                            startdate = DateTime.Parse(dpStart.SelectedDate.Value.ToShortDateString()),
                            enddate = DateTime.Parse(dpStart.SelectedDate.Value.ToShortDateString()),
                            employeeid = Convert.ToInt32(cmbEmployee.SelectedValue),
                            hrmanagerid = myid,
                            sicktypeid = Convert.ToInt32(cmbSickType.SelectedValue)
                        };

                        var newSickTypeJson = JsonConvert.SerializeObject(newSickLeav);
                        var payload = new StringContent(newSickTypeJson, Encoding.UTF8, "application/json");
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
                    MessageBox.Show("Выберите дату!");
                }
            }
            else
            {
                MessageBox.Show("Выберите строку в таблице");
            }
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (dtSicks.SelectedItems.Count != 0)
            {
                var cellinfo = dtSicks.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + $"/sickleav/{Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text)}");
                    var result = client.DeleteAsync(endpoint).Result.ToString();
                    initDataGrid();
                }
            }
            else
            {
                MessageBox.Show("Выберите строку в таблице");
            }
        }

    }
}
