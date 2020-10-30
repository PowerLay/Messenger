using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace CLient_CS_UWP
{
    internal class ServerResponse
    {
        private readonly ChatPage _chatPage;

        public ServerResponse(ChatPage cp)
        {
            _chatPage = cp;
        }

        private async Task GetHistory()
        {
            await _chatPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { _chatPage.UpdateHistory(); });
        }

        public async void ChatUpdater()
        {
            while (true)
            {
                await GetHistory();
                Thread.Sleep(ConfigManager.Config.MillisecondsSleep);
            }
        }


        private async Task PostOnline()
        {
            try
            {
                var httpWebRequest =
                    (HttpWebRequest) WebRequest.Create(
                        $"http://{ConfigManager.Config.IP}:{ConfigManager.Config.Port}/api/Online");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + ConfigManager.Config.Token);
                var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
                var streamReader = new StreamReader(httpResponse.GetResponseStream());
                var result = await streamReader.ReadToEndAsync();
                if (result != "ok") throw new Exception("Something went wrong");
            }
            catch (Exception)
            {
            }
        }

        public async void OnlineUpdater()
        {
            while (true)
            {
                await PostOnline();
                Thread.Sleep(500);
            }
        }
    }
}