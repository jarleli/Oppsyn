using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json.Linq;
using Oppsyn.SlackClients;
using Serilog;
using SlackConnector.Models;

namespace Oppsyn
{
    public class RunAzureVisionOnImageUpload : IMessageHandler
    {
        private readonly ISlackClientFactory _slackClientFactory;
        private readonly IComputerVisionClient _visionClient;
        private readonly ILogger _logger;
        private readonly List<VisualFeatureTypes> _visionVisualFeatures = new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects
            };


        public RunAzureVisionOnImageUpload(ILogger logger, ISlackClientFactory slackClientFactory, IComputerVisionClient visionClient)
        {
            _slackClientFactory = slackClientFactory ?? throw new ArgumentNullException(nameof(slackClientFactory));
            _visionClient = visionClient ?? throw new ArgumentNullException(nameof(visionClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> HandleMessage(SlackMessage message)
        {
            if (new[] { SlackChatHubType.DM , SlackChatHubType.Channel }.Contains(message.ChatHub.Type ))
            {
                if (message.Files.Any())
                {
                    foreach (var file in message.Files)
                    {
                        try
                        {
                            var analysis = await AnalyzeFileAsImage(file);
                            await PostImageAnalysis(message, file, analysis);

                        }
                        catch (Exception e)
                        {
                            _logger.Error(e,e.Message);
                        }                    
                    }
                }
            }
            return false;
        }

        private async Task PostImageAnalysis(SlackMessage message, SlackFile file, string analysis)
        {
            var jo = JObject.Parse(message.RawData);
            var tts = jo["thread_ts"]?.ToString();
            var ts = jo["ts"].ToString();
            
            var client = _slackClientFactory.GetMessageClient();
            await client.PostMessage(new ChatMessage() { 
                Chathub = message.ChatHub, 
                Text = $"Image '{file.Title ?? "__"}' contains: {analysis}", 
                ThreadTs = tts ?? ts
            });
        }

        private async Task<string> AnalyzeFileAsImage(SlackFile file)
        {
            var client = _slackClientFactory.GetFileClient();
            var response = await client.DownloadFile(file.UrlPrivateDownload);
            var fileStream =await response.Content.ReadAsStreamAsync();


            var results = await _visionClient.AnalyzeImageInStreamAsync(fileStream, _visionVisualFeatures);

            var details = new List<string>();
            if(results.Adult.IsAdultContent)
            { details.Add("Adult Content"); }
            var cap = results.Description.Captions.Where(d => d.Confidence > 0.8).Select(d => d.Text);
            details.AddRange(cap);
            var cats = results.Categories.Where(c => c.Score > .8 && c.Detail != null).SelectMany(c => c.Detail?.Celebrities?.Select(cel => cel.Name) ?? c.Detail?.Landmarks?.Select(lan => lan.Name));
            details.AddRange(cats);
            var objs = results.Objects.Where(o => o.Confidence > .9).Select(o => o.ObjectProperty);
            details.AddRange(objs);
            var brands = results.Brands.Where(b => b.Confidence > .9).Select(b => b.Name);
            details.AddRange(brands);
            var tags = results.Tags.Where(t => t.Confidence > 0.96).Select(t => t.Name);
            details.AddRange(tags);

            if (details.Count() < 5)
            {
                var faces = results.Faces.Take(3).Select(f => $"{f.Gender} ({f.Age})");
                details.AddRange(faces);
            }
            var str = string.Join(", ", details.Select(d => $"`{d}`"));

            return str;
        }
    }
}
