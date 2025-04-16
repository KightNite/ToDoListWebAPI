using Microsoft.IdentityModel.Tokens;

namespace ToDoListWebAPI.Models.DTO;

public static class DtoConverter
{
    public static ToDoItemDto ToToDoItemDto(ToDoItem toDoItem)
    {
        return new ToDoItemDto(
            toDoItem.Id,
            toDoItem.Title,
            toDoItem.Description,
            toDoItem.IsDone,
            toDoItem.DoneDate,
            toDoItem.ToDoListId
        );
    }
    
    public static ToDoListDto ToToDoListDto(ToDoList toDoList)
    {
        ICollection<ToDoItemDto>? toDoItemDtos = null;
        if (!toDoList.ToDoItems.IsNullOrEmpty())
        {
            toDoItemDtos = toDoList.ToDoItems!.Select(ToToDoItemDto).ToList();
        }
        
        return new ToDoListDto(
            toDoList.Id,
            toDoList.Title,
            toDoItemDtos
        );
    }
}