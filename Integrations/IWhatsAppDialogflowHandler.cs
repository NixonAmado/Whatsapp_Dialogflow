using WebHook8.Models.WhatsappCloud;

namespace WebHook8.Integrations
{
    public interface IWhatsAppDialogflowHandler
    {
        Task HandleIncomingMessage(EWhatsAppMessage message);
    }
}
