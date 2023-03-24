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

namespace KivMusic.Views.Admin
{
    /// <summary>
    /// Логика взаимодействия для AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Page
    {
        ConAPI conAPI = new ConAPI();
        string uri;
        Hash hash = new Hash();
        ObservableCollection<Profiles> profiles = new ObservableCollection<Profiles>();
        ObservableCollection<Roles> roles = new ObservableCollection<Roles>();
        int myid;
        public AddUserWindow(int idprofile)
        {
            InitializeComponent();
            DataContext = this;
            uri = conAPI.ReadConJson();
            initDataGrid();
            initCombobox();
            myid = idprofile;
        }

        public void initDataGrid()
        {

            try
            {
                profiles.Clear();
                string url = uri + $"/profile";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locProfiles = JsonConvert.DeserializeObject<List<Profiles>>(response);

                foreach (Profiles profile in locProfiles)
                {
                    string urlrole = uri + $"/role/{profile.rolesid}";
                    HttpWebRequest httpWebRequestRole = (HttpWebRequest)WebRequest.Create(urlrole);
                    HttpWebResponse httpWebResponseRole = (HttpWebResponse)httpWebRequestRole.GetResponse();
                    string responseRole;

                    using (StreamReader streamReader = new StreamReader(httpWebResponseRole.GetResponseStream()))
                    {
                        responseRole = streamReader.ReadToEnd();
                    }

                    Role role = JsonConvert.DeserializeObject<Role>(responseRole);
                    profiles.Add(new Profiles
                    {
                        id = profile.id,
                        lastname = profile.lastname,
                        firstname = profile.firstname,
                        middlename = profile.middlename,
                        profilelogin = profile.profilelogin,
                        profilepassword = profile.profilepassword,
                        rolename = role.rolename,
                        rolesid = profile.rolesid,
                    });
                }
                dtProfiles.ItemsSource = profiles;
                dtProfiles.Items.Refresh();
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
                roles.Clear();
                string url = uri + $"/role";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locRoles = JsonConvert.DeserializeObject<List<Roles>>(response);

                foreach (Roles role in locRoles)
                {
                    roles.Add(role);
                }
                cmbRole.ItemsSource = roles;
                cmbRole.Items.Refresh();
                cmbRole.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public class Profiles
        {
            public int id { get; set; }
            public string lastname { get; set; }
            public string firstname { get; set; }
            public string middlename { get; set; }
            public string profilelogin { get; set; }
            public string profilepassword { get; set; }
            public string rolename { get; set; }
            public int rolesid { get; set; }
        }

        public class Roles
        {
            public int id { get; set; }
            public string rolename { get; set; }
        }

        private void btnAddTypeCard_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TypeCardWindow());
        }

        private void btnAddBank_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new BankWindow());
        }

        private void btnAddPaymentSystem_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PaymentSystemWindow());
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void dtProfiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtProfiles.SelectedItems.Count != 0)
                {
                    var lastnameinfo = dtProfiles.SelectedCells[1];
                    var firstnameinfo = dtProfiles.SelectedCells[2];
                    var middlenameinfo = dtProfiles.SelectedCells[3];
                    var profilelogininfo = dtProfiles.SelectedCells[4];
                    var profilepasswordinfo = dtProfiles.SelectedCells[5];
                    var roleidinfo = dtProfiles.SelectedCells[7];
                    txtLastname.Text = (lastnameinfo.Column.GetCellContent(lastnameinfo.Item) as TextBlock).Text;
                    txtFirstname.Text = (firstnameinfo.Column.GetCellContent(firstnameinfo.Item) as TextBlock).Text;
                    txtMiddlename.Text = (middlenameinfo.Column.GetCellContent(middlenameinfo.Item) as TextBlock).Text;
                    txtProfilelogin.Text = (profilelogininfo.Column.GetCellContent(profilelogininfo.Item) as TextBlock).Text;
                    txtProfilepassword.Text = (profilepasswordinfo.Column.GetCellContent(profilepasswordinfo.Item) as TextBlock).Text;
                    cmbRole.SelectedValue = Convert.ToInt32((roleidinfo.Column.GetCellContent(roleidinfo.Item) as TextBlock).Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtLastname.Text.Trim().Length == 0 && txtFirstname.Text.Trim().Length == 0 &&
                txtProfilelogin.Text.Trim().Length == 0 && txtProfilepassword.Text.Trim().Length == 0)
            {
                MessageBox.Show("Введите значния!");
            }
            else
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var endpoint = new Uri(uri + "/profile");
                        var newProfile = new Profile()
                        {
                            lastname = txtLastname.Text.Trim(),
                            firstname = txtFirstname.Text.Trim(),
                            middlename = txtMiddlename.Text.Trim(),
                            profilelogin = txtProfilelogin.Text.Trim(),
                            profilepassword = hash.GetHash(txtProfilepassword.Text.Trim()),
                            rolesid = Convert.ToInt32(cmbRole.SelectedValue)
                        };

                        var newProfileJson = JsonConvert.SerializeObject(newProfile);
                        var payload = new StringContent(newProfileJson, Encoding.UTF8, "application/json");
                        var result = client.PostAsync(endpoint, payload).Result.ToString();

                        if (result.StartsWith("StatusCode: 500"))
                        {
                            MessageBox.Show("Такой пользователь уже создан");
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
            if (dtProfiles.SelectedItems.Count != 0)
            {
                var cellinfo = dtProfiles.SelectedCells[0];
                if(myid != Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text))
                {
                    using (var client = new HttpClient())
                    {
                        var endpoint = new Uri(uri + "/profile");
                        var profilepasswordinfo = dtProfiles.SelectedCells[5];
                        if (txtProfilepassword.Text != (profilepasswordinfo.Column.GetCellContent(profilepasswordinfo.Item) as TextBlock).Text)
                        {
                            var newProfile = new Profile()
                            {
                                id = Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text),
                                lastname = txtLastname.Text.Trim(),
                                firstname = txtFirstname.Text.Trim(),
                                middlename = txtMiddlename.Text.Trim(),
                                profilelogin = txtProfilelogin.Text.Trim(),
                                profilepassword = hash.GetHash(txtProfilepassword.Text.Trim()),
                                rolesid = Convert.ToInt32(cmbRole.SelectedValue)
                            };
                            var newProfileJson = JsonConvert.SerializeObject(newProfile);
                            var payload = new StringContent(newProfileJson, Encoding.UTF8, "application/json");
                            var result = client.PutAsync(endpoint, payload).Result.ToString();

                            if (result.StartsWith("StatusCode: 500"))
                            {
                                MessageBox.Show("Нельзя изменить!");
                            }
                        }
                        else
                        {
                            var newProfile = new Profile()
                            {
                                id = Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text),
                                lastname = txtLastname.Text.Trim(),
                                firstname = txtFirstname.Text.Trim(),
                                middlename = txtMiddlename.Text.Trim(),
                                profilelogin = txtProfilelogin.Text.Trim(),
                                profilepassword = txtProfilepassword.Text.Trim(),
                                rolesid = Convert.ToInt32(cmbRole.SelectedValue)
                            };
                            var newProfileJson = JsonConvert.SerializeObject(newProfile);
                            var payload = new StringContent(newProfileJson, Encoding.UTF8, "application/json");
                            var result = client.PutAsync(endpoint, payload).Result.ToString();

                            if (result.StartsWith("StatusCode: 500"))
                            {
                                MessageBox.Show("Нельзя изменить!");
                            }
                        }
                        initDataGrid();
                    }
                }
                else
                {
                    MessageBox.Show("Вы не можете изменить данные у себя!");
                }
            }
            else
            {
                MessageBox.Show("Выберите строку в таблице");
            }
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (dtProfiles.SelectedItems.Count != 0)
            {

                var cellinfo = dtProfiles.SelectedCells[0];

                if(myid != Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text))
                {
                    using (var client = new HttpClient())
                    {
                        var endpoint = new Uri(uri + $"/profile/{Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text)}");
                        var result = client.DeleteAsync(endpoint).Result.ToString();
                        initDataGrid();
                    }
                }
                else
                {
                    MessageBox.Show("Вы не можете удалить себя!");
                }
            }
            else
            {
                MessageBox.Show("Выберите строку в таблице");
            }
        }

        private void btnProductHistoryWin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProductHistoryWin());
        }
    }
    
}
