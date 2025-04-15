using System.ComponentModel.DataAnnotations;

namespace ToDoListWebAPI.Models;

public class ToDoItem
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Title { get; set; }
    
    [StringLength(100)]
    public string? Description { get; set; }
    public bool IsDone { get; set; }
    
    public DateTime? DoneDate { get; set; }
    
    public int? ToDoListId { get; set; }
    public ToDoList? ToDoList { get; set; }
}