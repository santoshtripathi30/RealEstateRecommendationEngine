using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

using RealEstateRecommendationEngine;

using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace RealEstateRecommendationEngine.Tests.Controllers
{


    public class RecommendationControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public RecommendationControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetRecommendations_ShouldReturnOk_WhenRecommendationsExist()
        {
            // Act
            var response = await _client.GetAsync("/api/recommendation/user1");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Property", content); // Assuming the recommendations contain "Property"
        }

        [Fact]
        public async Task GetRecommendations_ShouldReturnNotFound_WhenRecommendationsDoNotExist()
        {
            // Act
            var response = await _client.GetAsync("/api/recommendation/userNotFound");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("Recommendations not found for this user.", content);
        }
    }

}
