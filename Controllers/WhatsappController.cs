using Google.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebHook8.Integrations;
using WebHook8.Models.WhatsappCloud;
using WebHook8.Services;
using WebHook8.Util;

namespace WebHook8.Controllers
{
    [ApiController]
    [Route("api/whatsapp")]
    public class WhatsappController : Controller
    {
        private readonly IWhatsappCloudSendMessage _whatsappCloudSendMessage;
        private readonly IUtil _util;
        private readonly EWhatsAppSettings _whatsappSettings;
        private readonly ILogger<WhatsappController> _logger;
        private readonly IWhatsAppDialogflowHandler _whatsAppDialogflowHandler;

        public WhatsappController(
            IWhatsappCloudSendMessage whatsappCloudSendMessage,
            IUtil util,
            IWhatsAppDialogflowHandler whatsAppDialogflowHandler,
            IOptions<EWhatsAppSettings> whatsappSettings,
            ILogger<WhatsappController> logger)
           

        {
            _whatsappCloudSendMessage = whatsappCloudSendMessage;
            _util = util;
            _whatsappSettings = whatsappSettings.Value;
            _logger = logger;
            _whatsAppDialogflowHandler = whatsAppDialogflowHandler;
        }


        [HttpGet]
        public IActionResult VerifyToken()
        {
            string accessToken = _whatsappSettings.AccessToken;
            var token = Request.Query["hub.verify_token"].ToString();
            var challenge = Request.Query["hub.challenge"].ToString();

            if (challenge != null && token != null && token == accessToken)
            {
                return Ok(challenge);
            }
            else
            {
                _logger.LogWarning("Token verification failed.");
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReceivedMessage([FromBody] EWhatsAppCloud body)
        {
            try
            {
                var verifyMmessage = body.Entry[0]?.Changes[0]?.Value?.Messages;

                if (verifyMmessage  != null)
                {
                    var message = verifyMmessage[0];
                    EWhatsAppMessage whatsappMessage = new EWhatsAppMessage
                    {
                        UserNumber = message.From,
                        UserText = GetUserText(message)
                    };

                    await _whatsAppDialogflowHandler.HandleIncomingMessage(whatsappMessage);
                    
                    _logger.LogInformation("Message sent successfully to {userNumber}", message.From);
                }
                return Ok("EVENT_RECEIVED");

            }
            catch (Exception ex)
            {
                // Es necesario, ya que si no esta, whatsapp genera un ciclo infinito
                _logger.LogError(ex, "Error receiving message");
                return Ok("EVENT_RECEIVED");


            }

        }

        private string GetUserText(Message message)
        {
            string messageType = message.Type.ToUpper();

            switch (messageType)
            {
                case "TEXT":
                    return message.Text.Body;

                case "INTERACTIVE":
                    string interactiveType = message.Interactive.Type.ToUpper();

                    return interactiveType switch
                    {
                        "LIST_REPLY" => message.Interactive.List_Reply.Title,
                        "BUTTON_REPLY" => message.Interactive.Button_Reply.Title,
                        _ => string.Empty
                    };

                default:
                    return string.Empty;
            }
        }

    }
}
