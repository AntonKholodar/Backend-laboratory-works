using System.ComponentModel.DataAnnotations;

namespace ChatApp.API.Models.Requests;

public class SendMessageRequest
{
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;
} 