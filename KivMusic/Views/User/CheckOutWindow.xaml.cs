using KivMusic.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace KivMusic.Views.User
{
    /// <summary>
    /// Логика взаимодействия для CheckOutWindow.xaml
    /// </summary>
    public partial class CheckOutWindow : Page
    {
        int myid;
        decimal sum;
        ConAPI conAPI = new ConAPI();
        string uri;
        ObservableCollection<ProductChecks> productChecks = new ObservableCollection<ProductChecks>();
        ObservableCollection<Cards> cards = new ObservableCollection<Cards>();
        ObservableCollection<Shops> shops = new ObservableCollection<Shops>();
        ObservableCollection<ConsumerCarts> consumerCartss = new ObservableCollection<ConsumerCarts>();
        public CheckOutWindow(int myid, decimal sum, ObservableCollection<MainBuyWindow.ConsumerCarts> consumerCart)
        {
            InitializeComponent();
            this.myid = myid;
            this.sum = sum;
            uri = conAPI.ReadConJson();
            DataContext = this;
            initComboboxCards();
            initComboboxShop();
            foreach (MainBuyWindow.ConsumerCarts carts in consumerCart)
            {
                consumerCartss.Add(new ConsumerCarts
                {
                    id = carts.id,
                    productname = carts.productname,
                    productprice = carts.productprice,
                    quantityOfProduct = carts.quantityOfProduct,
                });
            }
        }

        public class ConsumerCarts
        {
            public int id { get; set; }
            public string productname { get; set; }
            public decimal productprice { get; set; }
            public int quantityOfProduct { get; set; }
        }

        public class Cards
        {
            public int id { get; set; }
            public string cardnumber { get; set; }
        }

        public class Shops
        {
            public int id { get; set; }
            public string shopname { get; set; }
        }

        public class ProductChecks
        {
            public string checknumber { get; set; }
            public string shiftnumber { get; set; }
            public DateTime purchasedate { get; set; }
            public decimal totalsum { get; set; }
            public decimal inputsum { get; set; }
            public int shopid { get; set; }
            public int profilecardid { get; set; }
            public int profileid { get; set; }

        }

        public void initComboboxCards()
        {
            try
            {
                cards.Clear();
                string url = uri + $"/profilecard";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locProfilecard = JsonConvert.DeserializeObject<List<ProfileCard>>(response);

                foreach (ProfileCard profilecard in locProfilecard)
                {
                    if (profilecard.profileid == myid)
                    {
                        string urlCard = uri + $"/card";
                        HttpWebRequest httpWebRequestCard = (HttpWebRequest)WebRequest.Create(urlCard);
                        HttpWebResponse httpWebResponseCard = (HttpWebResponse)httpWebRequestCard.GetResponse();
                        string responseCard;

                        using (StreamReader streamReader = new StreamReader(httpWebResponseCard.GetResponseStream()))
                        {
                            responseCard = streamReader.ReadToEnd();
                        }

                        var locCard = JsonConvert.DeserializeObject<List<Card>>(responseCard);

                        foreach (Card card in locCard)
                        {
                            if (profilecard.cardid == card.id)
                            {
                                cards.Add(new Cards
                                {
                                    id = profilecard.id,
                                    cardnumber = card.cardnumber,
                                });
                            }
                        }
                    }
                }
                cmbCard.ItemsSource = cards;
                cmbCard.Items.Refresh();
                cmbCard.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void initComboboxShop()
        {
            try
            {
                shops.Clear();
                string url = uri + $"/shop";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locShop = JsonConvert.DeserializeObject<List<Shop>>(response);

                foreach (Shop shop in locShop)
                {
                    shops.Add(new Shops
                    {
                        id = shop.id,
                        shopname = shop.shopname,
                    });
                }
                cmbShop.ItemsSource = shops;
                cmbShop.Items.Refresh();
                cmbShop.SelectedIndex = 0;
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

        private void btnCardWin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CardWindow(myid));
        }

        private void btnPay_Click(object sender, RoutedEventArgs e)
        {
            if(cmbCard.SelectedItem != null && cmbShop.SelectedItem != null)
            {
                MessageBoxResult resultBox = MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (resultBox == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            var endpoint = new Uri(uri + "/productcheck");
                            var newProductCheck = new ProductCheck()
                            {
                                checknumber = $"000000{DateTime.Now.ToString("yyyyMMddHHmmss")}",
                                purchasedate = DateTime.Now,
                                shiftnumber = $"000000000000{DateTime.Now.ToString("yyyyMMdd")}",
                                totalsum = sum,
                                inputsum = sum,
                                profilecardid = Convert.ToInt32(cmbCard.SelectedValue),
                                profileid = myid,
                                shopid = Convert.ToInt32(cmbShop.SelectedValue),
                            };

                            var newProductCheckJson = JsonConvert.SerializeObject(newProductCheck);
                            var payload = new StringContent(newProductCheckJson, Encoding.UTF8, "application/json");
                            var result = client.PostAsync(endpoint, payload).Result.ToString();

                            if (result.StartsWith("StatusCode: 500"))
                            {
                                MessageBox.Show("Такой чек уже создан");
                            }

                            string urlProductCheck = uri + $"/productcheck";
                            HttpWebRequest httpWebRequestProductCheck = (HttpWebRequest)WebRequest.Create(urlProductCheck);
                            HttpWebResponse httpWebResponseProductCheck = (HttpWebResponse)httpWebRequestProductCheck.GetResponse();
                            string responseProductCheck;

                            using (StreamReader streamReader = new StreamReader(httpWebResponseProductCheck.GetResponseStream()))
                            {
                                responseProductCheck = streamReader.ReadToEnd();
                            }

                            var productCheckCheck = JsonConvert.DeserializeObject<List<ProductCheck>>(responseProductCheck);

                            foreach(ProductCheck productCheck in productCheckCheck)
                            {
                                if (productCheck.checknumber == newProductCheck.checknumber &&
                                productCheck.shiftnumber == newProductCheck.shiftnumber &&
                                productCheck.inputsum == newProductCheck.inputsum &&
                                productCheck.totalsum == newProductCheck.totalsum &&
                                productCheck.profilecardid == newProductCheck.profilecardid &&
                                productCheck.profileid == newProductCheck.profileid &&
                                productCheck.shopid == newProductCheck.shopid)
                                {
                                    foreach (ConsumerCarts carts in consumerCartss)
                                    {
                                        var endpointConsumerCart = new Uri(uri + "/consumercart");
                                        var newConsumerCart = new ConsumerCart()
                                        {
                                            quantityOfProduct = carts.quantityOfProduct,
                                            productid = carts.id,
                                            productcheckid = productCheck.id,
                                        };
                                        var newConsumerCartJson = JsonConvert.SerializeObject(newConsumerCart);
                                        var payloadConsumercart = new StringContent(newConsumerCartJson, Encoding.UTF8, "application/json");
                                        var resultConsumerCart = client.PostAsync(endpointConsumerCart, payloadConsumercart).Result.ToString();

                                        string urlLocWarehouse = uri + $"/locationwarehouse";
                                        HttpWebRequest httpWebRequestLocWarehouse = (HttpWebRequest)WebRequest.Create(urlLocWarehouse);
                                        HttpWebResponse httpWebResponseLocWarehouse = (HttpWebResponse)httpWebRequestLocWarehouse.GetResponse();
                                        string responseLocWarehouse;

                                        using (StreamReader streamReader = new StreamReader(httpWebResponseLocWarehouse.GetResponseStream()))
                                        {
                                            responseLocWarehouse = streamReader.ReadToEnd();
                                        }

                                        var locLocationwarehouse = JsonConvert.DeserializeObject<List<LocationWarehouse>>(responseLocWarehouse);

                                        foreach (LocationWarehouse warehouse in locLocationwarehouse)
                                        {
                                            if (warehouse.productid == carts.id)
                                            {
                                                var endpointLocWarehouse = new Uri(uri + "/locationwarehouse");
                                                var newLocWarehouse = new LocationWarehouse()
                                                {
                                                    id = warehouse.id,
                                                    productid = warehouse.productid,
                                                    quantityofgoodsonwarehouse = warehouse.quantityofgoodsonwarehouse - carts.quantityOfProduct,
                                                    warehouseid = warehouse.id,
                                                    profileid = warehouse.profileid,
                                                };

                                                var newLocWarehouseJson = JsonConvert.SerializeObject(newLocWarehouse);
                                                var payloadLocWarehouse = new StringContent(newLocWarehouseJson, Encoding.UTF8, "application/json");
                                                var resultLocWarehouse = client.PutAsync(endpointLocWarehouse, payloadLocWarehouse).Result.ToString();
                                            }
                                        }

                                        if (resultConsumerCart.StartsWith("StatusCode: 500"))
                                        {
                                            MessageBox.Show("Действие не возможно");
                                        }
                                    }
                                }
                            }
                            NavigationService.Navigate(new PaySuccess(myid, sum, consumerCartss, newProductCheck.checknumber, newProductCheck.purchasedate, newProductCheck.shiftnumber));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Нет магазина или карты!");
            }
        }

    }
}
