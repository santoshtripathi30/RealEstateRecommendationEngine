using Microsoft.AspNetCore.Mvc;

using RealEstateRecommendationEngine.Services;

namespace RealEstateRecommendationEngine.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataPrepController : ControllerBase
    {
        private readonly IAIPipelineServices _aIPipelineServices;
        private readonly IPropertyServices _propertyServices;

        public DataPrepController(IAIPipelineServices aIPipelineServices, IPropertyServices propertyServices)
        {
            _aIPipelineServices = aIPipelineServices;
            _propertyServices = propertyServices;
        }

        #region AI Model

        [HttpPost("GenerateAiModelZip")]
        public IActionResult GenerateAiModelZip()
        {
            try
            {
                _aIPipelineServices.CreateModelZipFile();
                return Ok("Model zip file created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating model: {ex.Message}");
            }
        }


        [HttpPost("LoadModel")]
        public IActionResult LoadModel()
        {
            try
            {
                _aIPipelineServices.LoadModel();
                return Ok("Latest AI Model loaded successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error loading AI Model: {ex.Message}");
            }
        }

        [HttpPost("ReloadAiModel")]
        public IActionResult ReloadAiModel()
        {
            try
            {
                _aIPipelineServices.LoadModel();
                return Ok("Latest AI Model reloaded successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error reloading AI Model: {ex.Message}");
            }
        }

        #endregion

        #region PropertyInfo
        [HttpPost("GetPropertyInfoJson")]
        public IActionResult GetPropertyInfoJson()
        {
            try
            {
                _propertyServices.ExportPropertiesToJson();
                return Ok("GetPropertyInfoJson successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error GetPropertyInfoJson: {ex.Message}");
            }
        }
        [HttpPost("LoadPropertiesFromJson")]
        public IActionResult LoadPropertiesFromJson()
        {
            try
            {
                _propertyServices.LoadPropertiesFromJson();
                return Ok("LoadPropertiesFromJson successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error LoadPropertiesFromJson: {ex.Message}");
            }
        }
 
        #endregion

    }
}
