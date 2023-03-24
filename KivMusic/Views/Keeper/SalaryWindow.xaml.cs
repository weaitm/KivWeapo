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

namespace KivMusic.Views.Keeper
{
    /// <summary>
    /// Логика взаимодействия для SalaryWindow.xaml
    /// </summary>
    public partial class SalaryWindow : Page
    {
        ObservableCollection<Salaries> salaries = new ObservableCollection<Salaries>();
        ObservableCollection<Employees> employees = new ObservableCollection<Employees>();
        ObservableCollection<PayTypes> payTypes = new ObservableCollection<PayTypes>();
        int myid;
        ConAPI conAPI = new ConAPI();
        string uri;
        public SalaryWindow(int myid)
        {
            InitializeComponent();
            this.myid = myid;
            uri = conAPI.ReadConJson();
            DataContext = this;
            initDataGrid();
            initComboboxEmployees();
            initComboboxPayTypes();

        }

        public class Salaries
        {
            public int id { get; set; }
            public DateTime paydate { get; set; }
            public decimal paysum { get; set; }
            public int employeeid { get; set; }
            public string employeeFIO { get; set; }
            public int bookkeeperid { get; set; }
            public string bookkeeperFIO { get; set; }
            public int paytypeid { get; set; }
            public string paytypename { get; set; }
        }

        public class Employees
        {
            public int id { get; set; }
            public string fullname { get; set; }
        }

        public class PayTypes
        {
            public int id { get; set; }
            public string typename { get; set; }
        }

        public void initDataGrid()
        {

            try
            {
                salaries.Clear();
                string url = uri + $"/salary";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locSalary = JsonConvert.DeserializeObject<List<Salaries>>(response);

                foreach (Salaries salary in locSalary)
                {
                    string urlemployee = uri + $"/profile/{salary.employeeid}";
                    HttpWebRequest httpWebRequestEmployee = (HttpWebRequest)WebRequest.Create(urlemployee);
                    HttpWebResponse httpWebResponseEmployee = (HttpWebResponse)httpWebRequestEmployee.GetResponse();
                    string responseEmployee;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseEmployee.GetResponseStream()))
                    {
                        responseEmployee = streamReader.ReadToEnd();
                    }

                    Profile employee = JsonConvert.DeserializeObject<Profile>(responseEmployee);

                    string urlKeeper = uri + $"/profile/{salary.bookkeeperid}";
                    HttpWebRequest httpWebRequestKeeper = (HttpWebRequest)WebRequest.Create(urlKeeper);
                    HttpWebResponse httpWebResponseKeeper = (HttpWebResponse)httpWebRequestKeeper.GetResponse();
                    string responseKeeper;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseKeeper.GetResponseStream()))
                    {
                        responseKeeper = streamReader.ReadToEnd();
                    }

                    Profile keeper = JsonConvert.DeserializeObject<Profile>(responseKeeper);

                    string urlPayType = uri + $"/paytype/{salary.paytypeid}";
                    HttpWebRequest httpWebRequestPayType = (HttpWebRequest)WebRequest.Create(urlPayType);
                    HttpWebResponse httpWebResponsePayType = (HttpWebResponse)httpWebRequestPayType.GetResponse();
                    string responsePayType;

                    using (StreamReader streamReader = new StreamReader(httpWebResponsePayType.GetResponseStream()))
                    {
                        responsePayType = streamReader.ReadToEnd();
                    }

                    PayType payType = JsonConvert.DeserializeObject<PayType>(responsePayType);

