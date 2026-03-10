# FounderReadinessAssessment

Startup Fit Check is an ASP.NET Core 8 Razor Pages prototype for structured founder lead qualification in an incubation/startup support context.

## Project Overview

The app provides:

- A professional landing page introducing the assessment workflow
- A 12-question required assessment across 4 weighted dimensions
- A rule-driven result report with profile assignment, flags, recommendations, and score visualization
- Stateless result transfer from `Assessment` to `Result` using serialized `TempData`
- Bilingual interface (`EN`/`IT`) with language switch in the top navigation

No database, authentication, external API, JS framework, or CSS framework is used.

## Folder Structure

```text
FounderReadinessAssessment/
  Models/
    AssessmentDimension.cs
    AssessmentQuestion.cs
    AssessmentOption.cs
    AssessmentSubmission.cs
    AssessmentScores.cs
    AssessmentResult.cs
    LeadProfile.cs
    AssessmentFlag.cs
  Pages/
    Index.cshtml
    Index.cshtml.cs
    Assessment.cshtml
    Assessment.cshtml.cs
    Result.cshtml
    Result.cshtml.cs
    Shared/
      _Layout.cshtml
  Services/
    AssessmentEngine.cs
    AssessmentQuestionRepository.cs
  wwwroot/
    css/
      site.css
  Program.cs
  README.md
```

## Scoring Model

- Dimensions and weights:
  - Problem Clarity: `30%`
  - Validation Evidence: `30%`
  - Execution Readiness: `20%`
  - Commercial Readiness: `20%`
- Each dimension contains 3 questions
- Each question has max score `4`, so max raw per dimension is `12`
- Dimension score formula:
  - `round((rawScore / 12.0) * 100)`
- Overall weighted formula:
  - `round(problemClarity * 0.30 + validationEvidence * 0.30 + executionReadiness * 0.20 + commercialReadiness * 0.20)`

All scores are integers in range `0-100`.

## Profile Logic

Profiles are assigned in this order:

1. `Momentum` if overall >= 75 and Problem Clarity >= 70 and Validation Evidence >= 60
2. `Builder` if overall >= 60 and Problem Clarity >= 50 and Validation Evidence >= 40
3. `Validator` if overall >= 40 and Problem Clarity >= 40
4. Else `Explorer`

Rule enforcement:

- If Problem Clarity < 40, profile cannot be Builder or Momentum
- If that condition occurs, downgrade to Validator (if overall >= 40) else Explorer

Flags and decision rules:

- `Needs Validation First` when Validation Evidence < 30
- `Overbuilding Risk` when Execution Readiness > 70 and Validation Evidence < 30
- `High Potential` when Problem Clarity > 70 and Validation Evidence > 60
- `Nurture Lead` when Overall < 40
- If Validation Evidence < 30, sales recommendation will not be high-priority direct follow-up

## How to Run

From the project directory:

```bash
dotnet restore
dotnet build
dotnet run
```

Open the local URL printed in the terminal (for example `https://localhost:7xxx`).

Use the `EN / IT` selector in the top-right header to switch the UI language.

## Customization Points

- Question content/options: `Services/AssessmentQuestionRepository.cs`
- Thresholds, flags, profile logic, recommendations: `Services/AssessmentEngine.cs`
- UI styling and layout: `wwwroot/css/site.css` and `Pages/Shared/_Layout.cshtml`
- Page structure/content: `Pages/Index.cshtml`, `Pages/Assessment.cshtml`, `Pages/Result.cshtml`
