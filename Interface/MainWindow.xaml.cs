using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                    listBoxMain.Items.Add(value);
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

            string json = new JavaScriptSerializer().Serialize(Msg);

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            HttpClient httpClient = new HttpClient();
            try
            {
                HttpResponseMessage httpResponse = await httpClient.PostAsync("https://localhost:44343/api/messaging", httpContent);

                if (httpResponse.Content != null)
                {
                    string responseContent = await httpResponse.Content.ReadAsStringAsync();
                    listBoxMain.Items.Add(responseContent.Trim());
                }
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
