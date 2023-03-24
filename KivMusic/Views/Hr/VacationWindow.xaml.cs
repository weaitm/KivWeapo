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
using static KivMusic.Views.Hr.SickWindow;

namespace KivMusic.Views.Hr
{
    /// <summary>
    /// Логика взаимодействия для VacationWindow.xaml
    /// </summary>
    public partial class VacationWindow : Page
    {
        ObservableCollection<Vacations> vacations = new ObservableCollection<Vacations>();
        ObservableCollection<Employees> employees = new ObservableCollection<Employees>();
        ObservableCollection<VacationTypes> vacationTypes = new ObservableCollection<VacationTypes>();
        int myid;
        ConAPI conAPI = new ConAPI();
        string uri;
        public VacationWindow(int myid)
        {
            InitializeComponent();
            this.myid = myid;
            uri = conAPI.ReadConJson();
            DataContext = this;
            initDataGrid();
            initComboboxEmployees();
            initComboboxVacationTypes();
        }

        public class Vacations
        {
            public int id { get; set; }
            public DateTime startdate { get; set; }
            public DateTime enddate { get; set; }
            public int employeeid { get; set; }
            public string employeeFIO { get; set; }
            public int hrmanagerid { get; set; }
            public string hrmanagerFIO { get; set; }
            public int vacationtypeid { get; set; }
            public string vacationtypename { get; set; }
        }

        public class Employees
        {
            public int id { get; set; }
            public string fullname { get; set; }
        }

        public class VacationTypes
        {
            public int id { get; set; }
            public string typename { get; set; }
        }

        public void initDataGrid()
        {

            try
            {
                vacations.Clear();
                string url = uri + $"/vacation";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locVacation = JsonConvert.DeserializeObject<List<Vacation>>(response);

                foreach (Vacation vacation in locVacation)
                {
                    string urlemployee = uri + $"/profile/{vacation.employeeid}";
                    HttpWebRequest httpWebRequestEmployee = (HttpWebRequest)WebRequest.Create(urlemployee);
                    HttpWebResponse httpWebResponseEmployee = (HttpWebResponse)httpWebRequestEmployee.GetResponse();
                    string responseEmployee;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseEmployee.GetResponseStream()))
                    {
                        responseEmployee = streamReader.ReadToEnd();
                    }

                    Profile employee = JsonConvert.DeserializeObject<Profile>(responseEmployee);

                    string urlHr = uri + $"/profile/{vacation.hrmanagerid}";
                    HttpWebRequest httpWebRequestHr = (HttpWebRequest)WebRequest.Create(urlHr);
                    HttpWebResponse httpWebResponseHr = (HttpWebResponse)httpWebRequestHr.GetResponse();
                    string responseHr;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseHr.GetResponseStream()))
                    {
                        responseHr = streamReader.ReadToEnd();
                    }

                    Profile hr = JsonConvert.DeserializeObject<Profile>(responseHr);

                    string urlVacationType = uri + $"/vacationtype/{vacation.vacationtypeid}";
                    HttpWebRequest httpWebRequestVacationType = (HttpWebRequest)WebRequest.Create(urlVacationType);
                    HttpWebResponse httpWebResponseVacationType = (HttpWebResponse)httpWebRequestVacationType.GetResponse();
                    string responseVacationType;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseVacationType.GetResponseStream()))
                    {
                        responseVacationType = streamReader.ReadToEnd();
                    }

                    VacationType vacationType = JsonConvert.DeserializeObject<VacationType>(responseVacationType);

                    vacations.Add(new Vacations
                    {
                        id = vacation.id,
                        startdate = vacation.startvacationdate,
                        enddate = vacation.endvacationdate,
                        employeeid = vacation.employeeid,
                        employeeFIO = $"{employee.lastname} {employee.firstname.Substring(0, 1)}. {employee.middlename.Substring(0, 1)}.",
                        hrmanagerid = vacation.hrmanagerid,
                        hrmanagerFIO = $"{hr.lastname} {hr.firstname.Substring(0, 1)}. {hr.middlename.Substring(0, 1)}.",
                        vacationtypeid = vacation.vacationtypeid,
                        vacationtypename = vacationType.typename,
                    });
                }
                dtVacation.ItemsSource = vacations;
                dtVacation.Items.Refresh();
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

                    if (role.rolename == "user")
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

        public void initComboboxVacationTypes()
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

                var locVacation = JsonConvert.DeserializeObject<List<VacationType>>(response);

                foreach (VacationType type in locVacation)
                {
                    vacationTypes.Add(new VacationTypes
                    {
                        id = type.id,
                        typename = type.typename,
                    });
                }
                cmbVacation.ItemsSource = vacationTypes;
                cmbVacation.Items.Refresh();
                cmbVacation.SelectedIndex = 0;
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

        private void dtVacation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtVacation.SelectedItems.Count != 0)
                {
                    var employeeidInfo = dtVacation.SelectedCells[4];
                    var vacationtypeidInfo = dtVacation.SelectedCells[8];
                    cmbEmployee.SelectedValue = Convert.ToInt32((employeeidInfo.Column.GetCellContent(employeeidInfo.Item) as TextBlock).Text);
                    cmbVacation.SelectedValue = Convert.ToInt32((vacationtypeidInfo.Column.GetCellContent(vacationtypeidInfo.Item) as TextBlock).Text);
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
            else if (dpStart.SelectedDate < DateTime.Today && dpEnd.SelectedDate < dpStart.SelectedDate && dpEnd.SelectedDate < DateTime.Today)
            {
                MessageBox.Show("Неверное значение у даты");
            }
            else
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var endpoint = new Uri(uri + "/vacation");
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                        var newVacation = new Vacation()
                        {
                            startvacationdate = DateTime.Parse(dpStart.SelectedDate.Value.ToShortDateString()),
                            endvacationdate = DateTime.Parse(dpStart.SelectedDate.Value.ToShortDateString()),
                            employeeid = Convert.ToInt32(cmbEmployee.SelectedValue),
                            hrmanagerid = myid,
                            vacationtypeid = Convert.ToInt32(cmbVacation.SelectedValue)
                        };

                        var newVacationJson = JsonConvert.SerializeObject(newVacation);
                        var payload = new StringContent(newVacationJson, Encoding.UTF8, "application/json");
                        var result = client.PostAsync(endpoint, payload).Result.ToString();

                        if (result.StartsWith("StatusCode: 500"))
                        {
                            MessageBox.Show("Такой отпуск уже создан");
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
            if (dtVacation.SelectedItems.Count != 0)
            {
                if(dpStart.SelectedDate != null && dpEnd.SelectedDate != null)
                {
                    var cellinfo = dtVacation.SelectedCells[0];
                    using (var client = new HttpClient())
                    {
                        var endpoint = new Uri(uri + "/vacation");
                        var newVacation = new Vacation()
                        {
                            id = Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text),
                            startvacationdate = DateTime.Parse(dpStart.SelectedDate.Value.ToShortDateString()),
                            endvacationdate = DateTime.Parse(dpStart.SelectedDate.Value.ToShortDateString()),
                            employeeid = Convert.ToInt32(cmbEmployee.SelectedValue),
                            hrmanagerid = myid,
                            vacationtypeid = Convert.ToInt32(cmbVacation.SelectedValue)
                        };

                        var newVacationJson = JsonConvert.SerializeObject(newVacation);
                        var payload = new StringContent(newVacationJson, Encoding.UTF8, "application/json");
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
            if (dtVacation.SelectedItems.Count != 0)
            {
                var cellinfo = dtVacation.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + $"/vacation/{Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text)}");
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
