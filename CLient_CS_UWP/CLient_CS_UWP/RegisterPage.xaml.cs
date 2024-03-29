﻿using System;
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
    ///     Страница регистрации
    /// </summary>
    public sealed partial class RegisterPage : Page
    {
        /// <summary>
        ///     Инициализация страницы регистрации
        /// </summary>
        public RegisterPage()
        {
            InitializeComponent();
            LoginBox.Focus(FocusState.Programmatic);
        }

        /// <summary>
        ///     Обработка нажатия кнопки регистрации
        /// </summary>
        private async void Register_OnClick(object sender, RoutedEventArgs e)
        {
            RegisterButton.IsEnabled = false;
            if (LoginBox.Text.Length >= 20 || LoginBox.Text == "" || LoginBox.Text.Contains(" "))
            {
                WarningText.Text = "Invalid nickname format";
                RegisterButton.IsEnabled = true;
                return;
            }

            if (await CheckNickUnicall())
            {
                WarningText.Text = "Nickname is busy";
                RegisterButton.IsEnabled = true;
                return;
            }

            if (PasswordBox1.Password != PasswordBox2.Password)
            {
                WarningText.Text = "Passwords do not match";
                RegisterButton.IsEnabled = true;
                return;
            }

            var httpWebRequest =
                (HttpWebRequest) WebRequest.Create(
                    $"http://{ConfigManager.Config.IP}:{ConfigManager.Config.Port}/api/Login");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            var regData = new RegData {Username = LoginBox.Text, Password = PasswordBox1.Password};
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
                RegisterButton.IsEnabled = true;
                return;
            }

            var temp = JsonConvert.DeserializeAnonymousType(result, new Config {Token = ""});

            ConfigManager.Config.Token = temp.Token;

            ConfigManager.Config.RegData = regData;
            WarningText.Text = "Success!";
            RegisterButton.IsEnabled = true;
            ConfigManager.WriteConfig();
            var nvMain = (NavigationView) Frame.FindName("nvMain");
            nvMain.SelectedItem = nvMain.MenuItems.OfType<NavigationViewItem>().Last();
        }

        /// <summary>
        ///     Проверка на уникальность ника
        /// </summary>
        /// <returns>Если уникален то истина</returns>
        private async Task<bool> CheckNickUnicall()
        {
            var httpWebRequest =
                (HttpWebRequest) WebRequest.Create(
                    $"http://{ConfigManager.Config.IP}:{ConfigManager.Config.Port}/api/Login?username={LoginBox.Text}");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            try
            {
                WarningText.Text = "Connecting...";
                LoadBar.IsActive = true;
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                var streamReader = new StreamReader(httpResponse.GetResponseStream());
                var result = streamReader.ReadToEnd();
                LoadBar.IsActive = false;
                WarningText.Text = "Done!";

                return JsonConvert.DeserializeAnonymousType(result, new { response = false }).response;
            }
            catch
            {
                LoadBar.IsActive = false;
                ShowMessage();
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
        private void Box_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
                Register_OnClick(sender, e);
        }
    }
}