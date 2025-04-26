using Microsoft.AspNetCore.Mvc;

using Moq;

using RealEstateRecommendationEngine.Controllers;
using RealEstateRecommendationEngine.Model;
using RealEstateRecommendationEngine.Services;

namespace RealEstateRecommendationEngine.Tests.Controllers
{


    public class RecommendationControllerTests
    {
        private readonly Mock<IRecommendationService> _mockRecommendationService;
        private readonly RecommendationController _controller;

        public RecommendationControllerTests()
        {
            _mockRecommendationService = new Mock<IRecommendationService>();
            _controller = new RecommendationController(_mockRecommendationService.Object);
        }

        [Fact]
        public void GetRecommendations_ShouldReturnOk_WhenRecommendationsFound()
        {
            // Arrange
            string userId = "user1";

            // Mock list of RealEstatePropertyInfo objects with your class properties
            var recommendations = new List<RealEstatePropertyInfo>
    {
        new RealEstatePropertyInfo
        {
            PropertyID = 1,
            Address = "123 Main St",
            BedRoom = 3,
            CarpetAreaSft = 1500.0,
            ConstructionStatus = "Completed",
            Description = "Spacious 3BHK apartment",
            FacingDirection = "East",
            PricePsftTentative = 5000.0m,
            ProjectName = "Sunrise Residency",
            SpecBathroom = "Attached",
            SpecKitchen = "Modular",
            SpecWindows = "Double Glazed"
        },
        new RealEstatePropertyInfo
        {
            PropertyID = 2,
            Address = "456 Oak Rd",
            BedRoom = 2,
            CarpetAreaSft = 1200.0,
            ConstructionStatus = "Under Construction",
            Description = "2BHK apartment with garden view",
            FacingDirection = "West",
            PricePsftTentative = 5500.0m,
            ProjectName = "Oak Valley",
            SpecBathroom = "Common",
            SpecKitchen = "Semi Modular",
            SpecWindows = "Aluminum"
        }
    };

            // Setup the mock to return the list of RealEstatePropertyInfo objects
            _mockRecommendationService.Setup(service => service.GetRecommendations(userId)).Returns(recommendations);

            // Act
            var result = _controller.GetRecommendations(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<List<RealEstatePropertyInfo>>(okResult.Value);

            // Check if the recommendation count matches
            Assert.Equal(recommendations.Count, returnValue.Count);

            // Check properties of the first property in the list
            Assert.Equal(recommendations[0].PropertyID, returnValue[0].PropertyID);
            Assert.Equal(recommendations[0].Address, returnValue[0].Address);
            Assert.Equal(recommendations[0].BedRoom, returnValue[0].BedRoom);
            Assert.Equal(recommendations[0].PricePsftTentative, returnValue[0].PricePsftTentative);
            // You can add other assertions for other properties similarly.
        }


        [Fact]
        public void GetRecommendations_ShouldReturnNotFound_WhenRecommendationsNotFound()
        {
            // Arrange
            string userId = "user2";

            // Mocking the service to return an empty list (simulating no recommendations)
            _mockRecommendationService.Setup(service => service.GetRecommendations(userId)).Returns(new List<RealEstatePropertyInfo>());

            // Act
            var result = _controller.GetRecommendations(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Recommendations not found for this user.", notFoundResult.Value);
        }


    }

}
