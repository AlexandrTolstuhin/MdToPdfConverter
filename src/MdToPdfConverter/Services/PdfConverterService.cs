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
                CustomHeadContent = $@"<style>
                    body, p, li, td, th, div, span {{ font-size: {fontSize}px !important; }}
                    h1 {{ font-size: {fontSize * 2}px !important; }}
                    h2 {{ font-size: {fontSize * 1.75}px !important; }}
                    h3 {{ font-size: {fontSize * 1.5}px !important; }}
                    h4 {{ font-size: {fontSize * 1.25}px !important; }}
                    h5 {{ font-size: {fontSize * 1.1}px !important; }}
                    h6 {{ font-size: {fontSize}px !important; }}
                    code {{ font-size: {fontSize * 0.9}px !important; }}
                </style>",
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
