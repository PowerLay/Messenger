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
        public DateTime MsgTime { get; set; }
        public string Name { get; set; }
        public string Msg { get; set; }

        public override string ToString()
        {
            return $"[{MsgTime}] {Name}: {Msg}";
        }
    }
}