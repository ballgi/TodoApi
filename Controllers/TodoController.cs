using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using Microsoft.Extensions.Logging;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private static List<Todo> _todos = new List<Todo>();
    private static int _nextId = 1;
    private readonly ILogger<TodoController> _logger;

    public TodoController(ILogger<TodoController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Todo>> GetAll()
    {
        _logger.LogInformation("正在獲取所有待辦事項，目前共有 {Count} 項", _todos.Count);
        return _todos;
    }

    [HttpGet("{id}")]
    public ActionResult<Todo> GetById(int id)
    {
        _logger.LogInformation("正在查詢 ID 為 {Id} 的待辦事項", id);
        var todo = _todos.FirstOrDefault(t => t.Id == id);
        if (todo == null)
        {
            _logger.LogWarning("找不到 ID 為 {Id} 的待辦事項", id);
            return NotFound();
        }
        _logger.LogInformation("成功找到 ID 為 {Id} 的待辦事項：{Title}", id, todo.Title);
        return todo;
    }

    [HttpPost]
    public ActionResult<Todo> Create(Todo todo)
    {
        todo.Id = _nextId++;
        _todos.Add(todo);
        _logger.LogInformation("已創建新的待辦事項：ID = {Id}, 標題 = {Title}", todo.Id, todo.Title);
        return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Todo todo)
    {
        _logger.LogInformation("正在更新 ID 為 {Id} 的待辦事項", id);
        var existingTodo = _todos.FirstOrDefault(t => t.Id == id);
        if (existingTodo == null)
        {
            _logger.LogWarning("找不到要更新的待辦事項，ID = {Id}", id);
            return NotFound();
        }

        existingTodo.Title = todo.Title;
        existingTodo.IsComplete = todo.IsComplete;
        _logger.LogInformation("已更新待辦事項：ID = {Id}, 新標題 = {Title}, 完成狀態 = {IsComplete}", 
            id, todo.Title, todo.IsComplete);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _logger.LogInformation("正在刪除 ID 為 {Id} 的待辦事項", id);
        var todo = _todos.FirstOrDefault(t => t.Id == id);
        if (todo == null)
        {
            _logger.LogWarning("找不到要刪除的待辦事項，ID = {Id}", id);
            return NotFound();
        }

        _todos.Remove(todo);
        _logger.LogInformation("已刪除待辦事項：ID = {Id}, 標題 = {Title}", id, todo.Title);
        return NoContent();
    }
} 