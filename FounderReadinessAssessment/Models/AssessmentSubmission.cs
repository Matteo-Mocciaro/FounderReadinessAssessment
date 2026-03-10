using System.ComponentModel.DataAnnotations;

namespace FounderReadinessAssessment.Models;

public sealed class AssessmentSubmission
{
    [Required(ErrorMessage = "All questions must be completed before submission.")]
    [MinLength(12, ErrorMessage = "All 12 questions must be completed before submission.")]
    public List<QuestionAnswerInput> Answers { get; init; } = new();
}

public sealed class QuestionAnswerInput
{
    [Required]
    public string QuestionId { get; init; } = string.Empty;

    [Required(ErrorMessage = "Please select one option.")]
    public string? SelectedOptionId { get; set; }
}
