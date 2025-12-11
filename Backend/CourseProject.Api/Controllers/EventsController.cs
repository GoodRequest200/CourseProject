using CourseProject.Api.DTO;
using CourseProject.Core.Abstractions;
using CourseProject.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CourseProject.Api.Services;

namespace CourseProject.Api.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private const string DefaultImage = "/images/default.jpg";
        private readonly IEventRepository _repo;
        private readonly IImageStorageService _imageStorage;

        public EventsController(IEventRepository repo, IImageStorageService imageStorage)
        {
            _repo = repo;
            _imageStorage = imageStorage;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var events = await _repo.GetAllAsync();

            var dto = events.Select(e =>
                new EventResponseDto(
                    e.Id,
                    e.Title,
                    e.Content,
                    e.MainImagePath ?? DefaultImage,
                    e.EventDate,
                    e.IsCompleted,
                    e.Departments.Select(d => new DepartmentResponseDto(d.Id, d.Name))
                )
            );

            return Ok(dto);
        }

        // GET BY ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return NotFound();

            var dto = new EventResponseDto(
                e.Id,
                e.Title,
                e.Content,
                e.MainImagePath ?? DefaultImage,
                e.EventDate,
                e.IsCompleted,
                e.Departments.Select(d => new DepartmentResponseDto(d.Id, d.Name))
            );

            return Ok(dto);
        }

        // CREATE
        [HttpPost]
        //[Authorize(Roles = "administrator")]
        public async Task<IActionResult> Create([FromForm] EventRequestDto dto)
        {
            string? imagePath = null;

            if (dto.MainImage != null)
            {
                imagePath = await _imageStorage.SaveAsync(dto.MainImage);
            }

            var departmentIds = dto.DepartmentIds ?? Enumerable.Empty<int>();

            var modelResult = EventModel.Create(
                id: 0,
                dto.Title,
                dto.Content,
                imagePath,
                dto.EventDate,
                dto.IsCompleted
            );

            if (modelResult.IsFailure)
                return BadRequest(modelResult.Error);

            var created = await _repo.CreateAsync(modelResult.Value, departmentIds);

            var response = new EventResponseDto(
                created.Id,
                created.Title,
                created.Content,
                created.MainImagePath ?? DefaultImage,
                created.EventDate,
                created.IsCompleted,
                created.Departments.Select(d => new DepartmentResponseDto(d.Id, d.Name))
            );

            return Ok(response);
        }

        // UPDATE
        [HttpPut("{id:int}")]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> Update(int id, [FromForm] EventRequestDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            var imagePath = existing.MainImagePath;

            if (dto.MainImage != null)
            {
                imagePath = await _imageStorage.SaveAsync(dto.MainImage);
            }

            var departmentIds = dto.DepartmentIds ?? Enumerable.Empty<int>();

            var modelResult = EventModel.Create(
                id,
                dto.Title,
                dto.Content,
                imagePath,
                dto.EventDate,
                dto.IsCompleted
            );

            if (modelResult.IsFailure)
                return BadRequest(modelResult.Error);

            var updated = await _repo.UpdateAsync(modelResult.Value, departmentIds);
            if (updated == null) return NotFound();

            var response = new EventResponseDto(
                updated.Id,
                updated.Title,
                updated.Content,
                updated.MainImagePath ?? DefaultImage,
                updated.EventDate,
                updated.IsCompleted,
                updated.Departments.Select(d => new DepartmentResponseDto(d.Id, d.Name))
            );

            return Ok(response);
        }

        // DELETE
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _repo.DeleteAsync(id);
            return ok ? Ok() : NotFound();
        }

        // FILTER BY DEPARTMENT
        [HttpGet("department/{deptId:int}")]
        public async Task<IActionResult> GetByDepartment(int deptId)
        {
            var events = await _repo.GetByDepartmentAsync(deptId);

            var dto = events.Select(e =>
                new EventResponseDto(
                    e.Id,
                    e.Title,
                    e.Content,
                    e.MainImagePath ?? DefaultImage,
                    e.EventDate,
                    e.IsCompleted,
                    e.Departments.Select(d => new DepartmentResponseDto(d.Id, d.Name))
                )
            );

            return Ok(dto);
        }
    }


}
