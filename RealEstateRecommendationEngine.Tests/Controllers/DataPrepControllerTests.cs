using Microsoft.AspNetCore.Mvc;

using Moq;

using RealEstateRecommendationEngine.Controllers;
using RealEstateRecommendationEngine.Services;

namespace RealEstateRecommendationEngine.Tests.Controllers
{


    public class DataPrepControllerTests
    {
        private readonly Mock<IAIPipelineServices> _mockAIPipelineServices;
        private readonly Mock<IPropertyServices> _mockPropertyServices;
        private readonly DataPrepController _controller;

        public DataPrepControllerTests()
        {
            _mockAIPipelineServices = new Mock<IAIPipelineServices>();
            _mockPropertyServices = new Mock<IPropertyServices>();
            _controller = new DataPrepController(_mockAIPipelineServices.Object, _mockPropertyServices.Object);
        }

        [Fact]
        public void GenerateAiModelZip_ShouldReturnOk_WhenModelCreatedSuccessfully()
        {
            // Arrange
            _mockAIPipelineServices.Setup(service => service.CreateModelZipFile()).Verifiable();

            // Act
            var result = _controller.GenerateAiModelZip();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Model zip file created successfully.", okResult.Value);
            _mockAIPipelineServices.Verify(service => service.CreateModelZipFile(), Times.Once);
        }

        [Fact]
        public void GenerateAiModelZip_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            _mockAIPipelineServices.Setup(service => service.CreateModelZipFile()).Throws(new Exception("Test Exception"));

            // Act
            var result = _controller.GenerateAiModelZip();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error creating model: Test Exception", badRequestResult.Value);
        }

        [Fact]
        public void GetPropertyInfoJson_ShouldReturnOk_WhenExportSuccess()
        {
            // Arrange
            _mockPropertyServices.Setup(service => service.ExportPropertiesToJson()).Verifiable();

            // Act
            var result = _controller.GetPropertyInfoJson();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("GetPropertyInfoJson successfully.", okResult.Value);
            _mockPropertyServices.Verify(service => service.ExportPropertiesToJson(), Times.Once);
        }

        [Fact]
        public void GetPropertyInfoJson_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            _mockPropertyServices.Setup(service => service.ExportPropertiesToJson()).Throws(new Exception("Test Exception"));

            // Act
            var result = _controller.GetPropertyInfoJson();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error GetPropertyInfoJson: Test Exception", badRequestResult.Value);
        }
    }

}
