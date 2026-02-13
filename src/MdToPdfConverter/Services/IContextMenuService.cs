namespace MdToPdfConverter.Services;

public interface IContextMenuService
{
    void Register(string exePath);
    void Unregister();
    bool IsRegistered();
}
