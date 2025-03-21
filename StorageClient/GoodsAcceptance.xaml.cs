using ClassesLib;
using Newtonsoft.Json;
using PrefixLib;
using StorageClient.Classes;
using StorageDBCodeFirst.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StorageClient
{
    public partial class GoodsAcceptance : Window
    {       
        private ClientVM clientVM = new();


        public GoodsAcceptance(ClientVM clientVM )
        {
            InitializeComponent();
         
            this.clientVM = clientVM;            
            DataContext = clientVM;
        }

        // documentScreen
        private void docNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (docNumberTextBox.Text.Length == 6)
            {
                nextButton.IsEnabled = true;
            }
            else { nextButton.IsEnabled = false; }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            documentScreen.Visibility = Visibility.Hidden;
            productScreen.Visibility = Visibility.Visible;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e) =>
            DialogResult = false;


        // productScreen
        private void productComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productComboBox.SelectedItem is null) return;

            partyComboBox.IsEnabled = true;
            addPartyButton.IsEnabled = true;

            UserProduct product = (UserProduct)productComboBox.SelectedItem;

            clientVM.ProductParty.Clear();

            List<Product> products = clientVM.Products.Where(pr => pr.ProductTypeId == product.Id).ToList();
            foreach(Product prod in products)
            {
                clientVM.ProductParty.Add(prod.Party);
            }
        }               

        private void addPartyButton_Click(object sender, RoutedEventArgs e)
        {
            newPartyTextBox.IsEnabled = true;            
        }      

        private void newPartyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (newPartyTextBox.Text.Length == 6)
            {
                acceptPartyButton.IsEnabled = true;
            }
            else
            {
                acceptPartyButton.IsEnabled = false;
            }
        }

        private void acceptPartyButton_Click(object sender, RoutedEventArgs e)
        {
            clientVM.ProductParty.Add(newPartyTextBox.Text);
        }

        private void partyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            amountTextBox.IsEnabled = true;
        }

        private void amountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            rackComboBox.IsEnabled = true;
        }

        private void rackComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            nextGoodButton.IsEnabled = true;
        }

        private void nextGoodButton_Click(object sender, RoutedEventArgs e)
        {
            UserProduct product = (UserProduct)productComboBox.SelectedItem;

            string prodParty = "";
            bool isNew ;
            if (newPartyTextBox.Text.Length < 1)
            {
                prodParty = (string)partyComboBox.SelectedItem;
                isNew = false;
            }
            else
            {
                prodParty = newPartyTextBox.Text;
                isNew = true;
            }

            Location location = (Location)rackComboBox.SelectedItem;

            // добавить товар в массив товаров
            clientVM.ProductAcceptances.Add(new ProductAcceptance(
                docNumberTextBox.Text,
                product.Id,
                prodParty,
                int.Parse(amountTextBox.Text),
                location.Id,
                isNew,
                product.ProductName));

            productComboBox.SelectedIndex = -1;
            partyComboBox.SelectedIndex = -1;
            partyComboBox.IsEnabled = false;
            newPartyTextBox.Text = "";
            newPartyTextBox.IsEnabled = false;
            amountTextBox.Text = "";
            amountTextBox.IsEnabled = false;
            rackComboBox .SelectedIndex = -1;
            rackComboBox.IsEnabled = false;
            nextGoodButton.IsEnabled = false;
        }

        private void abortButton_Click(object sender, RoutedEventArgs e)
        {
            clientVM.ProductAcceptances.Clear();
            DialogResult = false;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e) => DialogResult = true;
    }
}
