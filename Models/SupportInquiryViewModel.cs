using System.ComponentModel.DataAnnotations;

namespace ASP_Fund_Project.Models;

public class SupportInquiryViewModel
{
    [Required]
    [StringLength(80, MinimumLength = 2)]
    [Display(Name = "Your name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [StringLength(1500, MinimumLength = 15)]
    [Display(Name = "Message")]
    public string Message { get; set; } = string.Empty;
}
