using System;
using System.IO;
using System.Windows;

namespace KivMusic
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ConAPI conAPI = new ConAPI();
        public MainWindow()
        {
            InitializeComponent();
            string path = GetConPath();
            if (!File.Exists(path + "\\ConnectionAPI.json"))
            {
                MainFrame.Content = new ConnectionOptions(path);
            }
            else
            {
                MainFrame.Content = new Sign_in();
            }
        }

        public string GetConPath()
        {
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string path = Path.Combine(docPath, "KivMusic");
            return path;
        }
    }
}
