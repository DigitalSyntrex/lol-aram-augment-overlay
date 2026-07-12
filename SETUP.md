# Setup Instructions

## Prerequisites

1. **Visual Studio 2022** or later (or VS Code with C# extension)
2. **.NET 6.0 SDK** or higher
3. **Windows 10/11**

## Installation Steps

### 1. Clone Repository
```bash
git clone https://github.com/DigitalSyntrex/lol-aram-augment-overlay.git
cd lol-aram-augment-overlay
```

### 2. Install Dependencies

Dependencies are defined in `AugmentOverlay/AugmentOverlay.csproj` and will be restored automatically when you build.

Manually restore if needed:
```bash
cd AugmentOverlay
dotnet restore
```

### 3. Download Tesseract Data

The Tesseract OCR engine requires language data files:

1. Download tessdata from: https://github.com/UB-Mannheim/tesseract/wiki
2. Extract to: `AugmentOverlay/tessdata/`
3. Ensure `eng.traineddata` is present in that directory

### 4. Build Project

```bash
cd AugmentOverlay
dotnet build -c Release
```

### 5. Run Application

```bash
dotnet run
```

## Configuration

### Augment Data

Augment recommendations are stored in `Data/augment_recommendations.json`. Update this file with:
- Champion names
- Augment priorities (higher = better)
- Augment descriptions

### Screen Regions

Depending on your screen resolution and LoL settings, you may need to adjust:
- Augment UI detection region in `ScreenCaptureService.cs`
- Champion detection region in `ChampionDetectionService.cs`

## Troubleshooting

### OCR Not Working
- Ensure tessdata folder exists and contains `eng.traineddata`
- Check file paths are correct

### Champion/Augment Not Detecting
- Run with debug output enabled
- Verify screen resolution matches expected coordinates
- Check that game UI is visible during detection

### Overlay Not Appearing
- Ensure League of Legends is running in windowed or windowed fullscreen mode
- Check that overlay window is not behind the game window

## Next Steps

1. Fine-tune screen region detection for your resolution
2. Expand augment_recommendations.json with all champions
3. Implement more robust champion detection
4. Add custom hotkeys for enabling/disabling overlay
