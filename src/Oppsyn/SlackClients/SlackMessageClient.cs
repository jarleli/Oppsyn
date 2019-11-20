using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Oppsyn.SlackClients
{
    public class SlackMessageClient: ISlackMessageClient
    {
        private readonly HttpClient _client;
        private readonly Uri _baseUri =new Uri("https://slack.com/api/chat.postMessage");

        public SlackMessageClient(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }


        public async Task PostMessage(ChatMessage message)
        {
            var uri = _baseUri;
            var body = new 
            {
                channel = message.Chathub.Id,
                text = message.Text,
                thread_ts = message.ThreadTs

            };
            var stringcontent = JsonConvert.SerializeObject(body);
            var content = new StringContent(stringcontent, Encoding.UTF8);
            content.Headers.ContentType.MediaType = "application/json";
            var res = await _client.PostAsync(uri, content, new CancellationTokenSource(5000).Token);
            res.EnsureSuccessStatusCode();
            //var resString = await res.Content.ReadAsStringAsync();
        }
    }

    public interface ISlackMessageClient
    {
        public Task PostMessage(ChatMessage mssage);
    }

}
