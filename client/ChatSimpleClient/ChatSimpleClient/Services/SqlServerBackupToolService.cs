using ChatSimpleClient.Models;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;

namespace ChatSimpleClient.Services;

public class SqlServerBackupToolService(
    IOptions<SseMcpServerSettings> serverSettings,
    ILogger<SqlServerBackupToolService> logger)
{
    public async Task<IList<McpClientTool>> GetAIToolsAsync(CancellationToken cancellationToken = default)
    {
        var clientTransport = new SseClientTransport(new()
        {
            Endpoint = new Uri(serverSettings.Value.Endpoint)
        });

        await using var mcpClient =
            await McpClientFactory.CreateAsync(clientTransport, cancellationToken: cancellationToken);

        var tools = await mcpClient.ListToolsAsync(cancellationToken: cancellationToken);

        logger.LogInformation("Found {toolsCount} tools", tools.Count);

        return tools;
    }
}