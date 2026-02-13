namespace MdToPdfConverter.Services;

public interface IAutoStartService
{
    void Enable(string exePath);
    void Disable();
    bool IsEnabled();
}
