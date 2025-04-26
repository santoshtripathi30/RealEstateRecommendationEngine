using Xunit;
using System.ComponentModel.DataAnnotations;
using RealEstateRecommendationEngine.Model;
namespace RealEstateRecommendationEngine.Tests.Model;
public class PropertyPredictionTests
{
    [Fact]
    public void PropertyPredictionWithId_ValidModel_ShouldPass()
    {
        // Arrange
        var propertyPrediction = new PropertyPredictionWithId
        {
            PropertyId = "101",
            Score = 0.9f
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(propertyPrediction, new ValidationContext(propertyPrediction), validationResults, true);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void PropertyPredictionWithId_InvalidPropertyId_ShouldFailValidation()
    {
        // Arrange
        var propertyPrediction = new PropertyPredictionWithId
        {
            PropertyId = string.Empty, // Invalid PropertyId
            Score = 0.9f
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(propertyPrediction, new ValidationContext(propertyPrediction), validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, r => r.MemberNames.Contains("PropertyId"));
    }

    [Fact]
    public void PropertyRating_ValidModel_ShouldPass()
    {
        // Arrange
        var propertyRating = new PropertyRating
        {
            UserId = "user123",
            PropertyId = "101",
            Rating = 4.5f
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(propertyRating, new ValidationContext(propertyRating), validationResults, true);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void PropertyRating_InvalidRating_ShouldFailValidation()
    {
        // Arrange
        var propertyRating = new PropertyRating
        {
            UserId = "user123",
            PropertyId = "101",
            Rating = -1f // Invalid Rating (should be in valid range)
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(propertyRating, new ValidationContext(propertyRating), validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, r => r.MemberNames.Contains("Rating"));
    }
}
