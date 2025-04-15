using ToDoListWebAPI.Models;
using ToDoListWebAPI.Models.DTO;

namespace ToDoListWebAPI.Repository.Interfaces;

public interface IToDoListRepository: IEntityRepository<ToDoListDto, ToDoList, int>
{
    
}