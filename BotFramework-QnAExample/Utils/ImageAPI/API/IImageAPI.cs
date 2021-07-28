using System.Threading.Tasks;

namespace BotFramework_QnAExample.Utils.ImageAPI.API
{
    public interface IImageAPI
    {
        public Task<string> UploadImageFromStream(byte[] imageStream, string contentType);

        public Task<string> UploadImageFromFile(string imagePath);
    }
}