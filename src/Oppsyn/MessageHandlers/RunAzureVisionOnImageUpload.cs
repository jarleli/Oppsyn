using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oppsyn.Clients;
using Oppsyn.ExtensionsClasses;
using Oppsyn.Models;
using Oppsyn.SlackClients;
using Serilog;
using SlackConnector.Models;

namespace Oppsyn
{
    public class RunAzureVisionOnImageUpload : IMessageHandler
    {
        private readonly ISlackClientFactory _slackClientFactory;
        private readonly IVisionClientFactory _visionClientFactory;
        private readonly BotConfig _config;
        private readonly ILogger _logger;
        private readonly List<VisualFeatureTypes> _visionVisualFeatures = new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects
            };


        public RunAzureVisionOnImageUpload(ILogger logger, ISlackClientFactory slackClientFactory, IVisionClientFactory visionClientFactory, BotConfig config)
        {
            _slackClientFactory = slackClientFactory ?? throw new ArgumentNullException(nameof(slackClientFactory));
            _visionClientFactory = visionClientFactory ?? throw new ArgumentNullException(nameof(visionClientFactory));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> HandleMessage(SlackMessage message)
        {
            var jobject =  JObject.Parse(message.RawData);

            if (new[] { SlackChatHubType.DM , SlackChatHubType.Channel }.Contains(message.ChatHub.Type ))
            {
                if (message.Files.Any())
                {
                    await AnalyzeUploadedImages(message, jobject);
                }
                if(jobject["blocks"] != null)
                { 
                    await AnalyzeImagesInPostedLink(message, jobject); 
                }

            }
            return false;
        }

        /// <summary>
        /// If the posted message has a link we investigate the attachments created for it.
        /// If we find an image in the attachements we analyze it.
        /// </summary>
        /// <param name="incomingMessage"></param>
        /// <param name="jobject"></param>
        /// <returns></returns>
        private async Task AnalyzeImagesInPostedLink(SlackMessage incomingMessage, JObject jobject)
        {
            var message = JsonConvert.DeserializeObject<IncomingMessage>(incomingMessage.RawData);
            if (message.Blocks.BlockHasTypeLinkOrHasChildWithTypeLink()) {
                var client = _slackClientFactory.CreateMessageClient();
                var fullMessage = await client.GetSpecificMessage(message.Channel, message.Ts);

                _logger.Debug("Found link in text block and retrieved full message to examine attachments in {MessageTs}.",fullMessage.Ts);
                foreach (var attachment in fullMessage.Attachments)
                {
                    var imageUri = attachment.ImageUrl ?? attachment.ThumbUrl;
                    if(imageUri != null)
                    {
                        var analysis = await AnalyzeFileAsImage(imageUri);
                        var replyTs = fullMessage.ThreadTs ?? fullMessage.Ts ;
                        await PostImageAnalysis(incomingMessage.ChatHub.Id, attachment.Title, analysis, replyTs);
                    }
                }
            }
        }

        /// <summary>
        /// If file is of type image we try to download and analyze it
        /// </summary>
        /// <param name="message"></param>
        /// <param name="jobject"></param>
        /// <returns></returns>
        private async Task AnalyzeUploadedImages(SlackMessage message, JObject jobject)
        {
            foreach (var file in message.Files)
            {
                try
                {
                    if (file.Mimetype.StartsWith("image/"))
                    {
                        var analysis = await AnalyzeFileAsImage(file.UrlPrivateDownload);
                        if (_config.Slack.PostFindings)
                        {
                            var replyTs = jobject["thread_ts"]?.ToString() ?? jobject["ts"].ToString();
                            await PostImageAnalysis(message.ChatHub.Id, file.Title, analysis, replyTs);
                        }
                    }
                    else
                    {
                        _logger.Debug("Skipping file because it was not an image: {FileName}", file.Name);
                    }

                }
                catch (Exception e)
                {
                    _logger.Error(e, e.Message);
                }
            }
        }


        private async Task<string> AnalyzeFileAsImage(Uri uri)
        {
            var fileStream = await GetFileStream(uri);

            var visionClient = _visionClientFactory.CreateVisionClient();
            var results = await visionClient.AnalyzeImageInStreamAsync(fileStream, _visionVisualFeatures);

            var details = new List<string>();
            if (results.Adult.IsAdultContent)
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

        private async Task<System.IO.Stream> GetFileStream(Uri uri)
        {
            var client = _slackClientFactory.CreateFileClient();
            var response = await client.DownloadFile(uri);
            var fileStream = await response.Content.ReadAsStreamAsync();
            return fileStream;
        }

        private async Task PostImageAnalysis(string  channel, string title, string analysis, string replyTs)
        {
            var client = _slackClientFactory.CreateMessageClient();
            await client.PostMessage(new SimplePostMessage()
            {
                Channel = channel,
                Text = $"Image '{title ?? "__"}' contains: {analysis}",
                ThreadTs = replyTs
            });
        }
    }
}
