namespace FounderReadinessAssessment.Models;

public sealed class AssessmentResult
{
    public required string Language { get; init; }
    public required AssessmentScores Scores { get; init; }
    public required LeadProfile Profile { get; init; }
    public required IReadOnlyList<AssessmentFlag> Flags { get; init; }
    public required string ExecutiveSummary { get; init; }
    public required IReadOnlyList<string> Strengths { get; init; }
    public required IReadOnlyList<string> GapsRisks { get; init; }
    public required string NextBestAction { get; init; }
    public required string SalesRecommendation { get; init; }
}
