using CourseProject.Core.Models;

namespace CourseProject.Api.Services
{
    public interface ITokenService
    {
        string Generate(UserModel user, string roleName);
    }
}
