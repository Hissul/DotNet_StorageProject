using ClassesLib;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using StorageClient.Classes;
using StorageDBCodeFirst.Classes;
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
using System.Windows.Shapes;
using System.IO;

namespace StorageClient
{
    public partial class GoodsConsumption : Window
    {        
        private ClientVM clientVM = new();
        UserProduct product;
        string party = "";

        public GoodsConsumption(ClientVM clientVM)
        {
            InitializeComponent();

            this.clientVM = clientVM;
            DataContext = clientVM;
        }

        private void docNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (docNumberTextBox.Text.Length == 6)            
                nextButton.IsEnabled = true;            
            else  
                nextButton.IsEnabled = false; 
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            documentScreen.Visibility = Visibility.Hidden;
            productScreen.Visibility = Visibility.Visible;
        }

        private void productComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productComboBox.SelectedItem is null) return;

            partyComboBox.IsEnabled = true;            

            product = (UserProduct)productComboBox.SelectedItem;

            clientVM.ProductParty.Clear();

            List<Product> products = clientVM.Products.Where(pr => pr.ProductTypeId == product.Id).ToList();
            foreach (Product prod in products)
            {
                clientVM.ProductParty.Add(prod.Party);
            }
        }

        private void partyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            amountTextBox.IsEnabled = true;

            party = (string)partyComboBox.SelectedItem;

            List<int> racks = clientVM.Storages
                .Where(st => st.ProductName == product.ProductName && st.Party == party)
                .Select(st => st.RackNumber)
                .ToList();

            clientVM.ConsumptionRackNumbers.Clear();

            foreach (int rack in racks)
            {
                clientVM.ConsumptionRackNumbers.Add(rack);   
            }

            rackComboBox.SelectedIndex = -1;
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
            int loc = (int)rackComboBox.SelectedItem;

            var location = clientVM.Locations.Where(locat => locat.RackNumber == loc).First();

            clientVM.ProductConsumptions.Add(new ProductConsumption(
              docNumberTextBox.Text,
              product.Id,
              (string)partyComboBox.SelectedItem,
              int.Parse(amountTextBox.Text),
              location.Id,              
              product.ProductName,
              location.RackNumber));

            productComboBox.SelectedIndex = -1;
            partyComboBox.SelectedIndex = -1;
            partyComboBox.IsEnabled = false;            
            amountTextBox.Text = "";
            amountTextBox.IsEnabled = false;
            rackComboBox.SelectedIndex = -1;
            rackComboBox.IsEnabled = false;
            nextGoodButton.IsEnabled = false;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e) => DialogResult = false;

        private void saveButton_Click(object sender, RoutedEventArgs e) => DialogResult = true;

        private void abortButton_Click(object sender, RoutedEventArgs e)
        {
            clientVM.ProductConsumptions.Clear();
            DialogResult = false;
        }


    }
}
