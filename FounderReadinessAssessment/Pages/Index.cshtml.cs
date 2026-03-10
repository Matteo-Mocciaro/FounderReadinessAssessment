using Microsoft.AspNetCore.Mvc.RazorPages;
using FounderReadinessAssessment.Services;

namespace FounderReadinessAssessment.Pages;

public sealed class IndexModel : PageModel
{
    private readonly LocalizationService _localizationService;

    public IndexModel(LocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    public string CurrentLanguage { get; set; } = "en";

    public void OnGet()
    {
        Request.Cookies.TryGetValue(LocalizationService.LanguageCookieName, out var cookieLanguage);
        CurrentLanguage = _localizationService.NormalizeLanguage(cookieLanguage);
    }

    public string T(string english, string italian)
    {
        return _localizationService.Translate(CurrentLanguage, english, italian);
    }
}
