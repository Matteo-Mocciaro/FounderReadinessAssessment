using FounderReadinessAssessment.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FounderReadinessAssessment.Pages;

public sealed class ErrorModel : PageModel
{
    private readonly LocalizationService _localizationService;

    public ErrorModel(LocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    public string CurrentLanguage { get; private set; } = "en";

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
