using System.Text.Json;
using System.Threading.Tasks;
using BotFramework_QnAExample.Util.Image;
using BotFramework_QnAExample.Utils.ImageAPI.Model;
using RestSharp;

namespace BotFramework_QnAExample.Utils.ImageAPI.API
{
    public class ImgurAPI : IImageAPI
    {
        private string baseUrl = "https://api.imgur.com";

        public async Task<string> UploadImageFromStream(byte[] imageStream, string contentType)
        {
            var client = new RestClient(baseUrl+"/3/upload");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Client-ID dcb856ea174f210");
            request.AddFile("image", imageStream, contentType);
            
            IRestResponse response = client.Execute(request);
            string jsonResponse = response.Content;
            var model = JsonSerializer.Deserialize<ImgurUploadResponseModel>(jsonResponse);
            return model?.data?.link;
        }

        public async Task<string> UploadImageFromFile(string imagePath)
        {
            var client = new RestClient(baseUrl+"/3/upload");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Client-ID dcb856ea174f210");
            request.AddFile("image", imagePath);
            
            IRestResponse response = client.Execute(request);
            string jsonResponse = response.Content;
            var model = JsonSerializer.Deserialize<ImgurUploadResponseModel>(jsonResponse);
            return model?.data?.link;
        }
    }
}