using System.ComponentModel.DataAnnotations;

namespace ToDoListWebAPI.Models.DTO;

public record ToDoListDto(
    int Id,
    string Title,
    ICollection<ToDoItemDto>? ToDoItems
);

public record CreateToDoListDto(
    [Required] [StringLength(50)] string Title
);

public record UpdateToDoListDto(
    int Id,
    [Required] [StringLength(50)] string Title
);