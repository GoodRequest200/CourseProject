using CourseProject.Api.DTO;
using CourseProject.Core.Abstractions;
using CourseProject.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordService _passwordService;

        public UsersController(IUserRepository repo, IPasswordService passwordService)
        {
            _repo = repo;
            _passwordService = passwordService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _repo.GetAllAsync();
            var dto = users.Select(u => new UserResponseDto(
                u.Id, u.FirstName, u.LastName, u.Patronymic, u.Email, u.RoleId));
            return Ok(dto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var u = await _repo.GetByIdAsync(id);
            if (u == null) return NotFound();

            var dto = new UserResponseDto(
                u.Id, u.FirstName, u.LastName, u.Patronymic, u.Email, u.RoleId);
            return Ok(dto);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] UserRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || !IsValidEmail(dto.Email))
                return BadRequest("Некорректный email");

            var existing = await _repo.GetByEmailAsync(dto.Email);
            if (existing != null)
                return Conflict("Пользователь с таким email уже существует");

            var hash = _passwordService.Hash(dto.Password);

            var modelResult = UserModel.Create(
                id: 0,
                dto.FirstName,
                dto.LastName,
                dto.Patronymic,
                dto.Email,
                dto.RoleId,
                hash);

            if (modelResult.IsFailure)
                return BadRequest(modelResult.Error);

            var created = await _repo.CreateAsync(modelResult.Value);
            var response = new UserResponseDto(
                created.Id, created.FirstName, created.LastName, created.Patronymic, created.Email, created.RoleId);
            return Ok(response);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UserRequestDto dto)
        {
            var hash = _passwordService.Hash(dto.Password);

            var modelResult = UserModel.Create(
                id,
                dto.FirstName,
                dto.LastName,
                dto.Patronymic,
                dto.Email,
                dto.RoleId,
                hash);

            if (modelResult.IsFailure)
                return BadRequest(modelResult.Error);

            var updated = await _repo.UpdateAsync(modelResult.Value);
            if (updated == null) return NotFound();

            var response = new UserResponseDto(
                updated.Id, updated.FirstName, updated.LastName, updated.Patronymic, updated.Email, updated.RoleId);
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _repo.DeleteAsync(id);
            return ok ? Ok() : NotFound();
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var u = await _repo.GetByEmailAsync(email);
            if (u == null) return NotFound();

            var dto = new UserResponseDto(
                u.Id, u.FirstName, u.LastName, u.Patronymic, u.Email, u.RoleId);
            return Ok(dto);
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address.Equals(email, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }

}
