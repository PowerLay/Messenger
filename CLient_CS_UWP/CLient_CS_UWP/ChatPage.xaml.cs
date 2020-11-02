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
    ///     Страница чата и его логики
    /// </summary>
    public sealed partial class ChatPage : Page
    {
        /// <summary>
        ///     Предыдущая строчка которую вернул сервер на запрос GET api/Chat
        /// </summary>
        private static string _prevMessagesString = "";

        /// <summary>
        ///     Массив пользователей которые онлайн
        /// </summary>
        private List<string> _onlineUsers = new List<string>();

        /// <summary>
        ///     Инициализация страницы чата
        /// </summary>
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

        /// <summary>
        ///     Вызов диалогового окна и обработка нажатий
        ///     <br>Вызывается если нету токена в настройках</br>
        /// </summary>
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

            var nvMain = (NavigationView) Frame.FindName("nvMain");
            switch (result)
            {
                //Если нажата Login
                case ContentDialogResult.Primary:
                {
                    if (nvMain != null) nvMain.SelectedItem = nvMain.MenuItems.OfType<NavigationViewItem>().First();
                    break;
                }
                case ContentDialogResult.Secondary:
                {
                    if (nvMain != null)
                        nvMain.SelectedItem = nvMain.MenuItems.OfType<NavigationViewItem>().ElementAt(1);
                    break;
                }
                case ContentDialogResult.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     GET запрос на сервер
        /// </summary>
        /// <param name="uri">http ссылка на api</param>
        /// <returns>Строка которую вернул сервер</returns>
        public async Task<string> GetAsync(string uri)
        {
            var request = (HttpWebRequest) WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (var response = (HttpWebResponse) await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        /// <summary>
        ///     Обновление истории сообщений и онлайна с сервера
        /// </summary>
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

            if (res == "[]" && res == _prevMessagesString) return;
            if (onlineStatus == "[]") return;
            if (MessagesListView.Items == null) return;

            var messages = JsonConvert.DeserializeObject<List<Message>>(res);
            var onlineUsers = JsonConvert.DeserializeObject<List<string>>(onlineStatus);
            if (MessagesListView.Items?.Count == 0)
            {
                foreach (var message in messages)
                {
                    bool online;
                    if (string.IsNullOrEmpty(message.Name)) online = false;
                    else if (!onlineUsers.Contains(message.Name)) online = false;
                    else online = true;

                    MessagesListView.Items.Add(GetTrueMessage(message, online));
                }

                _onlineUsers = onlineUsers;
                _prevMessagesString = res;
                return;
            }

            if (MessagesListView.Items.Count != messages.Count)
                for (var i = MessagesListView.Items.Count; i < messages.Count; i++)
                {
                    bool online;
                    if (string.IsNullOrEmpty(messages[i].Name)) online = false;
                    else if (!onlineUsers.Contains(messages[i].Name)) online = false;
                    else online = true;

                    MessagesListView.Items?.Add(GetTrueMessage(messages[i], online));
                }

            _prevMessagesString = res;

            var eq = true;

            foreach (var onlineUser in _onlineUsers)
                if (!onlineUsers.Contains(onlineUser))
                    eq = false;
            foreach (var onlineUser in onlineUsers)
                if (!_onlineUsers.Contains(onlineUser))
                    eq = false;

            if (!eq)
                for (var i = 0; i < messages.Count; i++)
                {
                    var message = messages[i];
                    bool online;
                    if (string.IsNullOrEmpty(message.Name)) online = false;
                    else if (!onlineUsers.Contains(message.Name)) online = false;
                    else online = true;

                    MessagesListView.Items[i] = GetTrueMessage(message, online);
                }

            _onlineUsers = onlineUsers;
        }

        /// <summary>
        ///     Преобразует сообщение полученное от сервера в сообщение с конфигурацией для GUI
        /// </summary>
        /// <param name="message">Сообщение полученное от сервера</param>
        /// <param name="online">Состояние пользователя (В сети/не в сети)</param>
        /// <returns>сообщение с конфигурацией для GUI</returns>
        private static Message GetTrueMessage(Message message, bool online)
        {
            Message tempMsg;

            if (message.Name == ConfigManager.Config.RegData.Username)
                tempMsg = new Message(HorizontalAlignment.Right, online)
                    {Name = message.Name, Ts = message.Ts, Text = message.Text};
            else if (string.IsNullOrEmpty(message.Name))
                tempMsg = new Message(HorizontalAlignment.Center, online)
                    {Name = message.Name, Ts = message.Ts, Text = message.Text};
            else
                tempMsg = new Message(HorizontalAlignment.Left, online)
                    {Name = message.Name, Ts = message.Ts, Text = message.Text};

            return tempMsg;
        }

        private void Post()
        {
            var msg = MessageBox.Text;
            if (msg == "") return;
            var httpWebRequest =
                (HttpWebRequest) WebRequest.Create(
                    $"http://{ConfigManager.Config.IP}:{ConfigManager.Config.Port}/api/Chat");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", "Bearer " + ConfigManager.Config.Token);

            SendMessage(msg, httpWebRequest);
            GetAnswer(httpWebRequest);
        }

        /// <summary>
        ///     Получение ответа от сервера
        /// </summary>
        /// <param name="httpWebRequest">Запрос к серверу</param>
        private void GetAnswer(HttpWebRequest httpWebRequest)
        {
            try
            {
                var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
                var streamReader = new StreamReader(httpResponse.GetResponseStream());
                var result = streamReader.ReadToEnd();
                if (result != "ok") Console.WriteLine("Something went wrong");
            }
            catch (WebException)
            {
                //ignored
            }
        }

        /// <summary>
        ///     Отправка сообщения
        /// </summary>
        /// <param name="msg">Сообщение</param>
        /// <param name="httpWebRequest">Запрос к серверу</param>
        private static void SendMessage(string msg, HttpWebRequest httpWebRequest)
        {
            var json = JsonConvert.SerializeObject(new Message {Text = msg});
            var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write(json);
            streamWriter.Close();
        }

        /// <summary>
        ///     Событие нажатия клавиши
        ///     <br>ожидает нажатие enter для отправки сообщения</br>
        /// </summary>
        /// <param name="e">Нажатая клавиша</param>
        private void MessageBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                Post();
                MessageBox.Text = "";
            }
        }

        /// <summary>
        ///     Обработка нажатия на кнопку отправить
        /// </summary>
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            Post();
            MessageBox.Text = "";
        }

        /// <summary>
        ///     Обработка нажатия на кнопку эмоджи
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Focus(FocusState.Programmatic);
            CoreInputView.GetForCurrentView().TryShow(CoreInputViewKind.Emoji);
        }
    }
}