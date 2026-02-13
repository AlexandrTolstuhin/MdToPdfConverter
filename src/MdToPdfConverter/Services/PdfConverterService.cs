using Markdown2Pdf;
using Markdown2Pdf.Options;
using MdToPdfConverter.Models;

namespace MdToPdfConverter.Services;

public class PdfConverterService : IPdfConverterService
{
    public async Task<ConversionResult> ConvertAsync(string mdFilePath, int fontSize, int marginMm)
    {
        try
        {
            var outputPath = Path.ChangeExtension(mdFilePath, ".pdf");
            var margin = $"{marginMm}mm";

            var options = new Markdown2PdfOptions
            {
                Theme = Theme.Github,
                CustomHeadContent = $"<style>body {{ font-size: {fontSize}px; }}</style>",
                MarginOptions = new MarginOptions
                {
                    Top = margin,
                    Bottom = margin,
                    Left = margin,
                    Right = margin
                }
            };

            var converter = new Markdown2PdfConverter(options);
            await converter.Convert(mdFilePath, outputPath);

            return new ConversionResult { Success = true, OutputPath = outputPath };
        }
        catch (Exception ex)
        {
            return new ConversionResult { Success = false, ErrorMessage = ex.Message };
        }
    }
}
