# MdToPdfConverter

Simple Windows application to convert Markdown files to PDF with custom formatting options.

## Features

- Convert Markdown (.md) files to PDF via Windows Explorer context menu
- Customizable PDF settings:
  - Font size (8-24px)
  - Page margins (0-40mm)
  - Paper format (A3, A4, A5, Letter, Legal)
- GitHub-themed styling
- Auto-start with Windows (optional)

## Requirements

- Windows 10/11
- .NET 10

## Installation

1. Clone the repository
2. Build the project: `dotnet build`
3. Run: `dotnet run --project src/MdToPdfConverter`
4. Enable context menu in settings

## Settings

Access settings from the system tray icon. Configure:
- PDF appearance (font size, margins, paper format)
- Context menu integration
- Auto-start behavior

## License

MIT
