using System;
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

        public async void Start()
        {
            while (true)
            {
                await GetHistory();
                Thread.Sleep(ConfigManager.Config.MillisecondsSleep);
            }
        }
    }
}