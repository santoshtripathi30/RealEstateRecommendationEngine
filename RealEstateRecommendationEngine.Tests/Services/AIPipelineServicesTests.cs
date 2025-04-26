using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.ML;

using Moq;

using RealEstateRecommendationEngine.Infrastructure;
using RealEstateRecommendationEngine.Model;
using RealEstateRecommendationEngine.Services;

namespace RealEstateRecommendationEngine.Tests.Services
{
    public class AIPipelineServicesTests
    {
        private readonly Mock<ILogger<AIPipelineServices>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IFileHelper> _mockFileHelper;
        private readonly AIPipelineServices _service;

        public AIPipelineServicesTests()
        {
            _mockLogger = new Mock<ILogger<AIPipelineServices>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockFileHelper = new Mock<IFileHelper>();
            _service = new AIPipelineServices(_mockConfiguration.Object, _mockLogger.Object, _mockFileHelper.Object);
        }

        // Unit Test: Testing CreateModelZipFile method
        [Fact]
        public void CreateModelZipFile_ShouldSaveModelToFile()
        {
            // Arrange: Mock data and file system
            var ratings = new List<PropertyRating>
            {
                new () { UserId = "User1", PropertyId = "Property1", Rating = 5f },
                new () { UserId = "User2", PropertyId = "Property2", Rating = 3f }
            };

            _mockFileHelper.Setup(f => f.GetDataFileDirectory()).Returns("MockDirectory");
            _mockConfiguration.Setup(c => c.GetConnectionString(It.IsAny<string>())).Returns("ConnectionString");

            // Act: Call CreateModelZipFile
            _service.CreateModelZipFile();

            // Assert: Verify model creation and logging
            _mockLogger.Verify(logger => logger.LogInformation(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        }

        // Unit Test: Testing LoadModel method when a model is found
        [Fact]
        public void LoadModel_ShouldLoadExistingModel()
        {
            // Arrange: Mock the file retrieval
            var mockModelPath = "MockDirectory/model_20230401.zip";
            _mockFileHelper.Setup(f => f.GetDataFileDirectory()).Returns("MockDirectory");
            Directory.CreateDirectory("MockDirectory"); // Ensure directory exists
            File.Create(mockModelPath).Dispose(); // Create mock model file

            // Act: Load model
            var model = _service.LoadModel();

            // Assert: Ensure model is loaded
            Assert.NotNull(model);
        }

        // Unit Test: Testing LoadModel method when no model is found
        [Fact]
        public void LoadModel_ShouldThrowFileNotFoundException_WhenNoModelExists()
        {
            // Arrange: Mock the file system to return no model
            _mockFileHelper.Setup(f => f.GetDataFileDirectory()).Returns("MockDirectory");
            if (Directory.Exists("MockDirectory"))
            {
                Directory.Delete("MockDirectory", true);
            }

            // Act & Assert: Ensure exception is thrown
            var exception = Assert.Throws<FileNotFoundException>(() => _service.LoadModel());
            Assert.Equal("No trained model found.", exception.Message);
        }

        // Unit Test: Testing GetModel method
        [Fact]
        public void GetModel_ShouldReturnModel()
        {
            // Arrange: Mock LoadModel method to simulate lazy loading
            var mockModel = new Mock<ITransformer>();
            _mockFileHelper.Setup(f => f.GetDataFileDirectory()).Returns("MockDirectory");
            _service.ReloadModel(); // Trigger model reload

            // Act: Get model
            var model = _service.GetModel();

            // Assert: Ensure the model is returned
            Assert.NotNull(model);
        }

        // Unit Test: Testing ReloadModel method
        [Fact]
        public void ReloadModel_ShouldResetLazyModel()
        {
            // Arrange: Set initial lazy model
            _service.ReloadModel();

            // Act: Trigger model reload
            _service.ReloadModel();

            // Assert: Ensure the lazy model is reloaded
            var model = _service.GetModel();
            Assert.NotNull(model);
        }

        // Integration Test: Testing database interaction in LoadRatingsFromDatabase
        [Fact]
        public void LoadRatingsFromDatabase_ShouldReturnRatings()
        {
            // Arrange: Mock database interaction
            var mockRatings = new List<PropertyRating>
            {
                new () { UserId = "User1", PropertyId = "Property1", Rating = 4.5f },
                new () { UserId = "User2", PropertyId = "Property2", Rating = 2.5f }
            };

            var mockSqlConnection = new Mock<SqlConnection>("ConnectionString");
            var mockCommand = new Mock<SqlCommand>("SELECT UserId, PropertyId, Rating FROM vw_UserPropertyRating", mockSqlConnection.Object);
            var mockReader = new Mock<SqlDataReader>();

            // Mock reader to return predefined ratings
            mockReader.SetupSequence(r => r.Read())
                .Returns(true)
                .Returns(true)
                .Returns(false);
            mockReader.Setup(r => r.GetString(0)).Returns("User1").Callback(() => mockReader.Setup(r => r.GetString(0)).Returns("User2"));
            mockReader.Setup(r => r.GetString(1)).Returns("Property1").Callback(() => mockReader.Setup(r => r.GetString(1)).Returns("Property2"));
            mockReader.Setup(r => r.GetInt32(2)).Returns(5).Callback(() => mockReader.Setup(r => r.GetInt32(2)).Returns(3));

            // Act: Call LoadRatingsFromDatabase
            var ratings = _service.GetType().GetMethod("LoadRatingsFromDatabase", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_service, null) as List<PropertyRating>;

            // Assert: Ensure ratings are loaded
            Assert.NotNull(ratings);
            Assert.Equal(2, ratings.Count);
        }

        // Integration Test: Testing end-to-end model saving and loading
        [Fact]
        public void EndToEndModelTest_ShouldSaveAndLoadModel()
        {
            // Arrange: Prepare mock data and configuration
            var ratings = new List<PropertyRating>
            {
                new () { UserId = "User1", PropertyId = "Property1", Rating = 4.0f },
                new () { UserId = "User2", PropertyId = "Property2", Rating = 5.0f }
            };

            _mockFileHelper.Setup(f => f.GetDataFileDirectory()).Returns("MockDirectory");
            _mockConfiguration.Setup(c => c.GetConnectionString(It.IsAny<string>())).Returns("ConnectionString");

            // Act: Create model file
            _service.CreateModelZipFile();
            var model = _service.LoadModel();

            // Assert: Ensure model was saved and loaded correctly
            Assert.NotNull(model);
        }
    }
}
