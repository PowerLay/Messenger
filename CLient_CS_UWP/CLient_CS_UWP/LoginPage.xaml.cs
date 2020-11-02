using System;
using System.IO;
using System.Linq;
using System.Net;
using Windows.System;
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
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
            LoginBox.Text = ConfigManager.Config.RegData.Username;
            PasswordBox.Password = ConfigManager.Config.RegData.Password;
            LoginBox.Focus(FocusState.Programmatic);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (LoginBox.Text.Length >= 20 || LoginBox.Text == "" || LoginBox.Text.Contains(" "))
            {
                WarningText.Text = "Invalid nickname format";
                return;
            }

            if (!CheckNickUnicall())
            {
                WarningText.Text = "User with this nickname does not exist";
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
            nvMain.SelectedItem = nvMain.MenuItems.OfType<NavigationViewItem>().Last();
        }

        private bool CheckNickUnicall()
        {
            var httpWebRequest =
                (HttpWebRequest) WebRequest.Create(
                    $"http://{ConfigManager.Config.IP}:{ConfigManager.Config.Port}/api/Login?username={LoginBox.Text}");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
            var streamReader = new StreamReader(httpResponse.GetResponseStream());
            var result = streamReader.ReadToEnd();
            return JsonConvert.DeserializeAnonymousType(result, new {response = false}).response;
        }

        private void LoginBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
                ButtonBase_OnClick(sender, e);
        }

        private void PasswordBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
                ButtonBase_OnClick(sender, e);
        }
    }
}