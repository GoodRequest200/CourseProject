using CourseProject.Core.Abstractions;
using CourseProject.Core.Models;
using CourseProject.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Data.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _db;

        public CommentRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<CommentModel?> GetByIdAsync(int id)
        {
            var c = await _db.Comments.FindAsync(id);
            if (c is null) return null;

            return CommentModel.Create(
                c.Id, c.EventId, c.UserId, c.Content, c.CreatedAt
            ).Value;
        }

        public async Task<IEnumerable<CommentModel>> GetAllAsync()
        {
            var list = await _db.Comments.ToListAsync();

            return list.Select(c => CommentModel.Create(
                c.Id, c.EventId, c.UserId, c.Content, c.CreatedAt
            ).Value);
        }

        public async Task<CommentModel> CreateAsync(CommentModel model)
        {
            var entity = new Comment
            {
                EventId = model.EventId,
                UserId = model.UserId,
                Content = model.Content
            };

            _db.Comments.Add(entity);
            await _db.SaveChangesAsync();

            return CommentModel.Create(
                entity.Id, entity.EventId, entity.UserId, entity.Content, entity.CreatedAt
            ).Value;
        }

        public async Task<CommentModel?> UpdateAsync(CommentModel model)
        {
            var c = await _db.Comments.FindAsync(model.Id);
            if (c == null) return null;

            c.Content = model.Content;

            await _db.SaveChangesAsync();
            return CommentModel.Create(
                c.Id, c.EventId, c.UserId, c.Content, c.CreatedAt
            ).Value;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var c = await _db.Comments.FindAsync(id);
            if (c == null) return false;

            _db.Comments.Remove(c);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CommentModel>> GetByEventIdAsync(int eventId)
        {
            var list = await _db.Comments
                .Where(c => c.EventId == eventId)
                .ToListAsync();

            return list.Select(c => CommentModel.Create(
                c.Id, c.EventId, c.UserId, c.Content, c.CreatedAt
            ).Value);
        }
    }

}
