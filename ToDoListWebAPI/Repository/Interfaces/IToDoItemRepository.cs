using ToDoListWebAPI.Models;
using ToDoListWebAPI.Models.DTO;

namespace ToDoListWebAPI.Repository.Interfaces;

public interface IToDoItemRepository: IEntityRepository<ToDoItemDto, ToDoItem, int>
{
    Task<IEnumerable<ToDoItemDto>> GetHistoryAsync(bool noTracking = true);
    Task<ToDoItemDto?> MarkDoneAsync(int id);
}