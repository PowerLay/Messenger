using System;

namespace Server_CS
{
    /// <summary>
    /// <para>Класс Сообщение</para>
    /// <br>MsgTime - время отправки сообщения (по серверу)</br>
    /// <br>Name - имя клиента</br>
    /// <br>Msg - сообщение клиента</br>
    /// <br>ToString - функция преобразования полей класса в строку для печати</br>
    /// </summary>
    public class Message
    {
        public int TimesTamp { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return $"[{TimesTamp}] {Name}: {Text}";
        }
    }
}