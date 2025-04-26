using Xunit;
using Moq;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Data;
using RealEstateRecommendationEngine.Model;
using RealEstateRecommendationEngine.Services;
using System.Reflection;

namespace RealEstateRecommendationEngine.Tests.Services
{
    public class RecommendationServiceTests
    {
        private readonly Mock<IAIPipelineServices> _mockAiPipeline;
        private readonly Mock<IPropertyServices> _mockPropertyService;
        private readonly RecommendationService _recommendationService;
        private readonly MLContext _mlContext;

        public RecommendationServiceTests()
        {
            _mockAiPipeline = new Mock<IAIPipelineServices>();
            _mockPropertyService = new Mock<IPropertyServices>();
            _mlContext = new MLContext();

            _recommendationService = new RecommendationService(_mockAiPipeline.Object, _mockPropertyService.Object);
        }

        [Fact]
        public void GetRecommendations_ReturnsTop3Recommendations()
        {
            // Arrange
            var sampleProperties = new List<RealEstatePropertyInfo>
        {
            new () { PropertyID = 101 },
            new () { PropertyID = 102 },
            new () { PropertyID = 103 },
            new () { PropertyID = 104 }
        };

            _mockPropertyService.Setup(p => p.GetAllProperties()).Returns(sampleProperties);

            var predictionData = new List<PropertyPredictionWithId>
        {
            new() { PropertyId = "103", Score = 0.9f },
            new() { PropertyId = "101", Score = 0.8f },
            new() { PropertyId = "104", Score = 0.7f },
            new() { PropertyId = "102", Score = 0.6f }
        };

            var predictionDataView = _mlContext.Data.LoadFromEnumerable(predictionData);
            var mockTransformer = new Mock<ITransformer>();
            mockTransformer.Setup(t => t.Transform(It.IsAny<IDataView>())).Returns(predictionDataView);

            _mockAiPipeline.Setup(m => m.GetModel()).Returns(mockTransformer.Object);

            // Act
            var result = _recommendationService.GetRecommendations("test-user");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, r => r.PropertyID == 103);
            Assert.Contains(result, r => r.PropertyID == 101);
            Assert.Contains(result, r => r.PropertyID == 104);
        }
    }
}