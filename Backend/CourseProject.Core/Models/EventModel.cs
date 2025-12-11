using CSharpFunctionalExtensions;

namespace CourseProject.Core.Models
{
    public class EventModel
    {
        public int Id { get; }
        public string Title { get; }
        public string? Content { get; }
        public string? MainImagePath { get; }
        public DateOnly? EventDate { get; }
        public bool IsCompleted { get; }

        public IReadOnlyList<DepartmentModel> Departments { get; }
        public IReadOnlyList<CommentModel> Comments { get; }

        private EventModel(
            int id,
            string title,
            string? content,
            string? mainImagePath,
            DateOnly? eventDate,
            bool isCompleted,
            IReadOnlyList<DepartmentModel> departments,
            IReadOnlyList<CommentModel> comments)
        {
            Id = id;
            Title = title;
            Content = content;
            MainImagePath = mainImagePath;
            EventDate = eventDate;
            IsCompleted = isCompleted;
            Departments = departments;
            Comments = comments;
        }

        public static Result<EventModel> Create(
            int id,
            string title,
            string? content,
            string? mainImagePath,
            DateOnly? eventDate,
            bool isCompleted,
            IEnumerable<DepartmentModel>? departments = null,
            IEnumerable<CommentModel>? comments = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                return Result.Failure<EventModel>("Title is required");

            var ev = new EventModel(
                id,
                title,
                content,
                mainImagePath,
                eventDate,
                isCompleted,
                (departments ?? Enumerable.Empty<DepartmentModel>()).ToList(),
                (comments ?? Enumerable.Empty<CommentModel>()).ToList());

            return Result.Success(ev);
        }
    }


}
