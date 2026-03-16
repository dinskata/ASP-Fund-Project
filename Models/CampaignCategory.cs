using System.ComponentModel.DataAnnotations;

namespace ASP_Fund_Project.Models;

public enum CampaignCategory
{
    Education = 1,
    Health,
    Community,
    Emergency,
    Arts,
    [Display(Name = "Medical")]
    Medical,
    [Display(Name = "Animal care")]
    AnimalCare,
    [Display(Name = "Environment")]
    Environment,
    [Display(Name = "Sports")]
    Sports,
    [Display(Name = "Other category")]
    Other
}
