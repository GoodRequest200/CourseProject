namespace CourseProject.Api.DTO
{
    public record EventResponseDto(
        int Id,
        string Title,
        string? Content,
        string? MainImage,
        DateOnly? EventDate,
        bool IsCompleted,
        IEnumerable<DepartmentResponseDto> Departments
    );
}
