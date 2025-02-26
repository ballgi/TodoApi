using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models;

public class Todo
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    
    public bool IsComplete { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
} 