using TodoApi.Models;

namespace TodoApi.Interfaces;

public interface ITodoRepository
{
    Task<IEnumerable<Todo>> GetAllAsync();
    Task<Todo?> GetByIdAsync(int id);
    Task<Todo> CreateAsync(Todo todo);
    Task UpdateAsync(Todo todo);
    Task<bool> DeleteAsync(int id);
    Task<bool> TodoExistsAsync(int id);
} 