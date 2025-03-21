using ClassesLib;
using StorageClient.Classes;
using StorageDBCodeFirst.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace StorageClient
{    
    public partial class RackWindow : Window
    {

        private Rack rack = new();

        private AddDelRack addDelRack = new();
        private List<Location> locList = [];

        public RackWindow(List<Location> loc, AddDelRack addDelRack)
        {
            InitializeComponent();

            this.locList = loc;
            this.addDelRack = addDelRack;

            rack.RackNumber = "";

            foreach (Location l in locList)
            {
                rack.RackNumber += $"{l.RackNumber} ";
            }

            this.DataContext = rack;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (rackNumberTextBox.Text.Length < 1) return; 

            addGrid.Visibility = Visibility.Visible;

            rack.AddRack += $"{rackNumberTextBox.Text} ";

            addDelRack.AddRacks.Add(int.Parse(rackNumberTextBox.Text));
            rackNumberTextBox.Text = "";            
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (rackNumberTextBox.Text.Length < 1) return;

            delGrid.Visibility = Visibility.Visible;

            rack.DeleteRack += $"{rackNumberTextBox.Text} ";

            addDelRack.DeleteRacks.Add(int.Parse(rackNumberTextBox.Text));
            rackNumberTextBox.Text = "";
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            addDelRack.AddRacks.Clear();
            addDelRack.DeleteRacks.Clear();

            DialogResult = false;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
