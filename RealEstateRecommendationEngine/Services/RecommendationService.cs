using Microsoft.ML.Trainers;
using Microsoft.ML;
using RealEstateRecommendationEngine.Model;
using Microsoft.ML.Data;

namespace RealEstateRecommendationEngine.Services
{
    public class RecommendationService
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private PredictionEngine<PropertyRating, PropertyPrediction> _predictionEngine;
        private IDataView _trainingData;
        private ITransformer _transformer;

        public RecommendationService()
        {
            _mlContext = new MLContext();
            TrainModel();
        }

        private void TrainModel()
        {
            var data = new List<PropertyRating>
            {
                new() { UserId = "user1", PropertyId = "flat101", Rating = 5 },
                new() { UserId = "user1", PropertyId = "flat102", Rating = 4 },
                new() { UserId = "user2", PropertyId = "flat103", Rating = 3 },
                new() { UserId = "user2", PropertyId = "flat104", Rating = 5 },
                new() { UserId = "user3", PropertyId = "flat101", Rating = 2 },
            };

            _trainingData = _mlContext.Data.LoadFromEnumerable(data);

            // Apply the MapValueToKey transformations to encode UserId and PropertyId
            var dataPipeline = _mlContext.Transforms.Conversion
                .MapValueToKey("userIdEncoded", nameof(PropertyRating.UserId))
                .Append(_mlContext.Transforms.Conversion.MapValueToKey("propertyIdEncoded", nameof(PropertyRating.PropertyId)));

            // Transform the data with encoding
            var transformedData = dataPipeline.Fit(_trainingData).Transform(_trainingData);

            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = "userIdEncoded", // Encoded userId
                MatrixRowIndexColumnName = "propertyIdEncoded", // Encoded propertyId
                LabelColumnName = nameof(PropertyRating.Rating),
                NumberOfIterations = 20,
                ApproximationRank = 100
            };

            var pipeline = _mlContext.Recommendation().Trainers.MatrixFactorization(options);
            _model = pipeline.Fit(transformedData);

            // Set up the transformer to apply the same transformations to new data
            _transformer = dataPipeline.Fit(_trainingData); // Store the transformer
        }

        public List<string> GetRecommendations(string userId)
        {
            var allProperties = new[] { "flat101", "flat102", "flat103", "flat104", "flat105" };

            var data = allProperties.Select(p => new PropertyRating
            {
                UserId = userId,
                PropertyId = p,
                Rating = 0 // Placeholder
            }).ToList();

            var dataView = _mlContext.Data.LoadFromEnumerable(data);

            // Apply the same transformations as during training
            var transformedData = _transformer.Transform(dataView);

            // Use the trained model to score the transformed data
            var scoredData = _model.Transform(transformedData);

            // Extract the predictions
            var predictions = _mlContext.Data.CreateEnumerable<PropertyPredictionWithId>(scoredData, reuseRowObject: false)
                .Select((pred, idx) => new { Property = allProperties[idx], Score = pred.Score })
                .OrderByDescending(x => x.Score)
                .Take(3)
                .Select(x => x.Property)
                .ToList();

            return predictions;
        }

    }
}
