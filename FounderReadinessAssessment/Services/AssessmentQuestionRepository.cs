using FounderReadinessAssessment.Models;

namespace FounderReadinessAssessment.Services;

public sealed class AssessmentQuestionRepository
{
    private readonly LocalizationService _localizationService;

    public AssessmentQuestionRepository(LocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    public IReadOnlyList<AssessmentDimension> GetDimensions(string language)
    {
        return
        [
            new() { Id = "problem-clarity", Name = _localizationService.Translate(language, "Problem Clarity", "Chiarezza del problema"), Weight = 0.30m },
            new() { Id = "validation-evidence", Name = _localizationService.Translate(language, "Validation Evidence", "Evidenze di validazione"), Weight = 0.30m },
            new() { Id = "execution-readiness", Name = _localizationService.Translate(language, "Execution Readiness", "Capacità di esecuzione"), Weight = 0.20m },
            new() { Id = "commercial-readiness", Name = _localizationService.Translate(language, "Commercial Readiness", "Maturità commerciale"), Weight = 0.20m }
        ];
    }

    public IReadOnlyList<AssessmentQuestion> GetQuestions(string language)
    {
        return
        [
            new()
            {
                Number = 1,
                Id = "Q1",
                DimensionId = "problem-clarity",
                Text = _localizationService.Translate(language, "How clearly can you define the problem you are solving?", "Quanto è chiaro il problema che state risolvendo?"),
                Options =
                [
                    new() { Id = "Q1_A", Text = _localizationService.Translate(language, "Very clear and easy to explain in one sentence", "Molto chiaro e spiegabile in una frase"), Score = 4 },
                    new() { Id = "Q1_B", Text = _localizationService.Translate(language, "Clear overall, but some points still need refinement", "Chiaro nel complesso, ma con alcuni punti da affinare"), Score = 3 },
                    new() { Id = "Q1_C", Text = _localizationService.Translate(language, "Partially defined and not always consistent", "Definito solo in parte e non sempre coerente"), Score = 1 },
                    new() { Id = "Q1_D", Text = _localizationService.Translate(language, "Still vague or difficult to explain", "Ancora vago o difficile da spiegare"), Score = 0 }
                ]
            },
            new()
            {
                Number = 2,
                Id = "Q2",
                DimensionId = "problem-clarity",
                Text = _localizationService.Translate(language, "How specific is your target customer segment?", "Quanto è specifico il segmento cliente target?"),
                Options =
                [
                    new() { Id = "Q2_A", Text = _localizationService.Translate(language, "Clearly defined niche with specific characteristics", "Niche chiaramente definita con caratteristiche precise"), Score = 4 },
                    new() { Id = "Q2_B", Text = _localizationService.Translate(language, "Broad segment, but still reasonably identifiable", "Segmento ampio ma comunque identificabile"), Score = 3 },
                    new() { Id = "Q2_C", Text = _localizationService.Translate(language, "Generic audience with limited segmentation", "Pubblico generico con segmentazione limitata"), Score = 1 },
                    new() { Id = "Q2_D", Text = _localizationService.Translate(language, "Target customer is still unclear", "Il cliente target non è ancora chiaro"), Score = 0 }
                ]
            },
            new()
            {
                Number = 3,
                Id = "Q3",
                DimensionId = "problem-clarity",
                Text = _localizationService.Translate(language, "How urgent is this problem for your target customers?", "Quanto è urgente questo problema per i clienti target?"),
                Options =
                [
                    new() { Id = "Q3_A", Text = _localizationService.Translate(language, "High urgency and strong pain point", "Alta urgenza e pain point rilevante"), Score = 4 },
                    new() { Id = "Q3_B", Text = _localizationService.Translate(language, "Relevant issue, but urgency varies by case", "Tema rilevante, ma con urgenza variabile"), Score = 3 },
                    new() { Id = "Q3_C", Text = _localizationService.Translate(language, "Mostly a minor inconvenience", "Prevalentemente un disagio minore"), Score = 1 },
                    new() { Id = "Q3_D", Text = _localizationService.Translate(language, "Not yet clear whether it is a real problem", "Non è ancora chiaro se sia un problema reale"), Score = 0 }
                ]
            },
            new()
            {
                Number = 4,
                Id = "Q4",
                DimensionId = "validation-evidence",
                Text = _localizationService.Translate(language, "How many customer interviews or discovery calls have you completed?", "Quante interviste cliente o call di discovery avete completato?"),
                Options =
                [
                    new() { Id = "Q4_A", Text = _localizationService.Translate(language, "More than 15", "Più di 15"), Score = 4 },
                    new() { Id = "Q4_B", Text = _localizationService.Translate(language, "Between 5 and 15", "Tra 5 e 15"), Score = 3 },
                    new() { Id = "Q4_C", Text = _localizationService.Translate(language, "Fewer than 5", "Meno di 5"), Score = 1 },
                    new() { Id = "Q4_D", Text = _localizationService.Translate(language, "None so far", "Nessuna finora"), Score = 0 }
                ]
            },
            new()
            {
                Number = 5,
                Id = "Q5",
                DimensionId = "validation-evidence",
                Text = _localizationService.Translate(language, "Which validation activities have you run so far?", "Quali attività di validazione avete svolto finora?"),
                Options =
                [
                    new() { Id = "Q5_A", Text = _localizationService.Translate(language, "Multiple structured tests (landing page, waitlist, survey, smoke test, MVP feedback)", "Test strutturati multipli (landing page, waitlist, survey, smoke test, feedback MVP)"), Score = 4 },
                    new() { Id = "Q5_B", Text = _localizationService.Translate(language, "At least one structured validation test", "Almeno un test di validazione strutturato"), Score = 3 },
                    new() { Id = "Q5_C", Text = _localizationService.Translate(language, "Only informal conversations or assumptions", "Solo conversazioni informali o ipotesi"), Score = 1 },
                    new() { Id = "Q5_D", Text = _localizationService.Translate(language, "No validation activity yet", "Nessuna attività di validazione"), Score = 0 }
                ]
            },
            new()
            {
                Number = 6,
                Id = "Q6",
                DimensionId = "validation-evidence",
                Text = _localizationService.Translate(language, "What concrete market evidence do you have today?", "Quali evidenze concrete di mercato avete oggi?"),
                Options =
                [
                    new() { Id = "Q6_A", Text = _localizationService.Translate(language, "Early customers, active users, pre-orders, or strong sign-up volume", "Primi clienti, utenti attivi, preordini o volume di iscrizioni significativo"), Score = 4 },
                    new() { Id = "Q6_B", Text = _localizationService.Translate(language, "Strong feedback and clear signals of real demand", "Feedback forti e segnali chiari di domanda reale"), Score = 3 },
                    new() { Id = "Q6_C", Text = _localizationService.Translate(language, "Some positive feedback, but weak behavioral proof", "Alcuni feedback positivi, ma prove comportamentali deboli"), Score = 1 },
                    new() { Id = "Q6_D", Text = _localizationService.Translate(language, "No concrete evidence yet", "Nessuna evidenza concreta al momento"), Score = 0 }
                ]
            },
            new()
            {
                Number = 7,
                Id = "Q7",
                DimensionId = "execution-readiness",
                Text = _localizationService.Translate(language, "What is your current team setup?", "Qual è la configurazione attuale del team?"),
                Options =
                [
                    new() { Id = "Q7_A", Text = _localizationService.Translate(language, "Complementary team with the core skills to execute", "Team complementare con competenze chiave per eseguire"), Score = 4 },
                    new() { Id = "Q7_B", Text = _localizationService.Translate(language, "Two people or partial coverage of key roles", "Due persone o copertura parziale dei ruoli chiave"), Score = 3 },
                    new() { Id = "Q7_C", Text = _localizationService.Translate(language, "Solo founder with some execution capability", "Founder solo con una discreta capacità esecutiva"), Score = 2 },
                    new() { Id = "Q7_D", Text = _localizationService.Translate(language, "Solo founder with major execution gaps", "Founder solo con gap esecutivi rilevanti"), Score = 0 }
                ]
            },
            new()
            {
                Number = 8,
                Id = "Q8",
                DimensionId = "execution-readiness",
                Text = _localizationService.Translate(language, "How much time is currently dedicated to the startup?", "Quanto tempo è attualmente dedicato alla startup?"),
                Options =
                [
                    new() { Id = "Q8_A", Text = _localizationService.Translate(language, "Full-time and consistent", "Full-time e in modo continuativo"), Score = 4 },
                    new() { Id = "Q8_B", Text = _localizationService.Translate(language, "Strong and consistent part-time commitment", "Impegno part-time forte e costante"), Score = 3 },
                    new() { Id = "Q8_C", Text = _localizationService.Translate(language, "Occasional part-time effort", "Impegno part-time occasionale"), Score = 1 },
                    new() { Id = "Q8_D", Text = _localizationService.Translate(language, "Very limited time availability", "Disponibilità di tempo molto limitata"), Score = 0 }
                ]
            },
            new()
            {
                Number = 9,
                Id = "Q9",
                DimensionId = "execution-readiness",
                Text = _localizationService.Translate(language, "What have you already built?", "Cosa avete già costruito?"),
                Options =
                [
                    new() { Id = "Q9_A", Text = _localizationService.Translate(language, "Working MVP or live prototype", "MVP funzionante o prototipo live"), Score = 4 },
                    new() { Id = "Q9_B", Text = _localizationService.Translate(language, "Clickable prototype or detailed mockups", "Prototipo cliccabile o mockup dettagliati"), Score = 3 },
                    new() { Id = "Q9_C", Text = _localizationService.Translate(language, "Concept with only a rough structure", "Concept con struttura ancora preliminare"), Score = 1 },
                    new() { Id = "Q9_D", Text = _localizationService.Translate(language, "Nothing built yet", "Nessun output sviluppato finora"), Score = 0 }
                ]
            },
            new()
            {
                Number = 10,
                Id = "Q10",
                DimensionId = "commercial-readiness",
                Text = _localizationService.Translate(language, "How clear is your value proposition?", "Quanto è chiara la vostra value proposition?"),
                Options =
                [
                    new() { Id = "Q10_A", Text = _localizationService.Translate(language, "Very clear, specific, and easy to communicate", "Molto chiara, specifica e facile da comunicare"), Score = 4 },
                    new() { Id = "Q10_B", Text = _localizationService.Translate(language, "Mostly clear, with some refinement needed", "Abbastanza chiara, con alcuni affinamenti necessari"), Score = 3 },
                    new() { Id = "Q10_C", Text = _localizationService.Translate(language, "Still generic or too broad", "Ancora generica o troppo ampia"), Score = 1 },
                    new() { Id = "Q10_D", Text = _localizationService.Translate(language, "Not yet clearly articulated", "Non ancora articolata in modo chiaro"), Score = 0 }
                ]
            },
            new()
            {
                Number = 11,
                Id = "Q11",
                DimensionId = "commercial-readiness",
                Text = _localizationService.Translate(language, "How clear is your main objective for the next 90 days?", "Quanto è chiaro l'obiettivo principale dei prossimi 90 giorni?"),
                Options =
                [
                    new() { Id = "Q11_A", Text = _localizationService.Translate(language, "A focused and measurable milestone is already defined", "Un traguardo focalizzato e misurabile è già definito"), Score = 4 },
                    new() { Id = "Q11_B", Text = _localizationService.Translate(language, "A clear direction exists, but without full measurability", "Esiste una direzione chiara, ma non ancora pienamente misurabile"), Score = 3 },
                    new() { Id = "Q11_C", Text = _localizationService.Translate(language, "A generic intention exists, but no concrete milestone", "Esiste un'intenzione generale, ma senza un traguardo concreto"), Score = 1 },
                    new() { Id = "Q11_D", Text = _localizationService.Translate(language, "No concrete short-term objective", "Nessun obiettivo concreto di breve periodo"), Score = 0 }
                ]
            },
            new()
            {
                Number = 12,
                Id = "Q12",
                DimensionId = "commercial-readiness",
                Text = _localizationService.Translate(language, "How clearly do you understand the support you need right now?", "Quanto è chiaro il tipo di supporto di cui avete bisogno ora?"),
                Options =
                [
                    new() { Id = "Q12_A", Text = _localizationService.Translate(language, "Very clear, and aligned with your current stage", "Molto chiaro e coerente con lo stadio attuale"), Score = 4 },
                    new() { Id = "Q12_B", Text = _localizationService.Translate(language, "Mostly clear, with some remaining uncertainty", "Abbastanza chiaro, con alcune incertezze residue"), Score = 3 },
                    new() { Id = "Q12_C", Text = _localizationService.Translate(language, "Partially understood", "Compreso solo in parte"), Score = 1 },
                    new() { Id = "Q12_D", Text = _localizationService.Translate(language, "Not clear at all", "Per nulla chiaro"), Score = 0 }
                ]
            }
        ];
    }
}
