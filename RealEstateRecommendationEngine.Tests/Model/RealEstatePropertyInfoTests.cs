using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateRecommendationEngine.Tests.Model
{
    using Xunit;
    using RealEstateRecommendationEngine.Model;

    public class RealEstatePropertyInfoTests
    {
        [Fact]
        public void RealEstatePropertyInfo_ValidModel_ShouldInitializeCorrectly()
        {
            // Arrange
            var propertyInfo = new RealEstatePropertyInfo
            {
                PropertyID = 1,
                Address = "123 Main St",
                BedRoom = 3,
                CarpetAreaSft = 1500.5,
                ConstructionStatus = "Completed",
                Description = "Beautiful 3-bedroom home",
                FacingDirection = "North",
                PricePsftTentative = 1200.75m,
                ProjectName = "Sunrise Apartments",
                SpecBathroom = "2",
                SpecKitchen = "1",
                SpecWindows = "5"
            };

            // Act & Assert
            Assert.Equal(1, propertyInfo.PropertyID);
            Assert.Equal("123 Main St", propertyInfo.Address);
            Assert.Equal(3, propertyInfo.BedRoom);
            Assert.Equal(1500.5, propertyInfo.CarpetAreaSft);
            Assert.Equal("Completed", propertyInfo.ConstructionStatus);
            Assert.Equal("Beautiful 3-bedroom home", propertyInfo.Description);
            Assert.Equal("North", propertyInfo.FacingDirection);
            Assert.Equal(1200.75m, propertyInfo.PricePsftTentative);
            Assert.Equal("Sunrise Apartments", propertyInfo.ProjectName);
            Assert.Equal("2", propertyInfo.SpecBathroom);
            Assert.Equal("1", propertyInfo.SpecKitchen);
            Assert.Equal("5", propertyInfo.SpecWindows);
        }

        [Fact]
        public void RealEstatePropertyInfo_DefaultValues_ShouldBeEmptyOrZero()
        {
            // Arrange
            var propertyInfo = new RealEstatePropertyInfo();

            // Act & Assert
            Assert.Equal(0, propertyInfo.PropertyID);
            Assert.Equal(string.Empty, propertyInfo.Address);
            Assert.Equal(0, propertyInfo.BedRoom);
            Assert.Equal(0.0, propertyInfo.CarpetAreaSft);
            Assert.Equal(string.Empty, propertyInfo.ConstructionStatus);
            Assert.Equal(string.Empty, propertyInfo.Description);
            Assert.Equal(string.Empty, propertyInfo.FacingDirection);
            Assert.Equal(0.0m, propertyInfo.PricePsftTentative);
            Assert.Equal(string.Empty, propertyInfo.ProjectName);
            Assert.Equal(string.Empty, propertyInfo.SpecBathroom);
            Assert.Equal(string.Empty, propertyInfo.SpecKitchen);
            Assert.Equal(string.Empty, propertyInfo.SpecWindows);
        }
    }

}
