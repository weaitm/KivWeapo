using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Word = Microsoft.Office.Interop.Word;

namespace KivMusic.Views.User
{
    /// <summary>
    /// Логика взаимодействия для PaySuccess.xaml
    /// </summary>
    public partial class PaySuccess : Page
    {
        int myid;
        decimal sum;
        string checknumber;
        DateTime purchasedate;
        string shiftnumber;
        public PaySuccess(int myid, decimal sum, ObservableCollection<CheckOutWindow.ConsumerCarts> consumerCart, string checknumber, DateTime purchasedate, string shiftnumber)
        {                                                                                                               
            InitializeComponent();
            this.sum = sum;
            this.checknumber = checknumber;
            this.purchasedate = purchasedate;
            this.shiftnumber = shiftnumber;
            this.myid = myid;
            btnMainBuyWindow.IsEnabled = false;
            List<String> productList = new List<string>();
            ObservableCollection<ConsumerCarts> consumerCartss = new ObservableCollection<ConsumerCarts>();

            foreach (CheckOutWindow.ConsumerCarts carts in consumerCart)
            {
                consumerCartss.Add(new ConsumerCarts
                {
                    productname = carts.productname,
                    productprice = carts.productprice,
                    quantityOfProduct = carts.quantityOfProduct,
                });
            }

            foreach(ConsumerCarts consumer in consumerCartss)
            {
                productList.Add($@"

{consumer.productname} {consumer.quantityOfProduct} x {consumer.productprice}

");
            }
            txtBill.Text = $@"
Чек: {checknumber}
Смена: {shiftnumber}
Дата покупки: {purchasedate}
Товар:
";
            for(int i = 0; i < productList.Count; i++)
            {
                txtBill.Text += productList[i].ToString() + Environment.NewLine;
            }

            txtBill.Text += $"Итог: {sum}";
        }

        public class ConsumerCarts
        {
            public string productname { get; set; }
            public decimal productprice { get; set; }
            public int quantityOfProduct { get; set; }
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            Word.Application objWord = new Word.Application();
            objWord.Visible = false;

            Word.Document objDoc = objWord.Documents.Add();

            Word.Paragraph objPara;
            objPara = objDoc.Paragraphs.Add();
            objPara.Range.Text ="Ваш чек: \n" + txtBill.Text;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = $"Bill-{DateTime.Today.ToString("dd.MM.yyyy")}";
            saveFileDialog.Filter = "Doc files (*.docx)|*.docx";
            saveFileDialog.Title = "Выгрузка чека";
            
            if(saveFileDialog.ShowDialog() == true)
            {
                objDoc.SaveAs2(saveFileDialog.FileName);
            }
            objDoc.Close();
            objWord.Quit();
            btnMainBuyWindow.IsEnabled = true;
        }

        private void btnMainBuyWindow_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainBuyWindow(myid));
        }
    }
}
