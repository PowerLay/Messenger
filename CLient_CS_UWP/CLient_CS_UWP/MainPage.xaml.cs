using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http.Headers;
using Newtonsoft.Json;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace CLient_CS_UWP
{


    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static int _len;
        private static string prevRes = "";

        public MainPage()
        {
            this.InitializeComponent();
            GetHistoryMessages();
            //ConfigManager.LoadConfig();
        }
        private async void GetHistoryMessages() 
        {
            //DateTime time = new DateTime();
            while (true)
            {
                //if (time.AddMilliseconds(200) < DateTime.Now)
                //{
                await UpdateHistory();
                //    time = DateTime.Now;
                //}
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
        private async Task UpdateHistory()
        {
            var res = await GetAsync("http://localhost:5000/api/Chat");

            if (res != "[]" || res != prevRes)
            {
                var messages = JsonConvert.DeserializeObject<List<Message>>(res);

                if (_len != messages.Count)
                {
                    for (int i = _len; i < messages.Count; i++)
                    {
                        MessagesListViev.Items.Add(messages[i].ToString());
                        //MessagesListViev.Items.Add(new TextBlock() { Text = messages[i].ToString() });
                    }

                    _len = messages.Count;
                }
                prevRes = res;
            }
        }
        private void Post(string nick)
        {
            var msg = MessageBox.Text;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:5000/api/Chat");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            SendMessage(nick, msg, httpWebRequest);
            GetAnswer(httpWebRequest);
        }

        private void GetAnswer(HttpWebRequest httpWebRequest)
        {
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var streamReader = new StreamReader(httpResponse.GetResponseStream());
            var result = streamReader.ReadToEnd();
            if (result != "ok") Console.WriteLine("Something went wrong");
        }

        private static void SendMessage(string nick, string msg, HttpWebRequest httpWebRequest)
        {
            var json = JsonConvert.SerializeObject(new Message { Name = nick, Msg = msg });
            var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write(json);
            streamWriter.Close();
        }

        private void MessageBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                string nick = "Test";
                Post(nick);
                MessageBox.Text = "";
            }
        }

    }
}
