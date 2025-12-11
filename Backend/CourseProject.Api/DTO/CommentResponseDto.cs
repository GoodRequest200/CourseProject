namespace CourseProject.Api.DTO
{
    public record CommentResponseDto(
        int Id,
        int EventId,
        int UserId,
        string Content,
        DateTime CreatedAt
    );
}
