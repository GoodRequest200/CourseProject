using CourseProject.Api.DTO;
using CourseProject.Core.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Api.Controllers
{
    [ApiController]
    [Route("api/images")]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository _repo;

        public ImagesController(IImageRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var imgs = await _repo.GetAllAsync();
            //var dto = imgs.Select(i => new ImageResponseDto(i.FileName));
            return Ok(/*dto*/);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var img = await _repo.GetByIdAsync(id);
            return img == null ? NotFound() : Ok(/*new ImageResponseDto(img.FileName)*/);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _repo.DeleteAsync(id);
            return ok ? Ok() : NotFound();
        }
    }

}
