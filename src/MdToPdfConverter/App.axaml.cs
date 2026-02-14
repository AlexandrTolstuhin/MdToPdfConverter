using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Threading;
using MdToPdfConverter.Services;
using MdToPdfConverter.ViewModels;
using MdToPdfConverter.Views;

namespace MdToPdfConverter;

public partial class App : Application
{
    public static SingleInstanceService? SingleInstance { get; set; }

    private SettingsService _settingsService = null!;
    private PdfConverterService _pdfConverterService = null!;
    private ContextMenuService _contextMenuService = null!;
    private AutoStartService _autoStartService = null!;
    private ChromiumDownloadService _chromiumDownloadService = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            _settingsService = new SettingsService();
            _pdfConverterService = new PdfConverterService();
            _contextMenuService = new ContextMenuService();
            _autoStartService = new AutoStartService();
            _chromiumDownloadService = new ChromiumDownloadService();

            SetupTrayIcon();

            if (SingleInstance is not null)
            {
                SingleInstance.FileReceived += filePath =>
                    Dispatcher.UIThread.Post(() => _ = ConvertFileAsync(filePath));
            }

            var args = desktop.Args;
            if (args is { Length: > 0 } && File.Exists(args[0])
                && args[0].EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            {
                _ = ConvertFileAsync(args[0]);
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void SetupTrayIcon()
    {
        var settingsItem = new NativeMenuItem("Settings");
        settingsItem.Click += (_, _) => ShowSettings();

        var exitItem = new NativeMenuItem("Exit");
        exitItem.Click += (_, _) => ExitApp();

        var menu = new NativeMenu();
        menu.Items.Add(settingsItem);
        menu.Items.Add(new NativeMenuItemSeparator());
        menu.Items.Add(exitItem);

        var trayIcon = new TrayIcon
        {
            ToolTipText = "MD to PDF Converter",
            Menu = menu,
            Icon = new WindowIcon(AssetLoader.Open(new Uri("avares://MdToPdfConverter/Assets/app-icon.ico")))
        };

        var icons = new TrayIcons { trayIcon };
        TrayIcon.SetIcons(this, icons);
    }

    private void ShowSettings()
    {
        var vm = new SettingsWindowViewModel(_settingsService, _contextMenuService, _autoStartService);
        var window = new SettingsWindow { DataContext = vm };
        vm.CloseRequested += () => window.Close();
        window.Show();
    }

    private void ExitApp()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.Shutdown();
    }

    private async Task ConvertFileAsync(string mdFilePath)
    {
        var settings = await _settingsService.LoadAsync();

        if (!_chromiumDownloadService.IsChromiumInstalled())
        {
            var vm = new ChromiumDownloadViewModel(_chromiumDownloadService);
            var window = new ChromiumDownloadWindow { DataContext = vm };
            window.Show();
            await vm.DownloadAsync();
            window.Close();
        }

        var result = await _pdfConverterService.ConvertAsync(mdFilePath, settings.PdfFontSize, settings.PdfMarginMm, settings.PaperFormat);

        if (result.Success)
            ShowNotification("Conversion Complete", $"PDF saved: {Path.GetFileName(result.OutputPath)}");
        else
            ShowNotification("Conversion Failed", result.ErrorMessage ?? "Unknown error");
    }

    private void ShowNotification(string title, string message)
    {
        var window = new Window
        {
            Title = title,
            Width = 350,
            Height = 100,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.Manual,
            SystemDecorations = SystemDecorations.BorderOnly,
            ShowInTaskbar = false,
            Topmost = true,
            Content = new TextBlock
            {
                Text = $"{title}\n{message}",
                Margin = new Avalonia.Thickness(16),
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            }
        };

        var screen = window.Screens.Primary;
        if (screen is not null)
        {
            var workArea = screen.WorkingArea;
            window.Position = new Avalonia.PixelPoint(
                (int)(workArea.Right / screen.Scaling - 360),
                (int)(workArea.Bottom / screen.Scaling - 110));
        }

        window.Show();

        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            window.Close();
        };
        timer.Start();
    }
}
