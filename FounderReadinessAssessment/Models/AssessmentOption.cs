namespace FounderReadinessAssessment.Models;

public sealed class AssessmentOption
{
    public required string Id { get; init; }
    public required string Text { get; init; }
    public int Score { get; init; }
}
