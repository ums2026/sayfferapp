using System;
namespace Sayffer.Model
{
    public class CustomMessageToSend
    {
        public string Subject { get; set; }
        public string MessageText { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string MessageStatus { get; set; }
        public string Time { get; set; }

    }
}
