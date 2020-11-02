using System;
using System.Globalization;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace CLient_CS_UWP
{
    /// <summary>
    ///     Страница настроек
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        /// <summary>
        ///     Инициализация компонентов страницы
        /// </summary>
        public SettingsPage()
        {
            InitializeComponent();
            MillisecondsSleepSlider.Value = ConfigManager.Config.MillisecondsSleep;
            WindowW.Text = ConfigManager.Config.Size.Width.ToString(CultureInfo.InvariantCulture);
            WindowH.Text = ConfigManager.Config.Size.Height.ToString(CultureInfo.InvariantCulture);
            IPBox.Text = ConfigManager.Config.IP;
            PortBox.Text = ConfigManager.Config.Port.ToString();
        }

        /// <summary>
        ///     Обработчик изменения слайдера
        /// </summary>
        private void MillisecondsSleepSlider_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ConfigManager.Config.MillisecondsSleep = (int) MillisecondsSleepSlider.Value;
        }

        /// <summary>
        ///     Обработчик изменения ширины приложения
        /// </summary>
        private void WindowW_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(WindowW.Text, out var configSizeWidth))
                ConfigManager.Config.Size = new Size(configSizeWidth, ConfigManager.Config.Size.Height);
        }

        /// <summary>
        ///     Обработчик изменения высоты приложения
        /// </summary>
        private void WindowH_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(WindowH.Text, out var configSizeHeight))
                ConfigManager.Config.Size = new Size(ConfigManager.Config.Size.Width, configSizeHeight);
        }

        /// <summary>
        ///     Обработчик сохранения настроек в файл
        /// </summary>
        private void Page_LosingFocus(UIElement sender, LosingFocusEventArgs args)
        {
            ConfigManager.WriteConfig();
        }

        /// <summary>
        ///     Обработчик нажатия кнопки "о разработчиках"
        /// </summary>
        private async void ButtonDev_click(object sender, RoutedEventArgs e)
        {
            var deleteFileDialog = new ContentDialog
            {
                Title = "GROUP HW",
                Content = "Open the official website?",
                PrimaryButtonText = "Open",
                SecondaryButtonText = "Close"
            };

            var result = await deleteFileDialog.ShowAsync();

            if (result != ContentDialogResult.Primary) return;

            var uri = new Uri("http://group-hw.ru/");
            await Launcher.LaunchUriAsync(uri);
        }

        /// <summary>
        ///     Обработчик изменения адреса сервера
        /// </summary>
        private void IPBox_OnTextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            ConfigManager.Config.IP = IPBox.Text;
        }

        /// <summary>
        ///     Обработчик изменения порта сервера
        /// </summary>
        private void PortBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(IPBox.Text, out var Port))
                ConfigManager.Config.Port = Port;
        }
    }
}