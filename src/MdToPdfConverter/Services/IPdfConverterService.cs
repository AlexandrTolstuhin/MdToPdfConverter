using MdToPdfConverter.Models;

namespace MdToPdfConverter.Services;

public interface IPdfConverterService
{
    Task<ConversionResult> ConvertAsync(string mdFilePath, int fontSize, int marginMm, string paperFormat);
}
