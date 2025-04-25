using Microsoft.ML.Data;

namespace RealEstateRecommendationEngine.Model
{
    public class PropertyPrediction
    {
        [ColumnName("Score")]
        public float Score { get; set; }
    }

    public class PropertyPredictionWithId
    {
        public float Score { get; set; }
    }
}
