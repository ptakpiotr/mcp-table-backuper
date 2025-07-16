using System.ComponentModel.DataAnnotations;

namespace ChatSimpleClient.Models;

public class AzureOpenAiSettings
{
    [Required] public string Url { get; set; } = null!;

    [Required] public string Model { get; set; } = null!;
    
    [Required] public string DeploymentName { get; set; } = null!;
    
    [Required] public string ApiKey { get; set; } = null!;
}