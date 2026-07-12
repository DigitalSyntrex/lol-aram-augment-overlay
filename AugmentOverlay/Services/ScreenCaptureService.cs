using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using OpenCvSharp;
using Tesseract;
using AugmentOverlay.Models;

namespace AugmentOverlay.Services
{
    public class ScreenCaptureService
    {
        private TesseractEngine? _tesseractEngine;

        public ScreenCaptureService()
        {
            // Initialize Tesseract OCR
            try
            {
                _tesseractEngine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Tesseract initialization error: {ex.Message}");
            }
        }

        public Bitmap CaptureScreen()
        {
            var screenWidth = Screen.PrimaryScreen.Bounds.Width;
            var screenHeight = Screen.PrimaryScreen.Bounds.Height;
            var bitmap = new Bitmap(screenWidth, screenHeight);
            var graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(screenWidth, screenHeight));
            return bitmap;
        }

        public List<Augment> DetectAugments(Bitmap screenshot)
        {
            var augments = new List<Augment>();

            try
            {
                // Define augment selection UI region (typically bottom-center of screen)
                // Adjust these coordinates based on your LoL UI layout
                var roiX = screenshot.Width / 4;
                var roiY = screenshot.Height - 400;
                var roiWidth = screenshot.Width / 2;
                var roiHeight = 350;

                // Extract the region of interest
                var roi = new Bitmap(roiWidth, roiHeight);
                var graphics = Graphics.FromImage(roi);
                graphics.DrawImage(screenshot, 0, 0, new Rectangle(roiX, roiY, roiWidth, roiHeight), GraphicsUnit.Pixel);
                graphics.Dispose();

                // Save ROI to temporary file for OCR
                var tempPath = Path.Combine(Path.GetTempPath(), "augment_region.png");
                roi.Save(tempPath, ImageFormat.Png);
                roi.Dispose();

                // Use Tesseract OCR to detect augment names
                using (var pix = Pix.LoadFromFile(tempPath))
                {
                    if (pix != null && _tesseractEngine != null)
                    {
                        using (var page = _tesseractEngine.Process(pix))
                        {
                            var text = page.GetText();
                            System.Diagnostics.Debug.WriteLine($"OCR detected text: {text}");

                            // Parse augment names from text
                            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var line in lines)
                            {
                                var trimmedLine = line.Trim();
                                if (!string.IsNullOrWhiteSpace(trimmedLine) && trimmedLine.Length > 2)
                                {
                                    augments.Add(new Augment 
                                    { 
                                        Id = trimmedLine.ToLower(),
                                        Name = trimmedLine,
                                        Description = $"Augment: {trimmedLine}"
                                    });
                                    System.Diagnostics.Debug.WriteLine($"Detected augment: {trimmedLine}");
                                }
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
                System.Diagnostics.Debug.WriteLine($"Augment detection error: {ex.Message}");
            }

            return augments;
        }
    }
}
