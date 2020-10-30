using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.ViewManagement.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Newtonsoft.Json;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace CLient_CS_UWP
{
    /// <summary>
    ///     Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class ChatPage : Page
    {
        private static string prevRes = "";

        private Dictionary<string, bool> OnlineUsers = new Dictionary<string, bool>();

        public ChatPage()
        {
            InitializeComponent();
            var sr = new ServerResponse(this);
            var updaterThread = new Thread(sr.ChatUpdater);
            updaterThread.Start();

            var onlineUpdaterThread = new Thread(sr.OnlineUpdater);
            onlineUpdaterThread.Start();


            if (string.IsNullOrEmpty(ConfigManager.Config.Token))
            {
                MessageBox.IsEnabled = false;
                SendButton.IsEnabled = false;
                EmojiButton.IsEnabled = false;
                AskLogin();
            }
            else
            {
                MessageBox.Focus(FocusState.Programmatic);
            }
        }

        private async void AskLogin()
        {
            var dialog = new ContentDialog
            {
                Title = "You are not logged in!",
                Content = "Open login or register page",
                PrimaryButtonText = "Login",
                SecondaryButtonText = "Register"
            };

            var result = await dialog.ShowAsync();

            var nvMain = (NavigationView)Frame.FindName("nvMain");
            if (result == ContentDialogResult.Primary) //Если нажата Login
            {
                nvMain.SelectedItem = nvMain.MenuItems.OfType<NavigationViewItem>().First();
            }
            else if (result == ContentDialogResult.Secondary)
            {
                nvMain.SelectedItem = nvMain.MenuItems.OfType<NavigationViewItem>().ElementAt(1);
            }
        }

        public async Task<string> GetAsync(string uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async void UpdateHistory()
        {
            string res;
            string onlineStatus;
            try
            {
                res = await GetAsync($"http://{ConfigManager.Config.IP}:{ConfigManager.Config.Port}/api/Chat");
                onlineStatus =
                    await GetAsync($"http://{ConfigManager.Config.IP}:{ConfigManager.Config.Port}/api/Online");
            }
            catch (Exception)
            {
                return;
            }

            if (res == "[]" && res == prevRes) return;
            if (onlineStatus == "[]") return;
            if (MessagesListView.Items == null) return;

            var messages = JsonConvert.DeserializeObject<List<Message>>(res);
            var onlineUsers = JsonConvert.DeserializeObject<Dictionary<string, bool>>(onlineStatus);
            if (MessagesListView.Items?.Count == 0)
            {
                foreach (var message in messages)
                {
                    bool online;
                    if (string.IsNullOrEmpty(message.Name)) online = false;
                    else if (!onlineUsers.ContainsKey(message.Name)) online = false;
                    else online = onlineUsers[message.Name];

                    MessagesListView.Items.Add(GetTrueMessage(message, online));
                }

                OnlineUsers = onlineUsers;
                prevRes = res;
                return;
            }

            if (MessagesListView.Items.Count != messages.Count)
                for (var i = MessagesListView.Items.Count; i < messages.Count; i++)
                {
                    bool online;
                    if (string.IsNullOrEmpty(messages[i].Name)) online = false;
                    else if (!onlineUsers.ContainsKey(messages[i].Name)) online = false;
                    else online = onlineUsers[messages[i].Name];

                    MessagesListView.Items?.Add(GetTrueMessage(messages[i], online));
                }

            prevRes = res;

            var eq = true;

            foreach (var (key, value) in onlineUsers)
                if (value != OnlineUsers[key])
                    eq = false;

            if (!eq)
                for (var i = 0; i < messages.Count; i++)
                {
                    var message = messages[i];
                    bool online;
                    if (string.IsNullOrEmpty(message.Name)) online = false;
                    else if (!onlineUsers.ContainsKey(message.Name)) online = false;
                    else online = onlineUsers[message.Name];

                    MessagesListView.Items[i] = GetTrueMessage(message, online);
                }

            OnlineUsers = onlineUsers;
        }

        private static Message GetTrueMessage(Message message, bool online)
        {
            Message tempMsg;

            if (message.Name == ConfigManager.Config.RegData.Username)
                tempMsg = new Message(HorizontalAlignment.Right, online)
                { Name = message.Name, Ts = message.Ts, Text = message.Text };
            else if (string.IsNullOrEmpty(message.Name))
                tempMsg = new Message(HorizontalAlignment.Center, online)
                { Name = message.Name, Ts = message.Ts, Text = message.Text };
            else
                tempMsg = new Message(HorizontalAlignment.Left, online)
                { Name = message.Name, Ts = message.Ts, Text = message.Text };

            return tempMsg;
        }

        private void Post()
        {
            var msg = MessageBox.Text;
            if (msg == "") return;
            var httpWebRequest =
                (HttpWebRequest)WebRequest.Create(
                    $"http://{ConfigManager.Config.IP}:{ConfigManager.Config.Port}/api/Chat");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", "Bearer " + ConfigManager.Config.Token);

            SendMessage(msg, httpWebRequest);
            GetAnswer(httpWebRequest);
        }

        private void GetAnswer(HttpWebRequest httpWebRequest)
        {
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var streamReader = new StreamReader(httpResponse.GetResponseStream());
            var result = streamReader.ReadToEnd();
            if (result != "ok") Console.WriteLine("Something went wrong");
        }

        private static void SendMessage(string msg, HttpWebRequest httpWebRequest)
        {
            var json = JsonConvert.SerializeObject(new Message { Text = msg });
            var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write(json);
            streamWriter.Close();
        }

        private void MessageBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                Post();
                MessageBox.Text = "";
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            Post();
            MessageBox.Text = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Focus(FocusState.Programmatic);
            CoreInputView.GetForCurrentView().TryShow(CoreInputViewKind.Emoji);
        }
    }
}