# Stage 1: Build the test project using the .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app

COPY PlaywrightDemos/PlayDemo.csproj PlaywrightDemos/
RUN dotnet restore PlaywrightDemos/PlayDemo.csproj

COPY PlaywrightDemos/ PlaywrightDemos/
RUN dotnet build PlaywrightDemos/PlayDemo.csproj --no-restore -c Release

# Stage 2: Runtime image using the official Playwright Docker image
# (browsers and their system dependencies are pre-installed)
FROM mcr.microsoft.com/playwright/dotnet:v1.59.0-noble AS runtime

# Install .NET 10 SDK (the Playwright image ships with .NET 8; the project targets net10.0)
RUN apt-get update && apt-get install -y wget ca-certificates \
    && wget -q https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb \
    && apt-get update && apt-get install -y dotnet-sdk-10.0 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=build /app /app

ENTRYPOINT ["dotnet", "test", "PlaywrightDemos/PlayDemo.csproj", \
            "--no-build", "-c", "Release", "--verbosity", "normal", \
            "--filter", "TestCategory=CICD", \
            "--logger", "trx;LogFileName=test-results.trx", \
            "--logger", "nunit;LogFileName=test-results.xml", \
            "--logger", "console;verbosity=detailed"]
