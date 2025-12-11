using CSharpFunctionalExtensions;

namespace CourseProject.Core.Models
{
    public class RoleModel
    {
        public int Id { get; }
        public string Name { get; }

        private RoleModel(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static Result<RoleModel> Create(int id, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure<RoleModel>("Role name is required.");

            var role = new RoleModel(id, name);

            return Result.Success(role);
        }
    }
}
