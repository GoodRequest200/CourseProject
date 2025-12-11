using CourseProject.Api.DTO;
using CourseProject.Api.Services;
using CourseProject.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _users;
        private readonly IRoleRepository _roles;
        private readonly IPasswordService _passwords;
        private readonly ITokenService _tokens;

        public AuthController(IUserRepository users, IRoleRepository roles, IPasswordService passwords, ITokenService tokens)
        {
            _users = users;
            _roles = roles;
            _passwords = passwords;
            _tokens = tokens;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequestDto dto)
        {
            var user = await _users.GetByEmailAsync(dto.Email);
            if (user == null) return Unauthorized("Invalid credentials");

            if (!_passwords.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            var role = await _roles.GetByIdAsync(user.RoleId);
            var roleName = role?.Name ?? user.RoleId.ToString();

            var token = _tokens.Generate(user, roleName);
            return Ok(new AuthResponseDto(token));
        }
    }
}
