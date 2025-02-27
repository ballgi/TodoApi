using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;
using Microsoft.Extensions.Logging;

namespace TodoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoController : ControllerBase
{
    private readonly TodoDbContext _context;
    private readonly ILogger<TodoController> _logger;

    public TodoController(TodoDbContext context, ILogger<TodoController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Todo>>> GetAll()
    {
        _logger.LogInformation("正在獲取所有待辦事項");
        return await _context.Todos.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Todo>> GetById(int id)
    {
        _logger.LogInformation("正在查詢 ID 為 {Id} 的待辦事項", id);
        var todo = await _context.Todos.FindAsync(id);

        if (todo == null)
        {
            _logger.LogWarning("找不到 ID 為 {Id} 的待辦事項", id);
            return NotFound();
        }

        _logger.LogInformation("成功找到 ID 為 {Id} 的待辦事項：{Title}", id, todo.Title);
        return todo;
    }

    [HttpPost]
    public async Task<ActionResult<Todo>> Create(Todo todo)
    {
        todo.CreatedAt = DateTime.Now;
        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();

        _logger.LogInformation("已創建新的待辦事項：ID = {Id}, 標題 = {Title}", todo.Id, todo.Title);
        return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Todo todo)
    {
        _logger.LogInformation("正在更新 ID 為 {Id} 的待辦事項", id);
        
        var existingTodo = await _context.Todos.FindAsync(id);
        if (existingTodo == null)
        {
            _logger.LogWarning("找不到要更新的待辦事項，ID = {Id}", id);
            return NotFound();
        }

        existingTodo.Title = todo.Title;
        existingTodo.IsComplete = todo.IsComplete;
        existingTodo.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        
        _logger.LogInformation("已更新待辦事項：ID = {Id}, 新標題 = {Title}, 完成狀態 = {IsComplete}", 
            id, todo.Title, todo.IsComplete);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("正在刪除 ID 為 {Id} 的待辦事項", id);
        
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null)
        {
            _logger.LogWarning("找不到要刪除的待辦事項，ID = {Id}", id);
            return NotFound();
        }

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("已刪除待辦事項：ID = {Id}, 標題 = {Title}", id, todo.Title);
        return NoContent();
    }
} 