using Microsoft.Playwright.NUnit;
using Azure.Developer.Playwright;
using Azure.Identity;
using Microsoft.Playwright;

namespace PlaywrightDemos;

public class CloudBrowserPageTest : PageTest
{
    public override async Task<(string, BrowserTypeConnectOptions?)?> ConnectOptionsAsync()
    {
        PlaywrightServiceBrowserClient client = new PlaywrightServiceBrowserClient(credential: new DefaultAzureCredential());
        var connectOptions = await client.GetConnectOptionsAsync<BrowserTypeConnectOptions>();
        return (connectOptions.WsEndpoint, connectOptions.Options);
    }
}