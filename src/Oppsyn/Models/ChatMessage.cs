using System;
using SlackConnector.Models;

namespace Oppsyn
{
    public  class ChatMessage 
    {
        public SlackChatHub Chathub { get; set; }
        public string Text { get; set; }

        /// <summary>
        /// Should only exist on subtype ThreadMessage
        /// </summary>
        public string ThreadTs { get; set; }
    }
}