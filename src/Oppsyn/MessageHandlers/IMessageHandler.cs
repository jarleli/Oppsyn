using Noobot.Core.MessagingPipeline.Request;
using SlackConnector.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Oppsyn
{
    public interface IMessageHandler
    {
        Task<bool> HandleMessage(SlackMessage incomingMessage);
    }
}
