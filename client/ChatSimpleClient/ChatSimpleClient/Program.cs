using ChatSimpleClient.Models;
using ChatSimpleClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<OllamaSettings>()
    .Bind(builder.Configuration.GetSection("OllamaSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<SseMcpServerSettings>()
    .Bind(builder.Configuration.GetSection("McpServerSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<SqlServerBackupToolService>();

builder.Services.AddChatClient(sp =>
{
    var ollamaOptions = sp.GetRequiredService<IOptions<OllamaSettings>>();

    return new OllamaChatClient(ollamaOptions.Value.Url, ollamaOptions.Value.Model);
});

var app = builder.Build();

app.MapGet("/", () => Results.Ok(":)"));

app.MapPost("/",
    async ([FromBody] PromptModel prompt, [FromServices] IChatClient client,
        [FromServices] SqlServerBackupToolService backupToolService, CancellationToken cancellationToken) =>
    {
        var tools = await backupToolService.GetAIToolsAsync(cancellationToken);
        var chatOptions = new ChatOptions()
        {
            MaxOutputTokens = 1000,
            AllowMultipleToolCalls = true,
            Tools = [..tools],
            ToolMode = ChatToolMode.Auto
        };

        var resp = await client.GetResponseAsync(prompt.Prompt, chatOptions, cancellationToken);

        return Results.Ok(resp.Text);
    });

app.Run();