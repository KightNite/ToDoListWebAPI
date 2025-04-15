using System.ComponentModel.DataAnnotations;

namespace ToDoListWebAPI.Models.DTO;

public record ToDoItemDto(
    int Id,
    string Title,
    string? Description,
    bool IsDone,
    DateTime? DoneDate,
    int? ToDoListId
);
    
public record CreateToDoItemDto(
    [Required] [StringLength(50, MinimumLength = 3)] string Title,
    [StringLength(100)] string? Description,
    int? ToDoListId
);

public record UpdateToDoItemDto(
    int Id,
    [Required] [StringLength(50, MinimumLength = 3)] string Title,
    [StringLength(100)] string? Description,
    bool IsDone,
    int? ToDoListId
);