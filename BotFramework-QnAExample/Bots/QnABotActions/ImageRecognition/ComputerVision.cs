using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Bot.Schema;

namespace BotFramework_QnAExample.Bots.QnABotActions.ImageRecognition
{
    public class ComputerVision : IImgRecognition
    {
        // Computer Vision subscription key and endpoint
        string _subscriptionKey;// = "e75b9326fb254197bc99a65a87a7a4d2";
        string _endpoint;// = "https://bonsai-hiring-cv.cognitiveservices.azure.com/";
        private ComputerVisionClient _client;
        private List<VisualFeatureTypes?> _features;
        private ImageAnalysis _analyzedImg;

        public ComputerVision(string subscriptionKey, string endpoint)
        {
            _subscriptionKey = subscriptionKey;
            _endpoint = endpoint;
            _client = Authenticate(endpoint, subscriptionKey);
        }
        
        public async Task AnalyzeImgUrl(Attachment attachment)
        {
            _features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Tags
            };
            
            // Analyze the URL image
            //_analyzedImg = await _client.AnalyzeImageAsync(attachment.ContentUrl, visualFeatures: _features);
            _analyzedImg = await _client.AnalyzeImageAsync("https://i.imgur.com/5snElrr.jpeg", visualFeatures: _features);
        }

        public IEnumerable<string> GetImgTagNames()
        {
            var tags = new List<string>();
            foreach (var tag in _analyzedImg.Tags)
            {
                tags.Add(tag.Name);
            }

            return tags;
        }

        private static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
                new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
                    { Endpoint = endpoint };
            return client;
        }
    }
}