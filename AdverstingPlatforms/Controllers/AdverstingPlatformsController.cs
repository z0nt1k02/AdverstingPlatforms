using AdverstingPlatforms.Dtos;
using AdverstingPlatforms.Interfaces;
using AdverstingPlatforms.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AdverstingPlatforms.Controllers
{
    [Route("api/platforms")]
    [ApiController]
    public class AdverstingPlatformsController : ControllerBase
    {
        private readonly IAdverstingPlatformsService _service;
        public AdverstingPlatformsController(IAdverstingPlatformsService service)
        {
            _service = service;
        }

        [HttpGet]        
        public async Task<IActionResult> UpdatePlatforms()
        {
            try
            {
                var result = await _service.UpdatePlatformsAsync();
                return Ok("Данные успешно загружены");
            }
            catch(Exception ex)
            {
                return Problem(statusCode: 500, detail: ex.Message,title:"Произошла ошибка при загрузке данных с файла");
            }            
        }

        [HttpPost]
        public IActionResult GetPlatforms(PathDto pathDto)
        {
            try
            {
                var result = _service.FindPlatforms(pathDto.path);
                return Ok(result);
            }
            catch (ArgumentNullException)
            {
                return NotFound($"Платформы по запросу {pathDto.path} не найдены");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            

        }
    }
}
