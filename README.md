# PlaywrightDemos

This repository contains Playwright C# demo code used in conference talks to showcase end-to-end testing patterns with local and cloud browser execution.

## Overview

The project combines MSTest- and NUnit-based examples, organized by conference and scenario. Demonstrated techniques include:

- UI smoke tests with locator strategies (roles, CSS selectors, text, test IDs)
- Data-driven tests across multiple browsers (Chromium, Firefox, WebKit, Edge, Chrome)
- File download testing
- Device emulation (e.g. iPhone 13 landscape)
- Video recording during test execution
- Playwright Tracing (screenshots, snapshots, source links) saved as `.zip` artifacts
- Network mocking and request interception with `page.RouteAsync`
- Cloud browser execution via **Azure Playwright Testing Service**

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
│   ├── PlayDemo.csproj                         # Project file; single test project for MSTest and NUnit
│   │
│   │   # Local MSTest-based conference demos
│   ├── PlaywrightE2ETests_Basta2023.cs
│   ├── PlaywrightE2ETests_Basta2024.cs
│   ├── PlaywrightE2ETests_BastaSpring2026.cs   # Smoke, video, download, data-driven, device, route demos
│   ├── PlaywrightE2ETests_DDC2023.cs
│   ├── PlaywrightE2ETests_DDC2024.cs
│   ├── PlaywrightE2ETests_IT_Tage_2025.cs      # Smoke, data-driven, download, device demos
│   ├── PlaywrightE2ETests_KET2023.cs
│   ├── PlaywrightE2ETests_MDD2024.cs
│   ├── PlaywrightE2ETests_MDD2025.cs
│   ├── PlaywrightE2ETests_MDD2026.cs           # Smoke, data-driven, download, device, video, route demos
│   ├── PlaywrightE2ETests_WDC2023.cs
│   │
│   │   # NUnit-based Azure Playwright Testing Service demos
│   ├── AzurePlaywrightTests_BastaSpring2026.cs   # Step-by-step: basic connection
│   ├── AzurePlaywrightTests_BastaSpring2026_1.cs # + maximized window args
│   ├── AzurePlaywrightTests_BastaSpring2026_2.cs # + cookie-banner handling
│   ├── AzurePlaywrightTests_BastaSpring2026_3.cs # + final polished version
│   │
│   │   # Infrastructure
│   ├── CloudBrowserPageTest.cs    # Base class: connects PageTest to Azure Playwright Service
│   ├── PlawrightServiceSetup.cs   # NUnit SetUpFixture for Azure Playwright Service
│   ├── LocalServer.cs             # Helper to start/stop a local npm dev server
│   ├── TestAssemblyInfos.cs       # MSTest assembly-level configuration
│   ├── Usings.cs                  # Global using directives
│   │
│   └── testdaten/                 # Test data files (images used by network-mocking tests)
│
├── .github/workflows/
│   ├── dotnet.yml   # CI pipeline: build, test (CICD tag only), upload artifacts
│   └── docker.yml   # Docker-based workflow
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

Run only CI-tagged tests (the subset executed in the CI pipeline):

```bash
dotnet test ./PlaywrightDemos/PlayDemo.csproj --no-build --filter "TestCategory=CICD"
```

Run a specific test class:

```bash
dotnet test ./PlaywrightDemos/PlayDemo.csproj --filter "FullyQualifiedName~PlaywrightE2ETests_IT_Tage_2025"
```

Run with verbose output and all log formats:

```bash
dotnet test ./PlaywrightDemos/PlayDemo.csproj --no-build --verbosity normal \
  --logger "trx;LogFileName=test-results.trx" \
  --logger "nunit;LogFileName=test-results.xml" \
  --logger "html;logfilename=test-results.html"
```

## Conference Demos Included

| Conference | File(s) | Highlighted Scenarios |
|---|---|---|
| BASTA! 2023 | `PlaywrightE2ETests_Basta2023.cs` | Smoke test |
| BASTA! 2024 | `PlaywrightE2ETests_Basta2024.cs` | Smoke test |
| BASTA! Spring 2026 | `PlaywrightE2ETests_BastaSpring2026.cs` | Smoke, video recording, file download, data-driven (3 browsers), device emulation, network route blocking |
| BASTA! Spring 2026 (Azure) | `AzurePlaywrightTests_BastaSpring2026*.cs` | Cloud browser execution via Azure Playwright Testing Service (4 variants showing progressive refinement) |
| DDC 2023 | `PlaywrightE2ETests_DDC2023.cs` | Smoke test |
| DDC 2024 | `PlaywrightE2ETests_DDC2024.cs` | Smoke test |
| IT-Tage 2025 | `PlaywrightE2ETests_IT_Tage_2025.cs` | Smoke, data-driven (5 browsers), file download, device emulation, tracing |
| KET 2023 | `PlaywrightE2ETests_KET2023.cs` | Smoke test |
| MDD 2024 | `PlaywrightE2ETests_MDD2024.cs` | Smoke, data-driven (5 browsers), file download, device emulation, video recording |
| MDD 2025 | `PlaywrightE2ETests_MDD2025.cs` | Smoke, data-driven (5 browsers), file download, device emulation, video recording |
| MDD 2026 | `PlaywrightE2ETests_MDD2026.cs` | Smoke, data-driven (5 browsers), file download, device emulation, video recording, advanced network request interception |
| WDC 2023 | `PlaywrightE2ETests_WDC2023.cs` | Smoke test |

