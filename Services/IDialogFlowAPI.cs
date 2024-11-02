using Google.Apis.Auth.OAuth2;
using Google.Cloud.Dialogflow.V2;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace WebHook8.Services
{
    public interface IDialogflowService
    {
        Task<DetectIntentResponse> DetectIntentAsync(string sessionId, string text, string languageCode = "es");
    }
}
