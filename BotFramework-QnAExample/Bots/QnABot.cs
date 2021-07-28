using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BotFramework_QnAExample.Bots.QnABotActions;
using BotFramework_QnAExample.Bots.QnABotActions.ImageRecognition;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Streaming.Payloads;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BotFramework_QnAExample.Bots
{
    public class QnABot : ActivityHandler
    {
        private QnAAnswers _qnaAnswers;
        private IImgRecognition _imgRecognitionRep;
        
        public QnABot(IConfiguration configuration, ILogger<QnABot> logger, IHttpClientFactory httpClientFactory)
        {
            _qnaAnswers = new QnAAnswers(configuration, logger, httpClientFactory);
            _imgRecognitionRep = new ComputerVision("e75b9326fb254197bc99a65a87a7a4d2", "https://bonsai-hiring-cv.cognitiveservices.azure.com/");
        }
        
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await DialogManager(turnContext, cancellationToken);
        }

        private async Task DialogManager(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await AnswersDialog(turnContext, cancellationToken);
            await ImageTagDialog(turnContext, cancellationToken);
        }
        
        private async Task RespondToUser(string response, ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            //Sending the answer to the sender
            if (!String.IsNullOrEmpty(response))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(response), cancellationToken);
                return;
            }
            
            await turnContext.SendActivityAsync(MessageFactory.Text("I don't know the answer to that."), cancellationToken);
        }

        private async Task AnswersDialog(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var attachments = turnContext.Activity.Attachments;
            if (attachments != null)
            {
                return;
            }

            var response = await _qnaAnswers.Run(turnContext, cancellationToken);
            await RespondToUser(response, turnContext, cancellationToken);
        }

        private async Task ImageTagDialog(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var attachments = turnContext.Activity.Attachments;
            if (attachments == null || attachments.Count < 1)
            {
                return;
            }
            
            ///TODO: add a check to make sure the attachment is an image no a regular file
            
            await _imgRecognitionRep.AnalyzeImgUrl(attachments.First());

            var tags = _imgRecognitionRep.GetImgTagNames();
            var joinedTagsString = String.Join(", ", tags);
            var response = String.IsNullOrEmpty(joinedTagsString) ? "" : $"This image contains the following tags: {joinedTagsString}";
            
            await RespondToUser(response, turnContext, cancellationToken);
        }
        
    }
}