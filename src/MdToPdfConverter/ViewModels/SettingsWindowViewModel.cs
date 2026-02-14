using System.Reactive;
using MdToPdfConverter.Models;
using MdToPdfConverter.Services;
using ReactiveUI;

namespace MdToPdfConverter.ViewModels;

public class SettingsWindowViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;
    private readonly IContextMenuService _contextMenuService;
    private readonly IAutoStartService _autoStartService;

    private int _fontSize;
    private int _marginMm;
    private string _selectedPaperFormat;
    private bool _isContextMenuRegistered;
    private bool _isAutoStartEnabled;

    public SettingsWindowViewModel(
        ISettingsService settingsService,
        IContextMenuService contextMenuService,
        IAutoStartService autoStartService)
    {
        _settingsService = settingsService;
        _contextMenuService = contextMenuService;
        _autoStartService = autoStartService;

        var settings = settingsService.Current;
        _fontSize = settings.PdfFontSize;
        _marginMm = settings.PdfMarginMm;
        _selectedPaperFormat = settings.PaperFormat;
        _isContextMenuRegistered = contextMenuService.IsRegistered();
        _isAutoStartEnabled = autoStartService.IsEnabled();

        SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync);
        CancelCommand = ReactiveCommand.Create(() => CloseRequested?.Invoke());
    }

    public int FontSize
    {
        get => _fontSize;
        set => this.RaiseAndSetIfChanged(ref _fontSize, value);
    }

    public int MarginMm
    {
        get => _marginMm;
        set => this.RaiseAndSetIfChanged(ref _marginMm, value);
    }

    public string SelectedPaperFormat
    {
        get => _selectedPaperFormat;
        set => this.RaiseAndSetIfChanged(ref _selectedPaperFormat, value);
    }

    public List<string> PaperFormats { get; } = new()
    {
        "A4",
        "A3",
        "A5",
        "Letter",
        "Legal"
    };

    public bool IsContextMenuRegistered
    {
        get => _isContextMenuRegistered;
        set => this.RaiseAndSetIfChanged(ref _isContextMenuRegistered, value);
    }

    public bool IsAutoStartEnabled
    {
        get => _isAutoStartEnabled;
        set => this.RaiseAndSetIfChanged(ref _isAutoStartEnabled, value);
    }

    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    public event Action? CloseRequested;

    private async Task SaveAsync()
    {
        var exePath = Environment.ProcessPath ?? "";

        if (IsContextMenuRegistered)
            _contextMenuService.Register(exePath);
        else
            _contextMenuService.Unregister();

        if (IsAutoStartEnabled)
            _autoStartService.Enable(exePath);
        else
            _autoStartService.Disable();

        var settings = new AppSettings
        {
            PdfFontSize = FontSize,
            PdfMarginMm = MarginMm,
            PaperFormat = SelectedPaperFormat,
            IsContextMenuRegistered = IsContextMenuRegistered,
            IsAutoStartEnabled = IsAutoStartEnabled
        };

        await _settingsService.SaveAsync(settings);
        CloseRequested?.Invoke();
    }
}
