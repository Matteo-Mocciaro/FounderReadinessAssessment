using System.Text.Json;
using FounderReadinessAssessment.Models;

namespace FounderReadinessAssessment.Services;

public sealed class AssessmentEngine
{
    private const int MaxDimensionRawScore = 12;
    private readonly AssessmentQuestionRepository _questionRepository;
    private readonly LocalizationService _localizationService;

    public AssessmentEngine(AssessmentQuestionRepository questionRepository, LocalizationService localizationService)
    {
        _questionRepository = questionRepository;
        _localizationService = localizationService;
    }

    public IReadOnlyList<AssessmentDimension> GetDimensions(string language) => _questionRepository.GetDimensions(language);

    public IReadOnlyList<AssessmentQuestion> GetQuestions(string language) => _questionRepository.GetQuestions(language);

    public AssessmentSubmission CreateEmptySubmission(string language)
    {
        var submission = new AssessmentSubmission();
        foreach (var question in GetQuestions(language))
        {
            submission.Answers.Add(new QuestionAnswerInput
            {
                QuestionId = question.Id,
                SelectedOptionId = null
            });
        }

        return submission;
    }

    public AssessmentResult Evaluate(AssessmentSubmission submission, string language)
    {
        var normalizedLanguage = _localizationService.NormalizeLanguage(language);
        var questions = GetQuestions(normalizedLanguage);
        var selectedOptionByQuestion = BuildAnswerLookup(submission, questions);

        var problemRaw = SumRawForDimension("problem-clarity", questions, selectedOptionByQuestion);
        var validationRaw = SumRawForDimension("validation-evidence", questions, selectedOptionByQuestion);
        var executionRaw = SumRawForDimension("execution-readiness", questions, selectedOptionByQuestion);
        var commercialRaw = SumRawForDimension("commercial-readiness", questions, selectedOptionByQuestion);

        var problemClarity = NormalizeDimensionScore(problemRaw);
        var validationEvidence = NormalizeDimensionScore(validationRaw);
        var executionReadiness = NormalizeDimensionScore(executionRaw);
        var commercialReadiness = NormalizeDimensionScore(commercialRaw);

        var overall = (int)Math.Round(
            (problemClarity * 0.30m) +
            (validationEvidence * 0.30m) +
            (executionReadiness * 0.20m) +
            (commercialReadiness * 0.20m),
            MidpointRounding.AwayFromZero);

        var scores = new AssessmentScores
        {
            ProblemClarity = problemClarity,
            ValidationEvidence = validationEvidence,
            ExecutionReadiness = executionReadiness,
            CommercialReadiness = commercialReadiness,
            Overall = overall
        };

        var profile = AssignProfile(scores);
        profile = EnforceProblemClarityGate(scores, profile);

        var flags = BuildFlags(scores);
        var summary = BuildExecutiveSummary(profile, flags, normalizedLanguage);
        var strengths = BuildStrengths(scores, profile, normalizedLanguage);
        var gapsRisks = BuildGapsRisks(scores, profile, normalizedLanguage);
        var nextBestAction = BuildNextBestAction(scores, profile, normalizedLanguage);
        var salesRecommendation = BuildSalesRecommendation(scores, profile, flags, normalizedLanguage);

        return new AssessmentResult
        {
            Scores = scores,
            Profile = profile,
            Flags = flags,
            ExecutiveSummary = summary,
            Strengths = strengths,
            GapsRisks = gapsRisks,
            NextBestAction = nextBestAction,
            SalesRecommendation = salesRecommendation,
            Language = normalizedLanguage
        };
    }

    public string SerializeResult(AssessmentResult result)
    {
        return JsonSerializer.Serialize(result);
    }

    public AssessmentResult? DeserializeResult(string? serialized)
    {
        if (string.IsNullOrWhiteSpace(serialized))
        {
            return null;
        }

        return JsonSerializer.Deserialize<AssessmentResult>(serialized);
    }

    private static Dictionary<string, AssessmentOption> BuildAnswerLookup(
        AssessmentSubmission submission,
        IReadOnlyList<AssessmentQuestion> questions)
    {
        var questionById = questions.ToDictionary(q => q.Id, q => q);
        var selected = new Dictionary<string, AssessmentOption>(StringComparer.OrdinalIgnoreCase);

        foreach (var answer in submission.Answers)
        {
            if (!questionById.TryGetValue(answer.QuestionId, out var question))
            {
                continue;
            }

            var option = question.Options.FirstOrDefault(o => o.Id == answer.SelectedOptionId);
            if (option is null)
            {
                continue;
            }

            selected[question.Id] = option;
        }

        return selected;
    }

