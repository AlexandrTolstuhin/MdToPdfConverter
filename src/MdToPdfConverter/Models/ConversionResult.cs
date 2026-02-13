namespace MdToPdfConverter.Models;

public class ConversionResult
{
    public bool Success { get; set; }
    public string OutputPath { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}
