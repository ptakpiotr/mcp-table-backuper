using ChatSimpleClient.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;

namespace ChatSimpleClient.Services;

public class SqlServerBackupToolService(
    IOptions<SseMcpServerSettings> serverSettings,
    ILogger<SqlServerBackupToolService> logger,
    IChatClient client)
{
    private async Task<IList<McpClientTool>> GetAiToolsAsync(IMcpClient mcpClient, CancellationToken cancellationToken = default)
    {
        var tools = await mcpClient.ListToolsAsync(cancellationToken: cancellationToken);

        logger.LogInformation("Found {toolsCount} tools", tools.Count);

        return tools;
    }

    public async Task<string> BackupAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var clientTransport = new SseClientTransport(new()
        {
            Endpoint = new Uri(serverSettings.Value.Endpoint)
        });

        await using var mcpClient =
            await McpClientFactory.CreateAsync(clientTransport, cancellationToken: cancellationToken);
        
        var tools = await GetAiToolsAsync(mcpClient, cancellationToken);
        
        var chatOptions = new ChatOptions()
        {
            Tools = [..tools],
            ToolMode = ChatToolMode.RequireAny
        };

        var resp = await client.GetResponseAsync(prompt, chatOptions, cancellationToken);

        return resp.Text;
    }
}