using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Tesseract;
using AugmentOverlay.Models;

namespace AugmentOverlay.Services
{
    public class ChampionDetectionService
    {
        private TesseractEngine? _tesseractEngine;
        private Dictionary<string, Champion>? _championCache;

        public ChampionDetectionService()
        {
            _championCache = new Dictionary<string, Champion>();
            try
            {
                _tesseractEngine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Tesseract initialization error: {ex.Message}");
            }
        }

        public Champion? DetectChampion(Bitmap screenshot)
        {
            try
            {
                // Look for champion name in top-left corner of screen (HUD area)
                // LoL typically shows champion name in the top-left corner
                var championNameText = ExtractTextFromRegion(screenshot, 10, 10, 200, 50);
                
                if (!string.IsNullOrWhiteSpace(championNameText))
                {
                    // Clean up the text
                    var cleanedName = championNameText.Trim().ToLower();
                    
                    // Check cache first
                    if (_championCache != null && _championCache.ContainsKey(cleanedName))
                    {
                        return _championCache[cleanedName];
                    }

                    // Create a new champion entry
                    var champion = new Champion
                    {
                        Id = cleanedName,
                        Name = cleanedName,
                        Title = cleanedName
                    };

                    // Cache it
                    if (_championCache != null)
                    {
                        _championCache[cleanedName] = champion;
                    }

                    System.Diagnostics.Debug.WriteLine($"Detected champion: {cleanedName}");
                    return champion;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Champion detection error: {ex.Message}");
            }

            return null;
        }

        private string? ExtractTextFromRegion(Bitmap screenshot, int x, int y, int width, int height)
        {
            try
            {
                // Create a cropped region of the screenshot
                var region = new Bitmap(width, height);
                var graphics = System.Drawing.Graphics.FromImage(region);
                graphics.DrawImage(screenshot, 0, 0, new Rectangle(x, y, width, height), GraphicsUnit.Pixel);

                // Save region to temporary file
                var tempPath = Path.Combine(Path.GetTempPath(), "champion_region.png");
                region.Save(tempPath, System.Drawing.Imaging.ImageFormat.Png);
                region.Dispose();
                graphics.Dispose();

                // Use OCR to extract text
                using (var pix = Pix.LoadFromFile(tempPath))
                {
                    if (pix != null && _tesseractEngine != null)
                    {
                        using (var page = _tesseractEngine.Process(pix))
                        {
                            var text = page.GetText().Trim();
                            if (!string.IsNullOrWhiteSpace(text))
                            {
                                System.Diagnostics.Debug.WriteLine($"Extracted champion text: {text}");
                                return text;
                            }
                        }
                    }
                }

                // Clean up temp file
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Text extraction error: {ex.Message}");
            }
            return null;
        }
    }
}
