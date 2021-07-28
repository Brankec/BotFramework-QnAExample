using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace BotFramework_QnAExample.Util.Image
{
    public static class ImageUtils
    {
        public static Dictionary<string, string> contentTypes = new()
        {
            {"image/jpeg", ".jpeg"},
            {"image/gif", ".gif"},
            {"image/png", ".png"}
        };
        
        public static byte[] StreamImageFromUrl(string imageUrl, string contentType)
        {
            byte[] imageBytes;
            using (WebClient client = new WebClient())
            {
                var type = GetImageTypeFromContentType(contentType);
                imageBytes = client.DownloadData(new Uri(imageUrl + type));
            }

            return imageBytes;
        }
        
        public static void SaveImageFromUrl(string imageUrl, string contentType, string fileName)
        {
            byte[] imageBytes;
            using (WebClient client = new WebClient())
            {
                var type = GetImageTypeFromContentType(contentType);
                client.DownloadFileAsync(new Uri(imageUrl), fileName+type);
            }
        }

        public static string GetImageTypeFromContentType(string contentType)
        {
            var type = contentTypes.FirstOrDefault(x => x.Equals(contentType)).Value;

            return type;
        }

        public static bool IsContentTypeImage(string contentType)
        {
            var isImage = contentTypes.ContainsKey(contentType);

            return isImage;
        }
    }
}