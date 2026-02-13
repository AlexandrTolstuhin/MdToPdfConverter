using Avalonia;
using Avalonia.ReactiveUI;
using MdToPdfConverter.Services;

namespace MdToPdfConverter;

public class Program
{
    [STAThread]
    public static async Task Main(string[] args)
    {
        var singleInstance = new SingleInstanceService();

        if (!singleInstance.TryAcquire())
        {
            if (args.Length > 0 && File.Exists(args[0]))
                await SingleInstanceService.SendFilePathAsync(args[0]);
            return;
        }

        using (singleInstance)
        {
            singleInstance.StartListening();
            App.SingleInstance = singleInstance;

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseReactiveUI()
            .WithInterFont()
            .LogToTrace();
}
