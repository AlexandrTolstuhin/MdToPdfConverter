using PuppeteerSharp;

namespace MdToPdfConverter.Services;

public class ChromiumDownloadService
{
    public bool IsChromiumInstalled()
    {
        var fetcher = new BrowserFetcher();
        return fetcher.GetInstalledBrowsers().Any();
    }

    public async Task EnsureChromiumAsync()
    {
        var fetcher = new BrowserFetcher();
        if (fetcher.GetInstalledBrowsers().Any())
            return;

        await fetcher.DownloadAsync();
    }
}
