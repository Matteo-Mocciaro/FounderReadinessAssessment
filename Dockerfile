FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY FounderReadinessAssessment.sln ./
COPY FounderReadinessAssessment/FounderReadinessAssessment.csproj FounderReadinessAssessment/
RUN dotnet restore FounderReadinessAssessment/FounderReadinessAssessment.csproj

COPY . .
RUN dotnet publish FounderReadinessAssessment/FounderReadinessAssessment.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Render sets PORT at runtime
EXPOSE 10000
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT} dotnet FounderReadinessAssessment.dll"]
