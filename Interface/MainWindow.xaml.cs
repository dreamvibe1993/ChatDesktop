
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Interface.Classes;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
namespace Interface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HubConnection _connection;
        private string Msg;
        public MainWindow()
        {
            InitializeComponent();
            _connection = new HubConnectionBuilder().WithUrl("https://localhost:44343/chathub").Build();
            _connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _connection.StartAsync();
            };
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            _connection.On<string>("Connected", (connectionID) =>
            {
                tbMain.Text = connectionID;
            });
            _connection.On<string>("Posted", (value) =>
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    Message DeserializedMessage = JsonConvert.DeserializeObject<Message>(value);
                    listBoxMain.Items.Add(DeserializedMessage.txt);
                }));
            });
            try
            {
                await _connection.StartAsync();
                listBoxMain.Items.Add("Connection established");
                btnConnect.IsEnabled = false;
            }
            catch (Exception exception)
            {
                listBoxMain.Items.Add(exception.Message);
                listBoxMain.Items.Add(exception.StackTrace);
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            Message Message = new Message() { name = "test", id = "test2", txt = Msg };

            string json = JsonConvert.SerializeObject(Message);

            StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            HttpClient httpClient = new HttpClient();
            try
            {
                HttpResponseMessage httpResponse = await httpClient.PostAsync("https://localhost:44343/api/messaging", httpContent);
            }
            catch (Exception ex)
            {
                listBoxMain.Items.Add(ex.Message);
            }
        }

        private void MessageTextField_TextChanged(object sender, TextChangedEventArgs e)
        {
            Msg = MessageTextField.Text;
        }
    }
}