## Test Categories

Tests are annotated with `[TestCategory]` attributes to allow selective execution:

| Category | Description |
|---|---|
| `CICD` | Tests safe to run in CI (headless, no manual interaction). Only these run in the GitHub Actions pipeline. |
| `MSTest` | Tests using the MSTest framework. |
| `NUnit` | Tests using the NUnit framework (Azure Playwright Testing Service demos). |

Most `PlaywrightE2ETests_*` classes are disabled (`[TestClass]` commented out) to avoid running all demos simultaneously. Enable individual classes as needed.

## Azure Playwright Testing Service

The `AzurePlaywrightTests_BastaSpring2026*.cs` files demonstrate how to run Playwright tests against remote cloud browsers managed by Azure.

### How It Works

1. `PlawrightServiceSetup.cs` registers the NUnit `SetUpFixture` that initialises the service connection.
2. `CloudBrowserPageTest.cs` overrides `ConnectOptionsAsync()` on the NUnit `PageTest` base class to redirect browser connections to the Azure endpoint.
3. Individual test classes extend `CloudBrowserPageTest` — no other changes to the test code are required.

### Prerequisites for Azure Tests

- An active [Azure Playwright Testing](https://azure.microsoft.com/en-us/products/playwright-testing) workspace.
- The `PLAYWRIGHT_SERVICE_URL` environment variable set to your workspace endpoint.
- Azure credentials available (the code uses `DefaultAzureCredential`, which supports Managed Identity, environment variables, Azure CLI, and more).

```bash
export PLAYWRIGHT_SERVICE_URL="wss://<your-workspace>.api.playwright.microsoft.com/accounts/<account-id>/browsers"
dotnet test ./PlaywrightDemos/PlayDemo.csproj --filter "TestCategory=NUnit"
```

## Key Test Patterns

| Pattern | API Used | Example File |
|---|---|---|
| Simple smoke test | `page.GotoAsync`, `page.TitleAsync`, `Assert` | All `PlaywrightE2ETests_*.cs` |
| Role-based locators | `page.GetByRole` | `PlaywrightE2ETests_BastaSpring2026.cs` |
| CSS / attribute locators | `page.Locator("[href*=playwright]")` | All |
| Data-driven (multi-browser) | `[DataRow]` / `[TestCase]` | `PlaywrightE2ETests_MDD2026.cs` |
| File download | `page.RunAndWaitForDownloadAsync` | `PlaywrightE2ETests_BastaSpring2026.cs` |
| Device emulation | `playwright.Devices["iPhone 13 landscape"]` | `PlaywrightE2ETests_MDD2025.cs` |
| Video recording | `BrowserNewContextOptions.RecordVideoDir` | `PlaywrightE2ETests_MDD2024.cs` |
| Tracing | `context.Tracing.StartAsync` / `StopAsync` | `PlaywrightE2ETests_IT_Tage_2025.cs` |
| Network blocking | `page.RouteAsync` + `route.FulfillAsync(404)` | `PlaywrightE2ETests_BastaSpring2026.cs` |
| Response interception | `route.FetchAsync` + `route.FulfillAsync` with modified body | `PlaywrightE2ETests_MDD2026.cs` |
| Cloud browser | `CloudBrowserPageTest` + `PlaywrightServiceBrowserClient` | `AzurePlaywrightTests_BastaSpring2026.cs` |

## CI/CD Pipeline

The GitHub Actions workflow (`.github/workflows/dotnet.yml`) runs on every push and pull request to `main`:

1. **Setup** — restores and builds the project using .NET 10.
2. **Browser install** — installs Playwright browsers and WebKit system dependencies.
3. **Test** — runs only `TestCategory=CICD` tests; produces TRX, NUnit XML, and HTML reports.
4. **Test Reporter** — publishes results inline in the pull request via `dorny/test-reporter`.
5. **Artifacts uploaded:**
   - `test-results` — TRX and NUnit XML files (90-day retention).
   - `playwright-diagnostics` — all screenshots (`.png`/`.jpg`), videos (`.webm`), and traces (`.zip`) collected during the run (90-day retention).
   - `ErrorArtifact` — full workspace snapshot uploaded only on failure.

## Key Dependencies

| Package | Version |
|---|---|
| `Microsoft.Playwright.MSTest` | `1.58.0` |
| `Microsoft.Playwright.NUnit` | `1.58.0` |
| `Azure.Developer.Playwright.NUnit` | `1.0.0` |
| `MSTest` / `MSTest.TestAdapter` / `MSTest.TestFramework` | `4.1.0` |
| `NUnit3TestAdapter` | `6.1.0` |
| `Azure.Identity` | `1.18.0` |
| `NunitXml.TestLogger` | `8.0.0` |
| `coverlet.collector` | `8.0.0` |

## AI-Assisted Automation Prompt

The repository also includes a Copilot prompt:

- [`generateImage.prompt.md`](.github/prompts/generateImage.prompt.md)

## License

MIT License — see [LICENSE](LICENSE).

## Author

Nico Orschel
