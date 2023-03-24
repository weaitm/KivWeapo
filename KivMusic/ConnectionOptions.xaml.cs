using System;
using System.Collections.Generic;
using System.Linq;
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

namespace KivMusic
{
    /// <summary>
    /// Логика взаимодействия для ConnectionOptions.xaml
    /// </summary>
    public partial class ConnectionOptions : Page
    {
        ConAPI conAPI = new ConAPI();
        string path;
        public ConnectionOptions(string path)
        {
            InitializeComponent();
            this.path = path;
            btnSignUp.IsEnabled = false;
        }
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (txtApiAddress.Text.Trim().Length != 0)
            {
                conAPI.GetDocPath(path);
                conAPI.CreateConJson(txtApiAddress);
                btnCreate.IsEnabled = false;
                btnSignUp.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Поле \"ApiAdress\" пустое");
            }
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SignUp());
        }
    }
}
