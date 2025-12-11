using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject.Core.Models
{
    public class CommentModel
    {
        public int Id { get; }
        public int EventId { get; }
        public int UserId { get; }
        public string Content { get; }
        public DateTime CreatedAt { get; }

        private CommentModel(int id, int eventId, int userId, string content, DateTime createdAt)
        {
            Id = id;
            EventId = eventId;
            UserId = userId;
            Content = content;
            CreatedAt = createdAt;
        }

        public static Result<CommentModel> Create(
            int id,
            int eventId,
            int userId,
            string content,
            DateTime? createdAt = null)
        {
            if (string.IsNullOrWhiteSpace(content))
                return Result.Failure<CommentModel>("Content cannot be empty.");

            var comment = new CommentModel(id, eventId, userId, content, createdAt ?? DateTime.UtcNow);

            return Result.Success(comment);
        }
    }
}
