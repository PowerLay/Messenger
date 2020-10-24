using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;

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
                Thread.Sleep(ConfigManager.config.MillisecondsSleep);
            }
        }
    }
}