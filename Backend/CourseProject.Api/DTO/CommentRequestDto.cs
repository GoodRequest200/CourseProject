namespace CourseProject.Api.DTO
{
    public record CommentRequestDto(
        int EventId,
        int UserId,
        string Content
    );
}
