using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using PrefixLib;
using Request;

namespace StorageClient
{
    public partial class MainWindow : Window
    {
       
        private TcpClient server = new();
        


        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] config = await File.ReadAllLinesAsync("Config.txt");
            await server.ConnectAsync(config[0], int.Parse(config[1]));

            while (true) {
                await ListenServer();
            }         
        }

        private async Task ListenServer()
        {
            string answer = await server.ReceiveString();

            if (answer == ServerAction.Allow.ToString()) 
            {
                Storage storage = new Storage(server);

                if (storage.ShowDialog() == true)
                {
                    // ???????????????
                }
            }
            else
            {
                wrongTextBlock.Visibility = Visibility.Visible;
            }
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            if ( loginBox.Text.Length == 0 || passwordBox.Password.Length == 0 ) 
            {
                warningTextBlock.Visibility = Visibility.Visible;
                return;
            }

            // отправка логина
            string loginPassword = $"{loginBox.Text},{passwordBox.Password}"; // без переменной???
           
            await server.SendString(loginPassword);
        }

        private void loginBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            warningTextBlock.Visibility = Visibility.Hidden;
            wrongTextBlock.Visibility = Visibility.Hidden;
        }

        private void passwordBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            warningTextBlock.Visibility = Visibility.Hidden;
            wrongTextBlock.Visibility = Visibility.Hidden;
        }

       
    }
}