using BotFramework_QnAExample.Utils.Settings;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BotFramework_QnAExample.Bots.QnABotActions
{
    public class UserCommands
    {

        public bool IsCommand(ITurnContext<IMessageActivity> turnContext)
        {
            try
            {
                var userInput = turnContext.Activity.Text;
                var firstChar = userInput[0].ToString();
                if (firstChar == "/")
                {
                    return true;
                }
            }
            catch (Exception) { };

            return false;
        }

        public async Task Start(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var state = Run(turnContext);

            switch(state.Result)
            {
                case -1:
                    await turnContext.SendActivityAsync(MessageFactory.Text("Unknown command."), cancellationToken);
                    break;
                case 0:
                    await turnContext.SendActivityAsync(MessageFactory.Text("Command ran successfully!"), cancellationToken);
                    break;
                case 1:
                        await turnContext.SendActivityAsync(MessageFactory.Text("Command failed."), cancellationToken);
                    break;
            }
        }

        private async Task<int> Run(ITurnContext<IMessageActivity> turnContext)
        {
            //Checking if the user command exists in one of the case statements and then executing the corresponding function for that command
            var userInput = turnContext.Activity.Text;
            switch (userInput)
            {
                case string x when x.Contains("/Change image recognition:"):
                    return await  ChangeImageRecognition(userInput);
                default:
                    return -1;
                    
            }
        }

        private async Task<int> ChangeImageRecognition(string userInput)
        {
            try
            {
                var newValue = userInput.Substring(userInput.LastIndexOf(':') + 1);
                newValue = String.Concat(newValue.Where(c => !Char.IsWhiteSpace(c))); //Clearing all empty spaces

                var settings = SettingsManager.GetSettings();
                settings.ImageProcessingType = newValue;
                SettingsManager.ModifyLoadedSettings(settings);
            }
            catch(Exception)
            {
                return 1;
            }

            return 0;
        }
    }
}
