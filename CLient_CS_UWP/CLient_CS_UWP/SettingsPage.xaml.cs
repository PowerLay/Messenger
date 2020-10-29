using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace CLient_CS_UWP
{
    /// <summary>
    ///     Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            MillisecondsSleepSlider.Value = ConfigManager.Config.MillisecondsSleep;
            WindowW.Text = ConfigManager.Config.Size.Width.ToString();
            WindowH.Text = ConfigManager.Config.Size.Height.ToString();
        }

        private void MillisecondsSleepSlider_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ConfigManager.Config.MillisecondsSleep = (int) MillisecondsSleepSlider.Value;
        }

        private void WindowW_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(WindowW.Text, out var configSizeWidth))
                ConfigManager.Config.Size = new Size(configSizeWidth, ConfigManager.Config.Size.Height);
        }

        private void WindowH_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(WindowH.Text, out var configSizeHeight))
                ConfigManager.Config.Size = new Size(ConfigManager.Config.Size.Width, configSizeHeight);
        }

        private void Page_LosingFocus(UIElement sender, LosingFocusEventArgs args)
        {
            ConfigManager.WriteConfig();
        }

        private async void ButtonDev_click(object sender, RoutedEventArgs e)
        {
            ContentDialog deleteFileDialog = new ContentDialog()
            {
                Title = "DEVELOPERS",
                Content = "GROUP HW",
                PrimaryButtonText = "More",
                SecondaryButtonText = "Close"
            };

            ContentDialogResult result = await deleteFileDialog.ShowAsync();
            
            if (result == ContentDialogResult.Primary) //Если нажата MORE
            {
                System.Diagnostics.Process.Start("http://group-hw.ru/"); //Я тут был и ВНЁС СВОЙ ВКЛАД! УРА!
            }
        }
    }
}