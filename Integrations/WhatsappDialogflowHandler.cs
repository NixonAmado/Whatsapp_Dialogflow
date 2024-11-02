namespace WebHook8.Integrations
{
    using System.Threading.Tasks;
    using WebHook8.Models.WhatsappCloud;
    using WebHook8.Services; // Asegúrate de que esta ruta sea correcta
    using WebHook8.Util; // Asegúrate de que esta ruta sea correcta

    public class WhatsappDialogflowHandler : IWhatsAppDialogflowHandler
    {
        private readonly IDialogflowService _dialogflowService;
        private readonly IWhatsappCloudSendMessage _whatsappCloudSendMessage;
        private readonly IUtil _util;

        public WhatsappDialogflowHandler(IDialogflowService dialogflowService, IWhatsappCloudSendMessage whatsappCloudSendMessage, IUtil util)
        {
            _dialogflowService = dialogflowService;
            _whatsappCloudSendMessage = whatsappCloudSendMessage;
            _util = util;
        }

        public async Task HandleIncomingMessage(EWhatsAppMessage message)
        {
            // Procesar el mensaje de WhatsApp
                // Llamar a Dialogflow
                var response = await _dialogflowService.DetectIntentAsync(message.UserNumber, message.UserText);

                // Preparar la respuesta para enviar a WhatsApp
                var dialogflowResponseMessage = response.QueryResult.FulfillmentText;

                // Enviar la respuesta de Dialogflow de vuelta a WhatsApp
                await SendMessageToWhatsApp(message.UserNumber, dialogflowResponseMessage);
        }



        private async Task SendMessageToWhatsApp(string userNumber, string messageText)
        {
            var messageObject = _util.TextMessage(messageText, userNumber);
            await _whatsappCloudSendMessage.Execute(messageObject);
        }
    }

}