                    salaries.Add(new Salaries
                    {
                        id = salary.id,
                        paydate = salary.paydate,
                        paysum = salary.paysum,
                        employeeid = salary.employeeid,
                        employeeFIO = $"{employee.lastname} {employee.firstname.Substring(0, 1)}. {employee.middlename.Substring(0, 1)}.",
                        bookkeeperid = salary.bookkeeperid,
                        bookkeeperFIO = $"{keeper.lastname} {keeper.firstname.Substring(0, 1)}. {keeper.middlename.Substring(0, 1)}.",
                        paytypeid = salary.paytypeid,
                        paytypename = payType.typename,
                    });
                }
                dtSalaries.ItemsSource = salaries;
                dtSalaries.Items.Refresh();
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

        public void initComboboxPayTypes()
        {
            try
            {
                payTypes.Clear();
                string url = uri + $"/paytype";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locPayType = JsonConvert.DeserializeObject<List<PayType>>(response);

                foreach (PayType type in locPayType)
                {
                    payTypes.Add(new PayTypes
                    {
                        id = type.id,
                        typename = type.typename,
                    });
                }
                cmbSalaryType.ItemsSource = payTypes;
                cmbSalaryType.Items.Refresh();
                cmbSalaryType.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dtSalaries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtSalaries.SelectedItems.Count != 0)
                {
                    var paysum = dtSalaries.SelectedCells[2];
                    var employeeidInfo = dtSalaries.SelectedCells[4];
                    var paytypeidInfo = dtSalaries.SelectedCells[8];
                    txtSum.Text = (paysum.Column.GetCellContent(paysum.Item) as TextBlock).Text;
                    cmbEmployee.SelectedValue = Convert.ToInt32((employeeidInfo.Column.GetCellContent(employeeidInfo.Item) as TextBlock).Text);
                    cmbSalaryType.SelectedValue = Convert.ToInt32((paytypeidInfo.Column.GetCellContent(paytypeidInfo.Item) as TextBlock).Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (dpPayday.SelectedDate == null && txtSum.Text.Trim().Length == 0)
            {
                MessageBox.Show("Введите значния!");
            }
            else if (dpPayday.SelectedDate < DateTime.Today)
            {
                MessageBox.Show("Неверное значение у даты");
            }
            else
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var endpoint = new Uri(uri + "/salary");
                        var newSalary = new Salary()
                        {
                            paydate = DateTime.Parse(dpPayday.SelectedDate.Value.ToShortDateString()),
                            paysum = Convert.ToDecimal(txtSum.Text),
                            employeeid = Convert.ToInt32(cmbEmployee.SelectedValue),
                            bookkeeperid = myid,
                            paytypeid = Convert.ToInt32(cmbSalaryType.SelectedValue)
                        };

                        var newSalaryJson = JsonConvert.SerializeObject(newSalary);
                        var payload = new StringContent(newSalaryJson, Encoding.UTF8, "application/json");
                        var result = client.PostAsync(endpoint, payload).Result.ToString();

                        if (result.StartsWith("StatusCode: 500"))
                        {
                            MessageBox.Show("Такая зарплата уже создана");
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
            if (dtSalaries.SelectedItems.Count != 0)
            {
                if (dpPayday.SelectedDate == null && txtSum.Text.Trim().Length == 0)
                {
                    MessageBox.Show("Введите значния!");
                }
                else if (dpPayday.SelectedDate < DateTime.Today)
                {
                    MessageBox.Show("Неверное значение у даты");
                }
                else
                {
                    var cellinfo = dtSalaries.SelectedCells[0];
                    using (var client = new HttpClient())
                    {
                        var endpoint = new Uri(uri + "/salary");
                        var newSalary = new Salary()
                        {
                            id = Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text),
                            paydate = DateTime.Parse(dpPayday.SelectedDate.Value.ToShortDateString()),
                            paysum = Convert.ToDecimal(txtSum.Text),
                            employeeid = Convert.ToInt32(cmbEmployee.SelectedValue),
                            bookkeeperid = myid,
                            paytypeid = Convert.ToInt32(cmbSalaryType.SelectedValue)
                        };

                        var newSalaryJson = JsonConvert.SerializeObject(newSalary);
                        var payload = new StringContent(newSalaryJson, Encoding.UTF8, "application/json");
                        var result = client.PutAsync(endpoint, payload).Result.ToString();

                        if (result.StartsWith("StatusCode: 500"))
                        {
                            MessageBox.Show("Нельзя изменить!");
                        }
                        initDataGrid();
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите строку в таблице");
            }
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (dtSalaries.SelectedItems.Count != 0)
            {
                var cellinfo = dtSalaries.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + $"/salary/{Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text)}");
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
