using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using SlackConnector.Models;

namespace Oppsyn
{
    public class MessageDispatcher
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IMessageHandler> _messageHandlers;

        public MessageDispatcher(ILogger logger, IEnumerable<IMessageHandler> messageHandlers)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageHandlers = messageHandlers ?? throw new ArgumentNullException(nameof(messageHandlers));
        }
        public async Task HandleMessage(SlackMessage message)
        {
            foreach (var handler in _messageHandlers)
            {
                try
                {
                    await handler.HandleMessage(message);
                }
                catch (System.Exception e)
                {
                    _logger.Error(e, e.Message);
                }
            }
        }
    }
}