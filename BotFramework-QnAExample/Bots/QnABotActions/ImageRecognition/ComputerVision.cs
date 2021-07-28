using System.Collections.Generic;
using System.Threading.Tasks;
using BotFramework_QnAExample.Util.Image;
using BotFramework_QnAExample.Utils.ImageAPI.API;
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
            
            //saving the image sent by the user as bytes
            var imageBytes = ImageUtils.StreamImageFromUrl(attachment.ContentUrl, ""); //This image url doesn't contain content type in the url
            
            //Uploading the sent image and getting the link to access it
            IImageAPI imgurApi = new ImgurAPI();
            var imgLink = await imgurApi.UploadImageFromStream(imageBytes, attachment.ContentType);
            
            // Analyze the Image link
            _analyzedImg = await _client.AnalyzeImageAsync(imgLink, visualFeatures: _features);
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
            //authenticating computer vision
            ComputerVisionClient client =
                new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
                    { Endpoint = endpoint };
            return client;
        }
    }
}