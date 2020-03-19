using System;

namespace Hello
{
    public class HelloMessage
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Payload { get; set; }
        public string Identifier { get; set; }
    }
}