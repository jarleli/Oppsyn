using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Oppsyn.ExtensionsClasses;
using Oppsyn.Models;
using Oppsyn.SlackClients;
using Serilog;
using SlackConnector;
using SlackConnector.Models;

namespace Oppsyn
{
    public class SlackConnectionHost : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly BotConfig _config;
        private readonly MessageDispatcher _dispatcher;
        private bool _isDisconnecting = false;
        private ISlackConnection _slackConnection;
        //private CancellationToken _cancelToken; //if we are interrupted during startup
        //private CancellationToken _stoppingToken; //if we are signaled to stop

        public SlackConnectionHost(ILogger logger, BotConfig config,  MessageDispatcher dispatcher, ISlackClientFactory factory)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _config = config ?? throw new System.ArgumentNullException(nameof(config));
            _dispatcher = dispatcher ?? throw new System.ArgumentNullException(nameof(dispatcher));
        }


        public override Task StartAsync(CancellationToken cancellationToken)
        {
            //_cancelToken = cancellationToken;
            return base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken token)
        {
            var task = Disconnect();
            await base.StopAsync(token);
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return ConnectToSlack();
        }

        private async Task ConnectToSlack()
        {
            try
            {
                var slackConnector = new SlackConnector.SlackConnector();
                _slackConnection = await slackConnector.Connect(_config.Slack.BotUserToken);

                _slackConnection.OnMessageReceived += MessageReceived;
                _slackConnection.OnDisconnect += OnDisconnect;
                _slackConnection.OnReconnecting += OnReconnecting;
                _slackConnection.OnReconnect += OnReconnect;

                _logger.Information("Connected!");
                _logger.Verbose($"Team Name: {_slackConnection.Team.Name}");
                _logger.Verbose($"Bots Name: {_slackConnection.Self.Name}");

            }
            catch (System.Exception e)
            {
                _logger.Error(e, e.Message);
                throw;
            }        
        }

        private Task OnReconnect()
        {
            _logger.Information("SlackConnection Restored!");
            return Task.CompletedTask;
        }

        private Task OnReconnecting()
        {
            _logger.Information("Attempting to reconnect to Slack...");
            return Task.CompletedTask;
        }

        private async Task Disconnect()
        {
            _isDisconnecting = true;

            if (_slackConnection != null && _slackConnection.IsConnected)
            {
                await _slackConnection.Close();
            }
        }

        private void OnDisconnect()
        {
            if (_isDisconnecting)
            {
                _logger.Information("Disconnected.");
            }
            else
            {
                _logger.Information("Disconnected from server, attempting to reconnect...");
                Reconnect();
            }
        }

        private void Reconnect()
        {
            _logger.Information("Reconnecting...");
            if (_slackConnection != null)
            {
                _slackConnection.OnMessageReceived -= MessageReceived;
                _slackConnection.OnDisconnect -= OnDisconnect;
                _slackConnection = null;
            }

            _isDisconnecting = false;
            ConnectToSlack()
                .ContinueWith(task =>
                {
                    if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                    {
                        _logger.Information("SlackConnection restored.");
                    }
                    else
                    {
                        _logger.Information($"Error while reconnecting: {task.Exception}");
                    }
                })
                .Wait();
        }

        private async Task MessageReceived(SlackMessage message)
        {
            _logger.Information("[Message found from '{FromUserName} in channel {ChannelName}']:\n" +
                "{MessageText}", message.User.Name, message.ChatHub.Name, message.Text.SafeSubstring(0, 90));

            try
            {
                await _dispatcher.HandleMessage(message);
            }
            catch (System.Exception e)
            {
                _logger.Error(e, e.Message);
            }
        }


    }
}