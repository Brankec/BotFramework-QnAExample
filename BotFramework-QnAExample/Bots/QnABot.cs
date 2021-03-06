using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BotFramework_QnAExample.Bots.QnABotActions;
using BotFramework_QnAExample.Bots.QnABotActions.ImageRecognition;
using BotFramework_QnAExample.Util.Image;
using BotFramework_QnAExample.Utils.Settings;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BotFramework_QnAExample.Bots
{
    public class QnABot : ActivityHandler
    {
        private QnAAnswers _qnaAnswers;
        private IImgRecognition _imgRecognition;
        private UserCommands userCommands = new UserCommands();
        
        public QnABot(IConfiguration configuration, ILogger<QnABot> logger, IHttpClientFactory httpClientFactory)
        {
            _qnaAnswers = new QnAAnswers(configuration, logger, httpClientFactory);
            
            //Loading the image recognition defined in the appsettings.json file
            _imgRecognition = ImgProcessing.GetImgRecognition();
        }
        
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (userCommands.IsCommand(turnContext))
            {
                await userCommands.Run(turnContext, cancellationToken);
                return;
            }
            
            await Dialogs(turnContext, cancellationToken);
        }

        private async Task Dialogs(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await AnswersDialog(turnContext, cancellationToken);
            await ImageTagDialog(turnContext, cancellationToken);
        }
        
        private async Task RespondToUser(string response, ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            //Sending an answer to the sender
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
            //checking if the user sent a file
            if (attachments != null)
            {
                return;
            }

            //Getting the response from the QnA Maker and then sending it
            var response = await _qnaAnswers.Run(turnContext, cancellationToken);
            await RespondToUser(response, turnContext, cancellationToken);
        }

        private async Task ImageTagDialog(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var response = "";
            var attachments = turnContext.Activity.Attachments;
            //Checking if the user sent any files
            if (attachments == null || attachments.Count < 1)
            {
                return;
            }
            
            //Checking if the file sent is an image file
            if (!ImageUtils.IsContentTypeImage(attachments.First().ContentType))
            {
                return;
            }

            try
            {
                //Analyzing the the image
                await _imgRecognition.AnalyzeImgAttachment(attachments.First());

                //Getting the image tags, then forming and sending a response
                response = _imgRecognition.GetImgTagNames();
            }
            catch (Exception e)
            {
                response = "Failed to analyze the image. Please try again later";
            }

            await RespondToUser(response, turnContext, cancellationToken);
        }
        
    }
}