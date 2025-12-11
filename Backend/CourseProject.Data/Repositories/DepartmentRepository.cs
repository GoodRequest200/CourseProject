using CourseProject.Core.Abstractions;
using CourseProject.Core.Models;
using CourseProject.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Data.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _context;

        public DepartmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DepartmentModel?> GetByIdAsync(int id)
        {
            var entity = await _context.Departments.FindAsync(id);
            if (entity == null) return null;

            return DepartmentModel.Create(entity.Id, entity.Name).Value;
        }

        public async Task<IEnumerable<DepartmentModel>> GetAllAsync()
        {
            var list = await _context.Departments.ToListAsync();

            return list.Select(d => DepartmentModel.Create(d.Id, d.Name).Value);
        }

        public async Task<DepartmentModel> CreateAsync(DepartmentModel model)
        {
            var entity = new Department { Name = model.Name };

            _context.Departments.Add(entity);
            await _context.SaveChangesAsync();

            return DepartmentModel.Create(entity.Id, entity.Name).Value;
        }

        public async Task<DepartmentModel> UpdateAsync(DepartmentModel model)
        {
            var entity = await _context.Departments.FindAsync(model.Id);
            if (entity == null) throw new Exception("Department not found");

            entity.Name = model.Name;
            await _context.SaveChangesAsync();

            return DepartmentModel.Create(entity.Id, entity.Name).Value;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Departments.FindAsync(id);
            if (entity == null) return false;

            _context.Departments.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
