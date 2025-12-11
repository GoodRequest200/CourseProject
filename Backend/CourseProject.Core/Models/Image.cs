using CSharpFunctionalExtensions;

namespace CourseProject.Core.Models
{
    // Currently not used by events; kept for compatibility with image repository.
    public class Image
    {
        public int EventId { get; set; }
        public string FileName { get; set; } = string.Empty;

        private Image(string fileName)
        {
            FileName = fileName;
        }

        public static Result<Image> Create(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return Result.Failure<Image>("File path is required.");

            var image = new Image(filePath);

            return Result.Success(image);
        }
    }
}
