using Microsoft.EntityFrameworkCore;
using ToDoListWebAPI.Models;
using ToDoListWebAPI.Models.DTO;
using ToDoListWebAPI.Repository.Interfaces;

namespace ToDoListWebAPI.Repository.Services;

public class ToDoItemRepository: IToDoItemRepository
{
    protected readonly ToDoContext _context;
    protected readonly DbSet<ToDoItem> RepoDbSet;
    
    public ToDoItemRepository(ToDoContext toDoContext)
    {
        _context = toDoContext;
        RepoDbSet = _context.Set<ToDoItem>();
    }
    
    protected virtual IQueryable<ToDoItem> CreateQuery(bool noTracking = true)
    {
        var query = RepoDbSet.AsQueryable();
        if (noTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }

    
    public ToDoItemDto Add(ToDoItemDto entity)
    {
        return DtoConverter.ToToDoItemDto(RepoDbSet.Add(
            new ToDoItem {
                Title = entity.Title,
                Description = entity.Description,
                IsDone = false,
                ToDoListId = entity.ToDoListId
            }
            ).Entity
        );
    }

    public ToDoItemDto? Update(ToDoItemDto entity)
    {
        var result = CreateQuery(false).FirstOrDefault(item => item.Id == entity.Id);
        if (result != null)
        {
            result.Title = entity.Title;
            result.Description = entity.Description;
            result.IsDone = entity.IsDone;
            result.ToDoListId = entity.ToDoListId;

            RepoDbSet.Entry(result).State = EntityState.Modified;
            _context.SaveChanges();

            return DtoConverter.ToToDoItemDto(result);
        }

        return null;
    }

    public ToDoItemDto Remove(ToDoItem entity)
    {
        var toDoItem = RepoDbSet.Remove(entity).Entity;
        _context.SaveChanges();
        return DtoConverter.ToToDoItemDto(toDoItem);
    }

    public ToDoItemDto Remove(int id)
    {
        var result = CreateQuery().FirstOrDefault(item => item.Id == id);
        if (result == null)
        {
            throw new NullReferenceException($"Entity {nameof(ToDoItem)} with id {id} was not found");
        }
        return Remove(result);
    }

    public ToDoItemDto? FirstOrDefault(int id, bool noTracking = true)
    {
        var result = CreateQuery(noTracking).FirstOrDefault(item => item.Id == id);
        if (result == null) return null;
        
        return DtoConverter.ToToDoItemDto(result);
    }

    public IEnumerable<ToDoItemDto> GetAll(bool noTracking = true)
    {
        return CreateQuery(noTracking).Select(item => DtoConverter.ToToDoItemDto(item)).ToList();
    }

    public bool Exists(int id)
    {
        return RepoDbSet.Any(e => e.Id == id);
    }

    public async Task<ToDoItemDto> AddAsync(ToDoItemDto entity)
    {
        if (entity.ToDoListId != null && !await _context.TodoLists.AnyAsync(list => list.Id == entity.ToDoListId))
        {
            throw new NullReferenceException("No ToDo List with that id found.");
        }

        var result = (await RepoDbSet.AddAsync(
            new ToDoItem
            {
                Title = entity.Title,
                Description = entity.Description,
                IsDone = false,
                ToDoListId = entity.ToDoListId
            }
        )).Entity;
        await _context.SaveChangesAsync();
        return DtoConverter.ToToDoItemDto(result);
    }
    
    public async Task<ToDoItemDto?> UpdateAsync(ToDoItemDto entity)
    {
        if (entity.ToDoListId != null && !await _context.TodoLists.AnyAsync(list => list.Id == entity.ToDoListId))
        {
            throw new NullReferenceException("No ToDo List with that id found.");
        }
        
        var result = await CreateQuery(false).FirstOrDefaultAsync(item => item.Id == entity.Id);
        if (result != null)
        {
            result.Title = entity.Title;
            result.Description = entity.Description;
            if (entity.IsDone != result.IsDone && entity.IsDone)
            {
                result.DoneDate = DateTime.Now;
            }
            result.IsDone = entity.IsDone;
            result.ToDoListId = entity.ToDoListId;

            RepoDbSet.Entry(result).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return DtoConverter.ToToDoItemDto(result);
        }

        return null;
    }
    
    public async Task<ToDoItemDto?> MarkDoneAsync(int id)
    {
        var result = await CreateQuery(false).FirstOrDefaultAsync(item => item.Id == id);
        if (result == null) return null;
        
        if (result.IsDone) return DtoConverter.ToToDoItemDto(result);
        
        result.DoneDate = DateTime.Now;
        result.IsDone = true;

        await _context.SaveChangesAsync();

        return DtoConverter.ToToDoItemDto(result);

    }
    
    public async Task<ToDoItemDto?> FirstOrDefaultAsync(int id, bool noTracking = true)
    {
        var result = await CreateQuery(noTracking).FirstOrDefaultAsync(item => item.Id == id);
        if (result == null) return null;
        
        return DtoConverter.ToToDoItemDto(result);
    }

    public async Task<IEnumerable<ToDoItemDto>> GetAllAsync(bool noTracking = true)
    {
        return await CreateQuery(noTracking).Select(item => DtoConverter.ToToDoItemDto(item)).ToListAsync();
    }
    
    public async Task<IEnumerable<ToDoItemDto>> GetHistoryAsync(bool noTracking = true)
    {
        return await CreateQuery(noTracking)
            .Where(item => item.IsDone)
            .OrderBy(item => item.DoneDate)
            .Select(item => DtoConverter.ToToDoItemDto(item))
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await RepoDbSet.AnyAsync(e => e.Id == id);
    }

    public async Task<ToDoItemDto> RemoveAsync(int id)
    {
        var result = await CreateQuery().FirstOrDefaultAsync(item => item.Id == id);
        if (result == null)
        {
            throw new NullReferenceException($"Entity {nameof(ToDoItem)} with id {id} was not found");
        }
        return Remove(result);
    }
}