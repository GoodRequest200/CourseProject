using CourseProject.Api.DTO;
using CourseProject.Core.Abstractions;
using CourseProject.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Api.Controllers
{
    [ApiController]
    [Route("api/roles")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRoleRepository _repo;

        public RolesController(IRoleRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _repo.GetAllAsync();
            var dto = roles.Select(r => new RoleResponseDto(r.Id, r.Name));
            return Ok(dto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var r = await _repo.GetByIdAsync(id);
            if (r == null) return NotFound();
            return Ok(new RoleResponseDto(r.Id, r.Name));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleRequestDto dto)
        {
            var modelResult = RoleModel.Create(0, dto.Name);
            if (modelResult.IsFailure) return BadRequest(modelResult.Error);

            var created = await _repo.CreateAsync(modelResult.Value);
            return Ok(new RoleResponseDto(created.Id, created.Name));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RoleRequestDto dto)
        {
            var modelResult = RoleModel.Create(id, dto.Name);
            if (modelResult.IsFailure) return BadRequest(modelResult.Error);

            var updated = await _repo.UpdateAsync(modelResult.Value);
            return Ok(new RoleResponseDto(updated.Id, updated.Name));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _repo.DeleteAsync(id);
            return ok ? Ok() : NotFound();
        }
    }

}
