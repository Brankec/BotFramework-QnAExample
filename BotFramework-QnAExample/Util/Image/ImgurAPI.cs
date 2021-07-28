using System.Threading.Tasks;
using BotFramework_QnAExample.Util.Image.Model;

namespace BotFramework_QnAExample.Util.Image
{
    public class ImgurAPI : IImageAPI<ImgurUploadResponseModel>
    {
        public Task<ImgurUploadResponseModel> UploadImage()
        {
            return null;
        }
    }
}