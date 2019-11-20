using SlackConnector;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Oppsyn
{
    [Obsolete("Dont use.")]
    public class SlackConnectionProvider : ISlackConnectionProvider
    {
        private ISlackConnection _connection;
        private readonly ManualResetEvent _wait;
        private readonly TimeSpan _getConnectionDelay;

        public SlackConnectionProvider()
        {
            _wait = new ManualResetEvent(false);
            _getConnectionDelay = TimeSpan.FromSeconds(2);

        }
        public Task<ISlackConnection> GetConnection()
        {
            return GetConnection(new CancellationTokenSource(_getConnectionDelay).Token);
        }

        public Task<ISlackConnection> GetConnection(CancellationToken token)
        {
            var task = new Task<ISlackConnection>(() =>
            {
                if (_connection == null)
                {
                    _wait.WaitOne();
                }
                return _connection;
            }, token);
            task.Start();
            return task;
        }
        public void SetSlackConnection(ISlackConnection connection)
        {
            _connection = connection;
            _wait.Set();
        }

        internal void SetStoppingToken(CancellationToken stoppingToken)
        {
            stoppingToken.Register(()=>_wait.Set());
        }
    }

    public interface ISlackConnectionProvider
    {
        Task<ISlackConnection> GetConnection();
        Task<ISlackConnection> GetConnection(CancellationToken token);
    }
}