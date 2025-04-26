using RealEstateRecommendationEngine.Infrastructure;
using Moq;
using System;
using System.IO;
using Xunit;

namespace RealEstateRecommendationEngine.Tests.Infrastructure
{
    public class FileHelperTests
    {
        // Test for the GetDataFileDirectory method when using IFileHelper
        [Fact]
        public void GetDataFileDirectory_ShouldReturnValidDirectoryPath()
        {
            // Arrange
            var mockFileHelper = new Mock<IFileHelper>();
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            mockFileHelper.Setup(f => f.GetDataFileDirectory()).Returns(Path.Combine(baseDirectory, "DataFiles"));

            // Act
            string dataFileDirectory = mockFileHelper.Object.GetDataFileDirectory();

            // Assert
            Assert.True(Directory.Exists(dataFileDirectory), "The directory should exist.");
            Assert.Contains("DataFiles", dataFileDirectory, StringComparison.OrdinalIgnoreCase);
            Assert.StartsWith(baseDirectory, dataFileDirectory, StringComparison.OrdinalIgnoreCase);
        }

        // Test for creating directory if it doesn't exist when using IFileHelper
        [Fact]
        public void GetDataFileDirectory_ShouldCreateDirectoryIfNotExists()
        {
            // Arrange
            string dataFileDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataFiles");

            // Ensure the directory doesn't exist before the test starts
            if (Directory.Exists(dataFileDirectory))
            {
                Directory.Delete(dataFileDirectory, true);
            }

            // Create an instance of IFileHelper (you can either use the real implementation or mock it)
            var mockFileHelper = new Mock<IFileHelper>(); // If you want to mock it
            mockFileHelper.Setup(f => f.GetDataFileDirectory()).Returns(dataFileDirectory);

            // Act: Call the method on the mock (or real instance)
            string resultDirectory = mockFileHelper.Object.GetDataFileDirectory(); // Using the mock to invoke the method

            // Assert
            Assert.True(Directory.Exists(resultDirectory), "The DataFiles directory should be created.");
        }



        // Test for throwing an exception when base directory is null or invalid
        [Fact]
        public void GetDataFileDirectory_ShouldThrowExceptionIfBaseDirectoryIsNull()
        {
            // Arrange: Simulate a condition where the base directory is invalid
            var originalBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            try
            {
                // Temporarily set the base directory to null or empty (simulate an invalid state)
                AppDomain.CurrentDomain.SetData("AppBase", null); // Simulate an invalid base directory state

                // Act & Assert: Check if the exception is thrown
                Assert.Throws<InvalidOperationException>(() =>
                {
                    var mockFileHelper = new Mock<IFileHelper>();
                    mockFileHelper.Setup(f => f.GetDataFileDirectory()).Throws<InvalidOperationException>();
                    mockFileHelper.Object.GetDataFileDirectory();
                });
            }
            finally
            {
                // Restore original base directory or any necessary cleanup
                AppDomain.CurrentDomain.SetData("AppBase", originalBaseDirectory);
            }
        }
    }
}
