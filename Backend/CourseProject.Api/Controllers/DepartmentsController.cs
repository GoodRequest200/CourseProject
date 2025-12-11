using CourseProject.Api.DTO;
using CourseProject.Core.Abstractions;
using CourseProject.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Api.Controllers
{
    [ApiController]
    [Route("api/departments")]
    //[Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentRepository _repo;

        public DepartmentsController(IDepartmentRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _repo.GetAllAsync();
            var dto = items.Select(d => new DepartmentResponseDto(d.Id, d.Name));
            return Ok(dto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(new DepartmentResponseDto(item.Id, item.Name));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DepartmentRequestDto dto)
        {
            var modelResult = DepartmentModel.Create(0, dto.Name);
            if (modelResult.IsFailure) return BadRequest(modelResult.Error);

            var created = await _repo.CreateAsync(modelResult.Value);
            return Ok(new DepartmentResponseDto(created.Id, created.Name));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] DepartmentRequestDto dto)
        {
            var modelResult = DepartmentModel.Create(id, dto.Name);
            if (modelResult.IsFailure) return BadRequest(modelResult.Error);

            var updated = await _repo.UpdateAsync(modelResult.Value);
            return Ok(new DepartmentResponseDto(updated.Id, updated.Name));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _repo.DeleteAsync(id);
            return ok ? Ok() : NotFound();
        }
    }

}
