using System;

namespace Server_CS
{
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