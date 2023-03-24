using KivMusic.Models;
using KivMusic.Views.Admin;
using KivMusic.Views.Hr;
using KivMusic.Views.Keeper;
using KivMusic.Views.Provider;
using KivMusic.Views.Seller;
using KivMusic.Views.Stockman;
using KivMusic.Views.User;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace KivMusic
{
    /// <summary>
    /// Логика взаимодействия для Sign_in.xaml
    /// </summary>
    public partial class Sign_in : Page
    {
        ConAPI conAPI = new ConAPI();
        Hash hash = new Hash();
        string uri;
        public Sign_in()
        {
            InitializeComponent();
            uri = conAPI.ReadConJson();
            txtLogin.Focus();
        }
        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtLogin.Text.Trim().Length != 0 && txtPass.Password.Trim().Length != 0)
                {
                    string url = uri + $"/profile/login/{txtLogin.Text.Trim()}";
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    string response;

                    using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        response = streamReader.ReadToEnd();
                    }

                    Profile profile = JsonConvert.DeserializeObject<Profile>(response);
                    if (profile != null)
                    {
                        if (profile.profilepassword == hash.GetHash(txtPass.Password.Trim()))
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

                            switch (role.rolename)
                            {
                                case "admin":
                                    NavigationService.Navigate(new AddUserWindow(profile.id));
                                    break;
                                case "user":
                                    NavigationService.Navigate(new MainBuyWindow(profile.id));
                                    break;
                                case "seller":
                                    NavigationService.Navigate(new SellerWindow(profile.id));
                                    break;
                                case "stockman":
                                    NavigationService.Navigate(new StockManWindow(profile.id));
                                    break;
                                case "keeper":
                                    NavigationService.Navigate(new KeeperWindow(profile.id));
                                    break;
                                case "provider":
                                    NavigationService.Navigate(new ProviderWindow(profile.id));
                                    break;
                                case "hr":
                                    NavigationService.Navigate(new HrWindow(profile.id));
                                    break;
                                default:
                                    MessageBox.Show("У вас нет роли, обратитесь к системному администратору!");
                                    break;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Неправельный логин или пароль!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неправельный логин или пароль!");
                    }

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SignUp());
        }

        private void Grid_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                btnSignIn_Click(sender, e);
            }
        }
    }
}
