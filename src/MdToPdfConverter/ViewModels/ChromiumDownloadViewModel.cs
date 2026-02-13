using MdToPdfConverter.Services;
using ReactiveUI;

namespace MdToPdfConverter.ViewModels;

public class ChromiumDownloadViewModel : ViewModelBase
{
    private readonly ChromiumDownloadService _downloadService;

    private int _progressPercent;
    private string _statusText = "Preparing download...";
    private bool _isDownloading;

    public ChromiumDownloadViewModel(ChromiumDownloadService downloadService)
    {
        _downloadService = downloadService;
    }

    public int ProgressPercent
    {
        get => _progressPercent;
        set => this.RaiseAndSetIfChanged(ref _progressPercent, value);
    }

    public string StatusText
    {
        get => _statusText;
        set => this.RaiseAndSetIfChanged(ref _statusText, value);
    }

    public bool IsDownloading
    {
        get => _isDownloading;
        set => this.RaiseAndSetIfChanged(ref _isDownloading, value);
    }

    public async Task DownloadAsync()
    {
        IsDownloading = true;
        StatusText = "Downloading browser engine...";
        ProgressPercent = 0;

        try
        {
            await _downloadService.EnsureChromiumAsync();
            ProgressPercent = 100;
            StatusText = "Download complete.";
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
        finally
        {
            IsDownloading = false;
        }
    }
}
