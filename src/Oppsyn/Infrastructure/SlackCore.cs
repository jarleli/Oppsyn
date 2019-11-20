using System;
using System.Threading.Tasks;
using Oppsyn.ExtensionsClasses;
using Serilog;
using SlackConnector;
using SlackConnector.Models;

namespace Oppsyn
{
    [Obsolete("Dont use")]
    public class SlackCore //: BackgroundService
    {
        private readonly ILogger _logger;
        private readonly MessageDispatcher _dispatcher;

        public ISlackConnection SlackConnection { get; private set; }

        //public IReadOnlyDictionary<string, SlackUser> UserCache => SlackConnection.UserCache;

        public ISlackConnectionProvider ConnectionProvider { get; }

        public SlackCore(ILogger logger,  MessageDispatcher dispatcher, ISlackConnectionProvider connectionProvider)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _dispatcher = dispatcher ?? throw new System.ArgumentNullException(nameof(dispatcher));
            ConnectionProvider = connectionProvider ?? throw new System.ArgumentNullException(nameof(connectionProvider));
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    var connection = await ConnectionProvider.GetConnection();
        //    connection.OnMessageReceived += MessageReceived;

        //}

        public async Task MessageReceived(SlackMessage message)
        {
            _logger.Information("[Message found from '{FromUserName}']", message.User.Name);
            _logger.Debug($"MSG: {message.Text.SafeSubstring(0, 90)}");

            try
            {
                await _dispatcher.HandleMessage(message);
            }
            catch (System.Exception e)
            {
                _logger.Error(e, e.Message);
            }
        }

        public async Task SendMessage(BotMessage message)
        {
            _logger.Information($"Sending message '{message.Text.SafeSubstring(0, 90)}'");
            await SlackConnection.Say(message);
        }

    }

}
