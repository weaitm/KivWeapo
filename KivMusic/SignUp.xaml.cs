using KivMusic.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace KivMusic
{
    /// <summary>
    /// Логика взаимодействия для SignUp.xaml
    /// </summary>
    public partial class SignUp : Page
    {
        ConAPI conAPI = new ConAPI();
        Hash hash = new Hash();
        string uri;
        public SignUp()
        {
            InitializeComponent();
            uri = conAPI.ReadConJson();
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            if (txtLogin.Text.Trim().Length == 0 &&
                txtLastName.Text.Trim().Length == 0 &&
                txtFirtsName.Text.Trim().Length == 0 &&
                txtPass.Text.Trim().Length == 0)
            {
                MessageBox.Show("Введите значния!");
            }
            else
            {
                if (txtLogin.Text.Trim().Length >= 3 && txtPass.Text.Trim().Length >= 8)
                {
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            var endpoint = new Uri(uri + "/profile");
                            string urlrole = uri + $"/role/name/user";
                            HttpWebRequest httpWebRequestRole = (HttpWebRequest)WebRequest.Create(urlrole);
                            HttpWebResponse httpWebResponseRole = (HttpWebResponse)httpWebRequestRole.GetResponse();
                            string responseRole;

                            using (StreamReader streamReader = new StreamReader(httpWebResponseRole.GetResponseStream()))
                            {
                                responseRole = streamReader.ReadToEnd();
                            }

                            Role role = JsonConvert.DeserializeObject<Role>(responseRole);

                            var newProfile = new Profile()
                            {
                                lastname = txtLastName.Text.Trim(),
                                firstname = txtFirtsName.Text.Trim(),
                                middlename = txtPatronymic.Text.Trim(),
                                profilelogin = txtLogin.Text.Trim(),
                                profilepassword = hash.GetHash(txtPass.Text.Trim()),
                                rolesid = role.Id
                            };

                            var newProfileJson = JsonConvert.SerializeObject(newProfile);
                            var payload = new StringContent(newProfileJson, Encoding.UTF8, "application/json");
                            var result = client.PostAsync(endpoint, payload).Result.ToString();

                            if (result.StartsWith("StatusCode: 500"))
                            {
                                MessageBox.Show("Такой пользователь уже создан");
                            }
                            else
                            {
                                MessageBox.Show("Аккаунт успешно создан!");
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("Недопустимая длинна логина или пароля");
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
