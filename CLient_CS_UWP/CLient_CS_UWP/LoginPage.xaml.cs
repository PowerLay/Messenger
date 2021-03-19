using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Newtonsoft.Json;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace CLient_CS_UWP
{
    /// <summary>
    ///     Страница авторизации
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        /// <summary>
        ///     Инициализация страницы авторизации
        /// </summary>
        public LoginPage()
        {
            InitializeComponent();
            LoginBox.Text = ConfigManager.Config.RegData.Username;
            PasswordBox.Password = ConfigManager.Config.RegData.Password;
            LoginBox.Focus(FocusState.Programmatic);
        }

        /// <summary>
        ///     Обработка нажатия кнопки авторизации
        /// </summary>
        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (LoginBox.Text.Length >= 20 || LoginBox.Text == "" || LoginBox.Text.Contains(" "))
            {
                WarningText.Text = "Invalid nickname format";
                return;
            }

            if (!(await CheckNickUnicall()))
            {
                return;
            }

            var httpWebRequest =
                (HttpWebRequest) WebRequest.Create(
                    $"http://{ConfigManager.Config.IP}:{ConfigManager.Config.Port}/api/Login");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            var regData = new RegData {Username = LoginBox.Text, Password = PasswordBox.Password};
            var json = JsonConvert.SerializeObject(regData);
            var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write(json);
            streamWriter.Close();

            string result;

            try
            {
                var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
                var streamReader = new StreamReader(httpResponse.GetResponseStream());
                result = streamReader.ReadToEnd();
            }
            catch (Exception)
            {
                WarningText.Text = "Unauthorized";
                return;
            }

            var temp = JsonConvert.DeserializeAnonymousType(result, new Config {Token = ""});


            ConfigManager.Config.Token = temp.Token;

            ConfigManager.Config.RegData = regData;
            WarningText.Text = "Success!";
            ConfigManager.WriteConfig();
            var nvMain = (NavigationView) Frame.FindName("nvMain");
            if (nvMain != null) nvMain.SelectedItem = nvMain.MenuItems.OfType<NavigationViewItem>().Last();
        }

        /// <summary>
        ///     Проверка на уникальность ника
        /// </summary>
        /// <returns>Если уникален то истина</returns>
        private async Task<bool>  CheckNickUnicall()
        {
            var httpWebRequest =
                (HttpWebRequest) WebRequest.Create(
                    $"http://{ConfigManager.Config.IP}:{ConfigManager.Config.Port}/api/Login?username={LoginBox.Text}");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            try
            {
                WarningText.Text = "Connecting...";
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                var streamReader = new StreamReader(httpResponse.GetResponseStream());
                var result = streamReader.ReadToEnd();
                WarningText.Text = "Done!";

                var checkNickUnicall = JsonConvert.DeserializeAnonymousType(result, new { response = false }).response;
                if(!checkNickUnicall)
                    WarningText.Text = "User with this nickname does not exist";

                return checkNickUnicall;
            }
            catch
            {
                WarningText.Text = "The server is not responding!";
                return false;
            }
        }

        private async void ShowMessage()
        {
            var deleteFileDialog = new ContentDialog
            {
                Title = "Error",
                Content = "The server is not responding!",
                PrimaryButtonText = "Close"
            };

            var result = await deleteFileDialog.ShowAsync();

            if (result != ContentDialogResult.Primary) return;
        }
        /// <summary>
        ///     Обработка нажатия enter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
                ButtonBase_OnClick(sender, e);
        }

        /// <summary>
        ///     Обработка нажатия enter
        /// </summary>
        private void PasswordBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
                ButtonBase_OnClick(sender, e);
        }

        private void ContentFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {

        }
    }
}