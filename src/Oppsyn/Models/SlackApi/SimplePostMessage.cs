using System;
using SlackConnector.Models;

namespace Oppsyn
{
    public class SimplePostMessage 
    {
        public string Channel { get; internal set; }
        public string Text { get; set; }

        /// <summary>
        /// Should only exist on subtype ThreadMessage
        /// </summary>
        public string ThreadTs { get; set; }
    }
}