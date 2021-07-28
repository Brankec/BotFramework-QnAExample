using System;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Drawing;
using BotFramework_QnAExample.Util.Image.Model;

namespace BotFramework_QnAExample.Util.Image
{
    public interface IImageAPI<T>
    {
        public Task<T> UploadImage();
    }
}