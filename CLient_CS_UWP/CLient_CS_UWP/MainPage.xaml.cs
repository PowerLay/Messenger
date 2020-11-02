using System.Linq;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace CLient_CS_UWP
{
    /// <summary>
    ///     Главная страница содержащая навигационную панель и её логику
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        ///     Инициализация страницы
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            Load();
        }

        /// <summary>
        ///     Загрузка настроек и выбор первой панели на навигационной панели
        /// </summary>
        private async void Load()
        {
            await ConfigManager.LoadConfig();
            nvMain.SelectedItem = nvMain.MenuItems.OfType<NavigationViewItem>().First();
            ApplicationView.PreferredLaunchViewSize = ConfigManager.Config.Size;
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
        }

        /// <summary>
        ///     Обработчик события изменения текущей страницы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args">Навигационная панель</param>
        private void NavigationView_SelectionChanged(NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                ContentFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                var selectedItem = (NavigationViewItem) args.SelectedItem;
                if (selectedItem == null) return;
                if (((string) selectedItem.Tag).Equals("ChatPage")) ContentFrame.Navigate(typeof(ChatPage));
                if (((string) selectedItem.Tag).Equals("LoginPage")) ContentFrame.Navigate(typeof(LoginPage));
                if (((string) selectedItem.Tag).Equals("RegisterPage")) ContentFrame.Navigate(typeof(RegisterPage));
            }
        }
    }
}