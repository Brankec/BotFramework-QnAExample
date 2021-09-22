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

        public async Task Run(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            //Checking if the user command exists in one of the case statements and then executing the corresponding function for that command
            var userInput = turnContext.Activity.Text;
            var commandSuccessful = false;
            switch (userInput)
            {
                case string x when x.Contains("/Change image recognition:"):
                    commandSuccessful = await ChangeImageRecognition(userInput);
                    break;
                default:
                    await turnContext.SendActivityAsync(MessageFactory.Text("Unknown command."), cancellationToken);
                    return;
            }

            if (commandSuccessful)
                await turnContext.SendActivityAsync(MessageFactory.Text("Command ran successfully!"), cancellationToken);
            else
                await turnContext.SendActivityAsync(MessageFactory.Text("Command failed."), cancellationToken);
        }

        private async Task<bool> ChangeImageRecognition(string userInput)
        {
            var newValue = userInput.Substring(userInput.LastIndexOf(':') + 1);
            newValue = String.Concat(newValue.Where(c => !Char.IsWhiteSpace(c))); //Clearing all empty spaces

            var settings = SettingsManager.GetSettings();
            settings.ImageProcessingType = newValue;
            SettingsManager.ModifyLoadedSettings(settings);

            return true;
        }
    }
}
