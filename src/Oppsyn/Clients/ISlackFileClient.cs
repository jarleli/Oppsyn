using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Oppsyn.SlackClients
{
    public class SlackFileClient : ISlackFileClient
    {
        private readonly HttpClient _client;

        public SlackFileClient(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }


        public async Task<HttpResponseMessage> DownloadFile(Uri uri)
        {
            var res = await  _client.GetAsync(uri,HttpCompletionOption.ResponseHeadersRead);
            res.EnsureSuccessStatusCode();
            return res;
        }
    }
    public interface ISlackFileClient
    {
        public Task<HttpResponseMessage> DownloadFile(Uri uri);
    }
}


