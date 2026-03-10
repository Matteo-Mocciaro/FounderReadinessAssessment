using FounderReadinessAssessment.Models;

namespace FounderReadinessAssessment.Services;

public sealed class LocalizationService
{
    public const string LanguageCookieName = "startupfitcheck_lang";

    public string NormalizeLanguage(string? language)
    {
        return string.Equals(language, "it", StringComparison.OrdinalIgnoreCase) ? "it" : "en";
    }

    public string Translate(string language, string english, string italian)
    {
        return NormalizeLanguage(language) == "it" ? italian : english;
    }

    public string ProfileLabel(LeadProfile profile, string language)
    {
        return profile switch
        {
            LeadProfile.Explorer => Translate(language, "Explorer", "Explorer"),
            LeadProfile.Validator => Translate(language, "Validator", "Validator"),
            LeadProfile.Builder => Translate(language, "Builder", "Builder"),
            LeadProfile.Momentum => Translate(language, "Momentum", "Momentum"),
            _ => Translate(language, "Unknown", "Non definito")
        };
    }

    public string FlagLabel(string flagName, string language)
    {
        return flagName switch
        {
            "High Potential" => Translate(language, "High Potential", "Alto potenziale"),
            "Overbuilding Risk" => Translate(language, "Overbuilding Risk", "Rischio di overbuilding"),
            "Needs Validation First" => Translate(language, "Needs Validation First", "Validazione prioritaria"),
            "Nurture Lead" => Translate(language, "Nurture Lead", "Lead da seguire nel tempo"),
            _ => flagName
        };
    }
}
