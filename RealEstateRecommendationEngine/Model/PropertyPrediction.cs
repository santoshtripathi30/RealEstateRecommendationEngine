using Microsoft.ML.Data;

using System.ComponentModel.DataAnnotations;

namespace RealEstateRecommendationEngine.Model
{
    /// <summary>
    /// Prediction result for a property recommendation.
    /// </summary>
    public class PropertyPredictionWithId
    {
        [Required]
        public string PropertyId { get; set; } = string.Empty;

        [ColumnName("Score")]
        public float Score { get; set; }
    }


    /// <summary>
    /// Input model for training and predicting user-property ratings.
    /// </summary>
    public class PropertyRating
    {
        [Required]
        public string UserId { get; set; } = string.Empty;  
        [Required]
        public string PropertyId { get; set; } = string.Empty;
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
        public float Rating { get; set; }
    }
}