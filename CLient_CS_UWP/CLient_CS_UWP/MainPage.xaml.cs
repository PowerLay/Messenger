using System.Linq;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace CLient_CS_UWP
{
    /// <summary>
    ///     Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            nvMain.SelectedItem = nvMain.MenuItems.OfType<NavigationViewItem>().First();
            ChageWindowSize();
        }

        private static async void ChageWindowSize()
        {
            await ConfigManager.LoadConfig();

            ApplicationView.PreferredLaunchViewSize = ConfigManager.Config.Size;
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }

        private void NavigationView_SelectionChanged(NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                ContentFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                var selectedItem = (NavigationViewItem)args.SelectedItem;
                if (selectedItem == null) return;
                if (((string)selectedItem.Tag).Equals("ChatPage")) ContentFrame.Navigate(typeof(ChatPage));
                if (((string)selectedItem.Tag).Equals("LoginPage")) ContentFrame.Navigate(typeof(LoginPage));
            }
        }
    }
}