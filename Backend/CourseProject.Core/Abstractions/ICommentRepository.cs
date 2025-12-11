using CourseProject.Core.Models;

namespace CourseProject.Core.Abstractions
{
    public interface ICommentRepository : IRepository<CommentModel>
    {
        Task<IEnumerable<CommentModel>> GetByEventIdAsync(int eventId);
    }
}
