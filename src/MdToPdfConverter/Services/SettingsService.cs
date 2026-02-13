using System.Text.Json;
using MdToPdfConverter.Models;

namespace MdToPdfConverter.Services;

public class SettingsService : ISettingsService
{
    private static readonly string SettingsDir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MdToPdfConverter");

    private static readonly string SettingsPath = Path.Combine(SettingsDir, "settings.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public AppSettings Current { get; private set; } = new();

    public async Task<AppSettings> LoadAsync()
    {
        if (!File.Exists(SettingsPath))
        {
            Current = new AppSettings();
            return Current;
        }

        await using var stream = File.OpenRead(SettingsPath);
        Current = await JsonSerializer.DeserializeAsync<AppSettings>(stream, JsonOptions) ?? new AppSettings();
        return Current;
    }

    public async Task SaveAsync(AppSettings settings)
    {
        Current = settings;
        Directory.CreateDirectory(SettingsDir);
        await using var stream = File.Create(SettingsPath);
        await JsonSerializer.SerializeAsync(stream, settings, JsonOptions);
    }
}
