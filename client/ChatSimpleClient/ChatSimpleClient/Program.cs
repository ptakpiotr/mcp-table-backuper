using Azure;
using Azure.AI.OpenAI;
using ChatSimpleClient.Models;
using ChatSimpleClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<AzureOpenAiSettings>()
    .Bind(builder.Configuration.GetSection("AzureOpenAiSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<SseMcpServerSettings>()
    .Bind(builder.Configuration.GetSection("McpServerSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<SqlServerBackupToolService>();

builder.Services.AddChatClient(sp =>
{
    var azureOpenAiSettings = sp.GetRequiredService<IOptions<AzureOpenAiSettings>>();

    var client = new AzureOpenAIClient(new Uri(azureOpenAiSettings.Value.Url),
        new AzureKeyCredential(azureOpenAiSettings.Value.ApiKey));
    
    return client.GetChatClient(azureOpenAiSettings.Value.DeploymentName).AsIChatClient()
        .AsBuilder()
        .UseFunctionInvocation()
        .Build();
});

var app = builder.Build();

app.MapGet("/", () => Results.Ok(":)"));

app.MapPost("/",
    async ([FromBody] PromptModel prompt,
        [FromServices] SqlServerBackupToolService backupToolService, CancellationToken cancellationToken) =>
    {
        var res = await backupToolService.BackupAsync(prompt.Prompt, cancellationToken);
        return Results.Ok(res);
    });

app.Run();