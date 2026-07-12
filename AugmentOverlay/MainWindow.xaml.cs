using System.Windows;
using AugmentOverlay.Services;
using AugmentOverlay.Models;

namespace AugmentOverlay
{
    public partial class MainWindow : Window
    {
        private ScreenCaptureService _screenCapture;
        private ChampionDetectionService _championDetection;
        private AugmentRecommendationService _augmentService;

        public MainWindow()
        {
            InitializeComponent();
            InitializeServices();
            StartMonitoring();
        }

        private void InitializeServices()
        {
            _augmentService = new AugmentRecommendationService();
            _championDetection = new ChampionDetectionService();
            _screenCapture = new ScreenCaptureService();
        }

        private async void StartMonitoring()
        {
            // Monitor screen for augment selection UI
            while (true)
            {
                try
                {
                    // Capture screen
                    var screenshot = _screenCapture.CaptureScreen();

                    // Detect champion from screen
                    var champion = _championDetection.DetectChampion(screenshot);
                    if (champion != null)
                    {
                        ChampionName.Text = champion.Name;
                    }

                    // Detect augments from screen
                    var augments = _screenCapture.DetectAugments(screenshot);
                    if (augments.Count > 0)
                    {
                        // Get recommendation
                        var recommendation = _augmentService.GetBestAugment(champion?.Name, augments);
                        if (recommendation != null)
                        {
                            AugmentName.Text = recommendation.Name;
                            AugmentDesc.Text = recommendation.Description;
                        }
                    }

                    await Task.Delay(500); // Check every 500ms
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}
