using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace CLient_CS_UWP
{
    /// <summary>
    ///     Класс Сообщение
    /// </summary>
    public class Message
    {
        /// <summary>
        ///     Время DateTime
        /// </summary>
        private DateTime _dateTime;

        /// <summary>
        ///     _ts - время отправки сообщения (по серверу)
        /// </summary>
        private int _ts;

        /// <summary>
        ///     Цвет рамки сообщения
        /// </summary>
        public SolidColorBrush BgColor;

        /// <summary>
        ///     Центрирование сообщения
        /// </summary>
        public HorizontalAlignment MsgAlignment;

        /// <summary>
        ///     Цвет ника в зависимости от онлайна
        /// </summary>
        public SolidColorBrush OnlineBrush;

        /// <summary>
        ///     Видимость PersonPicture
        /// </summary>
        public Visibility PersonPictureVisibility;

        /// <summary>
        ///     Высота поля с ником
        /// </summary>
        public string TopHeight;

        public Message()
        {
        }

        /// <summary>
        ///     Конструктор сообщения
        /// </summary>
        /// <param name="align">Центрирование сообщения</param>
        /// <param name="online">Состояние пользователя (В сети/не в сети)</param>
        public Message(HorizontalAlignment align, bool online)
        {
            MsgAlignment = align;
            OnlineBrush = online ? new SolidColorBrush(Colors.DarkGreen) : new SolidColorBrush(Colors.Black);

            switch (MsgAlignment)
            {
                // If received message, use accent background
                // If sent message, use light gray
                case HorizontalAlignment.Left:
                    BgColor = new SolidColorBrush(Colors.DarkGray);
                    PersonPictureVisibility = Visibility.Visible;
                    TopHeight = "*";
                    break;
                // If sent message, use light gray
                case HorizontalAlignment.Right:
                    BgColor = new SolidColorBrush(Colors.DeepSkyBlue);
                    PersonPictureVisibility = Visibility.Collapsed;
                    TopHeight = "0";
                    break;
                case HorizontalAlignment.Center:
                    BgColor = new SolidColorBrush(Colors.Gray);
                    PersonPictureVisibility = Visibility.Collapsed;
                    TopHeight = "0";
                    break;
            }
        }

        /// <summary>
        ///     время отправки сообщения (по серверу) unix формат
        /// </summary>
        public int Ts
        {
            get => _ts;
            set
            {
                _ts = value;
                _dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Ts);
            }
        }

        /// <summary>
        ///     Имя отправителя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Текст сообщения
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     <br>ToString - функция преобразования полей класса в строку для печати</br>
        /// </summary>
        /// <returns> [Time] Name: Text </returns>
        public override string ToString()
        {
            return $"[{_dateTime}] {Name}: {Text}";
        }

        /// <summary>
        ///     Время в коротком формате
        /// </summary>
        /// <returns>HH:mm</returns>
        public string HourMinute()
        {
            return _dateTime.ToString("HH:mm");
        }
    }
}