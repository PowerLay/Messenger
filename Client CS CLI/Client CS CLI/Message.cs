using System;

namespace Client_CS_CLI
{
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