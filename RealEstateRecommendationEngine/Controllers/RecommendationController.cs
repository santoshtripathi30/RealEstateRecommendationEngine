using Microsoft.AspNetCore.Mvc;

using RealEstateRecommendationEngine.Services;


namespace RealEstateRecommendationEngine.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecommendationController : ControllerBase
{
    private readonly RecommendationService _service;

    public RecommendationController(RecommendationService service)
    {
        _service = service;
    }

    [HttpGet("{userId}")]
    public IActionResult GetRecommendations(string userId)
    {
        var results = _service.GetRecommendations(userId);
        return Ok(results);
    }
}