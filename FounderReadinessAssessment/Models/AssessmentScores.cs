namespace FounderReadinessAssessment.Models;

public sealed class AssessmentScores
{
    public int ProblemClarity { get; init; }
    public int ValidationEvidence { get; init; }
    public int ExecutionReadiness { get; init; }
    public int CommercialReadiness { get; init; }
    public int Overall { get; init; }
}
