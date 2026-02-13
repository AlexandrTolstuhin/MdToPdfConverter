namespace MdToPdfConverter.Models;

public class AppSettings
{
    public int PdfFontSize { get; set; } = 14;
    public bool IsContextMenuRegistered { get; set; }
    public bool IsAutoStartEnabled { get; set; }
}
