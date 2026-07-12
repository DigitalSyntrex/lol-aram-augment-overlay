# LoL ARAM Augment Overlay

A C# application that provides an in-game overlay for League of Legends ARAM Mayhem mode. The overlay automatically detects your champion and highlights the best augment recommendation when you level up and receive augment options.

## Features

- **Auto Champion Detection**: Automatically detects your champion from the game screen
- **Screen Capture**: Monitors augment selection UI during level-ups
- **Augment Recommendation**: Highlights the best augment for your champion based on community data
- **Real-time Overlay**: Displays recommendations as an overlay on top of the game

## Architecture

- `AugmentOverlay/` - Main WPF application
- `ScreenCapture/` - Screen capture and OCR utilities
- `AugmentData/` - Augment database and champion recommendations
- `ChampionDetection/` - Champion detection from screen
- `Data/` - Augment and champion data files

## Requirements

- .NET 6.0 or higher
- Windows 10/11
- League of Legends (ARAM Mayhem mode)

## Setup

1. Clone the repository
2. Install dependencies (see below)
3. Run the application
4. Join an ARAM Mayhem game
5. The overlay will automatically activate when augment options appear

## Dependencies

- `Tesseract.Net.Core` - OCR for text recognition
- `OpenCvSharp` - Image processing and detection
- `Newtonsoft.Json` - JSON parsing

## License

MIT
