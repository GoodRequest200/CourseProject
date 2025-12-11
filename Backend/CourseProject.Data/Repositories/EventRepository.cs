using CourseProject.Core.Abstractions;
using CourseProject.Core.Models;
using CourseProject.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Data.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _db;

        public EventRepository(AppDbContext db)
        {
            _db = db;
        }

        private static string? NormalizeImageName(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;

            var trimmed = path.Trim();

            if (trimmed.Contains(':') || trimmed.Contains('\\'))
            {
                var file = trimmed.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                if (string.IsNullOrWhiteSpace(file)) return null;
                return file;
            }

            var normalized = trimmed.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
        }

        private static EventModel MapToModel(Event entity)
        {
            var departments = entity.Departments
                .Select(d => DepartmentModel.Create(d.Id, d.Name).Value);

            var comments = entity.Comments
                .Select(c => CommentModel.Create(
                    c.Id, c.EventId, c.UserId, c.Content, c.CreatedAt).Value);

            var normalizedImage = NormalizeImageName(entity.MainImage);

            return EventModel.Create(
                entity.Id,
                entity.Title,
                entity.Content,
                normalizedImage,
                entity.EventDate,
                entity.IsCompleted,
                departments,
                comments
            ).Value;
        }

        public async Task<EventModel?> GetByIdAsync(int id)
        {
            var e = await _db.Events
                .Include(x => x.Departments)
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (e is null)
                return null;

            return MapToModel(e);
        }

        public async Task<IEnumerable<EventModel>> GetAllAsync()
        {
            var events = await _db.Events
                .Include(x => x.Departments)
                .Include(x => x.Comments)
                .ToListAsync();

            return events.Select(MapToModel);
        }

        public async Task<EventModel> CreateAsync(EventModel model)
        {
            return await CreateAsync(model, Enumerable.Empty<int>());
        }

        public async Task<EventModel> CreateAsync(EventModel model, IEnumerable<int> departmentIds)
        {
            var normalizedImage = NormalizeImageName(model.MainImagePath);

            var entity = new Event
            {
                Title = model.Title,
                Content = model.Content,
                MainImage = normalizedImage,
                EventDate = model.EventDate,
                IsCompleted = model.IsCompleted
            };

            _db.Events.Add(entity);
            await _db.SaveChangesAsync();

            if (departmentIds != null && departmentIds.Any())
            {
                foreach (var depId in departmentIds)
                {
                    var depEntity = await _db.Departments.FindAsync(depId);
                    if (depEntity != null)
                    {
                        entity.Departments.Add(depEntity);
                    }
                }

                await _db.SaveChangesAsync();
            }

            await _db.Entry(entity).Collection(e => e.Departments).LoadAsync();
            await _db.Entry(entity).Collection(e => e.Comments).LoadAsync();

            return MapToModel(entity);
        }

        public async Task<EventModel?> UpdateAsync(EventModel model)
        {
            return await UpdateAsync(model, Enumerable.Empty<int>());
        }

        public async Task<EventModel?> UpdateAsync(EventModel model, IEnumerable<int> departmentIds)
        {
            var e = await _db.Events
                .Include(x => x.Departments)
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (e == null) return null;

            e.Title = model.Title;
            e.Content = model.Content;
            e.MainImage = NormalizeImageName(model.MainImagePath);
            e.EventDate = model.EventDate;
            e.IsCompleted = model.IsCompleted;

            e.Departments.Clear();

            if (departmentIds != null)
            {
                foreach (var depId in departmentIds)
                {
                    var depEntity = await _db.Departments.FindAsync(depId);
                    if (depEntity != null)
                        e.Departments.Add(depEntity);
                }
            }

            await _db.SaveChangesAsync();

            await _db.Entry(e).Collection(ev => ev.Departments).LoadAsync();
            await _db.Entry(e).Collection(ev => ev.Comments).LoadAsync();

            return MapToModel(e);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var e = await _db.Events.FindAsync(id);
            if (e == null) return false;

            _db.Events.Remove(e);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EventModel>> GetByDepartmentAsync(int departmentId)
        {
            var events = await _db.Events
                .Include(x => x.Departments)
                .Include(x => x.Comments)
                .Where(e => e.Departments.Any(d => d.Id == departmentId))
                .ToListAsync();

            return events.Select(MapToModel);
        }
    }
}
