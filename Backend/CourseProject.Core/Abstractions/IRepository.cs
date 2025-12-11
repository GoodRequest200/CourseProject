
namespace CourseProject.Core.Abstractions
{
    public interface IRepository<T>
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> CreateAsync(T item);
        Task<T?> UpdateAsync(T item);
        Task<bool> DeleteAsync(int id);
    }
}