    private static int SumRawForDimension(
        string dimensionId,
        IReadOnlyList<AssessmentQuestion> questions,
        Dictionary<string, AssessmentOption> selectedOptionByQuestion)
    {
        return questions
            .Where(q => q.DimensionId == dimensionId)
            .Sum(question => selectedOptionByQuestion.TryGetValue(question.Id, out var option) ? option.Score : 0);
    }

    private static int NormalizeDimensionScore(int rawScore)
    {
        return (int)Math.Round((rawScore / (double)MaxDimensionRawScore) * 100, MidpointRounding.AwayFromZero);
    }

    private static LeadProfile AssignProfile(AssessmentScores scores)
    {
        if (scores.Overall >= 75 &&
            scores.ProblemClarity >= 70 &&
            scores.ValidationEvidence >= 60)
        {
            return LeadProfile.Momentum;
        }

        if (scores.Overall >= 60 &&
            scores.ProblemClarity >= 50 &&
            scores.ValidationEvidence >= 40)
        {
            return LeadProfile.Builder;
        }

        if (scores.Overall >= 40 &&
            scores.ProblemClarity >= 40)
        {
            return LeadProfile.Validator;
        }

        return LeadProfile.Explorer;
    }

    private static LeadProfile EnforceProblemClarityGate(AssessmentScores scores, LeadProfile profile)
    {
        if (scores.ProblemClarity < 40 &&
            (profile == LeadProfile.Builder || profile == LeadProfile.Momentum))
        {
            return scores.Overall >= 40 ? LeadProfile.Validator : LeadProfile.Explorer;
        }

        return profile;
    }

    private static IReadOnlyList<AssessmentFlag> BuildFlags(AssessmentScores scores)
    {
        var flags = new List<AssessmentFlag>();

        if (scores.ValidationEvidence < 30)
        {
            flags.Add(new AssessmentFlag { Name = "Needs Validation First", Severity = "warning" });
        }

        if (scores.ExecutionReadiness > 70 && scores.ValidationEvidence < 30)
        {
            flags.Add(new AssessmentFlag { Name = "Overbuilding Risk", Severity = "risk" });
        }

        if (scores.ProblemClarity > 70 && scores.ValidationEvidence > 60)
        {
            flags.Add(new AssessmentFlag { Name = "High Potential", Severity = "positive" });
        }

        if (scores.Overall < 40)
        {
            flags.Add(new AssessmentFlag { Name = "Nurture Lead", Severity = "neutral" });
        }

        return flags;
    }

    private string BuildExecutiveSummary(LeadProfile profile, IReadOnlyList<AssessmentFlag> flags, string language)
    {
        var baseSummary = profile switch
        {
            LeadProfile.Explorer => _localizationService.Translate(language,
                "The startup is still in an early discovery phase. Before acceleration, the team should improve problem clarity and customer focus.",
                "La startup è ancora in una fase iniziale. Prima di accelerare, conviene chiarire meglio il problema e mettere meglio a fuoco il cliente target."),
            LeadProfile.Validator => _localizationService.Translate(language,
                "The startup has direction and early learning signals, but it still needs stronger validation evidence to reduce execution risk.",
                "La startup ha una direzione e alcuni segnali incoraggianti, ma servono evidenze di validazione più solide per ridurre il rischio operativo."),
            LeadProfile.Builder => _localizationService.Translate(language,
                "The startup shows credible execution readiness and can benefit from structured support on priorities, sequencing, and traction milestones.",
                "La startup mostra una buona capacità di esecuzione e può trarre beneficio da un supporto strutturato su priorità, organizzazione delle attività e obiettivi di crescita."),
            LeadProfile.Momentum => _localizationService.Translate(language,
                "The startup demonstrates strong maturity and high-quality validation. It is a high-fit candidate for priority follow-up and acceleration support.",
                "La startup dimostra una maturità elevata e una validazione solida. È un candidato ad alta priorità per un percorso di accelerazione."),
            _ => _localizationService.Translate(language,
                "The assessment highlights a mixed readiness profile that requires phased support.",
                "La valutazione evidenzia un profilo di maturità misto che richiede un supporto per fasi.")
        };

        if (flags.Any(f => f.Name == "High Potential"))
        {
            return _localizationService.Translate(language,
                $"{baseSummary} Current indicators suggest above-average potential if focus and execution discipline are maintained.",
                $"{baseSummary} Gli indicatori attuali suggeriscono un potenziale sopra la media, se il team mantiene focus e continuità operativa.");
        }

        if (flags.Any(f => f.Name == "Needs Validation First"))
        {
            return _localizationService.Translate(language,
                $"{baseSummary} The immediate priority is to increase validation depth before additional build investment.",
                $"{baseSummary} La priorità immediata è aumentare la qualità della validazione prima di investire altro nello sviluppo.");
        }

        return baseSummary;
    }

