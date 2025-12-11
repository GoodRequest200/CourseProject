namespace CourseProject.Api.DTO
{
    public record UserResponseDto(
        int Id,
        string FirstName,
        string LastName,
        string? Patronymic,
        string Email,
        int RoleId
    );
}
