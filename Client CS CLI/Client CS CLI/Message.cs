using System;

namespace Client_CS_CLI
{
    public class Message
    {
        public int TimesTamp { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return $"[{ (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(TimesTamp)}] {Name}: {Text}";
        }
    }
}