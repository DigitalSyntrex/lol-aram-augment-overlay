using System;
using System.Drawing;
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
                // Convert bitmap to Mat for OpenCV processing
                var mat = OpenCvSharp.BitmapConverter.ToMat(screenshot);

                // Look for augment UI region (typically bottom-center of screen)
                // Adjust these coordinates based on LoL UI layout
                var roi = new OpenCvSharp.Rect(
                    screenshot.Width / 4,
                    screenshot.Height - 400,
                    screenshot.Width / 2,
                    350
                );

                var roiMat = new Mat(mat, roi);

                // Use OCR to detect augment names
                using (var pix = Pix.LoadFromFile("temp_augment.png"))
                {
                    if (pix != null && _tesseractEngine != null)
                    {
                        using (var page = _tesseractEngine.Process(pix))
                        {
                            var text = page.GetText();
                            // Parse augment names from text
                            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var line in lines)
                            {
                                if (!string.IsNullOrWhiteSpace(line))
                                {
                                    augments.Add(new Augment { Name = line.Trim() });
                                }
                            }
                        }
                    }
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
