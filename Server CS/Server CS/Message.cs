using System;

namespace Server_CS
{
    /// <summary>
    ///     <para>Класс Сообщение</para>
    ///     <br>TimesTamp - время отправки сообщения (по серверу)</br>
    ///     <br>Name - имя клиента</br>
    ///     <br>Text - сообщение клиента</br>
    ///     <br>ToString - функция преобразования полей класса в строку для печати</br>
    /// </summary>
    public class Message
    {
        public int Ts { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            //TODO bad code hour +3, local tim was not realse
            return $"[{new DateTime(1970, 1, 1, 3, 0, 0, 0).AddSeconds(Ts)}] {Name}: {Text}";
        }
    }
}