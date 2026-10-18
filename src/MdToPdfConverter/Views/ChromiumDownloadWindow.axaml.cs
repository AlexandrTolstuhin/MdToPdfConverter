using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace MdToPdfConverter.Views;

public partial class ChromiumDownloadWindow : Window
{
    public ChromiumDownloadWindow()
    {
        InitializeComponent();
    }

    private void Header_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            BeginMoveDrag(e);
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e) => Close();
}
