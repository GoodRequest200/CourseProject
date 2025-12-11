using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject.Core.Models
{
    public class DepartmentModel
    {
        public int Id { get; }
        public string Name { get; }

        private DepartmentModel(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static Result<DepartmentModel> Create(int id, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure<DepartmentModel>("Department name is required.");

            var department = new DepartmentModel(id, name);

            return Result.Success(department);
        }
    }
}
