using Microsoft.AspNetCore.Mvc;


using RealEstateRecommendationEngine.Services;

namespace RealEstateRecommendationEngine.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;
    

        public RecommendationController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
            
        }

        // Get recommendations for a user
        [HttpGet("{userId}")]
        public IActionResult GetRecommendations(string userId)
        {
            var results = _recommendationService.GetRecommendations(userId);
            if (results == null)
            {
                return NotFound("Recommendations not found for this user.");
            }
            return Ok(results);
        }

    }
}
