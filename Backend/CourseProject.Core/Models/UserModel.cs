using CSharpFunctionalExtensions;

namespace CourseProject.Core.Models
{
    public class UserModel
    {
        public int Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string? Patronymic { get; }
        public string Email { get; }
        public int RoleId { get; }
        public string PasswordHash { get; }

        private UserModel(int id, string firstName, string lastName, string? patronymic, string email, int roleId, string passwordHash)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Patronymic = patronymic;
            Email = email;
            RoleId = roleId;
            PasswordHash = passwordHash;
        }

        public static Result<UserModel> Create(
            int id,
            string firstName,
            string lastName,
            string? patronymic,
            string email,
            int roleId,
            string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                return Result.Failure<UserModel>("First name is required.");

            if (string.IsNullOrWhiteSpace(lastName))
                return Result.Failure<UserModel>("Last name is required.");

            if (string.IsNullOrWhiteSpace(email))
                return Result.Failure<UserModel>("Email is required.");

            if (string.IsNullOrWhiteSpace(passwordHash))
                return Result.Failure<UserModel>("Password hash is required.");

            var user = new UserModel(id, firstName, lastName, patronymic, email, roleId, passwordHash);

            return Result.Success(user);
        }
    }
}
