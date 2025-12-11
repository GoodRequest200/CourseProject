namespace CourseProject.Api.DTO
{
    public record ImageResponseDto(
        int Id,
        string FilePath,
        DateTime UploadedAt
    );
}
