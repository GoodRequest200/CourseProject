using Microsoft.AspNetCore.Http;

namespace CourseProject.Api.DTO
{
    public record EventRequestDto(
        string Title,
        string? Content,
        DateOnly? EventDate,
        bool IsCompleted,
        IEnumerable<int>? DepartmentIds,
        IFormFile? MainImage
    );
}
