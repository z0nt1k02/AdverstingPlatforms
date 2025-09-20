using AdverstingPlatforms.Dtos;
using AdverstingPlatforms.Interfaces;
using AdverstingPlatforms.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdverstingPlatforms.Controllers
{
    [Route("api/platforms")]
    [ApiController]
    public class AdverstingPlatformsController : ControllerBase
    {
        private readonly AdverstingPlatformsService _service;
        public AdverstingPlatformsController(AdverstingPlatformsService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("update")]
        public async Task<IActionResult> UpdatePlatforms()
        {
            var result = await _service.UpdatePlatforms();
            return Ok(result);
        }
        [HttpGet]
        public IActionResult GetPlatforms()
        {
            var result = _service.GetPlatforms();
            return Ok(result);
        }

        [HttpPost]
        public IActionResult GetPlatforms(PathDto dto)
        {
            try
            {
                var result = _service.FindPlatforms(dto.path);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return Ok(ex.Message);
            }
        }
    }
}
