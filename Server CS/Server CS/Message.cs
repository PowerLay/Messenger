using System;

namespace Server_CS
{
    /// <summary>
    ///     <para>Класс Сообщение</para>
    /// </summary>
    public class Message
    {
        /// <summary>
        ///     <br>Ts - время отправки сообщения (по серверу)</br>
        /// </summary>
        public int Ts { get; set; }

        /// <summary>
        ///     <br>Name - имя клиента</br>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     <br>Text - сообщение клиента</br>
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     <br>ToString - функция преобразования полей класса в строку для печати</br>
        /// </summary>
        /// <returns> [Time] Name: Text </returns>
        public override string ToString()
        {
            //TODO bad code hour +3, local tim was not realse
            return $"[{new DateTime(1970, 1, 1, 3, 0, 0, 0).AddSeconds(Ts)}] {Name}: {Text}";
        }
    }
}