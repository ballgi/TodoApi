using TodoApi.Interfaces;
using TodoApi.Models;

namespace TodoApi.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _todoRepository;
    private readonly ILogger<TodoService> _logger;

    public TodoService(ITodoRepository todoRepository, ILogger<TodoService> logger)
    {
        _todoRepository = todoRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Todo>> GetAllTodosAsync()
    {
        _logger.LogInformation("正在獲取所有待辦事項");
        return await _todoRepository.GetAllAsync();
    }

    public async Task<Todo?> GetTodoByIdAsync(int id)
    {
        _logger.LogInformation("正在查詢 ID 為 {Id} 的待辦事項", id);
        var todo = await _todoRepository.GetByIdAsync(id);
        
        if (todo == null)
        {
            _logger.LogWarning("找不到 ID 為 {Id} 的待辦事項", id);
            return null;
        }
        
        _logger.LogInformation("成功找到 ID 為 {Id} 的待辦事項：{Context}", id, todo.Context);
        return todo;
    }

    public async Task<Todo> CreateTodoAsync(Todo todo)
    {
        todo.CreatedAt = DateTime.Now;
        var createdTodo = await _todoRepository.CreateAsync(todo);
        
        _logger.LogInformation("已創建新的待辦事項：ID = {Id}, 內容 = {Context}", 
            createdTodo.Id, createdTodo.Context);
            
        return createdTodo;
    }

    public async Task UpdateTodoAsync(int id, Todo todo)
    {
        _logger.LogInformation("正在更新 ID 為 {Id} 的待辦事項", id);
        
        var existingTodo = await _todoRepository.GetByIdAsync(id);
        if (existingTodo == null)
        {
            _logger.LogWarning("找不到要更新的待辦事項，ID = {Id}", id);
            throw new KeyNotFoundException($"找不到ID為{id}的待辦事項");
        }

        existingTodo.Context = todo.Context;
        existingTodo.IsComplete = todo.IsComplete;
        existingTodo.UpdatedAt = DateTime.Now;

        await _todoRepository.UpdateAsync(existingTodo);
        
        _logger.LogInformation("已更新待辦事項：ID = {Id}, 新內容 = {Context}, 完成狀態 = {IsComplete}", 
            id, todo.Context, todo.IsComplete);
    }

    public async Task<bool> DeleteTodoAsync(int id)
    {
        _logger.LogInformation("正在刪除 ID 為 {Id} 的待辦事項", id);
        
        var result = await _todoRepository.DeleteAsync(id);
        if (!result)
        {
            _logger.LogWarning("找不到要刪除的待辦事項，ID = {Id}", id);
            return false;
        }
        
        _logger.LogInformation("已刪除待辦事項：ID = {Id}", id);
        return true;
    }
} 