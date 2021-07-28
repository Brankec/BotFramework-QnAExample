using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Bot.Schema;

namespace BotFramework_QnAExample.Bots.QnABotActions.ImageRecognition
{
    public interface IImgRecognition
    {
        public Task AnalyzeImgUrl(Attachment attachment);

        public IEnumerable<string> GetImgTagNames();
    }
}