    private IReadOnlyList<string> BuildStrengths(AssessmentScores scores, LeadProfile profile, string language)
    {
        var strengths = new List<string>();

        strengths.Add(scores.ProblemClarity >= 60
            ? _localizationService.Translate(language,
                "Problem definition and customer focus are sufficiently clear to support practical decisions.",
                "La definizione del problema e il focus sul cliente sono abbastanza chiari da sostenere decisioni operative.")
            : _localizationService.Translate(language,
                "The team has started to structure the problem space and can consolidate this quickly.",
                "Il team ha iniziato a strutturare bene il problema e può consolidare questo lavoro rapidamente."));

        strengths.Add(scores.ExecutionReadiness >= 60
            ? _localizationService.Translate(language,
                "Execution capacity is visible in team setup, commitment, and delivery progress.",
                "La capacità di esecuzione è evidente nella composizione del team, nell'impegno e nell'avanzamento del lavoro.")
            : _localizationService.Translate(language,
                "Execution intent is present and can improve with sharper operating priorities.",
                "La volontà di eseguire c'è, e può migliorare con priorità operative più chiare."));

        strengths.Add(profile switch
        {
            LeadProfile.Momentum => _localizationService.Translate(language,
                "Commercial positioning and traction signals support a high-confidence support path.",
                "Posizionamento commerciale e segnali di mercato sostengono un percorso di supporto ad alta confidenza."),
            LeadProfile.Builder => _localizationService.Translate(language,
                "The startup has a solid base to accelerate through milestone-driven execution.",
                "La startup ha una base solida per accelerare con obiettivi chiari e misurabili."),
            LeadProfile.Validator => _localizationService.Translate(language,
                "There is enough learning momentum to convert insights into stronger validation evidence.",
                "C'è abbastanza apprendimento per trasformare gli insight in evidenze di validazione più robuste."),
            _ => _localizationService.Translate(language,
                "The concept direction is sufficiently defined to begin disciplined validation work.",
                "La direzione del concept è sufficientemente definita per avviare un lavoro di validazione strutturato.")
        });

        return strengths;
    }

    private IReadOnlyList<string> BuildGapsRisks(AssessmentScores scores, LeadProfile profile, string language)
    {
        var gaps = new List<string>();

        gaps.Add(scores.ValidationEvidence < 40
            ? _localizationService.Translate(language,
                "Validation evidence is not yet strong enough to support confident scaling decisions.",
                "Le evidenze di validazione non sono ancora sufficienti per sostenere decisioni di scala con fiducia.")
            : _localizationService.Translate(language,
                "Validation quality should be reinforced through consistent learning loops and measurable tests.",
                "La qualità della validazione va rafforzata con cicli di apprendimento costanti e test misurabili."));

        gaps.Add(scores.CommercialReadiness < 50
            ? _localizationService.Translate(language,
                "Commercial readiness remains under-structured, especially on short-term objectives and support path clarity.",
                "La preparazione commerciale è ancora poco strutturata, soprattutto su obiettivi di breve periodo e chiarezza del percorso di supporto.")
            : _localizationService.Translate(language,
                "Commercial readiness can improve through tighter 90-day KPI discipline.",
                "La preparazione commerciale può migliorare con obiettivi e indicatori più rigorosi sui prossimi 90 giorni."));

        gaps.Add(profile switch
        {
            LeadProfile.Explorer => _localizationService.Translate(language,
                "Primary risk: building too early before validating urgency and segment fit.",
                "Rischio principale: costruire troppo presto prima di validare urgenza e segment fit."),
            LeadProfile.Validator => _localizationService.Translate(language,
                "Primary risk: overestimating interest signals without behavioral traction data.",
                "Rischio principale: sovrastimare i segnali di interesse senza dati comportamentali concreti."),
            LeadProfile.Builder => _localizationService.Translate(language,
                "Primary risk: execution fragmentation if priorities and sequencing are not explicit.",
                "Rischio principale: frammentazione esecutiva se priorità e sequenza delle attività non sono esplicite."),
            LeadProfile.Momentum => _localizationService.Translate(language,
                "Primary risk: scaling faster than insight quality and operating cadence.",
                "Rischio principale: scalare più velocemente della qualità degli insight e della cadenza operativa."),
            _ => _localizationService.Translate(language,
                "The risk profile requires phased de-risking.",
                "Il profilo di rischio richiede un de-risking progressivo.")
        });

        return gaps;
    }

