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

namespace KivMusic.Views.Stockman
{
    /// <summary>
    /// Логика взаимодействия для StockManWindow.xaml
    /// </summary>
    public partial class StockManWindow : Page
    {
        ObservableCollection<Warehouses> warehouses = new ObservableCollection<Warehouses>();
        ObservableCollection<ProductAvals> productAvals = new ObservableCollection<ProductAvals>();
        int myid;
        ConAPI conAPI = new ConAPI();
        string uri;
        public StockManWindow(int myid)
        {
            InitializeComponent();
            this.myid = myid;
            uri = conAPI.ReadConJson();
            initDataGrid();
            initComboBox();
        }

        public class Warehouses
        {
            public int id { get; set; }
            public string warehousecell { get; set; }
            public string productavailabilityname { get; set; }
            public bool productavailability { get; set; }
        }

        public class ProductAvals
        {
            public bool value { get; set; }
            public string displayname { get; set; }
        }

        public void initDataGrid()
        {

            try
            {
                warehouses.Clear();
                string url = uri + $"/warehouse";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                var locWarehouse = JsonConvert.DeserializeObject<List<Warehouse>>(response);

                foreach (Warehouse warehouse in locWarehouse)
                {
                    if(warehouse.productavailability == true)
                    {
                        warehouses.Add(new Warehouses
                        {
                            id = warehouse.id,
                            warehousecell = warehouse.warehousecell,
                            productavailabilityname = "Ячейка используется",
                            productavailability = warehouse.productavailability,
                        });
                    }
                    else
                    {
                        warehouses.Add(new Warehouses
                        {
                            id=warehouse.id,
                            warehousecell = warehouse.warehousecell,
                            productavailabilityname = "Ячейка доступна",
                            productavailability = warehouse.productavailability,
                        });
                    }
                }
                dtWarehouse.ItemsSource = warehouses;
                dtWarehouse.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void initComboBox()
        {
            productAvals.Add(new ProductAvals
            {
                value = true,
                displayname = "Ячейка используется"
            });
            productAvals.Add(new ProductAvals
            {
                value = false,
                displayname = "Ячейка доступна"
            });
            cmbProductAvailability.ItemsSource = productAvals;
            cmbProductAvailability.Items.Refresh();
            cmbProductAvailability.SelectedIndex = 0;
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btnApplyDeliveryWin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ApplyDeliveryWindow(myid));
        }

        private void btnDeleteLocWarehouse_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DeleteLocWarehouseWindow());
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtWarehouseCell.Text.Trim().Length == 0)
            {
                MessageBox.Show("Введите значния!");
            }
            else
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var endpoint = new Uri(uri + "/warehouse");
                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                        HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                        var newWarehouse = new Warehouse()
                        {
                            warehousecell = txtWarehouseCell.Text,
                            productavailability = Convert.ToBoolean(cmbProductAvailability.SelectedValue),
                        };

                        var newWarehouseJson = JsonConvert.SerializeObject(newWarehouse);
                        var payload = new StringContent(newWarehouseJson, Encoding.UTF8, "application/json");
                        var result = client.PostAsync(endpoint, payload).Result.ToString();

                        if (result.StartsWith("StatusCode: 500"))
                        {
                            MessageBox.Show("Такая ячейка уже создана");
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
            if (dtWarehouse.SelectedItems.Count != 0 && txtWarehouseCell.Text.Trim().Length != 0)
            {
                var cellinfo = dtWarehouse.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + "/warehouse");
                    var newWarehouse = new Warehouse()
                    {
                        id = Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text),
                        warehousecell = txtWarehouseCell.Text,
                        productavailability = Convert.ToBoolean(cmbProductAvailability.SelectedValue),
                    };

                    var newWarehouseJson = JsonConvert.SerializeObject(newWarehouse);
                    var payload = new StringContent(newWarehouseJson, Encoding.UTF8, "application/json");
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
                MessageBox.Show("Выберите строку в таблице");
            }
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (dtWarehouse.SelectedItems.Count != 0)
            {
                var cellinfo = dtWarehouse.SelectedCells[0];
                using (var client = new HttpClient())
                {
                    var endpoint = new Uri(uri + $"/warehouse/{Convert.ToInt32((cellinfo.Column.GetCellContent(cellinfo.Item) as TextBlock).Text)}");
                    var result = client.DeleteAsync(endpoint).Result.ToString();
                    initDataGrid();
                }
            }
            else
            {
                MessageBox.Show("Выберите строку в таблице");
            }
        }

        private void dtWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtWarehouse.SelectedItems.Count != 0)
                {
                    var warehousecellinfo = dtWarehouse.SelectedCells[1];
                    var productavailabilityinfo = dtWarehouse.SelectedCells[3];
                    txtWarehouseCell.Text = (warehousecellinfo.Column.GetCellContent(warehousecellinfo.Item) as TextBlock).Text;
                    cmbProductAvailability.SelectedValue =Convert.ToBoolean((productavailabilityinfo.Column.GetCellContent(productavailabilityinfo.Item) as TextBlock).Text);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
