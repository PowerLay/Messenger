using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Newtonsoft.Json;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace CLient_CS_UWP
{
    internal class ServerResponse
    {
        private readonly MainPage mainPage;

        public ServerResponse(MainPage mp)
        {
            mainPage = mp;
        }

        private async Task GetHistory()
        {
            await mainPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { mainPage.UpdateHistory(); });
        }

        public async void Start()
        {
            while (true)
            {
                await GetHistory();
                Thread.Sleep(200);
            }
        }
    }

    /// <summary>
    ///     Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static int _len;
        private static string prevRes = "";

        public MainPage()
        {
            InitializeComponent();
            var sr = new ServerResponse(this);
            var updaterThread = new Thread(sr.Start);
            updaterThread.Start();
            //ConfigManager.LoadConfig();
        }

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

        public async void UpdateHistory()
        {
            string res;
            try
            {
                res = await GetAsync("http://localhost:5000/api/Chat");
            }
            catch (Exception)
            {
                return;
            }

            if (res != "[]" || res != prevRes)
            {
                var messages = JsonConvert.DeserializeObject<List<Message>>(res);

                if (_len != messages.Count)
                {
                    for (var i = _len; i < messages.Count; i++)
                        MessagesListViev.Items.Add(messages[i].ToString());

                    _len = messages.Count;
                }

                prevRes = res;
            }
        }

        private void Post(string nick)
        {
            var msg = MessageBox.Text;

            var httpWebRequest = (HttpWebRequest) WebRequest.Create("http://localhost:5000/api/Chat");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            SendMessage(nick, msg, httpWebRequest);
            GetAnswer(httpWebRequest);
        }

        private void GetAnswer(HttpWebRequest httpWebRequest)
        {
            var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
            var streamReader = new StreamReader(httpResponse.GetResponseStream());
            var result = streamReader.ReadToEnd();
            if (result != "ok") Console.WriteLine("Something went wrong");
        }

        private static void SendMessage(string nick, string msg, HttpWebRequest httpWebRequest)
        {
            var json = JsonConvert.SerializeObject(new Message {Name = nick, Text = msg});
            var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write(json);
            streamWriter.Close();
        }

        private void MessageBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                var nick = "Test";
                Post(nick);
                MessageBox.Text = "";
            }
        }
    }
}