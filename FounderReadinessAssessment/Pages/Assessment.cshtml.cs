using FounderReadinessAssessment.Models;
using FounderReadinessAssessment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FounderReadinessAssessment.Pages;

public sealed class AssessmentModel : PageModel
{
    private readonly AssessmentEngine _assessmentEngine;
    private readonly LocalizationService _localizationService;

    public AssessmentModel(AssessmentEngine assessmentEngine, LocalizationService localizationService)
    {
        _assessmentEngine = assessmentEngine;
        _localizationService = localizationService;
    }

    [BindProperty]
    public AssessmentSubmission Submission { get; set; } = new();

    public IReadOnlyList<AssessmentSectionViewModel> Sections { get; set; } = Array.Empty<AssessmentSectionViewModel>();

    public int AnsweredCount { get; set; }
    public string CurrentLanguage { get; set; } = "en";

    [TempData]
    public string? AssessmentResultJson { get; set; }

    public void OnGet()
    {
        CurrentLanguage = GetCurrentLanguage();
        Submission = _assessmentEngine.CreateEmptySubmission(CurrentLanguage);
        BuildSections();
    }

    public IActionResult OnPost()
    {
        CurrentLanguage = GetCurrentLanguage();
        ValidateSubmission();

        if (!ModelState.IsValid)
        {
            BuildSections();
            return Page();
        }

        var result = _assessmentEngine.Evaluate(Submission, CurrentLanguage);
        AssessmentResultJson = _assessmentEngine.SerializeResult(result);
        return RedirectToPage("/Result");
    }

    private void ValidateSubmission()
    {
        var questions = _assessmentEngine.GetQuestions(CurrentLanguage);
        if (Submission.Answers.Count != questions.Count)
        {
            ModelState.AddModelError(string.Empty, T("Please answer all questions before continuing.", "Completa tutte le domande prima di continuare."));
            BuildSections();
            return;
        }

        for (var i = 0; i < questions.Count; i++)
        {
            var question = questions[i];
            var answer = Submission.Answers[i];
            if (!string.Equals(answer.QuestionId, question.Id, StringComparison.Ordinal))
            {
                ModelState.AddModelError(string.Empty, T("One or more submitted answers are invalid. Please submit again.", "Una o più risposte inviate non sono valide. Invia nuovamente il questionario."));
                continue;
            }

            var isValidOption = question.Options.Any(option => option.Id == answer.SelectedOptionId);
            if (!isValidOption)
            {
                ModelState.AddModelError($"Submission.Answers[{i}].SelectedOptionId", T("Select one option to continue.", "Seleziona un'opzione per continuare."));
            }
        }
    }

    private void BuildSections()
    {
        CurrentLanguage = GetCurrentLanguage();
        var dimensions = _assessmentEngine.GetDimensions(CurrentLanguage);
        var questions = _assessmentEngine.GetQuestions(CurrentLanguage);

        Sections = dimensions
            .Select(d => new AssessmentSectionViewModel
            {
                Dimension = d,
                Questions = questions.Where(q => q.DimensionId == d.Id).ToList()
            })
            .ToList();

        AnsweredCount = Submission.Answers.Count(a => !string.IsNullOrWhiteSpace(a.SelectedOptionId));
    }

    private string GetCurrentLanguage()
    {
        Request.Cookies.TryGetValue(LocalizationService.LanguageCookieName, out var cookieLanguage);
        return _localizationService.NormalizeLanguage(cookieLanguage);
    }

    public string T(string english, string italian)
    {
        return _localizationService.Translate(CurrentLanguage, english, italian);
    }
}

public sealed class AssessmentSectionViewModel
{
    public required AssessmentDimension Dimension { get; init; }
    public required IReadOnlyList<AssessmentQuestion> Questions { get; init; }
}
