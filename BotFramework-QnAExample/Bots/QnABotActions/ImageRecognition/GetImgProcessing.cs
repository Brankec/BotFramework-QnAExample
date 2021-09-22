using BotFramework_QnAExample.Utils.Settings;
using NuGet.Configuration;

namespace BotFramework_QnAExample.Bots.QnABotActions.ImageRecognition
{
    public static class ImgProcessing
    {
        public static IImgRecognition GetImgRecognition()
        {
            switch (SettingsManager.GetSettings().ImageProcessingType)
            {
                case "CV":
                    return new ComputerVision("e75b9326fb254197bc99a65a87a7a4d2", "https://bonsai-hiring-cv.cognitiveservices.azure.com/");
                case "ML":
                    return new MachineLearningImg();
                case "FR":
                    return new FaceRecognition("2f58de9588d54f3e83b0d7049c162b84", "https://branimir.cognitiveservices.azure.com/");
            }
            
            return new ComputerVision("e75b9326fb254197bc99a65a87a7a4d2", "https://bonsai-hiring-cv.cognitiveservices.azure.com/");
        }
    }
}