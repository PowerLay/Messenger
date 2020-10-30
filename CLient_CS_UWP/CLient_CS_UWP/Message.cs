using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace CLient_CS_UWP
{
    /// <summary>
    ///     <para>Класс Сообщение</para>
    ///     <br>Ts - время отправки сообщения (по серверу)</br>
    ///     <br>Name - имя клиента</br>
    ///     <br>Text - сообщение клиента</br>
    ///     <br>ToString - функция преобразования полей класса в строку для печати</br>
    /// </summary>
    public class Message
    {
        private int _ts;
        public SolidColorBrush BgColor;

        public DateTime DateTime;
        public HorizontalAlignment MsgAlignment;
        public string TopHeight;
        public Visibility Visibility;

        public Message()
        {
        }

        public Message(HorizontalAlignment align)
        {
            MsgAlignment = align;

            // If received message, use accent background
            if (MsgAlignment == HorizontalAlignment.Left)
            {
                BgColor = new SolidColorBrush(Colors.DarkGray);
                Visibility = Visibility.Visible;
                TopHeight = "*";
            }

            // If sent message, use light gray
            else if (MsgAlignment == HorizontalAlignment.Right)
            {
                BgColor = new SolidColorBrush(Colors.DeepSkyBlue);
                Visibility = Visibility.Collapsed;
                TopHeight = "0";
            }
            // If sent message, use light gray
            else if (MsgAlignment == HorizontalAlignment.Center)
            {
                BgColor = new SolidColorBrush(Colors.Gray);
                Visibility = Visibility.Collapsed;
                TopHeight = "0";
            }
        }

        public int Ts
        {
            get => _ts;
            set
            {
                _ts = value;
                DateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Ts);
            }
        }

        public string Name { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return $"[{DateTime}] {Name}: {Text}";
        }

        public string HourMinute()
        {
            return DateTime.ToString("HH:mm");
        }
    }
}