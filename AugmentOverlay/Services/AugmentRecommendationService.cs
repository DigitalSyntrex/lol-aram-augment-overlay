using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using AugmentOverlay.Models;

namespace AugmentOverlay.Services
{
    public class AugmentRecommendationService
    {
        private Dictionary<string, List<AugmentRecommendation>>? _recommendations;

        public AugmentRecommendationService()
        {
            LoadRecommendations();
        }

        private void LoadRecommendations()
        {
            _recommendations = new Dictionary<string, List<AugmentRecommendation>>();
            
            try
            {
                // Load augment recommendations from JSON file
                var filePath = "./Data/augment_recommendations.json";
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    var data = JsonConvert.DeserializeObject<Dictionary<string, List<AugmentRecommendation>>>(json);
                    if (data != null)
                    {
                        _recommendations = data;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading recommendations: {ex.Message}");
            }
        }

        public AugmentRecommendation? GetBestAugment(string? championName, List<Augment> availableAugments)
        {
            if (string.IsNullOrEmpty(championName) || availableAugments == null || availableAugments.Count == 0 || _recommendations == null)
            {
                return null;
            }

            if (!_recommendations.ContainsKey(championName))
            {
                return null;
            }

            var championRecs = _recommendations[championName];
            
            // Find the best recommendation from available augments
            foreach (var rec in championRecs.OrderByDescending(r => r.Priority))
            {
                var match = availableAugments.FirstOrDefault(a => a.Name.Equals(rec.Name, StringComparison.OrdinalIgnoreCase));
                if (match != null)
                {
                    return rec;
                }
            }

            return null;
        }
    }
}
