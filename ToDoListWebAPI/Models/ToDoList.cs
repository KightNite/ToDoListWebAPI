using System.ComponentModel.DataAnnotations;

namespace ToDoListWebAPI.Models;

public class ToDoList
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Title { get; set; }
    public ICollection<ToDoItem>? ToDoItems { get; } = new List<ToDoItem>();
}