namespace WebHook8.Services
{
    public interface IWhatsappCloudSendMessage 
    {
        Task<bool> Execute(object model);
    }
}
