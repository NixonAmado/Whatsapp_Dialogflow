using Google.Apis.Auth.OAuth2;
using Google.Cloud.Dialogflow.V2;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Grpc.Core;
using Grpc.Auth;
using WebHook8.Services;

public class DialogflowService : IDialogflowService
{
    private readonly SessionsClient _sessionsClient;
    private readonly string _projectId;

    public DialogflowService(IOptions<DialogflowConfig> config)
    {
        var dialogflowConfig = config.Value;
        _projectId = dialogflowConfig.ProjectId;

        var credentials = GetCredentialsFromConfig(dialogflowConfig.Credentials);

        var builder = new SessionsClientBuilder
        {
            Endpoint = "dialogflow.googleapis.com:443",
            ChannelCredentials = credentials.ToChannelCredentials()
        };

        _sessionsClient = builder.Build();
    }

    private GoogleCredential GetCredentialsFromConfig(DialogflowCredentials credentials)
    {
        var credentialsJson = JsonSerializer.Serialize(credentials);
        return GoogleCredential.FromJson(credentialsJson)
            .CreateScoped(SessionsClient.DefaultScopes);
    }

    public async Task<DetectIntentResponse> DetectIntentAsync(string sessionId, string text, string languageCode = "es")
    {
        try
        {
            var sessionPath = SessionName.FromProjectSession(_projectId, sessionId);
            var textInput = new TextInput { Text = text, LanguageCode = languageCode };
            var queryInput = new QueryInput { Text = textInput };

            return await _sessionsClient.DetectIntentAsync(sessionPath, queryInput);
        }
        catch (Exception ex)
        {
            // Aquí puedes agregar logging
            throw;
        }
    }
}