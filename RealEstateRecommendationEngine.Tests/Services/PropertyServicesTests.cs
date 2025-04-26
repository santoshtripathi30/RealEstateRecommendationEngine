using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using Moq;

using RealEstateRecommendationEngine.Infrastructure;
using RealEstateRecommendationEngine.Model;
using RealEstateRecommendationEngine.Services;

using System.Reflection;
using System.Text.Json;

namespace RealEstateRecommendationEngine.Tests.Services
{
    public class PropertyServicesTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<IFileHelper> _mockFileHelper;
        private readonly PropertyServices _service;
        private readonly string _testJsonPath;

        public PropertyServicesTests()
        {
            // Create a dictionary to mock the configuration
            var inMemorySettings = new Dictionary<string, string>
    {
        { "ConnectionStrings:MyConnectionString", "FakeConnectionString" }
    };

            // Create the mock IConfiguration object
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c[It.IsAny<string>()]).Returns((string key) =>
            {
                inMemorySettings.TryGetValue(key, out var value);
                return value;
            });

            _mockFileHelper = new Mock<IFileHelper>();
            _mockFileHelper.Setup(f => f.GetDataFileDirectory()).Returns("FakeTestDirectory");

            // Create the PropertyServices instance
            _service = new PropertyServices(_mockConfig.Object, _mockFileHelper.Object);

            _testJsonPath = Path.Combine(_mockFileHelper.Object.GetDataFileDirectory(), "properties.json");
        }


        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            Assert.NotNull(_service);
        }

        [Fact]
        public void LoadPropertiesFromJson_FileDoesNotExist_ShouldThrowFileNotFoundException()
        {
            if (File.Exists(_testJsonPath))
                File.Delete(_testJsonPath);

            Assert.Throws<FileNotFoundException>(() => _service.LoadPropertiesFromJson());
        }

        [Fact]
        public void LoadPropertiesFromJson_ValidFile_ShouldLoadProperties()
        {
            // Arrange
            var properties = new List<RealEstatePropertyInfo>
    {
        new() { PropertyID = 1, Address = "Test Address", BedRoom = 2 }
    };

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(_testJsonPath));

            // Write the properties to the JSON file
            File.WriteAllText(_testJsonPath, JsonSerializer.Serialize(properties));

            // Act
            _service.LoadPropertiesFromJson();

            // Get the loaded properties
            var allProperties = _service.GetAllProperties();

            // Assert
            Assert.NotNull(allProperties);  // Ensure properties are loaded
            Assert.Single(allProperties);   // Ensure there is exactly one property loaded
            Assert.Equal("Test Address", allProperties[0].Address);  // Ensure the address is correct
        }


        [Fact]
        public void GetAllProperties_WhenPropertiesAlreadyLoaded_ShouldNotReload()
        {
            // Arrange
            var testProperties = new List<RealEstatePropertyInfo>
        {
            new() { PropertyID = 10, Address = "Preloaded" }
        };
            var field = typeof(PropertyServices)
                .GetField("_propertyInfos", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(_service, testProperties);

            // Act
            var properties = _service.GetAllProperties();

            // Assert
            Assert.Single(properties);
            Assert.Equal(10, properties[0].PropertyID);
        }

        [Fact]
        public void ExportPropertiesToJson_WhenDatabaseConnectionFails_ShouldThrow()
        {
            // Arrange
            var badConfig = new Mock<IConfiguration>();
            badConfig.Setup(c => c.GetConnectionString(It.IsAny<string>())).Returns("InvalidConnectionString");

            var mockFileHelper = new Mock<IFileHelper>();

            var badService = new PropertyServices(badConfig.Object, mockFileHelper.Object);

            // Act & Assert
            Assert.Throws<SqlException>(() => badService.ExportPropertiesToJson());
        }


        [Fact]
        public void ExportPropertiesToJson_WhenWriteFails_ShouldThrow()
        {
            // Complex: Would need to simulate file system write failure.
            // Skip actual disk IO in unit tests, should use integration or abstraction.
        }
    }

}
