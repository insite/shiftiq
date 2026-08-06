#pragma warning disable OPENAI001

using System.Collections.Concurrent;
using System.Text.Json;

using OpenAI.Responses;

using Shift.Common;

namespace Shift.Service.Chatbot.OpenAI;

public class OpenAiChatbotService: IChatbotService
{
    private const string DefaultSystemPrompt =
        """
        You are Jasper, an assistant for searching data in Shift iQ database.
        Your sole purpose is to help users search, analyze, and report on data stored in the Shift iQ database.

        You may:

        - Answer questions about data contained in the Shift iQ database.
        - Search for records using the tools provided.
        - Generate counts, summaries, aggregates, and reports from database data.
        - Export query results in supported formats (CSV, PDF, etc.).
        - Explain the meaning of database fields, tables, and report results.
        - Help users formulate searches and filters.

        You must NOT:

        - Answer general knowledge questions unrelated to Shift iQ data.
        - Provide opinions, personal advice, legal advice, medical advice, financial advice, or software development guidance unrelated to the database.
        - Invent, estimate, or assume database values that have not been returned by a tool.
        - Discuss topics outside the Shift iQ database domain.

        When a request is outside your scope, respond with:
        "I am a Shift iQ Database Assistant and can only help with searching, analyzing, and reporting on Shift iQ database information. Please ask a question related to Shift iQ data."

        Always use available tools when database information is required. Never claim to know database contents without querying the appropriate tool first.
        If a tool returns no data, clearly state that no matching records were found.

        When a CSV is exported:
            1. Tell the user the file name (rendered as a link to the file) and row count
            2. Use count tool to calculate the total row count
            3. You may display the first 3 rows in the chat as an example

        If you apply the limit user didn't asked for - explicitely tell the user about this.
        When asked for available fields, just do export with limit = 0, it will return only Columns.
        When asked what data can be searched or reports generated, provide this info based on tools you have.
        For tools that display result directly in the chat, assume the result was displayed when the tool was called.

        Keep answers concise.
        """;

    private class Settings
    {
        public required string Model { get; set; }
        public required string SystemPrompt { get; set; }
    }

    private readonly object _settingsSyncRoot = new();

    private readonly ResponsesClient _client;
    private readonly bool _allowTookits;
    private readonly IChatbotToolService _toolService;
    private readonly PermissionCache _permissionCache;
    private readonly FilePaths _filePaths;
    private readonly ConcurrentDictionary<(Guid, Guid), OpenAiChatbotSession> _sessions = [];

    public string Model { get; private set; }
    public string SystemPrompt { get; private set; }

    public OpenAiChatbotService(AppSettings settings, IChatbotToolService toolService, PermissionCache permissionCache, FilePaths filePaths)
    {
        var openAiSettings = settings.Integration.OpenAI;
        var apiKey = openAiSettings.ApiKey;
        var model = openAiSettings.Model;
        var allowToolkits = openAiSettings.AllowToolkits;

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("Set OpenAI:ApiKey in appsettings.json before starting the chatbot.");

        if (string.IsNullOrWhiteSpace(model))
            throw new InvalidOperationException("Set OpenAI:Model in appsettings.json before starting the chatbot.");

        _client = new(apiKey);
        _allowTookits = allowToolkits;
        _toolService = toolService;
        _permissionCache = permissionCache;
        _filePaths = filePaths;

        Model = model;
        SystemPrompt = DefaultSystemPrompt;

        InitSettings();
    }

    public IChatbotSession GetSession(IPrincipal principal)
    {
        var organizationId = principal.OrganizationId;
        var userId = principal.UserId;

        return _sessions.GetOrAdd((organizationId, userId), _ => CreateSession(principal));
    }

    private OpenAiChatbotSession CreateSession(IPrincipal principal)
    {
        var (toolExecutor, tools) = OpenAiTools.Create(_toolService.GetTools(principal), _allowTookits);
        var sessionId = UniqueIdentifier.Create();
        var organizationId = principal.OrganizationId;
        var userId = principal.UserId;

        return new OpenAiChatbotSession(sessionId, organizationId, userId, _permissionCache, Model, SystemPrompt, _client, toolExecutor, tools);
    }

    public string[] GetAvailableModels()
    {
        return 
        [
            "gpt-5.4",
            "gpt-5.2",
            "gpt-5.4-mini",
        ];
    }

    public void ModifyModel(string model)
    {
        if (!GetAvailableModels().Any(x => x == model))
            throw new ArgumentException($"Model: {model}");

        ModifySettings(model, SystemPrompt);
    }

    public void ModifySystemPrompt(string prompt)
    {
        ModifySettings(Model, prompt);
    }

    public void SetDefaultSystemPrompt()
    {
        ModifySettings(Model, DefaultSystemPrompt);
    }

    private void InitSettings()
    {
        if (!File.Exists(_filePaths.OpenAiSettingsFilePath))
            return;

        var content = File.ReadAllText(_filePaths.OpenAiSettingsFilePath);
        var settings = JsonSerializer.Deserialize<Settings>(content)!;

        Model = settings.Model;
        SystemPrompt = settings.SystemPrompt;
    }

    private void ModifySettings(string model, string systemPrompt)
    {
        lock (_settingsSyncRoot)
        {
            var settings = new Settings
            {
                Model = model,
                SystemPrompt = systemPrompt,
            };

            var content = JsonSerializer.Serialize(settings);

            File.WriteAllText(_filePaths.OpenAiSettingsFilePath, content);

            Model = model;
            SystemPrompt = systemPrompt;

            var sessions = _sessions.Values.ToArray();

            foreach (var session in sessions)
                session.ModifySettings(model, systemPrompt);
        }
    }
}