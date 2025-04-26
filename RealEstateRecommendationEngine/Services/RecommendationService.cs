using Microsoft.ML;

using RealEstateRecommendationEngine.Model;

namespace RealEstateRecommendationEngine.Services
{
    public interface IRecommendationService
    {
        public List<PropertyInfo> GetRecommendations(string userId);
    }
    public class RecommendationService : IRecommendationService
    {
        private readonly MLContext _mlContext;
        private readonly IAIPipelineServices _aIPipelineServices;
        private readonly IPropertyServices _propertyServices;
        private readonly Lazy<ITransformer> _modelLazy;

        public RecommendationService(IAIPipelineServices aIPipelineServices, IPropertyServices propertyServices)
        {
            _mlContext = new MLContext();
            _aIPipelineServices = aIPipelineServices;
            _propertyServices = propertyServices;

            _modelLazy = new Lazy<ITransformer>(() =>
            {
                var model = _aIPipelineServices.GetModel();
                if (model == null)
                {
                    throw new InvalidOperationException("Model is not loaded.");
                }

                return model;
            }, true);
        }

        public List<PropertyInfo> GetRecommendations(string userId)
        {
            var allProperties = _propertyServices.GetAllProperties();

            var data = allProperties.Select(p => new PropertyRating
            {
                UserId = userId,
                PropertyId = p.PropertyID.ToString(),
                Rating = 2 // You can enhance this dynamically
            }).ToList();

            var dataView = _mlContext.Data.LoadFromEnumerable(data);
            var model = _modelLazy.Value;
            var transformedData = model.Transform(dataView);

            var topPredictions = _mlContext.Data
                .CreateEnumerable<PropertyPredictionWithId>(transformedData, reuseRowObject: false)
                .OrderByDescending(x => x.Score)
                .Take(3)
                .Select(x => x.PropertyId)
                .ToList();

            var recommendedProperties = allProperties
                .Where(p => topPredictions.Contains(p.PropertyID.ToString()))
                .ToList();

            return recommendedProperties;
        }
    }

}
