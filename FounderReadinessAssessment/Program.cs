using FounderReadinessAssessment.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<AssessmentQuestionRepository>();
builder.Services.AddSingleton<AssessmentEngine>();
builder.Services.AddSingleton<LocalizationService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.Use(async (context, next) =>
{
    if (context.Request.Query.TryGetValue("lang", out var langFromQuery))
    {
        var selectedLanguage = string.Equals(langFromQuery.ToString(), "it", StringComparison.OrdinalIgnoreCase) ? "it" : "en";
        context.Response.Cookies.Append(
            LocalizationService.LanguageCookieName,
            selectedLanguage,
            new CookieOptions
            {
                HttpOnly = false,
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            });
    }

    await next();
});

app.MapRazorPages();

app.Run();
