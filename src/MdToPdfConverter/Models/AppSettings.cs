namespace MdToPdfConverter.Models;

public class AppSettings
{
    public int PdfFontSize { get; set; } = 14;
    public int PdfMarginMm { get; set; } = 20;
    public string PaperFormat { get; set; } = "A4";
    public bool IsContextMenuRegistered { get; set; }
    public bool IsAutoStartEnabled { get; set; }
}
