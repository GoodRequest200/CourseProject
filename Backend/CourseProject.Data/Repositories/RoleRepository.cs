using CourseProject.Core.Abstractions;
using CourseProject.Core.Models;
using CourseProject.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Data.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;
        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RoleModel?> GetByIdAsync(int id)
        {
            var entity = await _context.Roles.FindAsync(id);
            if (entity == null) return null;

            return RoleModel.Create(entity.Id, entity.Name).Value;
        }

        public async Task<IEnumerable<RoleModel>> GetAllAsync()
        {
            var list = await _context.Roles.ToListAsync();
            return list.Select(r => RoleModel.Create(r.Id, r.Name).Value);
        }

        public async Task<RoleModel> CreateAsync(RoleModel model)
        {
            var entity = new Role { Name = model.Name };
            _context.Roles.Add(entity);
            await _context.SaveChangesAsync();
            return RoleModel.Create(entity.Id, entity.Name).Value;
        }

        public async Task<RoleModel> UpdateAsync(RoleModel model)
        {
            var entity = await _context.Roles.FindAsync(model.Id);
            if (entity == null) throw new Exception("Role not found");

            entity.Name = model.Name;
            await _context.SaveChangesAsync();
            return RoleModel.Create(entity.Id, entity.Name).Value;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Roles.FindAsync(id);
            if (entity == null) return false;

            _context.Roles.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
