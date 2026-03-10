using FounderReadinessAssessment.Models;
using FounderReadinessAssessment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FounderReadinessAssessment.Pages;

public sealed class ResultModel : PageModel
{
    private readonly AssessmentEngine _assessmentEngine;
    private readonly LocalizationService _localizationService;

    public ResultModel(AssessmentEngine assessmentEngine, LocalizationService localizationService)
    {
        _assessmentEngine = assessmentEngine;
        _localizationService = localizationService;
    }

    public AssessmentResult? Result { get; set; }
    public string CurrentLanguage { get; set; } = "en";

    public IActionResult OnGet()
    {
        var serialized = TempData.Peek("AssessmentResultJson") as string;
        var parsed = _assessmentEngine.DeserializeResult(serialized);
        if (parsed is null)
        {
            return RedirectToPage("/Assessment");
        }

        Result = parsed;
        CurrentLanguage = _localizationService.NormalizeLanguage(parsed.Language);
        return Page();
    }

    public string GetProfileLabel()
    {
        return Result is null
            ? _localizationService.Translate(CurrentLanguage, "Unknown", "Non definito")
            : _localizationService.ProfileLabel(Result.Profile, CurrentLanguage);
    }

    public string GetProfileDescription()
    {
        if (Result is null)
        {
            return T("Profile data is not available.", "I dati del profilo non sono disponibili.");
        }

        return Result.Profile switch
        {
            LeadProfile.Explorer => T(
                "Early-stage profile focused on sharpening problem clarity and customer understanding before scaling build efforts.",
                "Profilo iniziale, focalizzato sul chiarire meglio il problema e comprendere meglio il cliente prima di aumentare lo sviluppo."),
            LeadProfile.Validator => T(
                "Emerging profile with promising direction, now requiring stronger validation evidence and repeatable learning loops.",
                "Profilo in evoluzione con direzione promettente, che richiede ora evidenze di validazione più forti e un apprendimento più strutturato."),
            LeadProfile.Builder => T(
                "Execution-oriented profile ready for structured support on priorities, sequencing, and traction milestones.",
                "Profilo orientato all'esecuzione, pronto per un supporto strutturato su priorità, organizzazione delle attività e crescita a breve termine."),
            LeadProfile.Momentum => T(
                "High-readiness profile with strong maturity signals, suitable for priority follow-up and acceleration pathways.",
                "Profilo avanzato con segnali di maturità robusti, adatto a un approfondimento prioritario e a percorsi di accelerazione."),
            _ => T("Profile interpretation unavailable.", "Interpretazione profilo non disponibile.")
        };
    }

    public string GetFlagLabel(string flagName)
    {
        return _localizationService.FlagLabel(flagName, CurrentLanguage);
    }

    public string T(string english, string italian)
    {
        return _localizationService.Translate(CurrentLanguage, english, italian);
    }
}
