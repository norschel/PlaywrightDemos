# PlaywrightDemos

This repository contains Playwright C# demo code showcasing end-to-end testing capabilities for software developer conference talks.

## Overview

This project demonstrates various Playwright testing scenarios using C# with both MSTest and NUnit frameworks. The demos are organized by conference presentations and showcase real-world testing patterns.

## Technologies

- **.NET 9.0** - Target framework
- **Playwright** - Browser automation framework
- **MSTest** - Primary testing framework
- **NUnit** - Alternative testing framework
- **Azure Playwright Testing Service** - Cloud-based browser testing

## Key Features & Scenarios

### Basic Testing

- **Simple smoke tests** - Open website, interact with elements, verify results
- **Data-driven tests** - Parameterized tests across multiple browsers
- **Element location** - Various locator strategies and best practices

### File Operations

- **Download testing** - Download files and verify existence
- **File handling** - Work with downloaded content

### Network & API

- **Network interception** - Intercept and modify network requests
- **API mocking** - Mock backend responses
- **Route blocking** - Block specific resources (images, scripts, etc.)

### Device & Browser Testing

- **Mobile device emulation** - Test with iPhone, iPad, and other devices
- **Cross-browser testing** - Chrome, Firefox, WebKit
- **Responsive design testing** - Viewport and device-specific scenarios

### Debugging & Diagnostics

- **Video recording** - Capture test execution videos
- **Screenshots** - Take screenshots on failure or demand
- **Tracing** - Playwright trace files for detailed debugging
- **Test artifacts** - Organized output in TestResults folder

### Cloud Testing

- **Azure Playwright Testing Service** - Cloud-based parallel test execution
- **Scalable infrastructure** - Run tests across multiple browsers simultaneously

### AI-Assisted Automation

This repository includes GitHub Copilot prompts that demonstrate AI-powered browser automation workflows (by @harrybin):

- **[generateImage.prompt.md](.github/prompts/generateImage.prompt.md)** - Automated image generation workflow using Playwright MCP tooling
  - Scrapes conference website to extract theme and styling information
  - Navigates to Google Gemini AI
  - Constructs AI prompts based on gathered context
  - Generates custom conference presentation images
  - Automates file downloads

This showcases how Playwright can be integrated with AI tools for complex multi-step automation tasks without writing traditional test code.

## Project Structure

```text
PlaywrightDemos/
├── AzurePlaywrightTests_*.cs    # Azure cloud-based tests (NUnit)
├── PlaywrightE2ETests_*.cs      # Conference-specific demos (MSTest)
├── CloudBrowserPageTest.cs      # Azure Playwright base class
├── PlawrightServiceSetup.cs     # Azure service configuration
├── LocalServer.cs               # Local test server utilities
├── testdaten/                   # Test data files
└── TestResults/                 # Test execution artifacts
```

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Playwright browsers (automatically installed on first run)

### Installation

1. Clone the repository:

```bash
git clone https://github.com/norschel/PlaywrightDemos.git
cd PlaywrightDemos
```

2. Restore dependencies:

```bash
dotnet restore
```

3. Install Playwright browsers:

```bash
dotnet build
pwsh PlaywrightDemos/bin/Debug/net9.0/playwright.ps1 install
```

### Running Tests

Run all tests:

```bash
dotnet test
```

Run specific test class:

```bash
dotnet test --filter "FullyQualifiedName~PlaywrightE2ETests_IT_Tage_2025"
```

Run tests by category:

```bash
dotnet test --filter "TestCategory=NUnit"
```

## Conference Presentations

This repository includes demo code from the following conferences:

- **IT-Tage 2025** - Latest demos with Azure Playwright Testing
- **MDD (Madgeburger Developer Days) 2024/2025**
- **DDC (.NET Developer Conference Cologne) 2023/2024**
- **BASTA! 2023/2024**
- **KET (Karlsruher Entwicklertage) 2023**
- **WDC (Web Developer Conference) 2023**

## Key Dependencies

- `Microsoft.Playwright.MSTest` (1.57.0)
- `Microsoft.Playwright.NUnit` (1.57.0)
- `Azure.Developer.Playwright.NUnit` (1.0.0)
- `MSTest` (4.0.2)
- `Azure.Identity` (1.17.1)

## Test Artifacts

Test execution produces:

- **Videos** - In `videos/` folder
- **Traces** - In `trace/` folder (open with Playwright Trace Viewer)
- **Screenshots** - Embedded in trace files
- **Test results** - XML/TRX format in `TestResults/`

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

Nico Orschel

---

*This repository is continuously updated with new demos and examples from conference presentations.*
