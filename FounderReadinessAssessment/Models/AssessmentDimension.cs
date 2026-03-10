namespace FounderReadinessAssessment.Models;

public sealed class AssessmentDimension
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public decimal Weight { get; init; }
}
