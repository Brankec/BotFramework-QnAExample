using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Logging;

namespace BotFramework_QnAExample.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class AnswersController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter; //a delegate with which we will invoke the bot implementation
        private readonly IBot _bot; //the bot implementation

        public AnswersController(IBotFrameworkHttpAdapter adapter, IBot bot)
        {
            _adapter = adapter;
            _bot = bot;
        }

        [HttpPost]
        public async Task PostAsync()
        {
            // invoking the bot
            await _adapter.ProcessAsync(Request, Response, _bot);
        }
    }
}