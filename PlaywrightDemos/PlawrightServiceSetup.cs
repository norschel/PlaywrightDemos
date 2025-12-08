using Azure.Developer.Playwright.NUnit;
using NUnit.Framework;
using Azure.Identity;

namespace PlaywrightDemos;

[SetUpFixture]
public class PlaywrightServiceNUnitSetup : PlaywrightServiceBrowserNUnit
{
    public PlaywrightServiceNUnitSetup() : base(
        credential: new DefaultAzureCredential()
    )
    {}
}