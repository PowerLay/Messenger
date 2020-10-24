using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

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
            NickTexBox.Text = ConfigManager.Config.Name;
            MillisecondsSleepSlider.Value = ConfigManager.Config.MillisecondsSleep;
        }

        private void NickTexBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ConfigManager.Config.Name = NickTexBox.Text;
        }

        private void MillisecondsSleepSlider_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ConfigManager.Config.MillisecondsSleep = (int) MillisecondsSleepSlider.Value;
        }

        private void contentSV_LostFocus(object sender, RoutedEventArgs e)
        {
            ConfigManager.WriteConfig();
        }
    }
}