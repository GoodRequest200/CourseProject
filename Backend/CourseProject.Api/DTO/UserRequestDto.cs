namespace CourseProject.Api.DTO
{
    public record UserRequestDto(
        string FirstName,
        string LastName,
        string? Patronymic,
        string Email,
        int RoleId,
        string Password      // пароль приходит только тут
    );
}
