using Google.Cloud.Dialogflow.V2;
using WebHook8.Services;

public class DialogflowAPI: IDialogflowAPI
{
    private readonly SessionsClient _sessionsClient;
    private readonly string _projectId;

    public DialogflowAPI(IConfiguration configuration)
    {
        _projectId = configuration["Dialogflow:ProjectId"];
        _sessionsClient = SessionsClient.Create();
    }

    public async Task<string> GetResponseAsync(string userText)
    {
        var sessionId = Guid.NewGuid().ToString();
        var sessionName = new SessionName(_projectId, sessionId);

        var queryInput = new QueryInput
        {
            Text = new TextInput
            {
                Text = userText,
                LanguageCode = "es"
            }
        };

        var response = await _sessionsClient.DetectIntentAsync(sessionName, queryInput);
        return response.QueryResult.FulfillmentText;
    }
}