# Stage 1: Build the test project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app

COPY PlaywrightDemos/PlayDemo.csproj PlaywrightDemos/
RUN dotnet restore PlaywrightDemos/PlayDemo.csproj

COPY PlaywrightDemos/ PlaywrightDemos/
RUN dotnet build PlaywrightDemos/PlayDemo.csproj --no-restore -c Release

# Stage 2: Runtime image with Playwright browsers installed
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS runtime

# Install PowerShell (required to run playwright.ps1) and Playwright system dependencies
RUN apt-get update && apt-get install -y \
    wget \
    ca-certificates \
    && wget -q https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb \
    && apt-get update && apt-get install -y powershell \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=build /app /app

# Install Playwright browsers and their system dependencies using the generated script
RUN pwsh PlaywrightDemos/bin/Release/net10.0/playwright.ps1 install --with-deps chromium firefox webkit

ENTRYPOINT ["dotnet", "test", "PlaywrightDemos/PlayDemo.csproj", \
            "--no-build", "-c", "Release", "--verbosity", "normal", \
            "--filter", "TestCategory=CICD", \
            "--logger", "trx;LogFileName=test-results.trx", \
            "--logger", "nunit;LogFileName=test-results.xml", \
            "--logger", "console;verbosity=detailed"]
