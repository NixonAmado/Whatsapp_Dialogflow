using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace WebHook8.Services
{
    public interface IDialogflowAPI
    {
        public Task<string> GetResponseAsync(string userText);
    }
}
