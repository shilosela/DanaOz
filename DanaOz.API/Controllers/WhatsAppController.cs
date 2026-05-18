using DanaOz.BLL.Models;
using DanaOz.BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Twilio.AspNet.Core;
using Twilio.Http;
using Twilio.TwiML;
using Twilio.TwiML.Messaging;

namespace DanaOz.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WhatsAppController : TwilioController
    {
        private readonly ConversationService _conversationService;
        private readonly ILogger<WhatsAppController> _logger;

        public WhatsAppController(
            ConversationService conversationService,
            ILogger<WhatsAppController> logger)
        {
            _conversationService = conversationService;
            _logger = logger;
        }

        [HttpPost("incoming")]
        public async Task<TwiMLResult> IncomingMessage()
        {
            var messagingResponse = new MessagingResponse();

            try
            {
                // Extract message details from Twilio request
                var from = Request.Form["From"].ToString();
                var body = Request.Form["Body"].ToString();

                _logger.LogInformation($"Incoming message from {from}: {body}");

                // Pass to BLL
                var incomingMessage = new MessageModel
                {
                    PhoneNumber = from,
                    MessageText = body,
                    ReceivedAt = DateTime.UtcNow
                };

                var response = await _conversationService.HandleIncomingMessageAsync(incomingMessage);

                // Send response back via Twilio
                messagingResponse.Append(new Message(response.MessageText));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing message: {ex.Message}");
                messagingResponse.Append(new Message(
                    "אוי, סליחה! 😓 חוויתי עומס רגעי. אנא נסה שוב בעוד כמה שניות 💪"));
            }

            return TwiML(messagingResponse);
        }
    }
}