using Microsoft.AspNetCore.Http;

namespace CourseProject.Api.Services
{
    public interface IImageStorageService
    {
        Task<string> SaveAsync(IFormFile file);
        Task DeleteAsync(string path);
    }
}
