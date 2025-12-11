using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace CourseProject.Api.Services
{
    public class ImageStorageService : IImageStorageService
    {
        private readonly string _root;
        private readonly string _imagesFolder;

        public ImageStorageService(IWebHostEnvironment env)
        {
            // keep images in solution-level folder: <ContentRoot>/images
            _root = env.ContentRootPath;
            _imagesFolder = Path.Combine(_root, "images");
            Directory.CreateDirectory(_imagesFolder);
        }

        public async Task<string> SaveAsync(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(_imagesFolder, fileName);

            using var stream = File.Create(fullPath);
            await file.CopyToAsync(stream);

            // return only file name; API will expose /images path
            return fileName;
        }

        public Task DeleteAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return Task.CompletedTask;

            var file = Path.GetFileName(path);
            var fullPath = Path.Combine(_imagesFolder, file);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }
    }
}
