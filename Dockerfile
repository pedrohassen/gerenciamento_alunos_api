FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY LearningLoop.GerenciamentoAlunosApp/LearningLoop.GerenciamentoAlunosApp.csproj LearningLoop.GerenciamentoAlunosApp/
RUN dotnet restore LearningLoop.GerenciamentoAlunosApp/LearningLoop.GerenciamentoAlunosApp.csproj

COPY LearningLoop.GerenciamentoAlunosApp/ LearningLoop.GerenciamentoAlunosApp/
RUN dotnet publish LearningLoop.GerenciamentoAlunosApp/LearningLoop.GerenciamentoAlunosApp.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

# Render injeta a porta via $PORT; localmente cai em 8080 se a variável não existir.
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} exec dotnet LearningLoop.GerenciamentoAlunosApp.dll"]
