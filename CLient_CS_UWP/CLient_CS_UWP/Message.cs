using System;

namespace CLient_CS_UWP
{
    public class Message
    {
        public int Ts { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return $"[{new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Ts)}] {Name}: {Text}";
        }
    }
}