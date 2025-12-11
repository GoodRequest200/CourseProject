using CourseProject.Core.Abstractions;
using CourseProject.Core.Models;
using CourseProject.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<UserModel?> GetByIdAsync(int id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u is null) return null;

            return UserModel.Create(
                u.Id, u.FirstName, u.LastName, u.Patronymic, u.Email, u.RoleId, u.PasswordHash
            ).Value;
        }

        public async Task<IEnumerable<UserModel>> GetAllAsync()
        {
            var list = await _db.Users.ToListAsync();

            return list.Select(u => UserModel.Create(
                u.Id, u.FirstName, u.LastName, u.Patronymic, u.Email, u.RoleId, u.PasswordHash
            ).Value);
        }

        public async Task<UserModel> CreateAsync(UserModel model)
        {
            var entity = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Patronymic = model.Patronymic,
                Email = model.Email,
                RoleId = model.RoleId,
                PasswordHash = model.PasswordHash
            };

            _db.Users.Add(entity);
            await _db.SaveChangesAsync();

            return UserModel.Create(
                entity.Id, entity.FirstName, entity.LastName, entity.Patronymic, entity.Email, entity.RoleId, entity.PasswordHash
            ).Value;
        }

        public async Task<UserModel?> UpdateAsync(UserModel model)
        {
            var u = await _db.Users.FindAsync(model.Id);
            if (u == null) return null;

            u.FirstName = model.FirstName;
            u.LastName = model.LastName;
            u.Patronymic = model.Patronymic;
            u.Email = model.Email;
            u.RoleId = model.RoleId;
            u.PasswordHash = model.PasswordHash;

            await _db.SaveChangesAsync();
            return UserModel.Create(
                u.Id, u.FirstName, u.LastName, u.Patronymic, u.Email, u.RoleId, u.PasswordHash
            ).Value;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return false;

            _db.Users.Remove(u);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<UserModel?> GetByEmailAsync(string email)
        {
            var u = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (u == null) return null;

            return UserModel.Create(
                u.Id, u.FirstName, u.LastName, u.Patronymic, u.Email, u.RoleId, u.PasswordHash
            ).Value;
        }
    }

}
