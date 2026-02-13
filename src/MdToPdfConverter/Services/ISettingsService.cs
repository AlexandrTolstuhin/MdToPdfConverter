using MdToPdfConverter.Models;

namespace MdToPdfConverter.Services;

public interface ISettingsService
{
    AppSettings Current { get; }
    Task<AppSettings> LoadAsync();
    Task SaveAsync(AppSettings settings);
}
