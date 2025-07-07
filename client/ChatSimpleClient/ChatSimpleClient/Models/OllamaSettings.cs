using System.ComponentModel.DataAnnotations;

namespace ChatSimpleClient.Models;

public class OllamaSettings
{
    [Required] public string Url { get; set; } = null!;

    [Required] public string Model { get; set; } = null!;
}