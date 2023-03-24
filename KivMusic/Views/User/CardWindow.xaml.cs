using KivMusic.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    /// Логика взаимодействия для CardWindow.xaml
    /// </summary>
    public partial class CardWindow : Page
    {
        ConAPI conAPI = new ConAPI();
        string uri;
        ObservableCollection<ProfileCards> profileCards = new ObservableCollection<ProfileCards>();
        ObservableCollection<PaymentSystems> paymentSystems = new ObservableCollection<PaymentSystems>();
        ObservableCollection<Banks> banks = new ObservableCollection<Banks>();
        ObservableCollection<TypeCards> typeCards = new ObservableCollection<TypeCards>();
        int myid;
        public CardWindow(int myid)
        {
            InitializeComponent();
            this.myid = myid;
            uri = conAPI.ReadConJson();
            DataContext = this;
            initDataGrid();
            initComboboxPaymentSystem();
            initComboboxBank();
            initComboboxTypeCard();
        }

        public class ProfileCards
        {
            public int id { get; set; }
            public int cardid { get; set; }
            public int profileid { get; set; }
            public string cardnumber { get; set; }
            public string cardholder { get; set; }
            public DateTime cardexpiry { get; set; }
            public string cvc { get; set; }
            public int paymentsystemid { get; set; }
            public string paymentsystemname { get; set; }
            public int bankid { get; set; }
            public string bankname { get; set; }
            public int typecardid { get; set; }
            public string typecardname { get; set; }
        }

        public class PaymentSystems
        {
            public int id { get; set; }
            public string namesystem { get; set; }
        }

        public class Banks
        {
            public int id { get; set; }
            public string bankname { get; set; }
        }

        public class TypeCards
        {
            public int id { get; set; }
            public string typename { get; set; }
        }

        public void initDataGrid()
        {

            try
            {
                profileCards.Clear();
                string url = uri + $"/profilecard";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locProfileCard = JsonConvert.DeserializeObject<List<ProfileCard>>(response);

                foreach (ProfileCard profCard in locProfileCard)
                {
                    if(profCard.profileid == myid)
                    {
                        string urlCard = uri + $"/card/{profCard.cardid}";
                        HttpWebRequest httpWebRequestCard = (HttpWebRequest)WebRequest.Create(urlCard);
                        HttpWebResponse httpWebResponseCard = (HttpWebResponse)httpWebRequestCard.GetResponse();
                        string responseCard;

                        using (StreamReader streamReader = new StreamReader(httpWebResponseCard.GetResponseStream()))
                        {
                            responseCard = streamReader.ReadToEnd();
                        }

                        Card card = JsonConvert.DeserializeObject<Card>(responseCard);



                        string urlPaymentSystem = uri + $"/paymentsystem/{card.paymentsystemid}";
                        HttpWebRequest httpWebRequestPaymentSystem = (HttpWebRequest)WebRequest.Create(urlPaymentSystem);
                        HttpWebResponse httpWebResponsePaymentSystem = (HttpWebResponse)httpWebRequestPaymentSystem.GetResponse();
                        string responsePaymentSystem;

                        using (StreamReader streamReader = new StreamReader(httpWebResponsePaymentSystem.GetResponseStream()))
                        {
                            responsePaymentSystem = streamReader.ReadToEnd();
                        }

                        PaymentSystem paymentSystem = JsonConvert.DeserializeObject<PaymentSystem>(responsePaymentSystem);

                        string urlBank= uri + $"/bank/{card.bankid}";
                        HttpWebRequest httpWebRequestBank = (HttpWebRequest)WebRequest.Create(urlBank);
                        HttpWebResponse httpWebResponseBank = (HttpWebResponse)httpWebRequestBank.GetResponse();
                        string responseBank;

                        using (StreamReader streamReader = new StreamReader(httpWebResponseBank.GetResponseStream()))
                        {
                            responseBank = streamReader.ReadToEnd();
                        }

                        Bank bank = JsonConvert.DeserializeObject<Bank>(responseBank);

                        string urlTypeCard= uri + $"/typecard/{card.typecardid}";
                        HttpWebRequest httpWebRequestTypeCard = (HttpWebRequest)WebRequest.Create(urlTypeCard);
                        HttpWebResponse httpWebResponseTypeCard = (HttpWebResponse)httpWebRequestTypeCard.GetResponse();
                        string responseTypeCard;

                        using (StreamReader streamReader = new StreamReader(httpWebResponseTypeCard.GetResponseStream()))
                        {
                            responseTypeCard = streamReader.ReadToEnd();
                        }

                        TypeCard typeCard = JsonConvert.DeserializeObject<TypeCard>(responseTypeCard);



                        profileCards.Add(new ProfileCards
                        {
                            id = profCard.id,
                            cardid = profCard.cardid,
                            profileid = profCard.profileid,
                            cardnumber = card.cardnumber,
                            cardholder = card.cardholder,
                            cardexpiry = card.cardexpirydate,
                            cvc = card.cvcccv,
                            paymentsystemid = card.paymentsystemid,
                            paymentsystemname = paymentSystem.namesystem,
                            bankid = card.bankid,
                            bankname = bank.bankname,
                            typecardid = card.typecardid,
                            typecardname = typeCard.typename,
                        });
                    }
                    else
                    {

                    }                  
                }
                dtProfileCards.ItemsSource = profileCards;
                dtProfileCards.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void initComboboxPaymentSystem()
        {
            try
            {
                paymentSystems.Clear();
                string url = uri + $"/paymentsystem";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locPaymentSystem = JsonConvert.DeserializeObject<List<PaymentSystems>>(response);

                foreach (PaymentSystems systems in locPaymentSystem)
                {
                    paymentSystems.Add(new PaymentSystems
                    {
                        id = systems.id,
                        namesystem = systems.namesystem,
                    });
                }
                cmbPaymentSystem.ItemsSource = paymentSystems;
                cmbPaymentSystem.Items.Refresh();
                cmbPaymentSystem.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void initComboboxBank()
        {
            try
            {
                banks.Clear();
                string url = uri + $"/bank";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locBank= JsonConvert.DeserializeObject<List<Banks>>(response);

                foreach (Banks bank in locBank)
                {
                    banks.Add(new Banks
                    {
                        id = bank.id,
                        bankname = bank.bankname,
                    });
                }
                cmbBank.ItemsSource = banks;
                cmbBank.Items.Refresh();
                cmbBank.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void initComboboxTypeCard()
        {
            try
            {
                typeCards.Clear();
                string url = uri + $"/typecard";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locTypeCard= JsonConvert.DeserializeObject<List<TypeCards>>(response);

                foreach (TypeCards card in locTypeCard)
                {
                    typeCards.Add(new TypeCards
                    {
                        id = card.id,
                        typename = card.typename,
                    });
                }
                cmbTypeCard.ItemsSource = typeCards;
                cmbTypeCard.Items.Refresh();
                cmbTypeCard.SelectedIndex = 0;
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

        private void dtProfileCards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtProfileCards.SelectedItems.Count != 0)
                {
                    var cardnumberInfo = dtProfileCards.SelectedCells[1];
                    var cardholderInfo = dtProfileCards.SelectedCells[2];
                    var cvcInfo = dtProfileCards.SelectedCells[4];
                    var paymentsystemidInfo = dtProfileCards.SelectedCells[6];
                    var bankidInfo = dtProfileCards.SelectedCells[8];
                    var typecardidInfo = dtProfileCards.SelectedCells[10];
                    txtCardNumber.Text = (cardnumberInfo.Column.GetCellContent(cardnumberInfo.Item) as TextBlock).Text;
                    txtCardHolder.Text = (cardholderInfo.Column.GetCellContent(cardholderInfo.Item) as TextBlock).Text;
                    txtCVC.Text = (cvcInfo.Column.GetCellContent(cvcInfo.Item) as TextBlock).Text;
                    cmbTypeCard.SelectedValue = Convert.ToInt32((typecardidInfo.Column.GetCellContent(typecardidInfo.Item) as TextBlock).Text);
                    cmbPaymentSystem.SelectedValue = Convert.ToInt32((paymentsystemidInfo.Column.GetCellContent(paymentsystemidInfo.Item) as TextBlock).Text);
                    cmbBank.SelectedValue = Convert.ToInt32((bankidInfo.Column.GetCellContent(bankidInfo.Item) as TextBlock).Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (dpDateExpire.SelectedDate == null && txtCardHolder.Text.Trim().Length == 0 || 
                txtCardNumber.Text.Trim().Length == 0 && txtCVC.Text.Trim().Length == 0)
            {
                MessageBox.Show("Введите значния!");
            }
            else if (dpDateExpire.SelectedDate < DateTime.Today)
            {
                MessageBox.Show("Неверное значение у даты");
            }
            else
            {
                try
                {
                    using (var client = new HttpClient())
                    {

                        var endpointCard = new Uri(uri + "/card");
                        var newCard = new Card()
                        {
                            cardnumber = txtCardNumber.Text,
                            cardholder = txtCardHolder.Text,
                            cardexpirydate = dpDateExpire.SelectedDate.Value.Date,
                            cvcccv = txtCVC.Text,
                            typecardid = Convert.ToInt32(cmbTypeCard.SelectedValue),
                            paymentsystemid = Convert.ToInt32(cmbPaymentSystem.SelectedValue),
                            bankid = Convert.ToInt32(cmbBank.SelectedValue),
                        };
                        var newCardJson = JsonConvert.SerializeObject(newCard);
                        var payloadCard = new StringContent(newCardJson, Encoding.UTF8, "application/json");
                        var resultCard = client.PostAsync(endpointCard, payloadCard).Result.ToString();
                        if (resultCard.StartsWith("StatusCode: 500"))
                        {
                            MessageBox.Show("Такая карта уже создана уже создана");
                        }

                        string urlCard = uri + $"/card";
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(urlCard);
                        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        string response;

                        using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                        {
                            response = streamReader.ReadToEnd();
                        }

                        var locCard = JsonConvert.DeserializeObject<List<Card>>(response);

                        foreach(Card card in locCard)
                        {
                            if (card.cardnumber == txtCardNumber.Text && card.cardholder == txtCardHolder.Text &&
                                card.cardexpirydate == dpDateExpire.SelectedDate.Value && card.cvcccv == txtCVC.Text &&
                                card.typecardid == Convert.ToInt32(cmbTypeCard.SelectedValue) && 
                                card.paymentsystemid == Convert.ToInt32(cmbPaymentSystem.SelectedValue) && 
                                card.bankid == Convert.ToInt32(cmbBank.SelectedValue))
                            {
                                var endpoint = new Uri(uri + "/profilecard");
                                var newProfilecard = new ProfileCard()
                                {
                                    cardid = card.id,
                                    profileid = myid,
                                };

                                var newProfilecardJson = JsonConvert.SerializeObject(newProfilecard);
                                var payload = new StringContent(newProfilecardJson, Encoding.UTF8, "application/json");
                                var result = client.PostAsync(endpoint, payload).Result.ToString();

                                if (result.StartsWith("StatusCode: 500"))
                                {
                                    MessageBox.Show("Действие невозможно!");
                                }
                            }
                            else
                            {

                            }
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

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (dtProfileCards.SelectedItems.Count != 0)
            {
                var cellinfo = dtProfileCards.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + $"/profilecard/{Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text)}");
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
