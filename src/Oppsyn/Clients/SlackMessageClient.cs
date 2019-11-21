using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Oppsyn.SlackClients
{
    public class SlackMessageClient: ISlackMessageClient
    {
        private readonly HttpClient _client;
        private readonly Uri _baseUri =new Uri("https://slack.com/api/");

        public SlackMessageClient(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task PostMessage(SimplePostMessage message)
        {
            var uri = new Uri(_baseUri, "chat.postMessage");
            var body = new 
            {
                channel = message.Channel,
                text = message.Text,
                thread_ts = message.ThreadTs

            };
            var stringcontent = JsonConvert.SerializeObject(body);
            var content = new StringContent(stringcontent, Encoding.UTF8);
            content.Headers.ContentType.MediaType = "application/json";
            var res = await _client.PostAsync(uri, content, new CancellationTokenSource(5000).Token);
            res.EnsureSuccessStatusCode();
        }

        public async Task<FullIncomingMessage> GetSpecificMessage(string channel, string ts)
        {
            var uri = new Uri(_baseUri, "conversations.history");
            var builder = new UriBuilder(uri);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["channel"] = channel;
            query["latest"] = ts;
            query["limit"] = "1";
            query["inclusive"] = "true";
            builder.Query = query.ToString();
            var fullUri = builder.ToString();

            var result = await _client.GetAsync(fullUri);
            var res = await DeserializeResultContent<SlackResult<FullIncomingMessage>>(result);
            if (res.Ok)
            {
                return res.Messages.Single();
            }
            else
            {
                throw new HttpRequestException($"Slack api says: '{res.Error}'");
            }
        }

        private static async Task<T> DeserializeResultContent<T>(HttpResponseMessage result)
        {
            result.EnsureSuccessStatusCode();
            //endre til stream
            var stringResult = await result.Content.ReadAsStringAsync();
            //{"ok":false,"error":"channel_not_found"}

            return JsonConvert.DeserializeObject<T>(stringResult);
        }
    }

    public interface ISlackMessageClient
    {
        public Task PostMessage(SimplePostMessage mssage);
        public Task<FullIncomingMessage> GetSpecificMessage(string channel, string ts);
    }

}
