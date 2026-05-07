# PlaywrightDemos

This repository contains Playwright C# demo code used in conference talks to showcase end-to-end testing patterns with local and cloud browser execution.

## Overview

The project combines MSTest- and NUnit-based examples, organized by conference and scenario (UI smoke tests, downloads, device emulation, network mocking, tracing/video diagnostics, and Azure Playwright Testing Service usage).

## Technologies

- **.NET 10.0** (target framework)
- **Microsoft Playwright for .NET**
- **MSTest 4.x**
- **NUnit + NUnit3TestAdapter**
- **Azure Playwright Testing Service**

## Project Structure

```text
PlaywrightDemos/
├── PlaywrightDemos/
│   ├── PlayDemo.csproj
│   ├── PlaywrightE2ETests_*.cs
│   ├── AzurePlaywrightTests_*.cs
│   ├── CloudBrowserPageTest.cs
│   ├── PlawrightServiceSetup.cs
│   ├── LocalServer.cs
│   ├── testdaten/
│   └── AzurePlaywrightServices/
├── .github/workflows/
│   ├── dotnet.yml
│   └── docker.yml
└── README.md
```

## Getting Started

### Prerequisites

- .NET 10.0 SDK
- PowerShell (`pwsh`) for Playwright browser installation

### Installation

```bash
git clone https://github.com/norschel/PlaywrightDemos.git
cd PlaywrightDemos
dotnet restore ./PlaywrightDemos/PlayDemo.csproj
dotnet build ./PlaywrightDemos/PlayDemo.csproj --no-restore
pwsh ./PlaywrightDemos/bin/Debug/net10.0/playwright.ps1 install
```

### Running Tests

Run all tests:

```bash
dotnet test ./PlaywrightDemos/PlayDemo.csproj --no-build
```

Run CI-tagged tests:

```bash
dotnet test ./PlaywrightDemos/PlayDemo.csproj --no-build --filter "TestCategory=CICD"
```

Run a specific test class:

```bash
dotnet test ./PlaywrightDemos/PlayDemo.csproj --filter "FullyQualifiedName~PlaywrightE2ETests_IT_Tage_2025"
```

## Conference Demos Included

- BASTA! 2023, 2024, Spring 2026
- MDD 2024, 2025, 2026
- DDC 2023, 2024
- IT-Tage 2025
- KET 2023
- WDC 2023

## Key Dependencies

- `Microsoft.Playwright.MSTest` (`1.58.0`)
- `Microsoft.Playwright.NUnit` (`1.58.0`)
- `Azure.Developer.Playwright.NUnit` (`1.0.0`)
- `MSTest` / `MSTest.TestAdapter` / `MSTest.TestFramework` (`4.1.0`)
- `Azure.Identity` (`1.18.0`)

## AI-Assisted Automation Prompt

The repository also includes a Copilot prompt:

- [`generateImage.prompt.md`](.github/prompts/generateImage.prompt.md)

## License

MIT License — see [LICENSE](LICENSE).

## Author

Nico Orschel
