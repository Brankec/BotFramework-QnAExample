using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BotFramework_QnAExample.Bots.QnABotActions
{
    public class QnAAnswers
    {
        private readonly IConfiguration _configuration; //Configuration for connecting to the qna middleware endpoint defined in appsettings
        private readonly ILogger<QnABot> _logger; //For logging activity
        private readonly IHttpClientFactory _httpClientFactory; //For creating an alternate client with which to talk to the QnA Maker.

        public QnAAnswers(IConfiguration configuration, ILogger<QnABot> logger, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> Run(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            //Adding an endpoint that the QnA Maker will use
            var qnaMaker = InitQnAMaker();

            // Making a call to the QnA Maker service.
            var response = await qnaMaker.GetAnswersAsync(turnContext);
            
            if (response != null && response.Length > 0)
            {
                return response[0].Answer;
            }

            return "";
        }
        
        private QnAMaker InitQnAMaker()
        {
            var httpClientFac = _httpClientFactory.CreateClient();
            
            //Setting up QnA Maker to use an endpoint defined in appsettings.json
            return new QnAMaker(new QnAMakerEndpoint
                {
                    KnowledgeBaseId = _configuration["QnAKnowledgebaseId"],
                    EndpointKey = _configuration["QnAEndpointKey"],
                    Host = _configuration["QnAEndpointHostName"]
                },
                null,
                httpClientFac);
        }
    }
}