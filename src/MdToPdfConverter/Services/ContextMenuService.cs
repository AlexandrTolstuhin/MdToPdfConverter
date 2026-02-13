using Microsoft.Win32;

namespace MdToPdfConverter.Services;

public class ContextMenuService : IContextMenuService
{
    private const string ShellKeyPath = @"Software\Classes\.md\shell\ConvertToPdf";
    private const string CommandKeyPath = @"Software\Classes\.md\shell\ConvertToPdf\command";

    public void Register(string exePath)
    {
        using var shellKey = Registry.CurrentUser.CreateSubKey(ShellKeyPath);
        shellKey.SetValue("", "Convert to PDF");
        shellKey.SetValue("Icon", $"\"{exePath}\",0");

        using var commandKey = Registry.CurrentUser.CreateSubKey(CommandKeyPath);
        commandKey.SetValue("", $"\"{exePath}\" \"%1\"");
    }

    public void Unregister()
    {
        Registry.CurrentUser.DeleteSubKeyTree(ShellKeyPath, throwOnMissingSubKey: false);
    }

    public bool IsRegistered()
    {
        using var key = Registry.CurrentUser.OpenSubKey(CommandKeyPath);
        return key is not null;
    }
}
