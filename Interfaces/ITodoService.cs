using TodoApi.Models;

namespace TodoApi.Interfaces;

public interface ITodoService
{
    Task<IEnumerable<Todo>> GetAllTodosAsync();
    Task<Todo?> GetTodoByIdAsync(int id);
    Task<Todo> CreateTodoAsync(Todo todo);
    Task UpdateTodoAsync(int id, Todo todo);
    Task<bool> DeleteTodoAsync(int id);
} 