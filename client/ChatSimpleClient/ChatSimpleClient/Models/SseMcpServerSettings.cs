using System.ComponentModel.DataAnnotations;

namespace ChatSimpleClient.Models;

public class SseMcpServerSettings
{
    [Required] public string Endpoint { get; set; } = null!;
}