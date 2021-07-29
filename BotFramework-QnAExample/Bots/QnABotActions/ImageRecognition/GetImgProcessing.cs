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
                    break;
                case "ML":
                    return new MachineLearningImg();
                    break;
            }
            
            return new ComputerVision("e75b9326fb254197bc99a65a87a7a4d2", "https://bonsai-hiring-cv.cognitiveservices.azure.com/");
        }
    }
}