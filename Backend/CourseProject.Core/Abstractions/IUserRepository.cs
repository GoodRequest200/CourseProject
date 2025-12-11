using CourseProject.Core.Models;

namespace CourseProject.Core.Abstractions
{
    public interface IUserRepository : IRepository<UserModel>
    {
        Task<UserModel?> GetByEmailAsync(string email);
    }

}
