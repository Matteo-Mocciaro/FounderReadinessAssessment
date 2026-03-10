namespace FounderReadinessAssessment.Models;

public sealed class AssessmentQuestion
{
    public required int Number { get; init; }
    public required string Id { get; init; }
    public required string DimensionId { get; init; }
    public required string Text { get; init; }
    public required IReadOnlyList<AssessmentOption> Options { get; init; }
}