    private string BuildNextBestAction(AssessmentScores scores, LeadProfile profile, string language)
    {
        return profile switch
        {
            LeadProfile.Explorer =>
                _localizationService.Translate(language,
                    "Run a 3-week discovery sprint: refine the problem statement, define one priority segment, and complete at least 10 structured customer interviews.",
                    "Pianificate uno sprint di scoperta di 3 settimane: chiarite meglio il problema, definite un segmento prioritario e completate almeno 10 interviste cliente strutturate."),
            LeadProfile.Validator =>
                _localizationService.Translate(language,
                    "Execute a focused validation plan with two measurable market tests and explicit go/no-go criteria before expanding product scope.",
                    "Eseguite un piano di validazione focalizzato con due test di mercato misurabili e criteri decisionali chiari prima di ampliare lo sviluppo prodotto."),
            LeadProfile.Builder =>
                scores.ValidationEvidence < 50
                    ? _localizationService.Translate(language,
                        "Combine execution support with recurring validation checkpoints so delivery remains evidence-led.",
                        "Affiancate il supporto operativo a checkpoint regolari di validazione, in modo che il lavoro resti guidato da evidenze.")
                    : _localizationService.Translate(language,
                        "Launch an acceleration plan centered on milestone sequencing, owner accountability, and traction outcomes.",
                        "Avviate un piano di accelerazione centrato su una chiara sequenza delle priorità, responsabilità definite e risultati di crescita."),
            LeadProfile.Momentum =>
                _localizationService.Translate(language,
                    "Move to high-touch follow-up with a structured growth plan, clear KPIs, and short operating review cycles.",
                    "Passate a un approfondimento ad alta priorità con un piano di crescita strutturato, indicatori chiari e verifiche frequenti."),
            _ =>
                _localizationService.Translate(language,
                    "Define phased milestones and reassess after the first validation cycle.",
                    "Definite milestone per fasi e rivalutate dopo il primo ciclo di validazione.")
        };
    }

    private string BuildSalesRecommendation(
        AssessmentScores scores,
        LeadProfile profile,
        IReadOnlyList<AssessmentFlag> flags,
        string language)
    {
        if (scores.ValidationEvidence < 30)
        {
            return _localizationService.Translate(language,
                "Validation-first recommendation: provide structured guidance, set evidence milestones, and postpone high-priority direct follow-up until validation improves.",
                "Raccomandazione prioritaria: lavorare prima sulla validazione, definire traguardi di evidenza e rinviare il contatto commerciale diretto ad alta priorità finché la validazione non migliora.");
        }

        if (profile == LeadProfile.Momentum)
        {
            return _localizationService.Translate(language,
                "High-priority direct follow-up: schedule a strategic qualification call within 5 business days and evaluate fit for advanced support programs.",
                "Approfondimento ad alta priorità: pianificare entro 5 giorni lavorativi una call strategica e valutare l'accesso a programmi di supporto avanzati.");
        }

        if (profile == LeadProfile.Builder)
        {
            return _localizationService.Translate(language,
                "Priority follow-up: propose an advisory session focused on execution bottlenecks, sequencing choices, and KPI discipline.",
                "Approfondimento prioritario: proporre una sessione di confronto focalizzata su colli di bottiglia operativi, sequenza delle attività e monitoraggio degli indicatori.");
        }

        if (flags.Any(f => f.Name == "Nurture Lead"))
        {
            return _localizationService.Translate(language,
                "Nurture recommendation: share structured startup readiness resources and plan a light check-in after foundational progress.",
                "Raccomandazione di accompagnamento: condividere risorse pratiche di preparazione startup e pianificare un check-in leggero dopo i progressi di base.");
        }

        return _localizationService.Translate(language,
            "Standard follow-up: share targeted recommendations and reassess after the next validation milestone.",
            "Follow-up standard: condividere raccomandazioni mirate e rivalutare dopo il prossimo traguardo di validazione.");
    }
}
