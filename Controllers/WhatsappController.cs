using Google.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
        public WhatsappController(IWhatsappCloudSendMessage whatsappCloudSendMessage, IUtil util, IOptions<EWhatsAppSettings> whatsappSettings)
        {
            _whatsappCloudSendMessage = whatsappCloudSendMessage;
            _util = util;
            _whatsappSettings = whatsappSettings.Value;
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
                    var userNumber = message.From;
                    var userText = GetUserText(message);
                    object objectMessage;
                    objectMessage = _util.TextMessage("Pruebas ejemplo de texto", userNumber);
                    await _whatsappCloudSendMessage.Execute(objectMessage);
                }
                return Ok("EVENT_RECEIVED");

            }
            catch (Exception ex)
            {
                // Es necesario, ya que si no esta, whatsapp genera un ciclo infinito
                return Ok("EVENT_RECEIVED");


            }

        }

        private string GetUserText(Message message)
        {
            string messageType = message.Type;
            if (messageType.ToUpper() == "TEXT")
            {
                return message.Text.Body;
            }
            else if (messageType.ToUpper() == "INTERACTIVE")
            {
                string interactiveType = message.Interactive.Type;
                if (interactiveType.ToUpper() == "LIST_REPLY")
                {
                    return message.Interactive.List_Reply.Title;
                }
                if (interactiveType.ToUpper() == "BUTTON_REPLY")
                {
                    return message.Interactive.Button_Reply.Title;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
