using CourseProject.Core.Models;

namespace CourseProject.Core.Abstractions
{
    public interface IEventRepository : IRepository<EventModel>
    {
        Task<IEnumerable<EventModel>> GetByDepartmentAsync(int departmentId);
        Task<EventModel> CreateAsync(EventModel item, IEnumerable<int> departmentIds);
        Task<EventModel?> UpdateAsync(EventModel item, IEnumerable<int> departmentIds);
    }
}
